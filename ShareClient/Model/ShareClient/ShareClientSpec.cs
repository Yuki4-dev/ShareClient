using ShareClient.Exceptions;
using System;

namespace ShareClient.Model.ShareClient
{
    public class ShareClientSpec : IClientData
    {
        public const int SIZE = ImageClientSpecVer1.SIZE;
        public int Size => SIZE;
        public int Version => ImageClientSpecVer1.VERSION;
        public int BufferSize { get; set; } = 8192;
        public int SendDelay { get; set; } = 10;
        public int SplitBufferSize { get; set; } = 10;
        public bool SendSameData { get; set; } = false;

        public byte[] ToByte()
        {
            var byteData = new byte[SIZE];
            byteData[ImageClientSpecVer1.VersionIndex] = (byte)Version;
            Array.Copy(BitConverter.GetBytes(BufferSize), 0, byteData, ImageClientSpecVer1.BufferSizeIndex, ImageClientSpecVer1.BufferSizeLength);
            Array.Copy(BitConverter.GetBytes(SendDelay), 0, byteData, ImageClientSpecVer1.SendDelayIndex, ImageClientSpecVer1.SendDelayLength);
            Array.Copy(BitConverter.GetBytes(SplitBufferSize), 0, byteData, ImageClientSpecVer1.SplitBufferSizeIndex, ImageClientSpecVer1.SplitBufferSizeLength);
            byteData[ImageClientSpecVer1.SendSameImageIndex] = SendSameData ? (byte)1 : (byte)0;
            return byteData;
        }

        public static ShareClientSpec FromByte(byte[] bytes)
        {
            if (bytes.Length < SIZE)
            {
                return null;
            }
            else if (bytes[ImageClientSpecVer1.VersionIndex] != ImageClientSpecVer1.VERSION)
            {
                throw new VersionDifferentException(typeof(ShareClientSpec), ImageClientSpecVer1.VERSION, bytes[ImageClientSpecVer1.VersionIndex]);
            }

            var byteSpan = bytes.AsSpan();
            return new ShareClientSpec
            {
                BufferSize = BitConverter.ToInt32(byteSpan.Slice(ImageClientSpecVer1.BufferSizeIndex, ImageClientSpecVer1.BufferSizeLength)),
                SendDelay = BitConverter.ToInt32(byteSpan.Slice(ImageClientSpecVer1.SendDelayIndex, ImageClientSpecVer1.SendDelayLength)),
                SplitBufferSize = BitConverter.ToInt32(byteSpan.Slice(ImageClientSpecVer1.SplitBufferSizeIndex, ImageClientSpecVer1.SplitBufferSizeLength)),
                SendSameData = byteSpan[ImageClientSpecVer1.SendSameImageIndex] == 1,
            };
        }

        private class ImageClientSpecVer1
        {
            public const byte SIZE = 14;
            public const byte VERSION = 1;

            public const int VersionIndex = 0;
            public const int BufferSizeIndex = 1;
            public const int SendDelayIndex = 5;
            public const int SplitBufferSizeIndex = 9;
            public const int SendSameImageIndex = 13;

            public const int BufferSizeLength = 4;
            public const int SendDelayLength = 4;
            public const int SplitBufferSizeLength = 4;
        }
    }
}
