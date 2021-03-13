using ShareClient.Model;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public abstract class ShareClientSocket : IClientSocket
    {
        public static IClientSocket CreateUdpSocket()
        {
            return new UdpClientSocket();
        }

        public ShareClientSocket() { }

        public abstract Task<byte[]> ReceiveAsync();
        public abstract void Send(byte[] sendData);
        public abstract void Open(Connection connection);
        public abstract ClientStatus Status { get; }
        public abstract void Dispose();
    }
}
