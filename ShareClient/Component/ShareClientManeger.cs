using ShareClient.Model;
using System;

namespace ShareClient.Component
{
    public class ShareClientManeger : IClientManeger
    {
        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; set; } = 2;
        public IShareClientLogger Logger { get; private set; } = new DebugLogger();

        public event Func<int, bool> SendDataSize;
        public event Action<int> RecieveDataSize;

        public ShareClientManeger(ShareClientSpec clientSpec)
        {
            ClientSpec = clientSpec;
        }

        public virtual bool HandleException(ShareClientException ex)
        {
            return false;
        }

        public virtual bool PreSendDataSize(int size)
        {
            if (SendDataSize == null)
            {
                return false;
            }

            return SendDataSize.Invoke(size);
        }

        public virtual void SetRecieveDataSize(int size)
        {
            if (RecieveDataSize == null)
            {
                return;
            }

            RecieveDataSize.Invoke(size);
        }
    }
}
