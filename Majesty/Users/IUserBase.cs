using Majesty.Messages;
using Majesty.UI;

namespace Majesty.Users
{
    interface IUserBase
    {
        string NickName { get; }
        Colors Color { get; }
        UserMessage UserMessage { get; }
        bool IsConnected { get; }
        string SenderHostname { get; }
        string ReceiverHostname { get; }
    }
}
