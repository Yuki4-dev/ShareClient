using ShareClient.Model;
using System;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IClientSocket : IClientStatus, IDisposable
    {
        public void Open(Connection Connection);
        public void Send(byte[] sendData);
        public Task<byte[]> ReciveAsync();
    }
}
