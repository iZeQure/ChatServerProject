using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Protocols
{
    interface IProtocolFactory
    {
        IProtocol Create(IProtocol protocol);
    }
}
