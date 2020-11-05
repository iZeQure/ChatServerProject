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
    
    public interface INotifyUI
    {
        public void SendMessageToUI(string message, LogLevels logLevels, Colors colors);
    }
}