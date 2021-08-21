using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShareClient.Model
{
    internal class DebugLogger : IShareClientLogger
    {
        public TextWriter TextWriter { get; set; } = Console.Out;

        public void Error(string message, Exception exception)
        {
#if DEBUG
            TextWriter.WriteLine(message);
#endif
        }

        public void Info(string message)
        {
#if DEBUG
            TextWriter.WriteLine(message);
#endif
        }

        public void Receive(EndPoint iPEndPoint, byte[] receiveData)
        {
#if DEBUG
            TextWriter.WriteLine($"Receive -> {iPEndPoint.ToString()}");
#endif
        }

        public void Send(EndPoint iPEndPoint, byte[] sendData)
        {
#if DEBUG
            TextWriter.WriteLine($"Send -> {iPEndPoint.ToString()}");
#endif
        }
    }
}
