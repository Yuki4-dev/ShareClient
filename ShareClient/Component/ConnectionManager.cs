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

        public bool IsConnect { get; private set; } = false;

        public ConnectionManager() { }

        public async Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData)
        {
            if (IsConnect)
            {
                throw new Exception();
            }
            IsConnect = true;

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
                if (IsConnect)
                {
                    throw new ShareClientException("Connect Failure : " + ex.Message, ex);
                }
                return null;
            }
            finally
            {
                IsConnect = false;
                _Client?.Dispose();
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

            Connection result = null;
            var response = ConnectionResponse.FromByte(cleintData.DataPart);
            if (response != null && response.IsConnect)
            {
                result = new Connection(response.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, (IPEndPoint)_Client.Client.RemoteEndPoint);
            }

            IsConnect = false;
            return result;
        }

        private ShareClientData GetClientData(ConnectionData connectionData)
        {
            var header = ShareClientHeader.CreateSystem((uint)connectionData.Size);
            return new ShareClientData(header, connectionData.ToByte());
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> acceptCallback)
        {
            return await AcceptAsync(endPoint, (receiveEp, connectionData) => new ConnectionResponse(acceptCallback(receiveEp, connectionData), connectionData));
        }

        public async Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback)
        {
            if (IsConnect)
            {
                throw new Exception();
            }
            IsConnect = true;

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
                if (IsConnect)
                {
                    throw new ShareClientException("Accept Failure : " + ex.Message, ex);
                }
                return null;
            }
            finally
            {
                IsConnect = false;
                _Client?.Dispose();
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
                IsConnect = false;
                return new Connection(result.ConnectionData.CleintSpec, (IPEndPoint)_Client.Client.LocalEndPoint, receiveEp);
            }

            return null;
        }

        private ShareClientData GetResponseData(ConnectionResponse response)
        {
            var header = ShareClientHeader.CreateSystem((uint)response.Size);
            return new ShareClientData(header, response.ToByte());
        }

        private async Task<Connection> WaitResponse(Func<Connection> work)
        {
            return await Task.Factory.StartNew(() =>
           {
               Connection con = null;
               while (IsConnect)
               {
                   con = work.Invoke();
                   Thread.Sleep(100);
               }

               return con;
           });
        }

        public void Cancel()
        {
            IsConnect = false;
            _Client?.Dispose();
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}
