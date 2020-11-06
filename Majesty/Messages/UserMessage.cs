using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    class UserMessage : IMessage
    {


        public byte[] Message
        {
            get;
            set;
        }

        public string UsersMessage { get; set; }
    }
}
