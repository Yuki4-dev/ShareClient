using ShareClient;
using ShareClient.Component;
using ShareClient.Model;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ShareClientTest
{
    internal class MockClientManager : IClientManeger
    {
        public int RetryCount { get; set; } = 2;
        public List<int> SendDataSize { get; } = new();
        public List<int> RecieveDataSize { get; } = new();
        public ShareClientSpec ClientSpec { get; } = new();
        public IShareClientLogger Logger => new MockShareClientLogger();

        public MockClientManager()
        {
        }

        public bool HandleException(ShareClientException ex)
        {
            return true;
        }

        public bool PreSendDataSize(int size)
        {
            SendDataSize.Add(size);
            return true;
        }

        public void SetRecieveDataSize(int size)
        {
            RecieveDataSize.Add(size);
        }
    }

    internal class MockClientSocket : IClientSocket
    {
        public List<byte[]> ImageBytes { get; } = new();
        public bool IsOpen { get; private set; } = true;

        public void Send(byte[] sendData)
        {
            ImageBytes.Add(sendData);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            if (ImageBytes.Count == 1)
            {
                IsOpen = false;
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

        public void Close()
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

    internal class MockShareClientLogger : IShareClientLogger
    {
        public void Error(string message, Exception exception)
        {
        }

        public void Info(string message)
        {
        }

        public void Receive(EndPoint iPEndPoint, byte[] receiveData)
        {
        }

        public void Send(EndPoint iPEndPoint, byte[] sendData)
        {
        }
    }
}
