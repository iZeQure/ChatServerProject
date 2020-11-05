using System;
using System.Net.Sockets;
using Majesty.Protocols;

namespace Majesty.Communication.Sockets
{
    class SocketHandlerFactory : IConnectionHandlerFactory
    {
        private readonly Socket _socket;
        private readonly IProtocol _protocol;

        public SocketHandlerFactory(Socket socket, IProtocol protocol)
        {
            _socket = socket;
            _protocol = protocol;
        }

        public IConnectionHandler Create(string handlerObject)
        {
            return handlerObject switch
            {
                "SocketHandler" => new SocketHandler(_socket, _protocol),
                _ => throw new NotSupportedException()
            };
        }
    }
}
