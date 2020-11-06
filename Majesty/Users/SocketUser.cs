using Majesty.Packages;
using System.Net;
using Majesty.UI;

namespace Majesty.Users
{
    class SocketUser : IUser<IPEndPoint>
    {
        private string _nickName;
        private Colors _color;
        private UserPackage _userPackage;
        private bool _isConnected;
        private string _senderHostname;
        private string _receiverHostname;
        private IPEndPoint _destinationTo;
        private IPEndPoint _destinationFrom;
        
        public string NickName
        {
            get => _nickName;
            set => _nickName = value;
        }

        public Colors Color
        {
            get => _color;
            set => _color = value;
        }

        public UserPackage UserPackage
        {
            get => _userPackage;
            set => _userPackage = value;
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => _isConnected = value;
        }

        public string SenderHostname
        {
            get => _senderHostname;
            set => _senderHostname = value;
        }

        public string ReceiverHostname
        {
            get => _receiverHostname;
            set => _receiverHostname = value;
        }

        public IPEndPoint DestinationTo
        {
            get => _destinationTo;
            set => _destinationTo = value;
        }

        public IPEndPoint DestinationFrom
        {
            get => _destinationFrom;
            set => _destinationFrom = value;
        }

        public override string ToString()
        {
            return $"{SenderHostname} {DestinationFrom.Address}";
        }
    }
}
