using Majesty.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Communication
{
    interface ICommunicationListener<T>
    {
        void Listen(T protocol);
    }
}
