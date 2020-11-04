using Majesty.Protocols;
using Majesty.Users;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Majesty.Communication.Sockets
{
    class SocketListener : ISocketListener
    {
        private List<IUserBase> _runTimeUsers;

        public IEnumerable<IUserBase> RunTimeUsers => _runTimeUsers;

        public bool Connect(IPEndPoint config)
        {
            throw new NotImplementedException();
        }

        public void Listen(IProtocol protocol)
        {
            throw new NotImplementedException();
        }
    }
}
