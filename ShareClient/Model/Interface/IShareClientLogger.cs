using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
