using ShareClient.Component;
using ShareClient.Component.Connect;
using ShareClient.Component.Core;
using ShareClient.Component.ShareClient;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace SharedClientForm.Component
{
    public class DisplayImageSender : IDisposable
    {
        private bool isDispose = false;
        private readonly DisplayImageCaputure _Caputure;
        private readonly System.Timers.Timer _SenderTimer = new();

        public ImageFormat Format { get; set; } = ImageFormat.Jpeg;
        public ShareClientSender Sender { get; }
        public CollectionDataShareClientManager ClientManager { get; }

        public DisplayImageSender(Connection connection, DisplayImageCaputure caputure, int interval)
        {
            ClientManager = new CollectionDataShareClientManager(connection.ClientSpec);

            var socket = ShareClientSocket.Udp;
            socket.Open(connection.LocalEndPoint, connection.RemoteEndPoint);
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
                    if (!isDispose)
                    {
                        _SenderTimer.Start();
                    }
                }
            }
        }

        public void Start()
        {
            _SenderTimer.Start();
        }

        public void Dispose()
        {
            isDispose = true;
            _SenderTimer.Dispose();
            Sender.Dispose();
        }
    }
}
