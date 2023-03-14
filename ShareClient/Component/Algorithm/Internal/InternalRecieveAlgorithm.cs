using ShareClient.Component.Core;
using ShareClient.Exceptions;
using ShareClient.Model.ShareClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareClient.Component.Algorithm.Internal
{
    internal class InternalReceiveAlgorithm : IReceiveAlgorithm
    {
        private readonly LinkedList<IConnectAlgorithm> _SplitBuffer = new();
        private readonly ShareClientSpec _ClientSpec;
        private readonly IShareAlgorithmManager _Manager;
        private readonly IShareClientSocket _Socket;

        public bool IsClosed { get; private set; } = false;

        public event EventHandler ShareAlgorithmClosed;

        public InternalReceiveAlgorithm(ShareClientSpec clientSpec, IShareAlgorithmManager manager, IShareClientSocket socket)
        {
            _ClientSpec = clientSpec;
            _Manager = manager;
            _Socket = socket;
        }

        public async Task ReceiveAsync(Action<byte[]> receiver)
        {
            CheckIfClosed();

            int count = 0;
            while (_Socket.IsOpen)
            {
                try
                {
                    await ReceiveData(receiver);
                }
                catch (Exception ex)
                {
                    _Manager.Logger.Error($"Socket Receive Throw Exception, Socket IsOpen : {_Socket.IsOpen}", ex);
                    if (_Socket.IsOpen)
                    {
                        _Manager.Logger.Error($"Exception Throw Count : {count + 1}, RetryCount: {_Manager.RetryCount}", ex);
                        if (++count > _Manager.RetryCount || _Manager.HandleException(ex))
                        {

                            var se = ex is ShareClientException ? (ShareClientException)ex : new ShareClientException(null, ex.Message, ex);
                            _Manager.Logger.Error($"Throw Exception.", se);
                            throw se;
                        }
                    }
                }
            }
        }

        private async Task ReceiveData(Action<byte[]> receiver)
        {
            var ReceiveData = await _Socket.ReceiveAsync();
            if (ReceiveData != null && ReceiveData.Length > 0)
            {
                var clientData = ShareClientData.FromBytes(ReceiveData);
                if (clientData != null)
                {
                    _Manager.SetReceiveDataSize(clientData.Size);
                    AnalyzeReceiveData(clientData, receiver);
                }
            }
        }

        private void AnalyzeReceiveData(ShareClientData receiveData, Action<byte[]> receiver)
        {
            if (receiveData.Header.DataType == SendDataType.Close)
            {
                _Manager.Logger.Info("Request Close.");
                Close();
            }
            else if (receiveData.Header.DataType == SendDataType.System)
            {
                _Manager.Logger.Info("Receive System Data.");
                ReceiveSystemData(receiveData);
            }
            else
            {
                try
                {
                    if (receiveData.Header.SplitCount == 1)
                    {
                        receiver.Invoke(receiveData.DataPart);
                    }
                    else
                    {
                        ConnectReceiveData(receiveData, receiver);
                    }
                }
                catch (Exception ex)
                {
                    var re = new ReceiveDataAnalyzeException(receiveData, "Fail Analyze ReceiveData.", ex);
                    _Manager.Logger.Error(re.Message, re);
                    throw re;
                }
            }
        }

        protected virtual void ReceiveSystemData(ShareClientData receiveData)
        {
            //
        }

        private void ConnectReceiveData(ShareClientData receiveData, Action<byte[]> receiver)
        {
            for (var node = _SplitBuffer.First; node != null; node = node.Next)
            {
                var connect = node.Value;
                if (connect.AddMember(receiveData))
                {
                    if (connect.IsComplete)
                    {
                        receiver.Invoke(connect.GetConnectData());
                        _SplitBuffer.Remove(node);
                    }
                    return;
                }
            }

            _ = _SplitBuffer.AddLast(InternalConnectAlgorithm.Create(receiveData));
            if (_SplitBuffer.Count > _ClientSpec.SplitBufferSize)
            {
                _SplitBuffer.RemoveFirst();
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (IsClosed)
            {
                _Manager.Logger.Info("Receive Algorithm Already Closed.");
                return;
            }

            IsClosed = true;
            _SplitBuffer.Clear();

            try
            {
                _Socket.Dispose();
                ShareAlgorithmClosed?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                _Manager.Logger.Info($"Close Fail : {ex.Message}");
            }

            _Manager.Logger.Info("Receive Algorithm Closed.");
        }

        private void CheckIfClosed()
        {
            if (IsClosed || !_Socket.IsOpen)
            {
                throw new ShareClientException(null, "Receive Algorithm is Closed.", null);
            }
        }
    }
}

