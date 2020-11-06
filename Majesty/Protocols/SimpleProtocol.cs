using Majesty.Packages;
using System;
using System.Collections.Generic;
using System.Text;
using Majesty.Users;

namespace Majesty.Protocols
{
    class SimpleProtocol : IProtocol
    {
        public byte[] ConvertMessageBack(IPackage package)
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
