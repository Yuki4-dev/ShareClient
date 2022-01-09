using ShareClient.Component.Algorithm.Internal;
using ShareClient.Component.Connect.Internal;
using ShareClient.Model;
using ShareClient.Model.Connect;
using ShareClient.Model.ShareClient;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ShareClient.Component.Connect
{
    public class Connection
    {
        public ShareClientSpec ClientSpec { get; }
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }

        internal Connection(ShareClientSpec cleintSpec, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            ClientSpec = cleintSpec;
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
        }

        public static ConnectionBuilder Builder()
        {
            return new ConnectionBuilder();
        }

        public class ConnectionBuilder
        {
            private bool isCancellation = false;
            private Func<IConnectionSocket> _LaunchSocket;
            private IPEndPoint _LocalEndPoint;
            private IShareClientLogger _Logger = new DebugLogger();
            private Func<bool> _IsCancellation = () => false;
            private Func<IPEndPoint, ConnectionData, ConnectionResponse> _AcceptRequest = (_, c) => new ConnectionResponse(true, c);
            private Action<ConnectionResponse> _ConnectResponse = (_) => { };

            public ConnectionBuilder SetSocket(Func<IConnectionSocket> launchSocket)
            {
                _LaunchSocket = launchSocket ?? throw new ArgumentNullException(nameof(launchSocket));
                return this;
            }

            public ConnectionBuilder SetLogger(IShareClientLogger logger)
            {
                _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
                return this;
            }

            public ConnectionBuilder SetLocalEndoPoint(IPEndPoint iPEndPoint)
            {
                _LocalEndPoint = iPEndPoint;
                return this;
            }

            public ConnectionBuilder SetCancellation(Func<bool> isCancellation)
            {
                _IsCancellation = isCancellation ?? throw new ArgumentNullException(nameof(isCancellation));
                return this;
            }

            public ConnectionBuilder SetAcceptRequest(Func<IPEndPoint, ConnectionData, ConnectionResponse> acceptRequest)
            {
                _AcceptRequest = acceptRequest ?? throw new ArgumentNullException(nameof(acceptRequest));
                return this;
            }

            public ConnectionBuilder SetConnectResponse(Action<ConnectionResponse> connectResponse)
            {
                _ConnectResponse = connectResponse ?? throw new ArgumentNullException(nameof(connectResponse));
                return this;
            }

            public Connection Connect(IPEndPoint connectEndPoint, ConnectionData connectionData)
            {
                CheckConnect();
                IConnectionSocket socket = Socket();

                var tokenSource = new CancellationTokenSource();
                RunCancellation(socket, tokenSource.Token);

                Connection connection = null;
                try
                {
                    connection = ConnectInternal(socket, connectEndPoint, connectionData);
                }
                catch (Exception ex)
                {
                    if (isCancellation)
                    {
                        _Logger.Info($"Socket Closed. : {ex.Message}");
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    tokenSource.Cancel();
                    socket.Dispose();
                }

                return connection;
            }

            private Connection ConnectInternal(IConnectionSocket socket, IPEndPoint connectEndPoint, ConnectionData connectionData)
            {
                _Logger.Info($"Start Connect. -> {connectEndPoint}");

                var sendData = new ShareClientData(ShareClientHeader.CreateSystem((uint)connectionData.Size), connectionData.ToByte());
                Send(socket, connectEndPoint, sendData);

                var recieveData = Recive(socket);
                var responseData = ShareClientData.FromBytes(recieveData.RecieveBytes);
                if (responseData == null)
                {
                    _Logger.Info($"ShareClientData Convert Fail.");
                    return null;
                }
                else if (responseData.Header.DataType != SendDataType.System)
                {
                    _Logger.Info($"ShareClientData Type is {responseData.Header.DataType}.");
                    return null;
                }

                var connectionResponse = ConnectionResponse.FromByte(responseData.DataPart);
                if (connectionResponse == null)
                {
                    _Logger.Info($"Response DataPart is Nothing.");
                    return null;
                }

                _ConnectResponse.Invoke(connectionResponse);
                if (!connectionResponse.IsConnect)
                {
                    _Logger.Info($"Reject Connect. -> {connectEndPoint}");
                    return null;
                }

                _Logger.Info($"Succes Connect. -> {connectEndPoint}");

                return new Connection(connectionResponse.ConnectionData.CleintSpec, socket.LocalEndPoint, connectEndPoint);
            }

            public Connection Accept(IPEndPoint localEndPont)
            {
                _LocalEndPoint = localEndPont;

                CheckRecieve();
                IConnectionSocket socket = Socket();

                var tokenSource = new CancellationTokenSource();
                RunCancellation(socket, tokenSource.Token);

                Connection connection = null;
                try
                {
                    connection = AcceptInternal(socket);
                }
                catch (Exception ex)
                {
                    if(isCancellation)
                    {
                        _Logger.Info($"Socket Closed. : {ex.Message}");
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    tokenSource.Cancel();
                    socket.Dispose();
                }

                return connection;
            }

            private Connection AcceptInternal(IConnectionSocket socket)
            {
                var recieveData = Recive(socket);
                var recieveConnectionData = ShareClientData.FromBytes(recieveData.RecieveBytes);
                if (recieveConnectionData == null)
                {
                    _Logger.Info($"ShareClientData Convert Fail.");
                    return null;
                }
                else if (recieveConnectionData.Header.DataType != SendDataType.System)
                {
                    _Logger.Info($"ShareClientData Type {recieveConnectionData?.Header.DataType}.");
                    return null;
                }

                var connectionData = ConnectionData.FromByte(recieveConnectionData.DataPart);
                if (connectionData == null)
                {
                    _Logger.Info($"ConnectionData Convert Fail.");
                    return null;
                }

                var remoteEndPoint = recieveData.ReciveEndPoint;
                var connectionResponse = _AcceptRequest.Invoke(remoteEndPoint, connectionData);
                _Logger.Info($"AcceptRequest is {connectionResponse.IsConnect}.");

                var responseData = new ShareClientData(ShareClientHeader.CreateSystem((uint)connectionResponse.Size), connectionResponse.ToByte());
                Send(socket, remoteEndPoint, responseData);

                Connection connection = null;
                if (connectionResponse.IsConnect)
                {
                    _Logger.Info($"Accept Succes.");
                    connection = new Connection(connectionResponse.ConnectionData.CleintSpec, socket.LocalEndPoint, remoteEndPoint);
                }
                return connection;
            }

            private IConnectionSocket Socket()
            {
                if (_LaunchSocket != null)
                {
                    return _LaunchSocket.Invoke();
                }

                IConnectionSocket socket = null;
                try
                {
                    if (_LocalEndPoint == null)
                    {
                        socket = new InternalConnectionSocket();
                    }
                    else
                    {
                        socket = new InternalConnectionSocket(_LocalEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    Throw(_LocalEndPoint, $"Fail Create ConnectionSocket. {ex.Message}", ex);
                }

                return socket;
            }

            private void CheckConnect()
            {
                //
            }

            private void CheckRecieve()
            {
                if (_LocalEndPoint == null)
                {
                    throw new InvalidOperationException($"{nameof(_LocalEndPoint)} is null.");
                }
            }

            private void RunCancellation(IConnectionSocket socket, CancellationToken token)
            {
                Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (_IsCancellation.Invoke())
                        {
                            isCancellation = true;
                            socket.Dispose();
                            _Logger.Info("Request Cancel.");
                            break;
                        }
                        Thread.Sleep(1);
                        Thread.Sleep(0);
                        Thread.Yield();
                    }
                });
            }

            private void Send(IConnectionSocket socket, IPEndPoint remoteEndPoint, ShareClientData shareClientData)
            {
                try
                {
                    socket.Send(remoteEndPoint, shareClientData.ToByte());
                }
                catch (Exception ex)
                {
                    Throw(remoteEndPoint, $"Fail Send. {ex.Message}", ex);
                }

                _Logger.Send(remoteEndPoint, shareClientData.ToByte());
            }

            private ConnectionSocketRecieveData Recive(IConnectionSocket socket)
            {
                ConnectionSocketRecieveData recieveData = null;
                try
                {
                    recieveData = socket.Recieve();
                    _Logger.Receive(recieveData.ReciveEndPoint, recieveData.RecieveBytes);
                }
                catch (Exception ex)
                {
                    Throw(_LocalEndPoint, $"Fail Recive. {ex.Message}", ex);
                }

                return recieveData;
            }

            private void Throw(IPEndPoint endPoint, string message, Exception inner = null)
            {
                var ce = new ConnectionException(endPoint, message, inner);
                _Logger.Error(ce.Message, ce);
                throw ce;
            }
        }
    }
}
