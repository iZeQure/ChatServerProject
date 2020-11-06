using Majesty.Encryption;
using Majesty.Packages;
using Majesty.Users;

namespace Majesty.Protocols
{
    interface IProtocol
    {
        IUserBase ProtocolConvertMessage(byte[] messageBytes);
    }
}
