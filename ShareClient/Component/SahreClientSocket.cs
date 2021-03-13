using ShareClient.Model;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public abstract class SahreClientSocket : IClientSocket
    {
        public static IClientSocket CreateUdpSocket()
        {
            return new UdpClientSocket();
        }

        public SahreClientSocket() { }

        public abstract Task<byte[]> ReciveAsync();
        public abstract void Send(byte[] sendData);
        public abstract void Open(Connection connection);
        public abstract ClientStatus Status { get; }
        public abstract void Dispose();
    }
}
