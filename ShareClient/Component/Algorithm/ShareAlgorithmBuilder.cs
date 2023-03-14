using ShareClient.Component.Algorithm.Internal;
using ShareClient.Component.Core;
using ShareClient.Component.Core.Internal;
using ShareClient.Model.ShareClient;
using System;
using System.Net;

namespace ShareClient.Component.Algorithm
{
    public class ShareAlgorithmBuilder
    {
        private IShareClientSocket _Socket;
        private IPEndPoint _ConnectEndPoint;
        private IPEndPoint _LocalEndPoint;

        public IShareAlgorithmManager ShareAlgorithmManager { get; private set; } = new InternalShareAlgorithmManager();
        public ShareClientSpec ClientSpec { get; private set; } = new ShareClientSpec();

        private ShareAlgorithmBuilder() { }

        public static ShareAlgorithmBuilder NewBuilder()
        {
            return new ShareAlgorithmBuilder();
        }

        public ShareAlgorithmBuilder SetShareAlgorithmManager(IShareAlgorithmManager algorithmManager)
        {
            ShareAlgorithmManager = algorithmManager ?? throw new ArgumentNullException(nameof(algorithmManager));
            return this;
        }

        public ShareAlgorithmBuilder SetShareClientSpec(ShareClientSpec clientSpec)
        {
            ClientSpec = clientSpec ?? throw new ArgumentNullException(nameof(clientSpec));
            return this;
        }

        public ShareAlgorithmBuilder SetSocket(IShareClientSocket socket)
        {
            _Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            return this;
        }

        public ShareAlgorithmBuilder SetConnectEndoPoint(IPEndPoint iPEndPoint)
        {
            _ConnectEndPoint = iPEndPoint;
            return this;
        }

        public ShareAlgorithmBuilder SetLocalEndoPoint(IPEndPoint iPEndPoint)
        {
            _LocalEndPoint = iPEndPoint;
            return this;
        }

        public ISendAlgorithm BuildSend(IPEndPoint connectEndPoint)
        {
            _ConnectEndPoint = connectEndPoint;

            CheckSend();

            IShareClientSocket socket = _Socket;
            socket ??= CreateUdpSocket();

            return new InternalSendAlgorithm(ClientSpec, ShareAlgorithmManager, socket);
        }

        public IReceiveAlgorithm BuildReceive(IPEndPoint localEndPoint)
        {
            _LocalEndPoint = localEndPoint;

            CheckReceive();

            IShareClientSocket socket = _Socket;
            socket ??= CreateUdpSocket();

            return new InternalReceiveAlgorithm(ClientSpec, ShareAlgorithmManager, socket);
        }

        private void CheckSend()
        {
            if (_ConnectEndPoint == null)
            {
                throw new InvalidOperationException("ConnectEndPoint is null.");
            }
        }

        private void CheckReceive()
        {
            if (_LocalEndPoint == null)
            {
                throw new InvalidOperationException("LocalEndPoint is null.");
            }
        }

        private IShareClientSocket CreateUdpSocket()
        {
            InternalUdpClientSocket socket = _LocalEndPoint == null ? new InternalUdpClientSocket() : new InternalUdpClientSocket(_LocalEndPoint);
            if (_ConnectEndPoint != null)
            {
                socket.Connect(_ConnectEndPoint);
            }

            return socket;
        }
    }
}
