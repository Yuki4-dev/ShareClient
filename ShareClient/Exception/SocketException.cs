using System;

namespace ShareClient
{
    public class SocketException : Exception
    {
        public bool IsOpen { get; }
        public SocketException(bool isOpen, string msg, Exception inner) : base(msg, inner)
        {
            IsOpen = isOpen;
        }
    }
}
