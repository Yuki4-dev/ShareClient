using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareClient.Model;

namespace ShareClient.Component
{
    public class ShareClientReciver : IShareClientReciver
    {
        private readonly LinkedList<ISplitConnect> _SplitBuffer = new LinkedList<ISplitConnect>();
        private readonly IReciveDataProvider _ReciveDataProvider;

        public event EventHandler ShareClientClosed;
        public IClientSocket Socket { get; }
        public IClientManeger ClientManager { get; }
        public ClientStatus Status => Socket.Status;

        public ShareClientReciver(IClientManeger clientManeger, IClientSocket socket, IReciveDataProvider provider)
        {
            ClientManager = clientManeger;
            Socket = socket;
            _ReciveDataProvider = provider;
        }

        public async Task ReciveAsync()
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
                    await ReciveData();
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

        private async Task ReciveData()
        {
            var reciveData = await Socket.ReciveAsync();
            if (reciveData != null && reciveData.Length > 0)
            {
                var clientData = ShareClientData.FromBytes(reciveData);
                if (clientData != null)
                {
                    ClientManager.SetDataSize(clientData.Size);
                    AnalyzeReciveData(clientData);
                }
            }
        }

        private void AnalyzeReciveData(ShareClientData reciveData)
        {
            if (reciveData.Header.DataType == SendDataType.Close)
            {
                Close();
            }
            else if (reciveData.Header.DataType == SendDataType.Application)
            {
                try
                {
                    if (reciveData.Header.SplitCount == 1)
                    {
                        AddReceiveData(reciveData.DataPart);
                    }
                    else
                    {
                        ConnectReciveData(reciveData);
                    }
                }
                catch (Exception ex)
                {
                    throw new ReceiveDataAnalyzeException(reciveData, "Analyze ReciveData Fail", ex);
                }
            }
        }

        private void ConnectReciveData(ShareClientData reciveData)
        {
            var buffer = _SplitBuffer.First;
            while (buffer != null)
            {
                var connect = buffer.Value;
                if (connect.AddMember(reciveData))
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

            _SplitBuffer.AddLast(SplitConnectFactory.Create(reciveData));
            if (_SplitBuffer.Count > ClientManager.ClientSpec.SplitBufferSize)
            {
                _SplitBuffer.RemoveFirst();
            }
        }

        private void AddReceiveData(byte[] data)
        {
           if( _ReciveDataProvider.CanReceive)
            {
                _ReciveDataProvider.Add(data);
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

