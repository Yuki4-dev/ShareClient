using ShareClient.Model;
using System;
using System.Diagnostics;
using System.Net;

namespace ShareClient.Component.ShareClient.Internal
{
    internal class DebugLogger : IShareClientLogger
    {
        public void Error(string message, Exception exception)
        {
            Debug.WriteLine(message);
        }

        public void Info(string message)
        {
            Debug.WriteLine(message);
        }

        public void Receive(EndPoint iPEndPoint, byte[] receiveData)
        {
            Debug.WriteLine($"Receive -> {iPEndPoint.ToString()}");
        }

        public void Send(EndPoint iPEndPoint, byte[] sendData)
        {
            Debug.WriteLine($"Send -> {iPEndPoint.ToString()}");
        }
    }
}
