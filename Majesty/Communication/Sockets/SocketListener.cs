using Majesty.Messages;
using Majesty.Protocols;
using Majesty.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
                listener.Bind(localEndPoint);
                listener.Listen(10);

                _socket = listener;

                return true;
            }
            catch (Exception)
            {
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
                    Socket newConnectionSocket  = _socket.Accept();
                    _handlerFactory = new SocketHandlerFactory(newConnectionSocket);
                    _handlerFactory.Create("SocketHandler");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
