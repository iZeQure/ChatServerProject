using Majesty.Packages;
using Majesty.UI;

namespace Majesty.Users
{
    interface IUserBase
    {
        string NickName { get; }
        Colors Color { get; }
        UserPackage UserPackage { get; }
        bool IsConnected { get; }
        string SenderHostname { get; }
        string ReceiverHostname { get; }
    }
}
