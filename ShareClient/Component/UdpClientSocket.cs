using ShareClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    internal class UdpClientSocket : IClientSocket
    {
        private Connection _Connection;
        private UdpClient _UdpClient;

        public bool IsOpen { get; private set; }

        public UdpClientSocket() { }

        public void Open(Connection connection)
        {
            if (IsOpen)
            {
                throw new SocketException(IsOpen, "Client is Open.", null);
            }
            _Connection = connection;

            try
            {
                _UdpClient = new(connection.LocalEndPoint);
                _UdpClient.Connect(connection.RemoteEndPoint);
                IsOpen = true;
            }
            catch (Exception ex)
            {
                throw new SocketException(IsOpen, "Open Failure : " + ex.Message, ex);
            }
        }

        public async Task<byte[]> ReceiveAsync()
        {
            CheckOpenAndThrow();
            return await Task.Run(Receive);
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
                    IsOpen = false;
                }
                else
                {
                    throw new SocketException(IsOpen, "Receive Failure : " + ex.Message, ex);
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
                    IsOpen = false;
                }
                else
                {
                    throw new SocketException(IsOpen, "Send Failure : " + ex.Message, ex);
                }
            }
        }

        private void CheckOpenAndThrow()
        {
            if (!IsOpen)
            {
                throw new SocketException(IsOpen, "Client is Not Open.", null);
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
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
