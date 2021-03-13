using ShareClient.Model;
using System.Collections.Generic;
using System.Threading;

namespace ShareClient.Component
{
    public class ShareClientManager : IClientManeger
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public ShareClientSpec ClientSpec { get; }
        public int RetryCount { get; set; } = 2;

        public int DataSizeCapacity { get; set; } = 1000;
        private readonly List<int> _DataSize = new List<int>();
        public IReadOnlyList<int> DataSize { get => _DataSize; }

        public ShareClientManager(ShareClientSpec clientSpec)
        {
            ClientSpec = clientSpec;
        }

        public virtual bool HandleException(ShareClientException ex)
        {
            return false;
        }

        public virtual void SetDataSize(int size)
        {
            if (DataSize.Count <= DataSizeCapacity)
            {
                DataAdd(size);
            }
        }

        private  void DataAdd(int size)
        {
            semaphore.Wait();
            _DataSize.Add(size);
            semaphore.Release();
        }

        public void DataSizeClear()
        {
            semaphore.Wait();
            _DataSize.Clear();
            semaphore.Release();
        }
    }
}
