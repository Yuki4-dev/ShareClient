using ShareClient.Model.ShareClient;
using System;

namespace ShareClient.Exceptions
{
    public class ReceiveDataAnalyzeException : ShareClientException
    {
        public ShareClientData ClientData { get; }
        public ReceiveDataAnalyzeException(ShareClientData clientData, string msg, Exception inner) : base(clientData?.Header, msg, inner)
        {
            ClientData = clientData;
        }
    }
}
