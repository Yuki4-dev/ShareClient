namespace ShareClient.Component.Algorithm
{
    public interface ISendAlgorithm : IShareAlgorithm
    {
        void Send(byte[] data);
    }
}
