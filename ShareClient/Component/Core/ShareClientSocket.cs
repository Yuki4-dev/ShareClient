using ShareClient.Component.Connect;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component.Core
{
    public abstract class ShareClientSocket : IClientSocket
    {
        public static IClientSocket Udp => new UdpClientSocket();

        public abstract bool IsOpen { get; }
        public abstract Task<byte[]> ReceiveAsync();
        public abstract void Send(byte[] sendData);
        public abstract void Close();
        public abstract void Dispose();
        public abstract void Open(IPEndPoint local, IPEndPoint remote);
    }
}
