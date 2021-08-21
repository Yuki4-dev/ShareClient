using ShareClient.Model;
using System.Net;
using System.Net.Sockets;

namespace ShareClient.Component
{
    public class ConnectionManager : IConnectionManager
    {
        private UdpClient _Client;
        private IShareClientLogger logger = new DebugLogger();

        public bool IsConnect { get; private set; } = false;

        public int ConnectionDelay { get; set; } = 100;


        public ConnectionManager() { }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData)
        {
            logger.Info($"Start Connect. -> {endPoint.Address} : {endPoint.Port}");
            return await ConnectAsync(endPoint, connectionData, (reponse) => reponse.IsConnect);
        }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData, Func<ConnectionResponse, bool> responseAccept)
        {
            if (IsConnect)
            {
                var ex = new InvalidOperationException($"ConnectAsync Already Run Another Connect Process. -> {endPoint.Address} : {endPoint.Port}");
                logger.Error(ex.Message, ex);
                throw ex;
            }
            IsConnect = true;

            return await Connect(endPoint, connectionData, responseAccept);
        }

        private async Task<Connection> Connect(IPEndPoint endPoint, ConnectionData connectionData, Func<ConnectionResponse, bool> responseAccept)
        {
            try
            {
                _Client = new();
                _Client.Connect(endPoint);
                return await WaitResponse(() => ConnectWork(GetClientData(connectionData), responseAccept));
            }
            catch (Exception ex)
            {
                if (IsConnect)
                {
                    var sce = new ShareClientException($"Fail Connect. {ex.Message} -> {endPoint.Address} : {endPoint.Port}", ex);
                    logger.Error(sce.Message, sce);
                    throw sce;
                }
                return null;
            }
            finally
            {
                IsConnect = false;
                _Client?.Dispose();
            }
        }

        private Connection ConnectWork(ShareClientData connectData, Func<ConnectionResponse, bool> responseAccept)
        {
            _Client.Send(connectData.ToByte(), connectData.Size);
            logger.Send(_Client.Client.RemoteEndPoint, connectData.ToByte());

            IPEndPoint receiveEp = null;
            var receiveData = _Client.Receive(ref receiveEp);
            logger.Receive(receiveEp, receiveData);

            var clientData = ShareClientData.FromBytes(receiveData);
            if (clientData == null || clientData.Header.DataType != SendDataType.System)
            {
                logger.Info($"ShareClientData Convert Fail or Type {clientData.Header.DataType}. -> {receiveEp.Address} : {receiveEp.Port}");
                return null;
            }

            Connection result = null;
            var response = ConnectionResponse.FromByte(clientData.DataPart);
            if (response != null && responseAccept.Invoke(response))
            {
                logger.Info($"Connect Succes. -> {receiveEp.Address} : {receiveEp.Port}");
                result = new(response.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, (IPEndPoint)_Client.Client.RemoteEndPoint);
            }

            IsConnect = false;
            return result;
        }

        private ShareClientData GetClientData(ConnectionData connectionData)
        {
            var header = ShareClientHeader.CreateSystem((uint)connectionData.Size);
            return new(header, connectionData.ToByte());
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> requestAccept)
        {
            logger.Info($"Start Accept. -> {endPoint.Address} : {endPoint.Port}");
            return await AcceptAsync(endPoint, (receiveEp, connectionData) => new ConnectionResponse(requestAccept(receiveEp, connectionData), connectionData));
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept)
        {
            if (IsConnect)
            {
                var ex = new InvalidOperationException($"AcceptAsync Already Run Another Connect Process. -> {endPoint.Address} : {endPoint.Port}");
                logger.Error(ex.Message, ex);
                throw ex;
            }
            IsConnect = true;

            return await Accept(endPoint, requestAccept);
        }

        private async Task<Connection> Accept(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept)
        {
            try
            {
                _Client = new(endPoint);
                return await WaitResponse(() => AcceptWork(requestAccept));
            }
            catch (Exception ex)
            {
                if (IsConnect)
                {
                    var se = new ShareClientException($"Fail Accept. {ex.Message} -> {endPoint.Address} : {endPoint.Port}", ex);
                    logger.Error(se.Message, se);
                    throw se;
                }
                return null;
            }
            finally
            {
                IsConnect = false;
                _Client?.Dispose();
            }
        }

        private Connection AcceptWork(Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept)
        {
            IPEndPoint receiveEp = null;
            var receiveData = _Client.Receive(ref receiveEp);
            logger.Receive(receiveEp, receiveData);

            var clientData = ShareClientData.FromBytes(receiveData);
            if (clientData == null || clientData.Header.DataType != SendDataType.System)
            {
                logger.Info($"ShareClientData Convert Fail or Type {clientData.Header.DataType}. -> {receiveEp.Address} : {receiveEp.Port}");
                return null;
            }

            var connectionData = ConnectionData.FromByte(clientData.DataPart);
            if (connectionData == null)
            {
                logger.Info($"ConnectionData Convert Fail. -> {receiveEp.Address} : {receiveEp.Port}");
                return null;
            }

            var result = requestAccept.Invoke(receiveEp, connectionData);
            logger.Info($"RequestAccept is {result.IsConnect}. -> {receiveEp.Address} : {receiveEp.Port}");

            var responseData = GetResponseData(result);
            _Client.Send(responseData.ToByte(), responseData.Size, receiveEp);
            logger.Send(receiveEp, responseData.ToByte());

            if (result.IsConnect)
            {
                IsConnect = false;
                logger.Info($"Accept Succes. -> {receiveEp.Address} : {receiveEp.Port}");
                return new(result.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, receiveEp);
            }

            return null;
        }

        private ShareClientData GetResponseData(ConnectionResponse response)
        {
            var header = ShareClientHeader.CreateSystem((uint)response.Size);
            return new(header, response.ToByte());
        }

        private async Task<Connection> WaitResponse(Func<Connection> work)
        {
            return await Task.Run(() =>
           {
               Connection con = null;
               while (IsConnect)
               {
                   logger.Info($"Run Wait Procces.");
                   con = work.Invoke();
                   Thread.Sleep(ConnectionDelay);
               }

               logger.Info($"Exit Wait Procces.");
               return con;
           });
        }

        public void SetLogger(IShareClientLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        public void Cancel()
        {
            logger.Info($"Cancel.");
            IsConnect = false;
            _Client?.Dispose();
        }

        public void Dispose()
        {
            logger.Info($"Dispose.");
            Cancel();
        }
    }
}
