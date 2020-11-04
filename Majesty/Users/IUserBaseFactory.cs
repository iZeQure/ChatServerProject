using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Users
{
    interface IUserBaseFactory
    {
        IUserBase Create(IUserBase userBase);
    }
}
