using System;

namespace ShareClient.Component
{
    public interface IShareClient : IDisposable
    {
        public event EventHandler ShareClientClosed;
        public void Close();
        public IClientSocket Socket { get; }
        public IClientManager ClientManager { get; }
    }
}
