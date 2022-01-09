using System;

namespace ShareClient.Component.Algorithm
{
    public interface IShareAlgorithm : IDisposable
    {
        public event EventHandler ShareAlgorithmClosed;
        public void Close();
    }
}
