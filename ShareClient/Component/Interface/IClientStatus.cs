using System;

namespace ShareClient.Component
{
    public interface IClientStatus : IDisposable
    {
        public ClientStatus Status { get; }
    }

    public enum ClientStatus
    {
        Init, Connect, Open, Close
    }
}
