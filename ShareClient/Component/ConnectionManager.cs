using ShareClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public class ConnectionManager : ShareClientStatus, IConnectionManager
    {
        private UdpClient _Client;

        public ConnectionManager() { }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData)
        {
            Connect();
            return await Connect(endPoint, connectionData);
        }

        private async Task<Connection> Connect(IPEndPoint endPoint, ConnectionData connectionData)
        {
            try
            {
                _Client = new UdpClient();
                _Client.Connect(endPoint);
                return await WaitResponse(() => ConnectWork(GetClientData(connectionData)));
            }
            catch (Exception ex)
            {
                if (Status == ClientStatus.Connect)
                {
                    throw new ShareClientException("Connect Failure : " + ex.Message, ex);
                }
                return null;
            }
            finally
            {
                _Client?.Dispose();
                if (Status == ClientStatus.Connect)
                {
                    init();
                }
            }
        }

        private Connection ConnectWork(ShareClientData connectData)
        {
            _Client.Send(connectData.ToByte(), connectData.Size);

            IPEndPoint receiveEp = null;
            var ReceiveData = _Client.Receive(ref receiveEp);
            var cleintData = ShareClientData.FromBytes(ReceiveData);
            if (cleintData == null || cleintData.Header.DataType != SendDataType.System)
            {
                return null;
            }

            var response = ConnectionResponse.FromByte(cleintData.DataPart);
            if (response != null && response.IsConnect)
            {
                Open();
                return new Connection(response.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, (IPEndPoint)_Client.Client.RemoteEndPoint);
            }
            else
            {
                init();
                return null;
            }
        }

        private ShareClientData GetClientData(ConnectionData connectionData)
        {
            var header = ShareClientHeader.SystemHeader((uint)connectionData.Size);
            return new ShareClientData(header, connectionData.ToByte());
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> acceptCallback)
        {
            return await AcceptAsync(endPoint, (receiveEp, connectionData) => new ConnectionResponse(acceptCallback(receiveEp, connectionData), connectionData));
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback)
        {
            Connect();
            return await Accept(endPoint, acceptCallback);
        }

        private async Task<Connection> Accept(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback)
        {
            try
            {
                _Client = new UdpClient(endPoint);
                return await WaitResponse(() => AcceptWork(acceptCallback));
            }
            catch (Exception ex)
            {
                if (Status == ClientStatus.Connect)
                {
                    throw new ShareClientException("Accept Failure : " + ex.Message, ex);
                }
                return null;
            }
            finally
            {
                _Client?.Dispose();
                if (Status == ClientStatus.Connect)
                {
                    init();
                }
            }
        }

        private Connection AcceptWork(Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback)
        {
            IPEndPoint receiveEp = null;
            var receiveData = _Client.Receive(ref receiveEp);
            var clientData = ShareClientData.FromBytes(receiveData);
            if (clientData == null || clientData.Header.DataType != SendDataType.System)
            {
                return null;
            }

            var connectionData = ConnectionData.FromByte(clientData.DataPart);
            if (connectionData == null)
            {
                return null;
            }

            var result = acceptCallback.Invoke(receiveEp, connectionData);
            var responseData = GetResponseData(result);
            _Client.Send(responseData.ToByte(), responseData.Size, receiveEp);
            if (result.IsConnect)
            {
                Open();
                return new Connection(result.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, receiveEp);
            }

            return null;
        }

        private ShareClientData GetResponseData(ConnectionResponse response)
        {
            var header = ShareClientHeader.SystemHeader((uint)response.Size);
            return new ShareClientData(header, response.ToByte());
        }

        private async Task<Connection> WaitResponse(Func<Connection> work)
        {
            return await Task.Factory.StartNew(() =>
           {
               Connection con = null;
               while (Status == ClientStatus.Connect)
               {
                   con = work.Invoke();
                   Thread.Sleep(100);
               }

               return con;
           });
        }

        protected override void CloseClient()
        {
            _Client?.Dispose();
        }

        public void Cancel()
        {
            init();
            _Client?.Dispose();
        }
    }
}
