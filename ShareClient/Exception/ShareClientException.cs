using System;

namespace ShareClient
{
    [Serializable()]
    public class ShareClientException : Exception
    {
        public ShareClientException() : base() { }
        public ShareClientException(string msg) : base(msg) { }
        public ShareClientException(string msg, Exception inner) : base(msg, inner) { }
    }
}
