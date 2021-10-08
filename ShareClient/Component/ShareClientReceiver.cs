using ShareClient.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public class ShareClientReceiver : IShareClient
    {
        private readonly LinkedList<ISplitConnect> _SplitBuffer = new();
        private readonly IReceiveDataProvider _ReceiveDataProvider;

        public event EventHandler ShareClientClosed;

        public IClientSocket Socket { get; }
        public IClientManager ClientManager { get; }

        public ShareClientReceiver(IClientManager clientManeger, IClientSocket socket, IReceiveDataProvider provider)
        {
            ClientManager = clientManeger;
            Socket = socket;
            _ReceiveDataProvider = provider;
        }

        public async Task ReceiveAsync()
        {
            int count = 0;
            while (Socket.IsOpen)
            {
                try
                {
                    await ReceiveData();
                }
                catch (ShareClientException ex)
                {
                    ClientManager.Logger.Error($"Sokect Receive Throw Exception, Socket IsOpen : {Socket.IsOpen}", ex);
                    if (Socket.IsOpen)
                    {
                        ClientManager.Logger.Error($"Exception Throw Count : {count + 1 }, RetryCount: {ClientManager.RetryCount}", null);
                        if (++count > ClientManager.RetryCount || ClientManager.HandleException(ex))
                        {
                            ClientManager.Logger.Error($"Throw Exception.", ex);
                            throw;
                        }
                    }
                }
            }
        }

        private async Task ReceiveData()
        {
            var ReceiveData = await Socket.ReceiveAsync();
            if (ReceiveData != null && ReceiveData.Length > 0)
            {
                var clientData = ShareClientData.FromBytes(ReceiveData);
                if (clientData != null)
                {
                    ClientManager.SetRecieveDataSize(clientData.Size);
                    AnalyzeReceiveData(clientData);
                }
            }
        }

        private void AnalyzeReceiveData(ShareClientData receiveData)
        {
            if (receiveData.Header.DataType == SendDataType.Close)
            {
                ClientManager.Logger.Info("Receive Request Close.");
                Close();
            }
            else if (receiveData.Header.DataType == SendDataType.System)
            {
                RecieveSystemData(receiveData);
            }
            else
            {
                try
                {
                    if (receiveData.Header.SplitCount == 1)
                    {
                        AddReceiveData(receiveData.DataPart);
                    }
                    else
                    {
                        ConnectReceiveData(receiveData);
                    }
                }
                catch (Exception ex)
                {
                    var re = new ReceiveDataAnalyzeException(receiveData, "Fail Analyze ReceiveData.", ex);
                    ClientManager.Logger.Error(re.Message, re);
                    throw re;
                }
            }
        }

        protected virtual void RecieveSystemData(ShareClientData receiveData)
        {
            //
        }

        private void ConnectReceiveData(ShareClientData receiveData)
        {
            for (var node = _SplitBuffer.First; node != null; node = node.Next)
            {
                var connect = node.Value;
                if (connect.AddMember(receiveData))
                {
                    if (connect.IsComplete)
                    {
                        AddReceiveData(connect.GetConnectData());
                        _SplitBuffer.Remove(node);
                    }
                    return;
                }
            }

            _SplitBuffer.AddLast(SplitConnectFactory.Create(receiveData));
            if (_SplitBuffer.Count > ClientManager.ClientSpec.SplitBufferSize)
            {
                _SplitBuffer.RemoveFirst();
            }
        }

        private void AddReceiveData(byte[] data)
        {
            _ReceiveDataProvider.Receive(data);
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

            Socket.Dispose();
            _SplitBuffer.Clear();
            ShareClientClosed?.Invoke(this, new());
            ClientManager.Logger.Info("Receiver Socket Close.");
        }
    }
}

