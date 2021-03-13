using System.Net;

namespace ShareClient.Model
{
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
