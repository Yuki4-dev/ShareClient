using System;
using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IShareClientReceiver : IShareClient
    {
        public Task ReceiveAsync();
    }
}
