using Majesty.Encryption;
using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Protocols
{
    class SymmetricProtocol : IProtocol, ICrypt
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
    }
}
