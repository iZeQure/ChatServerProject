using Majesty.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Majesty.Users;

namespace Majesty.Communication.Sockets
{
    class SocketListener : ISocketListener
    {
        private protected Socket _socket;
        private protected int _protocolPort;
        private protected IConnectionHandlerFactory _handlerFactory;

        public bool BindSocket()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _protocolPort); // Get from hostname
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("172.16.21.35"), _protocolPort); // Static
                
                
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

        public Task NewClientConnection()
        {
            throw new NotImplementedException();
        }

        public async void Listen(IProtocol protocol)
        {
            Console.WriteLine($"Started listening on {protocol.GetType().Name.ToUpper()}.");

            _protocolPort = protocol switch
            {
                SimpleProtocol => 50001,
                XmlProtocol => 50002,
                SymmetricProtocol => 50003,
                AsymmetricProtocol => 50004,
                _ => _protocolPort
            };

            try
            {
                if(BindSocket())
                {
                    while (true)
                    {
                        try
                        {
                            Socket newConnectionSocket = _socket.Accept();
                            _handlerFactory = new SocketHandlerFactory(newConnectionSocket, protocol);

                            try
                            {
                                var socketHandler = _handlerFactory.Create("SocketHandler");
                                await socketHandler.HandleConnection();
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
        /*public Task NewClientConnection()
        {
            if (!((List<SocketUser>) _socketUsersConnected).Any(u =>
                u.DestinationFrom == (IPEndPoint) _socket.RemoteEndPoint))
            {
                Console.WriteLine($"Client connected {((IPEndPoint) _socket.RemoteEndPoint).Address}");
                var newUser = UserBaseFactory.Create("SocketUser") as SocketUser;
                newUser.DestinationFrom = (IPEndPoint) _socket.RemoteEndPoint;
                _socketUsersConnected.Add(newUser);
                Console.WriteLine("a");
            }
            else
            {
                Console.WriteLine("Client already connected");
            }

            return Task.CompletedTask;
        }*/
    }
}
