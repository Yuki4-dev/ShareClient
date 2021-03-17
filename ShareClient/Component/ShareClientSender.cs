using ShareClient.Model;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace ShareClient.Component
{
    public class ShareClientSender : IShareClientSender
    {
        private int atomicCode = 0;
        private bool stopSendData = false;
        private byte[] oldHash = new byte[0];
        private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        private readonly HashAlgorithm _HashAlgorithm = new MD5CryptoServiceProvider();

        public event EventHandler ShareClientClosed;
        public IClientSocket Socket { get; }
        public IClientManeger ClientManager { get; }
        public ClientStatus Status => Socket.Status;

        public ShareClientSender(IClientManeger maneger, IClientSocket client)
        {
            ClientManager = maneger;
            Socket = client;
        }

        public void Send(byte[] data)
        {
            if (Status != ClientStatus.Open)
            {
                throw new InvalidOperationException($"Client : {Status}");
            }

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
            if (stopSendData)
            {
                return;
            }

            var sendData = new byte[sendLength];
            Array.Copy(srcData, sendIndex, sendData, 0, sendData.Length);

            var header = new ShareClientHeader(atomicCode,
                (byte)splitLength,
                (byte)splitIndex,
                SendDataType.Application,
                (uint)sendData.Length);

            SendData(new ShareClientData(header, sendData));
        }

        private void SendData(ShareClientData clientData)
        {
            var sendData = clientData.ToByte();
            int count = 0;
            while (Status == ClientStatus.Open)
            {
                try
                {
                    Socket.Send(sendData);
                    ClientManager.SetDataSize(sendData.Length);
                    Thread.Sleep(1);
                    break;
                }
                catch (ShareClientSocketException ex)
                {
                    if (Status == ClientStatus.Open)
                    {
                        if (++count > ClientManager.RetryCount || ClientManager.HandleException(ex))
                        {
                            ex.Header = clientData.Header;
                            throw ex;
                        }
                    }
                }
            }
        }

        private bool EqualsHash(byte[] val1, byte[] val2)
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

        private uint GetAtomicCodeAndIncrement(int splitCount)
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
            if (Status != ClientStatus.Open)
            {
                return;
            }

            try
            {
                stopSendData = true;
                SendData(new ShareClientData(ShareClientHeader.Close()));
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
