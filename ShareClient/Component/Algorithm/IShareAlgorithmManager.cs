using System;
using ShareClient.Model;

namespace ShareClient.Component.Algorithm
{
    public interface IShareAlgorithmManager
    {
        public int RetryCount { get; }
        public IShareClientLogger Logger { get; }
        public bool HandleException(Exception ex);
        public bool PreSendDataSize(int size);
        public void SetReceiveDataSize(int size);
    }
}
