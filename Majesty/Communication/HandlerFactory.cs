using Majesty.Communication.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Communication
{
    class HandlerFactory : IConnectionHandlerFactory
    {
        public IConnectionHandler Create(IConnectionHandler connectionHandler)
        {
            return connectionHandler switch
            {
                SocketHandler => new SocketHandler(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
