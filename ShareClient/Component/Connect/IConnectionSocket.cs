using System;
using System.Net;

namespace ShareClient.Component.Connect
{
    public interface IConnectionSocket : IDisposable
    {
        IPEndPoint LocalEndPoint { get; }
        void Send(IPEndPoint remoteEndPoint, byte[] sendData);
        ConnectionSocketReceiveData Receive();
    }

    public class ConnectionSocketReceiveData
    {
        public IPEndPoint ReceiveEndPoint { get; }
        public byte[] ReceiveBytes { get; }

        public ConnectionSocketReceiveData(IPEndPoint receiveEndPoint, byte[] receiveBytes)
        {
            ReceiveEndPoint = receiveEndPoint;
            ReceiveBytes = receiveBytes;
        }
    }
}
