namespace ShareClient.Model
{
    public interface ISplitConnect
    {
        public bool IsComplete { get; }
        public bool AddMember(ShareClientData connectData);
        public byte[] GetConnectData();
    }
}
