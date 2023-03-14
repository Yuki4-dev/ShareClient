using System;

namespace ShareClient.Exceptions
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
