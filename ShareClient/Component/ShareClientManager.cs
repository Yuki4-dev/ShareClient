using ShareClient.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ShareClient.Component
{
    public class ShareClientManager : IClientManeger
    {
        private readonly SemaphoreSlim _Semaphore = new(1);
        private readonly List<int> _SendDataSize = new();
        private readonly List<int> _RecieveDataSize = new();

        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; set; } = 2;
        public int DataSizeCapacity { get; set; } = 1000;

        public ShareClientManager(ShareClientSpec clientSpec)
        {
            ClientSpec = clientSpec;
        }

        public virtual bool HandleException(ShareClientException ex)
        {
            return false;
        }

        public virtual bool PreSendDataSize(int size)
        {
            if (_SendDataSize.Count <= DataSizeCapacity)
            {
                _Semaphore.Wait();
                _SendDataSize.Add(size);
                _Semaphore.Release();
            }

            return true;
        }

        public virtual void SetRecieveDataSize(int size)
        {
            if (_RecieveDataSize.Count <= DataSizeCapacity)
            {
                _Semaphore.Wait();
                _RecieveDataSize.Add(size);
                _Semaphore.Release();
            }
        }

        public int GetSendDataSize()
        {
            int size = 0;
            if (_SendDataSize.Count != 0)
            {
                _Semaphore.Wait();
                size = _SendDataSize.Sum();
                _Semaphore.Release();
            }

            return size;
        }

        public int GetRecieveDataSize()
        {
            int size = 0;
            if (_RecieveDataSize.Count != 0)
            {
                _Semaphore.Wait();
                size = _RecieveDataSize.Sum();
                _Semaphore.Release();
            }

            return size;
        }

        public void SendDataSizeClear()
        {
            if (_SendDataSize.Count != 0)
            {
                _Semaphore.Wait();
                _SendDataSize.Clear();
                _Semaphore.Release();
            }
        }

        public void RecieveDataSizeClear()
        {
            if (_RecieveDataSize.Count != 0)
            {
                _Semaphore.Wait();
                _RecieveDataSize.Clear();
                _Semaphore.Release();
            }
        }
    }
}
