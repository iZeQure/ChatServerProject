using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Majesty.Communication
{
    interface IConnectionHandler
    {
        void SendMessage(IMessage message);
        void ReceivedMessage(IMessage message);
    }
}
