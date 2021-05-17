using ShareClient.Model;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace ShareClient.Component
{
    public class ShareClientSender : IShareClient
    {
        private int atomicCode = 0;
        private byte[] oldHash = new byte[0];
        private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        private readonly HashAlgorithm _HashAlgorithm = new MD5CryptoServiceProvider();
        protected bool StopApplicationData { get; set; } = false;

        public event EventHandler ShareClientClosed;

        public IClientSocket Socket { get; }
        public IClientManeger ClientManager { get; }

        public ShareClientSender(IClientManeger maneger, IClientSocket client)
        {
            ClientManager = maneger;
            Socket = client;
        }

        public void Send(byte[] data)
        {
            if (!ClientManager.ClientSpec.SendSameData)
            {
                var hash = _HashAlgorithm.ComputeHash(data);
                if (EqualsHash(hash, oldHash))
                {
                    return;
                }
                oldHash = hash;
            }

            SendData(data);
            Thread.Sleep(ClientManager.ClientSpec.SendDelay);
        }

        private void SendData(byte[] bytes)
        {
            int sendSize = ClientManager.ClientSpec.BufferSize - ShareClientHeader.SIZE;
            int splitCount = (bytes.Length / sendSize) + 1;
            if (splitCount > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException($"SplitCount : {splitCount}");
            }

            int splitIndex = 0;
            int dataSize = bytes.Length;
            uint useAtCode = GetAtomicCodeAndIncrement(splitCount);
            while (dataSize > sendSize)
            {
                SendData(useAtCode++, bytes, sendSize, bytes.Length - dataSize, splitCount, splitIndex++);
                dataSize -= sendSize;
            }

            SendData(useAtCode, bytes, dataSize, bytes.Length - dataSize, splitCount, splitIndex);
        }

        private void SendData(uint atomicCode, byte[] srcData, int sendLength, int sendIndex, int splitLength, int splitIndex)
        {
            var sendData = new byte[sendLength];
            Array.Copy(srcData, sendIndex, sendData, 0, sendData.Length);

            var header = ShareClientHeader.CreateApplication(atomicCode,
                (byte)splitLength,
                (byte)splitIndex,
                (uint)sendData.Length);

            SendShareClientData(new ShareClientData(header, sendData));
        }

        protected void SendShareClientData(ShareClientData clientData)
        {
            if (StopApplicationData && clientData.Header.DataType == SendDataType.Application)
            {
                return;
            }

            var sendData = clientData.ToByte();
            if (!ClientManager.PreSendDataSize(sendData.Length))
            {
                return;
            }

            int count = 0;
            while (Socket.IsOpen)
            {
                try
                {
                    Socket.Send(sendData);
                    Thread.Sleep(1);
                    break;
                }
                catch (ShareClientSocketException ex)
                {
                    if (Socket.IsOpen)
                    {
                        if (++count > ClientManager.RetryCount || ClientManager.HandleException(ex))
                        {
                            ex.Header = clientData.Header;
                            throw;
                        }
                    }
                }
            }
        }

        protected bool EqualsHash(byte[] val1, byte[] val2)
        {
            if (val1.Length == val2.Length)
            {
                int i = 0;
                while ((i < val1.Length) && (val1[i] == val2[i]))
                {
                    i += 1;
                }
                if (i == val1.Length)
                {
                    return true;
                }
            }

            return false;
        }

        protected uint GetAtomicCodeAndIncrement(int splitCount)
        {
            _Semaphore.Wait();
            var at = atomicCode;
            atomicCode += splitCount;
            _Semaphore.Release();
            return (uint)at;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (!Socket.IsOpen)
            {
                return;
            }

            try
            {
                StopApplicationData = true;
                SendShareClientData(new ShareClientData(ShareClientHeader.CreateClose()));
            }
            catch
            {
            }
            finally
            {
                Socket.Dispose();
                ShareClientClosed?.Invoke(this, new EventArgs());
            }
        }
    }
}
