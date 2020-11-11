using Majesty.Protocols;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Majesty.Communication.Sockets
{
    interface ISocketListener : ICommunicationListener<IProtocol>
    {
        bool BindSocket();
        Task NewClientConnection();

    }
}
