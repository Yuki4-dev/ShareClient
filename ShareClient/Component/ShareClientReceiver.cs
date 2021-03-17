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

        private void AnalyzeReceiveData(ShareClientData ReceiveData)
        {
            if (ReceiveData.Header.DataType == SendDataType.Close)
            {
                Close();
            }
            else if (ReceiveData.Header.DataType == SendDataType.Application)
            {
                try
                {
                    if (ReceiveData.Header.SplitCount == 1)
                    {
                        AddReceiveData(ReceiveData.DataPart);
                    }
                    else
                    {
                        ConnectReceiveData(ReceiveData);
                    }
                }
                catch (Exception ex)
                {
                    throw new ReceiveDataAnalyzeException(ReceiveData, "Analyze ReceiveData Fail", ex);
                }
            }
        }

        private void ConnectReceiveData(ShareClientData ReceiveData)
        {
            var buffer = _SplitBuffer.First;
            while (buffer != null)
            {
                var connect = buffer.Value;
                if (connect.AddMember(ReceiveData))
                {
                    if (connect.IsComplete)
                    {
                        AddReceiveData(connect.GetConnectData());
                        _SplitBuffer.Remove(buffer);
                    }
                    return;
                }
                buffer = buffer.Next;
            }

            _SplitBuffer.AddLast(SplitConnectFactory.Create(ReceiveData));
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

