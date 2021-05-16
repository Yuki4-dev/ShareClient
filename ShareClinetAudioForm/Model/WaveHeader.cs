using System;
using System.Collections.Generic;
using System.Text;

namespace ShareClinetAudioForm
{
    public class WaveHeader
    {
        public const int HeaderSize = 44; 

        public int FileSize { get; set; }
        public int FormatChunkSize { get; set; }
        public short FormatID { get; set; }
        public short Channel { get; set; }
        public int SampleRate { get; set; }
        public int BytePerSec { get; set; }
        public short BlockSize { get; set; }
        public short BitPerSample { get; set; }
        public int DataChunkSize { get; set; }

        public byte[] ToBytes()
        {
            byte[] headerBytes = new byte[44];

            Array.Copy(Encoding.ASCII.GetBytes("RIFF"), 0, headerBytes, 0, 4);
            Array.Copy(BitConverter.GetBytes((uint)(FileSize - 8)), 0, headerBytes, 4, 4);
            Array.Copy(Encoding.ASCII.GetBytes("WAVE"), 0, headerBytes, 8, 4);
            Array.Copy(Encoding.ASCII.GetBytes("fmt "), 0, headerBytes, 12, 4);
            Array.Copy(BitConverter.GetBytes((uint)(FormatChunkSize)), 0, headerBytes, 16, 4);
            Array.Copy(BitConverter.GetBytes((ushort)(FormatID)), 0, headerBytes, 20, 2);
            Array.Copy(BitConverter.GetBytes((ushort)(Channel)), 0, headerBytes, 22, 2);
            Array.Copy(BitConverter.GetBytes((uint)(SampleRate)), 0, headerBytes, 24, 4);
            Array.Copy(BitConverter.GetBytes((uint)(BytePerSec)), 0, headerBytes, 28, 4);
            Array.Copy(BitConverter.GetBytes((ushort)(BlockSize)), 0, headerBytes, 32, 2);
            Array.Copy(BitConverter.GetBytes((ushort)(BitPerSample)), 0, headerBytes, 34, 2);
            Array.Copy(Encoding.ASCII.GetBytes("data"), 0, headerBytes, 36, 4);
            Array.Copy(BitConverter.GetBytes((uint)(DataChunkSize)), 0, headerBytes, 40, 4);

            return headerBytes;
        }
    }
}

