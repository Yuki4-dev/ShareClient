using ShareClient.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IConnectionManager : IDisposable
    {
        public bool IsConnect { get; }
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> acceptCallback);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptCallback);
        public void Cancel();
    }
}
