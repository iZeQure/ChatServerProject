using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Users
{
    class UserFactory : IUserBaseFactory
    {
        public IUserBase Create(string userObject)
        {
            return userObject switch
            {
                "SocketUser" => new SocketUser(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
