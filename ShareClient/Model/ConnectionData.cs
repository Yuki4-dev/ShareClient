using System;

namespace ShareClient.Model
{
    public class ConnectionData : IClientData
    {
        public int Size => CleintSpec.Size + MetaData.Length;
        public int Version => 0;
        public ShareClientSpec CleintSpec { get; }
        public byte[] MetaData { get; }

        public ConnectionData(ShareClientSpec spec):this(spec, new byte[0])
        {
        }

        public ConnectionData(ShareClientSpec spec, byte[] meta)
        {
            CleintSpec = spec;
            MetaData = meta;
        }

        public byte[] ToByte()
        {
            var byteData = new byte[Size];
            Array.Copy(CleintSpec.ToByte(), 0, byteData, 0, CleintSpec.Size);
            Array.Copy(MetaData, 0, byteData, CleintSpec.Size, MetaData.Length);
            return byteData;
        }

        public static ConnectionData FromByte(byte[] bytes)
        {
            var spec = ShareClientSpec.FromByte(bytes);
            if (spec == null)
            {
                return null;
            }
            return new ConnectionData(spec, bytes.AsSpan(spec.Size).ToArray());
        }
    }
}
