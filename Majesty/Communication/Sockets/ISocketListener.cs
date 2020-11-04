using Majesty.Protocols;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Majesty.Communication.Sockets
{
    interface ISocketListener : ICommunicationListener<IProtocol>
    {
        bool Connect();
    }
}
