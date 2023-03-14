using ShareClient.Component.Algorithm;
using ShareClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace ShareClientForm.Componet
{
    public class DataSizeShareAlgorithmManager : IShareAlgorithmManager
    {
        private readonly SemaphoreSlim _Semaphore = new(1);
        private readonly List<int> _SendDataSize = new();
        private readonly List<int> _ReceiveDataSize = new();

        public int RetryCount { get; set; } = 2;
        public int DataSizeCapacity { get; set; } = 1000;
        public IShareClientLogger Logger { get; private set; } = new DebugLogger();

        public DataSizeShareAlgorithmManager() { }

        public virtual bool HandleException(Exception ex)
        {
            return false;
        }

        public virtual bool PreSendDataSize(int size)
        {
            if (_SendDataSize.Count <= DataSizeCapacity)
            {
                _Semaphore.Wait();
                _SendDataSize.Add(size);
                _ = _Semaphore.Release();
            }

            return true;
        }

        public virtual void SetReceiveDataSize(int size)
        {
            if (_ReceiveDataSize.Count <= DataSizeCapacity)
            {
                _Semaphore.Wait();
                _ReceiveDataSize.Add(size);
                _ = _Semaphore.Release();
            }
        }

        public int GetSendDataSize()
        {
            int size = 0;
            if (_SendDataSize.Count != 0)
            {
                _Semaphore.Wait();
                size = _SendDataSize.Sum();
                _ = _Semaphore.Release();
            }

            return size;
        }

        public int GetReceiveDataSize()
        {
            int size = 0;
            if (_ReceiveDataSize.Count != 0)
            {
                _Semaphore.Wait();
                size = _ReceiveDataSize.Sum();
                _ = _Semaphore.Release();
            }

            return size;
        }

        public void SendDataSizeClear()
        {
            if (_SendDataSize.Count != 0)
            {
                _Semaphore.Wait();
                _SendDataSize.Clear();
                _ = _Semaphore.Release();
            }
        }

        public void ReceiveDataSizeClear()
        {
            if (_ReceiveDataSize.Count != 0)
            {
                _Semaphore.Wait();
                _ReceiveDataSize.Clear();
                _ = _Semaphore.Release();
            }
        }

        public void SetLogger(IShareClientLogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }

    internal class DebugLogger : IShareClientLogger
    {
        public void Error(string message, Exception exception)
        {
            Debug.WriteLine(message);
        }

        public void Info(string message)
        {
            Debug.WriteLine(message);
        }

        public void Receive(EndPoint iPEndPoint, byte[] receiveData)
        {
            Debug.WriteLine($"Receive -> {iPEndPoint}");
        }

        public void Send(EndPoint iPEndPoint, byte[] sendData)
        {
            Debug.WriteLine($"Send -> {iPEndPoint}");
        }
    }
}
