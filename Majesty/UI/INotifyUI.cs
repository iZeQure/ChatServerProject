using Majesty.Users;

namespace Majesty.UI
{
    public enum LogLevels
    {
        Critical = 1,
        Debug = 2,
        Error = 3,
        Info = 4,
        Verbose = 5,
        Warning = 6
    }
    
    public enum Colors
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
    
    public interface INotifyUI
    {
        public void SendMessageToUi(string message, LogLevels logLevels, Colors colors);
    }
}