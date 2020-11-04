using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Users
{
    interface IUser<T> : IUserBase
    {
        T DestinationTo { get; }
        T DestinationFrom { get; }
    }
}
