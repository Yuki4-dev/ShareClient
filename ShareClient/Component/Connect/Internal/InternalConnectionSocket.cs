using System.Net;
using System.Net.Sockets;

namespace ShareClient.Component.Connect.Internal
{
    internal class InternalConnectionSocket : IConnectionSocket
    {
        private readonly UdpClient _UdpClient;

        public IPEndPoint LocalEndPoint => (IPEndPoint)_UdpClient.Client.LocalEndPoint;

        public InternalConnectionSocket()
        {
            _UdpClient = new UdpClient();
        }

        public InternalConnectionSocket(IPEndPoint localEndPoint)
        {
            _UdpClient = new UdpClient(localEndPoint);
        }

        public void Send(IPEndPoint remoteEndPoint, byte[] sendData)
        {
            _ = _UdpClient.Send(sendData, sendData.Length, remoteEndPoint);
        }

        public ConnectionSocketReceiveData Receive()
        {
            IPEndPoint receiveEndoPoint = null;
            var receiveBytes = _UdpClient.Receive(ref receiveEndoPoint);
            return new ConnectionSocketReceiveData(receiveEndoPoint, receiveBytes);
        }

        public void Dispose()
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
