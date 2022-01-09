using ShareClient.Model;
using ShareClient.Model.ShareClient;
using System;
using System.Net;

namespace ShareClient.Component.Connect
{
    public interface IConnectionSocket : IDisposable
    {
        IPEndPoint LocalEndPoint { get; }
        void Send(IPEndPoint remoteEndPoint, ShareClientData sendData);
        ConnectionSocketRecieveData Recieve();
        public void Close();
    }

    public class ConnectionSocketRecieveData
    {
        public IPEndPoint ReciveEndPoint { get; }
        public byte[] RecieveBytes { get; }

        public ConnectionSocketRecieveData(IPEndPoint recuveEndPoint, byte[] recieveBytes)
        {
            ReciveEndPoint = recuveEndPoint;
            RecieveBytes = recieveBytes;
        }
    }
}
