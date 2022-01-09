using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareClient.Component.Core.Internal
{
    internal class InternalUdpClientSocket : IShareClientSocket
    {
        private readonly UdpClient _UdpClient;
        private IPEndPoint _Remote;

        public bool IsOpen { get; private set; } = false;

        public InternalUdpClientSocket()
        {
            try
            {
                _UdpClient = new UdpClient();
                IsOpen = true;
            }
            catch (Exception ex)
            {
                throw new SocketException(IsOpen, "Open Failure : " + ex.Message, ex);
            }
        }

        public InternalUdpClientSocket(IPEndPoint iPEndPoint)
        {
            try
            {
                _UdpClient = new UdpClient(iPEndPoint);
                IsOpen = true;
            }
            catch (Exception ex)
            {
                throw new SocketException(IsOpen, "Open Failure : " + ex.Message, ex);
            }
        }

        public void Connect(IPEndPoint iPEndPoint)
        {
            if (!IsOpen)
            {
                throw new SocketException(IsOpen, "Client is not Open.", null);
            }

            try
            {
                _UdpClient.Connect(iPEndPoint);
                _Remote = iPEndPoint;
            }
            catch (Exception ex)
            {
                throw new SocketException(IsOpen, "Connect Failure : " + ex.Message, ex);
            }
        }

        public async Task<byte[]> ReceiveAsync()
        {
            CheckClient();
            return await Task.Run(Receive);
        }

        private byte[] Receive()
        {
            try
            {
                IPEndPoint receiveEp = null;
                var recieveData = _UdpClient.Receive(ref receiveEp);
                if (_Remote == null || _Remote.Equals(receiveEp))
                {
                    return recieveData;
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
            CheckClient(true);

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

        private void CheckClient(bool withConnect = false)
        {
            if (!IsOpen)
            {
                throw new SocketException(IsOpen, "Client is Not Open.", null);
            }
            else if (withConnect && _Remote == null)
            {
                throw new SocketException(IsOpen, "Client is Not Connect.", null);
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
