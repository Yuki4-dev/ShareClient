﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareClient.Model;

namespace ShareClient.Component
{
    public class ShareClientReceiver : IShareClient
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
                    throw new ReceiveDataAnalyzeException(receiveData, "Analyze ReceiveData Fail", ex);
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

