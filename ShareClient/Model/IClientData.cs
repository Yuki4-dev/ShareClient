namespace ShareClient.Model
{
    public interface IClientData
    {
        public int Size { get; }
        public int Version { get; }
        public byte[] ToByte();
    }
}
