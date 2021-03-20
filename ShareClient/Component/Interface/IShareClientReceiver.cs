using System;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IShareClientReceiver : IShareClient
    {
        public event EventHandler<SystemDataRecieveEventArgs> SystemDataRecieved;
        public Task ReceiveAsync();
    }

    public class SystemDataRecieveEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public SystemDataRecieveEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}
