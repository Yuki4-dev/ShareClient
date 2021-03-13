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

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ShareClientSpec clientSpec)
        {
            return await ConnectAsync(endPoint, clientSpec, new byte[0]);
        }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ShareClientSpec clientSpec, byte[] meta)
        {
            Connect();
            return await Connect(endPoint, clientSpec, meta);
        }

        private async Task<Connection> Connect(IPEndPoint endPoint, ShareClientSpec clientSpec, byte[] meta)
        {
            try
            {
                _Client = new UdpClient();
                _Client.Connect(endPoint);
                return await WaitResponse(() => ConnectWork(GetConnectData(clientSpec, meta)));
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
            var imageData = ShareClientData.FromBytes(ReceiveData);
            if (imageData == null || imageData.Header.DataType != SendDataType.Connect)
            {
                return null;
            }

            var response = ConnectionResponse.FromByte(imageData.DataPart);
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

        private ShareClientData GetConnectData(ShareClientSpec clientSpec, byte[] meta)
        {
            var connectData = new ConnectionData(clientSpec, meta);
            var header = ShareClientHeader.Connect((uint)connectData.Size);
            return new ShareClientData(header, connectData.ToByte());
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
            var imageData = ShareClientData.FromBytes(receiveData);
            if (imageData == null || imageData.Header.DataType != SendDataType.Connect)
            {
                return null;
            }

            var connectionData = ConnectionData.FromByte(imageData.DataPart);
            if (connectionData == null)
            {
                return null;
            }

            var response = acceptCallback.Invoke(receiveEp, connectionData);
            var clientData = GetResponseData(response);
            _Client.Send(clientData.ToByte(), clientData.Size, receiveEp);
            if (response.IsConnect)
            {
                Open();
                return new Connection(response.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, receiveEp);
            }

            return null;
        }

        private ShareClientData GetResponseData(ConnectionResponse response)
        {
            var header = ShareClientHeader.Connect((uint)response.Size);
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
