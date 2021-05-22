using System;
using ShareClient.Component;
using ShareClient.Model;

namespace SharedClientForm.Component
{
    public class DisplayImageReciver : IDisposable
    {
        private readonly System.Timers.Timer _ReciverTimer = new();
        private readonly ReciveImageProvider _ReciveImageProvider = new();

        public ShareClientManager ClientManager { get; }
        public ShareClientReceiver Reciver { get; }
        public IPictureArea Area { get; }

        public DisplayImageReciver(Connection connection, int interval, IPictureArea area)
        {
            ClientManager = new ShareClientManager(connection.ClientSpec);

            var socket = ShareClientSocket.CreateUdpSocket();
            socket.Open(connection);
            Reciver = new ShareClientReceiver(ClientManager, socket, _ReciveImageProvider);

            Area = area;
            _ReciverTimer.Interval = interval;
            _ReciverTimer.Elapsed += PaintPicture;
        }

        private void PaintPicture(object sender, EventArgs e)
        {
            var bmp = _ReciveImageProvider.GetImage();
            if (bmp != null)
            {
                Area.PaintPicture(bmp);
            }
        }

        public void Start()
        {
            Reciver.ReceiveAsync();
            _ReciverTimer.Start();
        }

        public void Dispose()
        {
            _ReciverTimer.Dispose();
            Reciver.Dispose();
            _ReciveImageProvider.Dispose();
        }
    }
}
