﻿using ShareClient.Component.Algorithm;
using ShareClient.Component.Connect;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;

namespace ShareClientForm.Componet
{
    public class DisplayImageSendAlgorithm : IDisposable
    {
        private readonly Timer _SenderTimer = new();
        private readonly DisplayImageCapture _Capture;
        private readonly ISendAlgorithm _Sender;
        private readonly ImageFormat _Format;
        private readonly Action _Closing;

        public bool IsDisposed { get; private set; } = false;

        public DisplayImageSendAlgorithm(Connection connection,
                                         IShareAlgorithmManager manager,
                                         DisplayImageCapture capture,
                                         int interval,
                                         ImageFormat format,
                                         Action closed)
        {
            _Sender = ShareAlgorithmBuilder.NewBuilder()
                                           .SetShareClientSpec(connection.ClientSpec)
                                           .SetShareAlgorithmManager(manager)
                                           .SetLocalEndoPoint(connection.LocalEndPoint)
                                           .BuildSend(connection.RemoteEndPoint);
            _Sender.ShareAlgorithmClosed += Sender_ShareAlgorithmClosed;

            _Capture = capture;
            _Format = format;
            _Closing = closed;

            _SenderTimer.Interval = interval;
            _SenderTimer.Elapsed += Send;
        }

        private void Sender_ShareAlgorithmClosed(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                _SenderTimer.Dispose();
            }
            _Closing.Invoke();
        }

        private void Send(object sender, EventArgs e)
        {
            if (_Capture.TryGetWindowImage(out var sendImage))
            {
                _SenderTimer.Stop();
                using var ms = new MemoryStream();
                sendImage.Save(ms, _Format);
                _Sender.Send(ms.GetBuffer());
                sendImage.Dispose();

                if (!IsDisposed)
                {
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
            IsDisposed = true;
            _SenderTimer.Dispose();
            _Sender.Dispose();
        }
    }
}
