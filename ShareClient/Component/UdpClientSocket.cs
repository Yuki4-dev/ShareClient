using ShareClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    internal class UdpClientSocket : ShareClientStatus, IClientSocket
    {
        private Connection _Connection;
        private UdpClient _UdpClient;

        public UdpClientSocket() { }

        public void Open(Connection connection)
        {
            _Connection = connection;
            try
            {
                _UdpClient = new UdpClient(connection.LocalEndPoint);
                _UdpClient.Connect(connection.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                throw new ShareClientSocketException("Open Failure : " + ex.Message, ex);
            }

            Open();
        }

        public async Task<byte[]> ReceiveAsync()
        {
            CheckOpenAndThrow();
            return await Task.Factory.StartNew(Receive);
        }

        private byte[] Receive()
        {
            try
            {
                IPEndPoint receiveEp = null;
                var receiveData = _UdpClient.Receive(ref receiveEp);
                if (_Connection.RemoteEndPoint.Address.Equals(receiveEp.Address))
                {
                    return receiveData;
                }
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    Close();
                }
                else
                {
                    throw new ShareClientSocketException("Receive Failure : " + ex.Message, ex);
                }
            }

            return null;
        }

        public void Send(byte[] sendData)
        {
            CheckOpenAndThrow();
            try
            {
                _UdpClient.Send(sendData, sendData.Length);
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    Close();
                }
                else
                {
                    throw new ShareClientSocketException("Send Failure : " + ex.Message, ex);
                }
            }
        }

        private void CheckOpenAndThrow()
        {
            if (Status != ClientStatus.Open)
            {
                throw new InvalidOperationException($"Socket : {Status}");
            }
        }

        protected override void CloseClient()
        {
            try
            {
                _UdpClient?.Dispose();
            }
            catch
            {
            }
        }
    }
}
