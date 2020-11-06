using Majesty.Packages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Majesty.Users;

namespace Majesty.Communication
{
    interface IConnectionHandler
    {
        IEnumerable<IUserBase> UsersConnected { get; }
        
        void SendMessage(IPackage package);
        void ReceivedMessage(IPackage package);
    }
}
