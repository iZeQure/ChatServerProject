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
                catch (ArgumentNullException argNullE)
                {
                    //Console.WriteLine($"Socket Listener Connect Exception : LocalEndPoint was null.");
                    throw argNullE;
                }
                catch (SocketException socE)
                {
                    throw socE;
                    //Console.WriteLine($"Socket Listener Connect Exception : An error occurred when attempting to access the socket.");
                }
                catch (ObjectDisposedException objDisE)
                {
                    throw objDisE;
                    //Console.WriteLine($"Socket Listener Connect Exception : Socket has been closed.");
                }
                catch (SecurityException secE)
                {
                    throw secE;
                    //Console.WriteLine($"Socket Listener Connect Exception : A caller higher in the call stack does not have permission for the requested operation.");
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Socket Listener Connect Exception : Something blocked the socket bind.");
                    return false;
                    throw e;
                }

                try
                {
                    listener.Listen(10);
                }
                catch (SocketException socE)
                {
                    throw socE;
                    //Console.WriteLine($"Socket Listener Connect Exception : An error occurred when attempting to access the socket.");
                }
                catch (ObjectDisposedException objDisE)
                {
                    throw objDisE;
                    //Console.WriteLine($"Socket Listener Connect Exception : Socket has been closed.");
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Socket Listener Connect Exception");
                    return false;
                    throw e;
                }

                _socket = listener;

                return true;
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Socket Listener Connect Exception : Something went completely wrong.");
                return false;
                throw e;
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
                if(Connect())
                {
                    while (true)
                    {
                        try
                        {
                            Socket newConnectionSocket = _socket.Accept();
                            _handlerFactory = new SocketHandlerFactory(newConnectionSocket, protocol);

                            try
                            {
                                _handlerFactory.Create("SocketHandler");
                            }
                            catch (NotSupportedException notSuppE)
                            {
                                //Console.WriteLine($"Socket Handler Factory : Failed Initializing.");
                                throw notSuppE;
                            }
                        }
                        catch (SocketException se)
                        {
                            throw se;
                            //Console.WriteLine($"Socket Listener Exception : An error occurred when attempting to access the socket");
                        }
                        catch (ObjectDisposedException objDisE)
                        {
                            throw objDisE;
                            //Console.WriteLine($"Socket Listener Exception : Socket has been closed.");
                        }
                        catch (InvalidOperationException invOpeE)
                        {
                            throw invOpeE;
                            //Console.WriteLine($"Socket Listener Exception : Socket is not listenening for connections.");
                        }
                        catch (Exception e)
                        {
                            throw e;
                            //Console.WriteLine($"Socket Listener Exception : Something blocked the listener.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
                //Console.WriteLine($"Socket Listener Exception : Something went completely wrong.");
            }
        }
    }
}
