using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    internal class UdpClientSocket : IClientSocket
    {
        private UdpClient _UdpClient;

        public bool IsOpen { get; private set; }

        public UdpClientSocket() { }

        public void Open(IPEndPoint local, IPEndPoint remote)
        {
            if (IsOpen)
            {
                throw new SocketException(IsOpen, "Client is Open.", null);
            }

            try
            {
                _UdpClient = new UdpClient(local);
                _UdpClient.Connect(remote);
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
                return _UdpClient.Receive(ref receiveEp);
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
