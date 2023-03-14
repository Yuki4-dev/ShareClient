using System;
using System.Threading.Tasks;

namespace ShareClient.Component.Algorithm
{
    public interface IReceiveAlgorithm : IShareAlgorithm
    {
        Task ReceiveAsync(Action<byte[]> receiver);
    }
}
