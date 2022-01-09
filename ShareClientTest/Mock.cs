using ShareClient.Component.Algorithm;
using ShareClient.Component.Core;
using ShareClient.Model;
using ShareClient.Model.ShareClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ShareClientTest
{
    internal class MockClientManager : IShareAlgorithmManager
    {
        public int RetryCount { get; set; } = 2;
        public List<int> SendDataSize { get; } = new();
        public List<int> RecieveDataSize { get; } = new();
        public ShareClientSpec ClientSpec { get; } = new();
        public IShareClientLogger Logger => new MockShareClientLogger();

        public MockClientManager() { }

        public bool HandleException(Exception ex)
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

    internal class MockClientSocket : IShareClientSocket
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

        public void Close()
        {
        }

        public void Open(IPEndPoint local, IPEndPoint remote)
        {
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
