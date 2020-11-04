using Majesty.Protocols;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace Majesty.Communication.Sockets
{
    class SocketListener : ISocketListener
    {
        private protected Socket _socket;
        private protected int _protocolPort;
        private protected IConnectionHandlerFactory _handlerFactory;

        public bool Connect()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _protocolPort);

                Socket listener = new Socket(
                        ipAddress.AddressFamily,
                        SocketType.Stream,
                        ProtocolType.Tcp);
                try
                {
                    listener.Bind(localEndPoint);
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : LocalEndPoint was null.");
                }
                catch (SocketException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : An error occurred when attempting to access the socket.");
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : Socket has been closed.");
                }
                catch (SecurityException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : A caller higher in the call stack does not have permission for the requested operation.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : Something blocked the socket bind.");
                    return false;
                }

                try
                {
                    listener.Listen(10);
                }
                catch (SocketException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : An error occurred when attempting to access the socket.");
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine($"Socket Listener Connect Exception : Socket has been closed.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Socket Listener Connect Exception");
                    return false;
                }

                _socket = listener;

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine($"Socket Listener Connect Exception : Something went completely wrong.");
                return false;
            }
        }

        public void Listen(IProtocol protocol)
        {
            Console.WriteLine($"Started listening for <{protocol.GetType().Name}> protocol.");

            switch (protocol)
            {
                case SimpleProtocol:
                    _protocolPort = 50001;
                    break;
                case XmlProtocol:
                    _protocolPort = 50002;
                    break;
                case SymmetricProtocol:
                    _protocolPort = 50003;
                    break;
                case AsymmetricProtocol:
                    _protocolPort = 50004;
                    break;
            }

            try
            {
                if (Connect())
                {
                    try
                    {
                        Socket newConnectionSocket = _socket.Accept();
                        _handlerFactory = new SocketHandlerFactory(newConnectionSocket);

                        try
                        {
                            _handlerFactory.Create("SocketHandler");
                        }
                        catch (NotSupportedException)
                        {
                            Console.WriteLine($"Socket Handler Factory : Failed Initializing.");
                        }
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine($"Socket Listener Exception : An error occurred when attempting to access the socket");
                    }
                    catch (ObjectDisposedException)
                    {
                        Console.WriteLine($"Socket Listener Exception : Socket has been closed.");
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"Socket Listener Exception : Socket is not listenening for connections.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Socket Listener Exception : Something blocked the listener.");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Socket Listener Exception : Something went completely wrong.");
            }
        }
    }
}
