using ShareClient.Component.Connect;
using ShareClient.Model.Connect;
using ShareClientForm.Controls;
using System;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;

namespace ShareClientForm.Componet
{
    public class ImageShareAlgorithm : IDisposable
    {
        private bool isCancelAccept = false;
        private bool isCancelConnect = false;
        private DisplayImageReceiveAlgorithm _Receiver;
        private DisplayImageSendAlgorithm _Sender;

        public DataSizeShareAlgorithmManager ReceiveManager { get; } = new DataSizeShareAlgorithmManager();
        public DataSizeShareAlgorithmManager SendManager { get; } = new DataSizeShareAlgorithmManager();

        public async Task<Connection> ConnectAsync(IPEndPoint connectEndPoint, ConnectionData connectionData)
        {
            isCancelConnect = false;
            return await Task.Run(() => Connection.Builder()
                                                  .SetCancellation(() => isCancelConnect)
                                                  .Connect(connectEndPoint, connectionData));
        }

        public async Task<Connection> AcceptAsync(IPEndPoint localEndPoint,
                                                  Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptRequest)
        {
            isCancelAccept = false;
            return await Task.Run(() => Connection.Builder()
                                                  .SetCancellation(() => isCancelAccept)
                                                  .SetAcceptRequest(acceptRequest)
                                                  .Accept(localEndPoint));
        }

        public void Send(Connection connection,
                         DisplayImageCapture capture,
                         int frameLate,
                         ImageFormat format,
                         Action closing)
        {
            if (_Sender != null && !_Sender.IsDisposed)
            {
                throw new InvalidOperationException("Send Already Run.");
            }

            _Sender = new DisplayImageSendAlgorithm(connection, SendManager, capture, frameLate, format, closing);
            _Sender.Start();
        }

        public void Receive(Connection connection,
                                      int flameLate,
                                      IPictureArea pictureArea,
                                      Action closing)
        {
            if (_Receiver != null && !_Receiver.IsDisposed)
            {
                throw new InvalidOperationException("Receive Already Run.");
            }

            _Receiver = new DisplayImageReceiveAlgorithm(connection, ReceiveManager, flameLate, pictureArea, closing);
            _Receiver.Start();
        }

        public void CancelAccept()
        {
            isCancelAccept = true;
        }

        public void CancelConnect()
        {
            isCancelConnect = true;
        }

        public void Stop()
        {
            CancelAccept();
            CancelConnect();
            _Sender?.Dispose();
            _Receiver?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
