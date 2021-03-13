namespace ShareClient.Component
{
    public interface IReciveDataProvider
    {
        public bool CanReceive { get; }
        public void Receive(byte[] data);
    }
}
