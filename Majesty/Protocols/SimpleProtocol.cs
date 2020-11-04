using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Protocols
{
    class SimpleProtocol : IProtocol
    {
        public IMessage ConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertMessageBack(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
