using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    interface IMessageFactory
    {
        IMessage Create(IMessage message);
    }
}
