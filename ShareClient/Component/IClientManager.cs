using ShareClient.Model;
using ShareClient.Model.ShareClient;
using System;

namespace ShareClient.Component
{
    public interface IClientManager
    {
        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; }
        public IShareClientLogger Logger { get; }

        public bool HandleException(Exception ex);
        public bool PreSendDataSize(int size);
        public void SetRecieveDataSize(int size);
    }
}
