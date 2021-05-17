using ShareClient.Model;
using System;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IClientSocket : IDisposable
    {
        public bool IsOpen { get; }
        public void Open(Connection Connection);
        public void Close();
        public void Send(byte[] sendData);
        public Task<byte[]> ReceiveAsync();
    }
}
