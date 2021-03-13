using System;
using System.Drawing.Imaging;
using System.IO;
using ShareClient.Component;
using ShareClient.Model;

namespace SharedClientForm.Component
{
    public class DisplayImageSender : IDisposable
    {
        private readonly DisplayImageCaputure displayImageCaputure;
        private readonly System.Timers.Timer senderTimer = new System.Timers.Timer();

        public IShareClientSender Sender { get; }
        public ShareClientManager ClientManager { get; }

        public DisplayImageSender(Connection connection, DisplayImageCaputure caputure, int interval)
        {
            ClientManager = new ShareClientManager(connection.ClientSpec);

            var socket = ShareClientSocket.CreateUdpSocket();
            socket.Open(connection);
            Sender = new ShareClientSender(ClientManager, socket);

            displayImageCaputure = caputure;
            senderTimer.Interval = interval;
            senderTimer.Elapsed += Send;
        }

        private void Send(object sender, EventArgs e)
        {
            if (displayImageCaputure.TryGetWindowImage(out var sendImage))
            {
                senderTimer.Stop();
                try
                {
                    using var ms = new MemoryStream();
                    sendImage.Save(ms, ImageFormat.Jpeg);
                    Sender.Send(ms.GetBuffer());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sendImage.Dispose();
                    senderTimer.Start();
                }
            }
        }

        public void Start()
        {
            senderTimer.Start();
        }

        public void Dispose()
        {
            senderTimer.Dispose();
            Sender.Dispose();
        }
    }
}
