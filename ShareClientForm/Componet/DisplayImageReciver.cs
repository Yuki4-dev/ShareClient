using ShareClient.Component;
using ShareClient.Component.Algorithm;
using ShareClient.Component.Connect;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Timers;

namespace SharedClientForm.Component
{
    public class DisplayImageReciver : IDisposable
    {
        private readonly Timer _ReciverTimer = new Timer();
        private readonly ReciveImageProvider _ReciveImageProvider = new ReciveImageProvider();
        private readonly IPictureArea _PictureArea;

        public CollectionDataShareAlgorithmManager ClientManager { get; } = new CollectionDataShareAlgorithmManager();
        public IRecieveAlgorithm Reciver { get; }

        public DisplayImageReciver(Connection connection, int interval, IPictureArea area)
        {
            Reciver = ShareAlgorithmBuilder.NewBuilder()
                                           .SetShareClientSpec(connection.ClientSpec)
                                           .SetShareAlgorithmManager(ClientManager)
                                           .SetConnectEndoPoint(connection.RemoteEndPoint)
                                           .BuildRecieve(connection.LocalEndPoint);
            _PictureArea = area;
            _ReciverTimer.Interval = interval;
            _ReciverTimer.Elapsed += PaintPicture;
        }

        private void PaintPicture(object sender, EventArgs e)
        {
            var bmp = _ReciveImageProvider.GetImage();
            if (bmp != null)
            {
                _PictureArea.PaintPicture(bmp);
            }
        }

        public void Start()
        {
            Reciver.RecieveAsync(_ReciveImageProvider.Receive);
            _ReciverTimer.Start();
        }

        public void Dispose()
        {
            _ReciverTimer.Dispose();
            Reciver.Dispose();
            _ReciveImageProvider.Dispose();
        }
    }

    public class ReciveImageProvider
    {
        public int Capacity { get; set; } = 100;

        private readonly ConcurrentQueue<Image> imgQueue = new();

        public ReciveImageProvider() { }

        public Image GetImage()
        {
            if (imgQueue.IsEmpty)
            {
                return null;
            }

            imgQueue.TryDequeue(out var img);
            return img;
        }

        public void Dispose()
        {
            if (!imgQueue.IsEmpty)
            {
                foreach (var img in imgQueue)
                {
                    img.Dispose();
                }
                imgQueue.Clear();
            }
        }

        public void Receive(byte[] data)
        {
            var img = Image.FromStream(new MemoryStream(data));
            imgQueue.Enqueue(img);
        }
    }
}
