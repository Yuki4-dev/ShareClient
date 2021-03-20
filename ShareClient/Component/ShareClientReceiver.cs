using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareClient.Model;

namespace ShareClient.Component
{
    public class ShareClientReceiver : IShareClientReceiver
    {
        private readonly LinkedList<ISplitConnect> _SplitBuffer = new LinkedList<ISplitConnect>();
        private readonly IReceiveDataProvider _ReceiveDataProvider;

        public event EventHandler ShareClientClosed;
        public event EventHandler<SystemDataRecieveEventArgs> SystemDataRecieved;

        public IClientSocket Socket { get; }
        public IClientManeger ClientManager { get; }
        public ClientStatus Status => Socket.Status;

        public ShareClientReceiver(IClientManeger clientManeger, IClientSocket socket, IReceiveDataProvider provider)
        {
            ClientManager = clientManeger;
            Socket = socket;
            _ReceiveDataProvider = provider;
        }

        public async Task ReceiveAsync()
        {
            if (Status != ClientStatus.Open)
            {
                throw new InvalidOperationException($"Client : {Status}");
            }

            int count = 0;
            while (Status == ClientStatus.Open)
            {
                try
                {
                    await ReceiveData();
                }
                catch (ShareClientException ex)
                {
                    if (Status == ClientStatus.Open)
                    {
                        if (++count > ClientManager.RetryCount || ClientManager.HandleException(ex))
                        {
                            throw ex;
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
                    ClientManager.SetDataSize(clientData.Size);
                    AnalyzeReceiveData(clientData);
                }
            }
        }

        private void AnalyzeReceiveData(ShareClientData receiveData)
        {
            if (receiveData.Header.DataType == SendDataType.Close)
            {
                Close();
            }
            else if (receiveData.Header.DataType == SendDataType.System)
            {
                SystemDataRecieved?.Invoke(this, new SystemDataRecieveEventArgs(receiveData.DataPart));
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
                    throw new ReceiveDataAnalyzeException(receiveData, "Analyze ReceiveData Fail", ex);
                }
            }
        }

        private void ConnectReceiveData(ShareClientData receiveData)
        {

            foreach (var connect in _SplitBuffer)
            {
                if (connect.AddMember(receiveData))
                {
                    if (connect.IsComplete)
                    {
                        AddReceiveData(connect.GetConnectData());
                        _SplitBuffer.Remove(connect);
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
            if (_ReceiveDataProvider.CanReceive)
            {
                _ReceiveDataProvider.Receive(data);
            }
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

            Socket.Dispose();
            _SplitBuffer.Clear();
            ShareClientClosed?.Invoke(this, new EventArgs());
        }
    }
}

