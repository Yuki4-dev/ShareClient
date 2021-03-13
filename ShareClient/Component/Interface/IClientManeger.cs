using ShareClient.Model;

namespace ShareClient.Component
{
    public interface IClientManeger
    {
        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; }
        public bool HandleException(ShareClientException ex);
        public void SetDataSize(int size);
    }
}
