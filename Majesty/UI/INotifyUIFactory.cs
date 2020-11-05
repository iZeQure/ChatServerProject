using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.UI
{
    interface INotifyUIFactory
    {
        INotifyUI Create(string notificationUiObject);
    }
}
