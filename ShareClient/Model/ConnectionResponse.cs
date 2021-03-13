using System;
using System.Collections.Generic;
using System.Text;

namespace ShareClient.Model
{
    public class ConnectionResponse : IClientData
    {
        public int Size => ConnectionData.Size + 1;
        public int Version => 0;
        public bool IsConnect { get; }
        public ConnectionData ConnectionData { get; }

        public ConnectionResponse(bool result, ConnectionData connectionData)
        {
            IsConnect = result;
            ConnectionData = connectionData;
        }

        public byte[] ToByte()
        {
            var byteData = new byte[Size];
            byteData[0] = IsConnect ? (byte)1 : (byte)0;
            Array.Copy(ConnectionData.ToByte(), 0, byteData, 1, ConnectionData.Size);
            return byteData;
        }

        public static ConnectionResponse FromByte(byte[] bytes)
        {
            var connection = ConnectionData.FromByte(bytes.AsSpan().Slice(1).ToArray());
            if (connection == null)
            {
                return null;
            }
            return new ConnectionResponse(bytes[0] == 1, connection);
        }
    }
}
