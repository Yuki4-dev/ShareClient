using System;
using System.Threading.Tasks;

namespace ShareClient.Component.Core
{
    public interface IShareClientSocket : IDisposable
    {
        public bool IsOpen { get; }
        public void Send(byte[] sendData);
        public Task<byte[]> ReceiveAsync();
        public void Close();
    }
}
