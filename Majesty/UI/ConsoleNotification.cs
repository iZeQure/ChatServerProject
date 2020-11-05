using System;
using Majesty.Users;

namespace Majesty.UI
{
    public class ConsoleNotification : INotifyUI
    {
        public void SendMessageToUI(string message, LogLevels logLevels, Colors colors)
        {
            ConsoleColor consoleForgroundColor = ConsoleColor.White;

            switch (colors)
            {
                case Colors.Hot_Pink:
                    consoleForgroundColor = ConsoleColor.Magenta;
                    break;
                case Colors.Lime:
                    consoleForgroundColor = ConsoleColor.Green;
                    break;
                case Colors.Goldenrod:
                    consoleForgroundColor = ConsoleColor.Yellow;
                    break;
                case Colors.Tomato:
                    consoleForgroundColor = ConsoleColor.Red;
                    break;
                case Colors.Medium_Orchid:
                    consoleForgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case Colors.Aquamarine:
                    consoleForgroundColor = ConsoleColor.Cyan;
                    break;
                case Colors.Alice_Blue:
                    consoleForgroundColor = ConsoleColor.Blue;
                    break;
                case Colors.Dark_Salmon:
                    consoleForgroundColor = ConsoleColor.DarkGray;
                    break;
                case Colors.Crimson:
                    consoleForgroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.ForegroundColor = consoleForgroundColor;
            Console.WriteLine($"{logLevels}: {message}");
            Console.ResetColor();
        }
    }
}