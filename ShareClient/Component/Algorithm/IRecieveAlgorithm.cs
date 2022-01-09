using System;
using System.Threading.Tasks;

namespace ShareClient.Component.Algorithm
{
    public interface IRecieveAlgorithm : IShareAlgorithm
    {
        Task RecieveAsync(Action<byte[]> reciever);
    }
}
