using ShareClient.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IConnectionManager : IClientStatus, IDisposable
    {
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ShareClientSpec clientSpec);
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ShareClientSpec clientSpec, byte[] meta);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> acceptCallback);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback);
        public void Cancel();
    }
}
