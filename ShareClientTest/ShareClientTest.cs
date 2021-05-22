using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareClient.Component;
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
            var manager = new MockClientManager();
            var socket = new MockClientSocket();
            var provider = new MockReceiveImageProvider();

            var img = new Bitmap("test1.png");
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
}
