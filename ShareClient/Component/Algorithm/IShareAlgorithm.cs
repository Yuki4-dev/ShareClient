using System;

namespace ShareClient.Component.Algorithm
{
    public interface IShareAlgorithm : IDisposable
    {
        public bool IsClosed { get; }
        public event EventHandler ShareAlgorithmClosed;
    }
}
