using Majesty.Packages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Majesty.Protocols;
using Majesty.UI;
using Majesty.Users;

namespace Majesty.Communication.Sockets
{
    class SocketHandler : IConnectionHandler
    {
        private protected readonly Socket _socket;
        private protected readonly IProtocol _protocol;
        private protected IList<IUserBase> _usersConnected = new List<IUserBase>();

        public IEnumerable<IUserBase> UsersConnected => _usersConnected;
        private protected IUserBaseFactory UserBaseFactory { get; } = new UserFactory();
        private protected IPackageFactory PackageFactory { get; } = new PackageFactory();


        public SocketHandler(Socket socket, IProtocol protocol)
        {
            Console.WriteLine("Connected");   
            _socket = socket;
            _protocol = protocol;

            _ = Task.Run(() =>
            {
                bool isConnected = true;
                int bytesReceived = 0;

                
                while (isConnected)
                {
                    var buffer = new byte[2048];
                    string data = null;
                    
                    bool isReading = true;
                    while (isReading)
                    {
                        try
                        {
                            bytesReceived = _socket.Receive(buffer);

                            data += Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                            if (bytesReceived == 0)
                            {
                                // Client disconnect
                                Console.WriteLine("Client disconnected");
                                isConnected = false;
                                isReading = false;
                                //throw new Exception("Client disconnected");
                            }
                            else
                            {
                                isReading = !(data.IndexOf("%END%") > -1);
                            }

                        }
                        // TODO All exceptions must remove user from users connected list
                        catch (ArgumentNullException argNullE)
                        {
                            Console.WriteLine($"SocketHandler Exception : Buffer was empty.");
                            throw argNullE;
                        }
                        catch (SocketException socE)
                        {
                            Console.WriteLine(
                                $"SocketHandler Exception : Error occurred when attempting to access the socket.");
                            throw socE;
                        }
                        catch (ObjectDisposedException objDisE)
                        {
                            Console.WriteLine($"SocketHandler Exception : Socket has been closed.");
                            throw objDisE;
                        }
                        catch (SecurityException secE)
                        {
                            Console.WriteLine(
                                $"SocketHandler Exception : A caller in the call stack does not have the required permissions.");
                            throw secE;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //throw e;
                        }
                    }

                    if (bytesReceived > 0)
                    {
                        ReceivedPackage(Encoding.UTF8.GetBytes(data));
                    }

                }
            });
        }

        public void ReceivedPackage(byte[] packageBytes)
        {
            var package = PackageFactory.Create("UserPackage") as UserPackage;
            package.PackageBytes = packageBytes;
            var convertedData = _protocol.ProtocolConvertMessage(package) as SocketUser;
            _usersConnected.Add(convertedData);
            Console.WriteLine($"{_protocol.GetType().Name} listener received: {convertedData}");
        }


        public void SendPackage(IPackage package)
        {
            throw new NotImplementedException();
        }
    }
}