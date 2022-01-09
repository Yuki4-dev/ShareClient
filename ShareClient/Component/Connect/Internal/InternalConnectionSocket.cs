using ShareClient.Model;
using ShareClient.Model.ShareClient;
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

        public void Send(IPEndPoint remoteEndPoint, ShareClientData sendData)
        {
            _UdpClient.Send(sendData.ToByte(), sendData.Size, remoteEndPoint);
        }

        public ConnectionSocketRecieveData Recieve()
        {
            IPEndPoint recieveEndoPoint = null;
            var recieveBytes = _UdpClient.Receive(ref recieveEndoPoint);
            return new ConnectionSocketRecieveData(recieveEndoPoint, recieveBytes);
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
