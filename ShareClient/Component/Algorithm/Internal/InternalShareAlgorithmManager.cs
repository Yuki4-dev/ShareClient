using ShareClient.Model;
using System;

namespace ShareClient.Component.Algorithm.Internal
{
    internal class InternalShareAlgorithmManager : IShareAlgorithmManager
    {
        public int RetryCount { get; set; } = 2;
        public IShareClientLogger Logger { get; private set; } = new DebugLogger();

        public InternalShareAlgorithmManager() { }

        public void SetLogger(IShareClientLogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool HandleException(Exception ex)
        {
            return false;
        }

        public bool PreSendDataSize(int size)
        {
            return true;
        }

        public void SetReceiveDataSize(int size)
        {
            //
        }
    }
}
