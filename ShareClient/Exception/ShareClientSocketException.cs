using ShareClient.Model;
using System;

namespace ShareClient
{
    public class ShareClientSocketException : ShareClientException
    {
        public ShareClientHeader Header { get; set; }
        public ShareClientSocketException(string msg, Exception inner) : base(msg, inner) { }
    }
}
