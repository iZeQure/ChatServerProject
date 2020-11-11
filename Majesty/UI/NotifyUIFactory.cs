using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.UI
{
    class NotifyUIFactory : INotifyUIFactory
    {
        public INotifyUI Create(string notificationUiObject)
        {
            return notificationUiObject switch
            {
                "ConsoleNotification" => new ConsoleNotification(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
