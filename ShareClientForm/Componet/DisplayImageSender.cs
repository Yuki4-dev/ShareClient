using ShareClient.Component;
using ShareClient.Component.Algorithm;
using ShareClient.Component.Connect;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;

namespace SharedClientForm.Component
{
    public class DisplayImageSender : IDisposable
    {
        private bool isDispose = false;
        private readonly DisplayImageCaputure _Caputure;
        private readonly Timer _SenderTimer = new Timer();

        public ImageFormat Format { get; set; } = ImageFormat.Jpeg;
        public ISendAlgorithm Sender { get; }
        public CollectionDataShareAlgorithmManager ClientManager { get; } = new CollectionDataShareAlgorithmManager();

        public DisplayImageSender(Connection connection, DisplayImageCaputure caputure, int interval)
        {
            Sender = ShareAlgorithmBuilder.NewBuilder()
                                          .SetShareClientSpec(connection.ClientSpec)
                                          .SetShareAlgorithmManager(ClientManager)
                                          .SetLocalEndoPoint(connection.LocalEndPoint)
                                          .BuildSend(connection.RemoteEndPoint);
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
