using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Users
{
    class UserFactory : IUserBaseFactory
    {
        public IUserBase Create(IUserBase userBase)
        {
            return userBase switch
            {
                SocketUser => new SocketUser(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
