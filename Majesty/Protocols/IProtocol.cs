using Majesty.Encryption;
using Majesty.Messages;
using Majesty.Users;

namespace Majesty.Protocols
{
    interface IProtocol
    {
        IUserBase ProtocolConvertMessage(byte[] messageBytes);
    }
}
