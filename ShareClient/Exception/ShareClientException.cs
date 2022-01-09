using ShareClient.Model.ShareClient;
using System;

namespace ShareClient
{
    [Serializable()]
    public class ShareClientException : Exception
    {
        public ShareClientHeader Header { get; }
        public ShareClientException(ShareClientHeader header, string msg, Exception inner) : base(msg, inner)
        {
            Header = header;
        }
    }
}
