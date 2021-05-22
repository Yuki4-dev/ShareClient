using System;

namespace ShareClient.Model
{
    public class ShareClientHeader : IClientData
    {
        public const int SIZE = ImageClientHeaderVer1.HEADERSIZE;
        public int Size => SIZE;
        public int Version => ImageClientHeaderVer1.VERSION;
        public uint AtomicCode { get; }
        public byte SplitCount { get; }
        public byte SplitIndex { get; }
        public SendDataType DataType { get; }
        public uint DataPartSize { get; }

        private ShareClientHeader(uint atomicCode, byte splitCount, byte splitIndex, SendDataType dataType, uint dataSize)
        {
            AtomicCode = atomicCode;
            SplitCount = splitCount;
            SplitIndex = splitIndex;
            DataType = dataType;
            DataPartSize = dataSize;
        }

        public static ShareClientHeader CreateApplication(uint atomicCode, byte splitCount, byte splitIndex, uint dataSize)
        {
            return new ShareClientHeader(atomicCode, splitCount, splitIndex, SendDataType.Application, dataSize);
        }

        public static ShareClientHeader CreateSystem(uint dataPartLength)
        {
            return new ShareClientHeader(0, 0, 0, SendDataType.System, dataPartLength);
        }

        public static ShareClientHeader CreateClose()
        {
            return new ShareClientHeader(0, 0, 0, SendDataType.Close, 0);
        }

        public static ShareClientHeader FromByte(byte[] bytes)
        {
            if (!ImageClientHeaderVer1.CheckFormat(bytes))
            {
                return null;
            }
            else if (bytes[ImageClientHeaderVer1.VersionIndex] != ImageClientHeaderVer1.VERSION)
            {
                throw new VersionDifferentException(typeof(ShareClientHeader), ImageClientHeaderVer1.VERSION, bytes[ImageClientHeaderVer1.VersionIndex]);
            }

            var byteSpan = bytes.AsSpan();
            uint atomicCode = BitConverter.ToUInt32(byteSpan.Slice(ImageClientHeaderVer1.AtomicCodeIndex, ImageClientHeaderVer1.AtomicCodeLength));
            uint dataSize = BitConverter.ToUInt32(byteSpan.Slice(ImageClientHeaderVer1.DataPartSizeIndex, ImageClientHeaderVer1.DataPartSizeLength));
            return new ShareClientHeader(atomicCode, byteSpan[ImageClientHeaderVer1.SplitCountIndex], byteSpan[ImageClientHeaderVer1.SplitIndexIndex], (SendDataType)byteSpan[ImageClientHeaderVer1.DataTypeIndex], dataSize);
        }

        public byte[] ToByte()
        {
            var byteData = new byte[Size];
            Array.Copy(ImageClientHeaderVer1.SYNC_CODE, 0, byteData, ImageClientHeaderVer1.SyncCodeIndex, ImageClientHeaderVer1.SYNC_CODE.Length);
            Array.Copy(BitConverter.GetBytes(AtomicCode), 0, byteData, ImageClientHeaderVer1.AtomicCodeIndex, ImageClientHeaderVer1.AtomicCodeLength);
            byteData[ImageClientHeaderVer1.VersionIndex] = ImageClientHeaderVer1.VERSION;
            byteData[ImageClientHeaderVer1.SplitCountIndex] = SplitCount;
            byteData[ImageClientHeaderVer1.SplitIndexIndex] = SplitIndex;
            byteData[ImageClientHeaderVer1.DataTypeIndex] = (byte)DataType;
            Array.Copy(BitConverter.GetBytes(DataPartSize), 0, byteData, ImageClientHeaderVer1.DataPartSizeIndex, ImageClientHeaderVer1.DataPartSizeLength);
            return byteData;
        }

        private class ImageClientHeaderVer1
        {
            public static readonly byte[] SYNC_CODE = BitConverter.GetBytes(0xFFFFFF);
            public const byte VERSION = 1;
            public const int HEADERSIZE = 16;

            public const int SyncCodeIndex = 0;
            public const int AtomicCodeIndex = 4;
            public const int VersionIndex = 8;
            public const int SplitCountIndex = 9;
            public const int SplitIndexIndex = 10;
            public const int DataTypeIndex = 11;
            public const int DataPartSizeIndex = 12;

            public const int AtomicCodeLength = 4;
            public const int DataPartSizeLength = 4;

            public static bool CheckFormat(byte[] bytes)
            {
                if (bytes.Length < HEADERSIZE)
                {
                    return false;
                }

                var sync = bytes.AsSpan().Slice(SyncCodeIndex, SYNC_CODE.Length);
                for (int i = 0; i < SYNC_CODE.Length; i++)
                {
                    if (SYNC_CODE[i] != sync[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    public enum SendDataType
    {
        System, Close, Application
    }
}
