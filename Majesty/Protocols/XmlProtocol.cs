﻿using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Majesty.Encryption;
using Majesty.Users;

namespace Majesty.Protocols
{
    class XmlProtocol : IProtocol, IConvert
    {
        public IMessage ConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertMessageBack(IMessage message)
        {
            throw new NotImplementedException();
        }

        public IUserBase ProtocolConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }
    }
}
