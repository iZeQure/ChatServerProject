using Majesty.Packages;
using System;
using Majesty.Users;

namespace Majesty.Protocols
{
    class SimpleProtocol : IProtocol
    {
        public byte[] ConvertMessageBack(IPackage package)
        {
            throw new NotImplementedException();
        }

        public IUserBase ProtocolConvertMessage(IPackage package)
        {
            Console.WriteLine("SIMPLE");
            MessageFormatter messageFormatter = new MessageFormatter();
            return messageFormatter.FormatMessage(package);
        }
    }
}
