using ShareClient.Component;
using ShareClient.Component.Connect;
using ShareClient.Model.Connect;
using SharedClientForm;
using SharedClientForm.Component;
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
        private DisplayImageReciveAlgorithm _Receiver;
        private DisplayImageSendAlgorithm _Sender;

        public DataSizeShareAlgorithmManager ReciveManager { get; } = new DataSizeShareAlgorithmManager();
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
                         DisplayImageCaputure caputure,
                         int faleLate,
                         ImageFormat format,
                         Action closing)
        {
            if (_Sender != null && !_Sender.IsDisposed)
            {
                throw new InvalidOperationException("Send Already Run.");
            }

            _Sender = new DisplayImageSendAlgorithm(connection, SendManager, caputure, faleLate, format, closing);
            _Sender.Start();
        }

        public void Recieve(Connection connection,
                                      int flameLate,
                                      IPictureArea pictureareArea,
                                      Action closing)
        {
            if (_Receiver != null && !_Receiver.IsDisposed)
            {
                throw new InvalidOperationException("Recieve Already Run.");
            }

            _Receiver = new DisplayImageReciveAlgorithm(connection, ReciveManager, flameLate, pictureareArea, closing);
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
