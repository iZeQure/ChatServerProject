using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Majesty.Users;

namespace Majesty.Protocols
{
    class SimpleProtocol : IProtocol
    {
        public byte[] ConvertMessageBack(IMessage message)
        {
            throw new NotImplementedException();
        }

        public IUserBase ProtocolConvertMessage(byte[] messageBytes)
        {
            MessageFormatter messageFormatter = new MessageFormatter();
            return messageFormatter.FormatMessage(messageBytes);
        }
    }
}
