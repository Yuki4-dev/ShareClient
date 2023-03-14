using ShareClient.Model.ShareClient;
using System;
using System.Linq;

namespace ShareClient.Component.Algorithm.Internal
{
    internal interface IConnectAlgorithm
    {
        public bool IsComplete { get; }
        public bool AddMember(ShareClientData connectData);
        public byte[] GetConnectData();
    }

    internal class InternalConnectAlgorithm
    {
        public static IConnectAlgorithm Create(ShareClientData baseData)
        {
            return new ConnectAlgorithm(baseData);
        }

        private class ConnectAlgorithm : IConnectAlgorithm
        {
            private int count = 1;
            private readonly long maxCode;
            private readonly long minCode;
            private readonly int spritCount;
            private readonly ShareClientData[] buffer;

            public bool IsComplete => spritCount == count;

            public ConnectAlgorithm(ShareClientData baseData)
            {
                spritCount = baseData.Header.SplitCount;
                buffer = new ShareClientData[spritCount];
                buffer[baseData.Header.SplitIndex] = baseData;
                minCode = baseData.Header.AtomicCode - baseData.Header.SplitIndex;
                maxCode = baseData.Header.AtomicCode + (spritCount - 1 - baseData.Header.SplitIndex);
            }

            public bool AddMember(ShareClientData value)
            {
                if (maxCode < value.Header.AtomicCode || minCode >= value.Header.AtomicCode || IsComplete)
                {
                    return false;
                }

                buffer[value.Header.SplitIndex] = value;
                count++;
                return true;
            }

            public byte[] GetConnectData()
            {
                if (!IsComplete)
                {
                    return null;
                }

                var connectData = new byte[buffer.Sum(b => b.Header.DataPartSize)];
                uint destSize = 0;
                for (int i = 0; i < spritCount; i++)
                {
                    var clientData = buffer[i];
                    Array.Copy(clientData.DataPart, 0, connectData, destSize, clientData.Header.DataPartSize);
                    destSize += clientData.Header.DataPartSize;
                }
                return connectData;
            }
        }
    }
}

