using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    class UserMessage : IMessage
    {
        private byte[] _message;

        public byte[] Message { get { return _message; } set { _message = value; } }

        public string UsersMessage { get; set; }
    }
}
