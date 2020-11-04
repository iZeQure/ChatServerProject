using Majesty.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Communication
{
    interface ICommunicationListener<T>
    {
        IEnumerable<IUserBase> RunTimeUsers { get; }

        void Listen(T protocol);
    }
}
