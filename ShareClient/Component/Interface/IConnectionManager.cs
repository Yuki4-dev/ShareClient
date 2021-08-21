using ShareClient.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IConnectionManager : IDisposable
    {
        public bool IsConnect { get; }
        public int ConnectionDelay { get; set; }
        public void SetLogger(IShareClientLogger logger);
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData);
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData, Func<ConnectionResponse, bool> responseAccept);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> requestAccept);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept);
        public void Cancel();
    }
}
