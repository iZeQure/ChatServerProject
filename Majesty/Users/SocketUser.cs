﻿using Majesty.Messages;
using System.Net;
using Majesty.UI;

namespace Majesty.Users
{
    class SocketUser : IUser<IPEndPoint>
    {
        private string _nickName;
        private Colors _color;
        private UserMessage _userMessage;
        private bool _isConnected;
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

        public UserMessage UserMessage
        {
            get => _userMessage;
            set => _userMessage = value;
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => _isConnected = value;
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
    }
}
