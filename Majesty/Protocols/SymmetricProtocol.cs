using Majesty.Encryption;
using Majesty.Packages;
using System;
using System.Collections.Generic;
using System.Text;
using Majesty.Users;

namespace Majesty.Protocols
{
    class SymmetricProtocol : IProtocol, ICrypt, IConvert
    {
        public IPackage ConvertMessage(byte[] messageBytes)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertMessageBack(IPackage package)
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
