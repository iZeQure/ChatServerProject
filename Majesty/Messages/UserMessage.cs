using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    class UserMessage : IMessage
    {
        private byte[] _message;

        public byte[] Message => _message;

        public string UsersMessage { get; set; }
    }
}
