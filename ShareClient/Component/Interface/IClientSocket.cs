using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IClientSocket : IDisposable
    {
        public bool IsOpen { get; }
        public void Open(IPEndPoint local, IPEndPoint remote);
        public void Close();
        public void Send(byte[] sendData);
        public Task<byte[]> ReceiveAsync();
    }
}
