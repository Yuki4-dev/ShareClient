using System;
using System.Drawing.Imaging;
using System.IO;
using ShareClient.Component;
using ShareClient.Model;

namespace SharedClientForm.Component
{
    public class DisplayImageSender : IDisposable
    {
        private readonly DisplayImageCaputure _Caputure;
        private readonly System.Timers.Timer _SenderTimer = new System.Timers.Timer();

        public ImageFormat Format { get; set; } = ImageFormat.Jpeg;
        public ShareClientSender Sender { get; }
        public ShareClientManager ClientManager { get; }

        public DisplayImageSender(Connection connection, DisplayImageCaputure caputure, int interval)
        {
            ClientManager = new ShareClientManager(connection.ClientSpec);

            var socket = ShareClientSocket.CreateUdpSocket();
            socket.Open(connection);
            Sender = new ShareClientSender(ClientManager, socket);

            _Caputure = caputure;
            _SenderTimer.Interval = interval;
            _SenderTimer.Elapsed += Send;
        }

        private void Send(object sender, EventArgs e)
        {
            if (_Caputure.TryGetWindowImage(out var sendImage))
            {
                _SenderTimer.Stop();
                try
                {
                    using var ms = new MemoryStream();
                    sendImage.Save(ms, Format);
                    Sender.Send(ms.GetBuffer());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sendImage.Dispose();
                    _SenderTimer.Start();
                }
            }
        }

        public void Start()
        {
            _SenderTimer.Start();
        }

        public void Dispose()
        {
            _SenderTimer.Dispose();
            Sender.Dispose();
        }
    }
}
