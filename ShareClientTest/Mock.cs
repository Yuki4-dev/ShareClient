using ShareClient;
using ShareClient.Component;
using ShareClient.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareClientTest
{

    internal class MockClientManager : IClientManeger
    {
        public int RetryCount { get; set; } = 2;
        public List<int> DataSize { get; } = new List<int>();
        public ShareClientSpec ClientSpec { get; } = new ShareClientSpec();

        public MockClientManager()
        {
        }

        public void SetDataSize(int size)
        {
            DataSize.Add(size);
        }

        public bool HandleException(ShareClientException ex)
        {
            return true;
        }
    }

    internal class MockClientSocket : IClientSocket
    {
        public ClientStatus Status { get; private set; } = ClientStatus.Open;
        public List<byte[]> ImageBytes { get; } = new List<byte[]>();

        public void Send(byte[] sendData)
        {
            ImageBytes.Add(sendData);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            if (ImageBytes.Count == 1)
            {
                Status = ClientStatus.Close;
            }
            return await Task.Factory.StartNew(() =>
            {
                var img = ImageBytes[0];
                ImageBytes.RemoveAt(0);
                return img;
            });
        }

        public void Dispose()
        {
        }

        public void Open(Connection connection)
        {
        }
    }

    internal class MockReceiveImageProvider : IReceiveDataProvider
    {
        public byte[] Data { get; private set; }

        public bool CanReceive => true;

        public void Receive(byte[] data)
        {
            Data = data;
        }
    }
}
