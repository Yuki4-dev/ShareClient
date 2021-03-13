using System;
using ShareClient.Component;
using ShareClient.Model;

namespace SharedClientForm.Component
{
    public class DisplayImageReciver : IDisposable
    {
        private readonly System.Timers.Timer reciverTimer = new System.Timers.Timer();
        private readonly ReciveImageProvider reciveImageProvider = new ReciveImageProvider();

        public ShareClientManager ClientManager { get; }
        public ShareClientReceiver Reciver { get; }
        public IPictureArea Area { get; }

        public DisplayImageReciver(Connection connection, int interval, IPictureArea area)
        {
            ClientManager = new ShareClientManager(connection.ClientSpec);

            var socket = ShareClientSocket.CreateUdpSocket();
            socket.Open(connection);
            Reciver = new ShareClientReceiver(ClientManager, socket, reciveImageProvider);

            Area = area;
            reciverTimer.Interval = interval;
            reciverTimer.Elapsed += PaintPicture;
        }

        private void PaintPicture(object sender, EventArgs e)
        {
            var bmp = reciveImageProvider.GetImage();
            if (bmp != null)
            {
                Area.PaintPicture(bmp);
            }
        }

        public void Start()
        {
            Reciver.ReceiveAsync();
            reciverTimer.Start();
        }

        public void Dispose()
        {
            reciverTimer.Dispose();
            Reciver.Dispose();
            reciveImageProvider.Dispose();
        }
    }
}
