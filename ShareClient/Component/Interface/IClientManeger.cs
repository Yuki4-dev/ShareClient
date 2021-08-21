using ShareClient.Model;

namespace ShareClient.Component
{
    public interface IClientManeger
    {
        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; }
        public IShareClientLogger Logger { get; }

        public bool HandleException(ShareClientException ex);
        public bool PreSendDataSize(int size);
        public void SetRecieveDataSize(int size);
    }
}
