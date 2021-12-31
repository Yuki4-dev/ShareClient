using ShareClient.Model;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace ShareClient.Component
{
    public class ShareClientSender : IShareClient
    {
        private int atomicCode = 0;
        private byte[] oldHash = Array.Empty<byte>();
        private readonly object _LockObj = new();
        private readonly HashAlgorithm _HashAlgorithm = new MD5CryptoServiceProvider();
        protected bool StopApplicationData { get; set; } = false;

        public event EventHandler ShareClientClosed;

        public IClientSocket Socket { get; }
        public IClientManager ClientManager { get; }

        public ShareClientSender(IClientManager maneger, IClientSocket client)
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
                var ex = new ArgumentOutOfRangeException($"SplitCount : {splitCount}");
                ClientManager.Logger.Error($"Over SplitCount : {byte.MaxValue}", ex);
                throw ex;
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

            SendShareClientData(new(header, sendData));
        }

        protected void SendShareClientData(ShareClientData clientData)
        {
            if (StopApplicationData && clientData.Header.DataType == SendDataType.Application)
            {
                ClientManager.Logger.Info($"Stop Application or ShareClientData Convert Fail or Type {clientData.Header.DataType}");
                return;
            }

            var sendData = clientData.ToByte();
            if (!ClientManager.PreSendDataSize(sendData.Length))
            {
                ClientManager.Logger.Info($"Dont't Allow  Size of Send  Byte  : {sendData.Length}");
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
                catch (Exception ex)
                {
                    ClientManager.Logger.Error($"Sokect Send Throw Exception, Socket IsOpen : {Socket.IsOpen}", ex);
                    if (Socket.IsOpen)
                    {
                        ClientManager.Logger.Error($"Exception Throw Count : {count + 1 }, RetryCount: {ClientManager.RetryCount}", ex);
                        if (++count > ClientManager.RetryCount || ClientManager.HandleException(ex))
                        {
                            var se = new ShareClientException(clientData.Header, ex.Message, ex);
                            ClientManager.Logger.Error($"Throw Exception.", se);
                            throw se;
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
            lock (_LockObj)
            {
                var at = atomicCode;
                atomicCode += splitCount;
                return (uint)at;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (!Socket.IsOpen)
            {
                ClientManager.Logger.Info("Socket is Not Open.");
                return;
            }

            try
            {
                StopApplicationData = true;
                SendShareClientData(new(ShareClientHeader.CreateClose()));
            }
            catch (Exception ex)
            {
                ClientManager.Logger.Error("Fail Close.", ex);
            }
            finally
            {
                Socket.Dispose();
                ShareClientClosed?.Invoke(this, new());
                ClientManager.Logger.Info("Sender Socket Close.");
            }
        }
    }
}
