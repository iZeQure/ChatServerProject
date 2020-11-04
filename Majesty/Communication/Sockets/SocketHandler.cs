using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Communication.Sockets
{
    class SocketHandler : IConnectionHandler
    {
        public SocketHandler()
        {

        }

        public void ReceivedMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
