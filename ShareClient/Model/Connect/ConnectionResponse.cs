using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareClient.Model.Connect
{
    public class ConnectionResponse : IClientData
    {
        public int Size => ConnectionData.Size + 1;
        public int Version => 1;
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
            var connection = ConnectionData.FromByte(bytes.AsSpan()[1..].ToArray());
            return connection != null ? new ConnectionResponse(bytes[0] == 1, connection) : null;
        }
    }
}
