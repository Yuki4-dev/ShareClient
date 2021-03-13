using ShareClient.Model;
using System;

namespace ShareClient
{
    public class ReceiveDataAnalyzeException : ShareClientException
    {
        public ShareClientData ClientData { get; }
        public ReceiveDataAnalyzeException(ShareClientData clientData, string msg, Exception inner) : base(msg, inner)
        {
            ClientData = clientData;
        }
    }
}
