using ShareClient.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IConnectionManager : IDisposable
    {
        public bool IsConnecting { get; }
        public int ConnectionDelay { get; set; }
        public void SetLogger(IShareClientLogger logger);
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData);
        public Task<Connection> ConnectAsync(IPEndPoint endPoint, ConnectionData connectionData, Action<ConnectionResponse> responseCallback);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, bool> requestAccept);
        public Task<Connection> AcceptAsync(IPEndPoint endPoint, Func<IPEndPoint, ConnectionData, ConnectionResponse> requestAccept);
        public void Cancel();
    }

    public class Connection
    {
        public ShareClientSpec ClientSpec { get; }
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }

        public Connection(ShareClientSpec clientSpec, IPEndPoint local, IPEndPoint remote)
        {
            ClientSpec = clientSpec;
            LocalEndPoint = local;
            RemoteEndPoint = remote;
        }
    }
}
