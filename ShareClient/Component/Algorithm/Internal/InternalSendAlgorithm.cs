using ShareClient.Component.Core;
using ShareClient.Model.ShareClient;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace ShareClient.Component.Algorithm
{
    internal class InternalSendAlgorithm : ISendAlgorithm
    {
        private readonly object _LockObj = new object();
        private readonly HashAlgorithm _HashAlgorithm = new MD5CryptoServiceProvider();

        private int atomicCode = 0;
        private byte[] oldHash = Array.Empty<byte>();
        private bool stopApplicationData = false;

        private readonly ShareClientSpec _ClientSpec;
        private readonly IShareAlgorithmManager _Manager;
        private readonly IShareClientSocket _Socket;

        public event EventHandler ShareAlgorithmClosed;

        public InternalSendAlgorithm(ShareClientSpec clientSpec, IShareAlgorithmManager maneger, IShareClientSocket socket)
        {
            _ClientSpec = clientSpec;
            _Manager = maneger;
            _Socket = socket;
        }

        public void Send(byte[] data)
        {
            if (!_ClientSpec.SendSameData)
            {
                var hash = _HashAlgorithm.ComputeHash(data);
                if (EqualsHash(hash, oldHash))
                {
                    return;
                }
                oldHash = hash;
            }

            SendData(data);
            Thread.Sleep(_ClientSpec.SendDelay);
        }

        private void SendData(byte[] bytes)
        {
            int sendSize = _ClientSpec.BufferSize - ShareClientHeader.SIZE;
            int splitCount = (bytes.Length / sendSize) + 1;
            if (splitCount > byte.MaxValue)
            {
                var ex = new ArgumentOutOfRangeException($"SplitCount : {splitCount}");
                _Manager.Logger.Error($"Over SplitCount : {byte.MaxValue}", ex);
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
            if (stopApplicationData && clientData.Header.DataType == SendDataType.Application)
            {
                _Manager.Logger.Info($"Stop Application or ShareClientData Convert Fail or Type {clientData.Header.DataType}");
                return;
            }

            var sendData = clientData.ToByte();
            if (!_Manager.PreSendDataSize(sendData.Length))
            {
                _Manager.Logger.Info($"Dont't Allow  Size of Send  Byte  : {sendData.Length}");
                return;
            }

            int count = 0;
            while (_Socket.IsOpen)
            {
                try
                {
                    _Socket.Send(sendData);
                    Thread.Sleep(1);
                    break;
                }
                catch (Exception ex)
                {
                    _Manager.Logger.Error($"Sokect Send Throw Exception, Socket IsOpen : {_Socket.IsOpen}", ex);
                    if (_Socket.IsOpen)
                    {
                        _Manager.Logger.Error($"Exception Throw Count : {count + 1 }, RetryCount: {_Manager.RetryCount}", ex);
                        if (++count > _Manager.RetryCount || _Manager.HandleException(ex))
                        {
                            var se = new ShareClientException(clientData.Header, ex.Message, ex);
                            _Manager.Logger.Error($"Throw Exception.", se);
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
            if (!_Socket.IsOpen)
            {
                _Manager.Logger.Info("Socket is Not Open.");
                return;
            }

            try
            {
                stopApplicationData = true;
                SendShareClientData(new ShareClientData(ShareClientHeader.CreateClose()));
            }
            catch (Exception ex)
            {
                _Manager.Logger.Error("Fail Close.", ex);
            }
            finally
            {
                _Socket.Dispose();
                ShareAlgorithmClosed?.Invoke(this, new());
                _Manager.Logger.Info("Sender Socket Close.");
            }
        }
    }
}
