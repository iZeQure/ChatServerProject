using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Majesty.Users
{
    class SocketUser : IUser<IPEndPoint>
    {
        private string _nickName;
        private Colors _color;
        private UserMessage _userMessage;
        private bool _isConnected;
        private IPEndPoint _destinationTo;
        private IPEndPoint _destiantionFrom;

        public string NickName => _nickName;

        public Colors Color => _color;

        public UserMessage UserMessage => _userMessage;

        public bool IsConnected => _isConnected;

        public IPEndPoint DestinationTo => _destinationTo;

        public IPEndPoint DestinationFrom => _destiantionFrom;
    }
}
