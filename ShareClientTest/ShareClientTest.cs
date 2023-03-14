using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareClient.Component.Algorithm;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;

namespace ShareClientTest
{
    [TestClass]
    public class ShareClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var manager = new MockClientManager();
            var socket = new MockClientSocket();

            var img = new Bitmap("test1.png");
            using var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            var sendByte = ms.GetBuffer();

            var sender = ShareAlgorithmBuilder.NewBuilder()
                                              .SetShareAlgorithmManager(manager)
                                              .SetSocket(socket)
                                              .BuildSend(new IPEndPoint(0, 0));
            sender.Send(sendByte);

            var Receiver = ShareAlgorithmBuilder.NewBuilder()
                                                .SetShareAlgorithmManager(manager)
                                                .SetSocket(socket)
                                                .BuildReceive(new IPEndPoint(0, 0));

            byte[] recieveData = null;
            var task = Receiver.ReceiveAsync((data) => recieveData = data);

            while (!task.IsCompleted)
            {
                Thread.Yield();
            }

            var size1 = manager.SendDataSize[0];
            var size2 = manager.ReceiveDataSize[0];
            Assert.AreEqual(size1, size2);

            Assert.AreEqual(sendByte.Length, recieveData.Length);

            for (int i = 0; i < sendByte.Length; i++)
            {
                Assert.AreEqual(sendByte[i], recieveData[i]);
            }
        }
    }
}
