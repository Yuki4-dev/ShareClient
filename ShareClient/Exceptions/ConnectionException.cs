using System;
using System.Net;

namespace ShareClient.Exceptions
{
    public class ConnectionException : Exception
    {
        public IPEndPoint EndPoint { get; }
        public ConnectionException(IPEndPoint endPoint, string msg, Exception inner) : base(msg, inner)
        {
            EndPoint = endPoint;
        }
    }
}
