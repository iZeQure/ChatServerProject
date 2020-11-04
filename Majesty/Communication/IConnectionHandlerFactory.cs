using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Communication
{
    interface IConnectionHandlerFactory
    {
        IConnectionHandler Create(string  connectionHandlerObject);
    }
}
