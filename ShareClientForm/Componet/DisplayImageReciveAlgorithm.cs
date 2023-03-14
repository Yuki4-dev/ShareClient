using ShareClient.Component.Algorithm;
using ShareClient.Component.Connect;
using ShareClientForm.Controls;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Timers;

namespace ShareClientForm.Componet
{
    public class DisplayImageReceiveAlgorithm : IDisposable
    {
        private readonly Timer _RecoverTimer = new();
        private readonly ReceiveImageProvider _ReceiveImageProvider = new();
        private readonly IPictureArea _PictureArea;
        private readonly IReceiveAlgorithm _Receiver;
        private readonly Action _Closing;

        public bool IsDisposed { get; private set; } = false;

        public DisplayImageReceiveAlgorithm(Connection connection,
                                           IShareAlgorithmManager manager,
                                           int interval,
                                           IPictureArea area,
                                           Action closed)
        {
            _Receiver = ShareAlgorithmBuilder.NewBuilder()
                                           .SetShareClientSpec(connection.ClientSpec)
                                           .SetShareAlgorithmManager(manager)
                                           .SetConnectEndoPoint(connection.RemoteEndPoint)
                                           .BuildReceive(connection.LocalEndPoint);
            _Receiver.ShareAlgorithmClosed += Receiver_ShareAlgorithmClosed;

            _PictureArea = area;
            _Closing = closed;
            _RecoverTimer.Interval = interval;
            _RecoverTimer.Elapsed += PaintPicture;
        }

        private void Receiver_ShareAlgorithmClosed(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                _RecoverTimer.Dispose();
                _ReceiveImageProvider.Dispose();
            }
            _Closing?.Invoke();
        }

        private void PaintPicture(object sender, EventArgs e)
        {
            var bmp = _ReceiveImageProvider.GetImage();
            if (bmp != null)
            {
                _PictureArea.PaintPicture(bmp);
            }
        }

        public void Start()
        {
            _ = _Receiver.ReceiveAsync(_ReceiveImageProvider.Receive);
            _RecoverTimer.Start();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _RecoverTimer.Dispose();
            _ReceiveImageProvider.Dispose();
            _Receiver.Dispose();
        }
    }

    public class ReceiveImageProvider
    {
        public int Capacity { get; set; } = 100;

        private readonly ConcurrentQueue<Image> imgQueue = new();

        public ReceiveImageProvider() { }

        public Image GetImage()
        {
            if (imgQueue.IsEmpty)
            {
                return null;
            }

            _ = imgQueue.TryDequeue(out var img);
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
