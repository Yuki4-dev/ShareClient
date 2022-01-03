using ShareClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public class ConnectionManager : IConnectionManager
    {
        private UdpClient _Client;
        private IShareClientLogger logger = new DebugLogger();

        public bool IsConnecting { get; private set; } = false;

        public int ConnectionDelay { get; set; } = 100;

        public ConnectionManager() { }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData)
        {
            return await ConnectAsync(endPoint, connectionData, null);
        }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData, Action<ConnectionResponse> responseCallback)
        {
            logger.Info($"Start Connect. -> {endPoint.Address} : {endPoint.Port}");
            ConnectiongOrThrow();

            return await Connect(endPoint, connectionData, responseCallback);
        }

        private async Task<Connection> Connect(IPEndPoint endPoint, ConnectionData connectionData, Action<ConnectionResponse> responseCallback)
        {
            try
            {
                _Client = new UdpClient();
                _Client.Connect(endPoint);
                var shareClientData = new ShareClientData(ShareClientHeader.CreateSystem((uint)connectionData.Size), connectionData.ToByte());
                return await WaitResponse(() => ConnectWork(shareClientData, responseCallback));
            }
            catch (Exception ex)
            {
                if (IsConnecting)
                {
                    var ce = new ConnectionException(endPoint, $"Fail Connect. {ex.Message}", ex);
                    logger.Error(ce.Message, ce);
                    throw ce;
                }
                return null;
            }
            finally
            {
                IsConnecting = false;
                _Client?.Dispose();
            }
        }

        private Connection ConnectWork(ShareClientData connectData, Action<ConnectionResponse> responseCallback)
        {
            _Client.Send(connectData.ToByte(), connectData.Size);
            logger.Send(_Client.Client.RemoteEndPoint, connectData.ToByte());

            IPEndPoint receiveEp = null;
            var receiveData = _Client.Receive(ref receiveEp);
            logger.Receive(receiveEp, receiveData);

            var clientData = ShareClientData.FromBytes(receiveData);
            if (clientData == null || clientData.Header.DataType != SendDataType.System)
            {
                logger.Info($"ShareClientData Convert Fail or Type {clientData.Header.DataType}.");
                return null;
            }

            var response = ConnectionResponse.FromByte(clientData.DataPart);
            if (response == null)
            {
                logger.Info($"Response DataPart is Nothing.");
                return null;
            }

            Connection result = null;
            responseCallback?.Invoke(response);
            if (response.IsConnect)
            {
                logger.Info($"Succes Connect.");
                result = new Connection(response.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, (IPEndPoint)_Client.Client.RemoteEndPoint);
            }
            else
            {
                logger.Info($"Cancel Connect.");
            }

            IsConnecting = false;
            return result;
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> requestAccept)
        {
            return await AcceptAsync(endPoint, (receiveEp, connectionData) => new ConnectionResponse(requestAccept(receiveEp, connectionData), connectionData));
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept)
        {
            logger.Info($"Start Accept. -> {endPoint.Address} : {endPoint.Port}");
            ConnectiongOrThrow();

            return await Accept(endPoint, requestAccept);
        }

        private async Task<Connection> Accept(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept)
        {
            try
            {
                _Client = new UdpClient(endPoint);
                return await WaitResponse(() => AcceptWork(requestAccept));
            }
            catch (Exception ex)
            {
                if (IsConnecting)
                {
                    var ce = new ConnectionException(endPoint, $"Fail Accept. {ex.Message} -> {endPoint.Address} : {endPoint.Port}", ex);
                    logger.Error(ce.Message, ce);
                    throw ce;
                }
                return null;
            }
            finally
            {
                IsConnecting = false;
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
                logger.Info($"ShareClientData Convert Fail or Type {clientData.Header.DataType}.");
                return null;
            }

            var connectionData = ConnectionData.FromByte(clientData.DataPart);
            if (connectionData == null)
            {
                logger.Info($"ConnectionData Convert Fail.");
                return null;
            }

            var result = requestAccept.Invoke(receiveEp, connectionData);
            logger.Info($"RequestAccept is {result.IsConnect}.");

            var responseData = new ShareClientData(ShareClientHeader.CreateSystem((uint)result.Size), result.ToByte());
            _Client.Send(responseData.ToByte(), responseData.Size, receiveEp);
            logger.Send(receiveEp, responseData.ToByte());

            if (!result.IsConnect)
            {
                return null;
            }
            IsConnecting = false;
            logger.Info($"Accept Succes.");
            return new Connection(result.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, receiveEp);
        }

        private async Task<Connection> WaitResponse(Func<Connection> work)
        {
            return await Task.Run(() =>
           {
               Connection con = null;
               while (IsConnecting)
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
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Cancel()
        {
            logger.Info($"Cancel.");
            IsConnecting = false;
            _Client?.Dispose();
        }

        public void Dispose()
        {
            logger.Info($"Dispose.");
            Cancel();
        }

        private void ConnectiongOrThrow()
        {
            if (IsConnecting)
            {
                var ex = new InvalidOperationException("Already Run Another Connect Process.");
                logger.Error(ex.Message, ex);
                throw ex;
            }
            IsConnecting = true;
        }
    }
}
