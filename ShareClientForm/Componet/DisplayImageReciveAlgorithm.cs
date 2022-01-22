using ShareClient.Component.Algorithm;
using ShareClient.Component.Connect;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Timers;

namespace SharedClientForm.Component
{
    public class DisplayImageReciveAlgorithm : IDisposable
    {
        private readonly Timer _ReciverTimer = new Timer();
        private readonly ReciveImageProvider _ReciveImageProvider = new ReciveImageProvider();
        private readonly IPictureArea _PictureArea;
        private readonly IRecieveAlgorithm _Reciver;
        private readonly Action _Closing;

        public bool IsDisposed { get; private set; } = false;

        public DisplayImageReciveAlgorithm(Connection connection,
                                           IShareAlgorithmManager manager,
                                           int interval,
                                           IPictureArea area,
                                           Action closed)
        {
            _Reciver = ShareAlgorithmBuilder.NewBuilder()
                                           .SetShareClientSpec(connection.ClientSpec)
                                           .SetShareAlgorithmManager(manager)
                                           .SetConnectEndoPoint(connection.RemoteEndPoint)
                                           .BuildRecieve(connection.LocalEndPoint);
            _Reciver.ShareAlgorithmClosed += Reciver_ShareAlgorithmClosed;

            _PictureArea = area;
            _Closing = closed;
            _ReciverTimer.Interval = interval;
            _ReciverTimer.Elapsed += PaintPicture;
        }

        private void Reciver_ShareAlgorithmClosed(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                _ReciverTimer.Dispose();
                _ReciveImageProvider.Dispose();
            }
            _Closing?.Invoke();
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
            _Reciver.RecieveAsync(_ReciveImageProvider.Receive);
            _ReciverTimer.Start();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _ReciverTimer.Dispose();
            _ReciveImageProvider.Dispose();
            _Reciver.Dispose();
        }
    }

    public class ReciveImageProvider
    {
        public int Capacity { get; set; } = 100;

        private readonly ConcurrentQueue<Image> imgQueue = new ConcurrentQueue<Image>();

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
