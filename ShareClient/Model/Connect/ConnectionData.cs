using ShareClient.Model.ShareClient;
using System;

namespace ShareClient.Model.Connect
{
    public class ConnectionData : IClientData
    {
        public int Size => ClientSpec.Size + MetaData.Length;
        public int Version => 1;
        public ShareClientSpec ClientSpec { get; }
        public byte[] MetaData { get; }

        public ConnectionData(ShareClientSpec spec) : this(spec, Array.Empty<byte>()) { }

        public ConnectionData(ShareClientSpec spec, byte[] meta)
        {
            ClientSpec = spec;
            MetaData = meta;
        }

        public byte[] ToByte()
        {
            var byteData = new byte[Size];
            Array.Copy(ClientSpec.ToByte(), 0, byteData, 0, ClientSpec.Size);
            Array.Copy(MetaData, 0, byteData, ClientSpec.Size, MetaData.Length);
            return byteData;
        }

        public static ConnectionData FromByte(byte[] bytes)
        {
            var spec = ShareClientSpec.FromByte(bytes);
            return spec == null ? null : new ConnectionData(spec, bytes.AsSpan(spec.Size).ToArray());
        }
    }
}
