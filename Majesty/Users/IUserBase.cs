using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Users
{
    enum Colors
    {
        Hot_Pink = 0,
        Lime = 1,
        Goldenrod = 2,
        Tomato = 3,
        Medium_Orchid = 4,
        Aquamarine = 5,
        Alice_Blue = 6,
        Dark_Salmon = 7,
        Crimson = 8
    }

    interface IUserBase
    {
        string NickName { get; }
        Colors Color { get; }
        UserMessage UserMessage { get; }
        bool IsConnected { get; }
    }
}
