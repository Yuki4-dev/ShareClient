using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareClient.Component;
using ShareClient.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace ShareClientTest
{
    [TestClass]
    public class ShareClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var img = new Bitmap("test1.png");
            var manager = new MockClientManager();
            var socket = new MockClientSocket();
            var provider = new MockReceiveImageProvider();

            using var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            var sendByte = ms.GetBuffer();

            var sender = new ShareClientSender(manager, socket);
            sender.Send(sendByte);

            var Receiver = new ShareClientReceiver(manager, socket, provider);
            var task = Receiver.ReceiveAsync();

            while (!task.IsCompleted)
            {
                Thread.Yield();
            }

            var size1 = manager.SendDataSize[0];
            var size2 = manager.RecieveDataSize[0];
            Assert.AreEqual(size1, size2);
            Assert.AreEqual(sendByte.Length, provider.Data.Length);
            for (int i = 0; i < sendByte.Length; i++)
            {
                Assert.AreEqual(sendByte[i], provider.Data[i]);
            }
        }
    }

    [TestClass]
    public class ShareClientDataTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            uint dataSize = 7777;
            var header = ShareClientHeader.CreateApplication(1, 200, 0, dataSize);
            var dataPart = new byte[dataSize];
            Array.Fill(dataPart, (byte)1);

            var udpData1 = new ShareClientData(header, dataPart);
            var udpData2 = ShareClientData.FromBytes(udpData1.ToByte());

            var header1 = udpData1.Header;
            var header2 = udpData2.Header;

            Assert.AreEqual(header1.AtomicCode, header2.AtomicCode);
            Assert.AreEqual(header1.SplitCount, header2.SplitCount);
            Assert.AreEqual(header1.DataType, header2.DataType);
            Assert.AreEqual(header1.DataPartSize, header2.DataPartSize);

            var dataPart1 = udpData1.DataPart;
            var dataPart2 = udpData2.DataPart;

            Assert.AreEqual((uint)dataPart1.Length, dataSize);
            Assert.AreEqual((uint)dataPart2.Length, dataSize);

            for (int i = 0; i < dataSize; i++)
            {
                Assert.AreEqual(dataPart1[i], dataPart2[i]);
            }
        }
    }

    [TestClass]
    public class ConnectDataTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            uint dataSize = 10;
            var meta = new byte[dataSize];
            Array.Fill(meta, (byte)1);
            var connectData1 = new ConnectionData(new ShareClientSpec(), meta);
            var connectData2 = ConnectionData.FromByte(connectData1.ToByte());

            Assert.AreEqual(connectData1.Size, connectData2.Size);
            Assert.AreEqual(connectData1.Version, connectData2.Version);
            Assert.AreEqual(connectData1.CleintSpec.BufferSize, connectData2.CleintSpec.BufferSize);
            Assert.AreEqual(connectData1.CleintSpec.SendDelay, connectData2.CleintSpec.SendDelay);
            Assert.AreEqual(connectData1.CleintSpec.Size, connectData2.CleintSpec.Size);
            Assert.AreEqual(connectData1.CleintSpec.SplitBufferSize, connectData2.CleintSpec.SplitBufferSize);
            Assert.AreEqual(connectData1.CleintSpec.Version, connectData2.CleintSpec.Version);

            Assert.AreEqual((uint)connectData1.MetaData.Length, dataSize);
            Assert.AreEqual((uint)connectData1.MetaData.Length, dataSize);

            for (int i = 0; i < dataSize; i++)
            {
                Assert.AreEqual(connectData1.MetaData[i], connectData2.MetaData[i]);
            }
        }
    }
}
