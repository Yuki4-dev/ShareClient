namespace ShareClient.Component
{
    public interface IReceiveDataProvider
    {
        public bool CanReceive { get; }
        public void Receive(byte[] data);
    }
}
