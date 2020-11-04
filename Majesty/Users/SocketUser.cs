using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Majesty.Users
{
    class SocketUser : IUser<IPEndPoint>
    {
        private readonly string _nickName;
        private readonly Colors _color;
        private readonly UserMessage _userMessage;
        private readonly bool _isConnected;
        private readonly IPEndPoint _destinationTo;
        private readonly IPEndPoint _destiantionFrom;

        public string NickName => _nickName;

        public Colors Color => _color;

        public UserMessage UserMessage => _userMessage;

        public bool IsConnected => _isConnected;

        public IPEndPoint DestinationTo => _destinationTo;

        public IPEndPoint DestinationFrom => _destiantionFrom;
    }
}
