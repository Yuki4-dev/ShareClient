﻿using ShareClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    internal class UdpClientSocket : ShareClientStatus, IClientSocket
    {
        private Connection connection;
        private UdpClient udpClient;

        public UdpClientSocket() { }

        public void Open(Connection connection)
        {
            this.connection = connection;
            try
            {
                udpClient = new UdpClient(connection.LocalEndPoint);
                udpClient.Connect(connection.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                throw new ShareClientSocketException("Open Failure : " + ex.Message, ex);
            }

            Open();
        }

        public async Task<byte[]> ReciveAsync()
        {
            CheckOpenAndThrow();
            return await Task.Factory.StartNew(Recive);
        }

        private byte[] Recive()
        {
            try
            {
                IPEndPoint reciveEp = null;
                var reciveData = udpClient.Receive(ref reciveEp);
                if (connection.RemoteEndPoint.Address.Equals(reciveEp.Address))
                {
                    return reciveData;
                }
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    Close();
                }
                else
                {
                    throw new ShareClientSocketException("Recive Failure : " + ex.Message, ex);
                }
            }

            return null;
        }

        public void Send(byte[] sendData)
        {
            CheckOpenAndThrow();
            try
            {
                udpClient.Send(sendData, sendData.Length);
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    Close();
                }
                else
                {
                    throw new ShareClientSocketException("Send Failure : " + ex.Message, ex);
                }
            }
        }

        private void CheckOpenAndThrow()
        {
            if (Status != ClientStatus.Open)
            {
                throw new InvalidOperationException($"Socket : {Status}");
            }
        }

        protected override void CloseClient()
        {
            try
            {
                udpClient?.Dispose();
            }
            catch
            {
            }
        }
    }
}
