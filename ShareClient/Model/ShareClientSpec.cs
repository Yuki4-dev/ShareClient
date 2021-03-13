using System;

namespace ShareClient.Model
{
    public class ShareClientSpec : IClientData
    {
        public const int SIZE = ImageCleintSpecVer1.SIZE;
        public int Size => SIZE;
        public int Version => ImageCleintSpecVer1.VERSION;
        public int BufferSize { get; set; } = 8192;
        public int SendDelay { get; set; } = 10;
        public int SplitBufferSize { get; set; } = 10;
        public bool SendSameData { get; set; } = false;

        public byte[] ToByte()
        {
            var byteData = new byte[SIZE];
            byteData[ImageCleintSpecVer1.VersionIndex] = (byte)Version;
            Array.Copy(BitConverter.GetBytes(BufferSize), 0, byteData, ImageCleintSpecVer1.BufferSizeIndex, ImageCleintSpecVer1.BufferSizeLength);
            Array.Copy(BitConverter.GetBytes(SendDelay), 0, byteData, ImageCleintSpecVer1.SendDelayIndex, ImageCleintSpecVer1.SendDelayLength);
            Array.Copy(BitConverter.GetBytes(SplitBufferSize), 0, byteData, ImageCleintSpecVer1.SplitBufferSizeIndex, ImageCleintSpecVer1.SplitBufferSizeLength);
            byteData[ImageCleintSpecVer1.SendSameImageIndex] = SendSameData ? (byte)1 : (byte)0;
            return byteData;
        }

        public static ShareClientSpec FromByte(byte[] bytes)
        {
            if (bytes.Length < SIZE)
            {
                return null;
            }
            else if (bytes[ImageCleintSpecVer1.VersionIndex] != ImageCleintSpecVer1.VERSION)
            {
                throw new VersionDifferentException(typeof(ShareClientSpec), ImageCleintSpecVer1.VERSION, bytes[ImageCleintSpecVer1.VersionIndex]);
            }

            var byteSpan = bytes.AsSpan();
            return new ShareClientSpec
            {
                BufferSize = BitConverter.ToInt32(byteSpan.Slice(ImageCleintSpecVer1.BufferSizeIndex, ImageCleintSpecVer1.BufferSizeLength)),
                SendDelay = BitConverter.ToInt32(byteSpan.Slice(ImageCleintSpecVer1.SendDelayIndex, ImageCleintSpecVer1.SendDelayLength)),
                SplitBufferSize = BitConverter.ToInt32(byteSpan.Slice(ImageCleintSpecVer1.SplitBufferSizeIndex, ImageCleintSpecVer1.SplitBufferSizeLength)),
                SendSameData = byteSpan[ImageCleintSpecVer1.SendSameImageIndex] == 1,
            };
        }

        private class ImageCleintSpecVer1
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
