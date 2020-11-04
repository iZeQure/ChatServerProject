using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Messages
{
    class MessageFactory : IMessageFactory
    {
        public IMessage Create(string messageObject)
        {
            return messageObject switch
            {
                "UserMessage" => new UserMessage(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
