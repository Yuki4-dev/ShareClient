namespace ShareClient.Component
{
    public interface IShareClientSender : IShareClient
    {
        public void Send(byte[] data);
    }
}
