using Majesty.Packages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Majesty.Users;

namespace Majesty.Communication
{
    interface IConnectionHandler
    {
        IEnumerable<IUserBase> UsersConnected { get; }
        
        void SendPackage(IPackage package);
        void ReceivedPackage(byte[] packageBytes);
        Task HandleConnection();
    }
}
