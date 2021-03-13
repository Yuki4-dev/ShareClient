using System;

namespace ShareClient.Component
{
    public interface IShareClient : IClientStatus, IDisposable
    {
        public event EventHandler ShareClientClosed;
        public void Close();
        public IClientSocket Socket { get; }
        public IClientManeger ClientManager { get; }
    }
}
