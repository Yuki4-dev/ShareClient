using System;

namespace ShareClient.Model.ShareClient
{
    public class ShareClientData : IClientData
    {
        public int Size => Header.Size + DataPart.Length;
        public int Version => 0;
        public ShareClientHeader Header { get; }
        public byte[] DataPart { get; }

        public ShareClientData(ShareClientHeader header) : this(header, new byte[0]) { }

        public ShareClientData(ShareClientHeader header, byte[] dataPart)
        {
            Header = header;
            DataPart = dataPart;
        }

        public byte[] ToByte()
        {
            var data = new byte[Size];
            Array.Copy(Header.ToByte(), data, Header.Size);
            Array.Copy(DataPart, 0, data, Header.Size, DataPart.Length);
            return data;
        }

        public static ShareClientData FromBytes(byte[] bytes)
        {
            var header = ShareClientHeader.FromByte(bytes);
            if (header == null)
            {
                return null;
            }

            var dataPart = bytes.AsSpan(header.Size).ToArray();
            if (header.DataPartSize != dataPart.Length)
            {
                return null;
            }

            return new ShareClientData(header, dataPart);
        }
    }
}
