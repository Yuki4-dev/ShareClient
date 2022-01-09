using System;
using System.Net;

namespace ShareClient.Model
{
    public interface IShareClientLogger
    {
        public void Send(EndPoint iPEndPoint, byte[] sendData);
        public void Receive(EndPoint iPEndPoint, byte[] receiveData);
        public void Info(string message);
        public void Error(string message, Exception exception);
    }
}
