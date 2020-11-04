using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    interface IMessage
    {
        byte[] Message { get; }
    }
}
