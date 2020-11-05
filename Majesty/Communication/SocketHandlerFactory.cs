using Majesty.Communication.Sockets;
using System;
using System.Net.Sockets;

namespace Majesty.Communication
{
    class SocketHandlerFactory : IConnectionHandlerFactory
    {
        private readonly Socket _socket;

        public SocketHandlerFactory(Socket socket)
        {
            _socket = socket;
        }

        public IConnectionHandler Create(string handlerObject)
        {
            return handlerObject switch
            {
                "SocketHandler" => new SocketHandler(_socket),
                _ => throw new NotSupportedException()
            };
        }
    }
}
