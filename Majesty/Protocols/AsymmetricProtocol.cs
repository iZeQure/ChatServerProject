using Majesty.Encryption;
using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Majesty.Users;

namespace Majesty.Protocols
{
    class AsymmetricProtocol : IProtocol, ICrypt, IConvert
    {
        public IMessage ConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertMessageBack(IMessage message)
        {
            throw new NotImplementedException();
        }

        public byte[] Decrypt(byte[] toBeDecrypted)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(byte[] toBeEncrypted)
        {
            throw new NotImplementedException();
        }

        public IUserBase ProtocolConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }
    }
}
