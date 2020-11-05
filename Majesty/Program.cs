using Majesty.Communication.Sockets;
using Majesty.Protocols;
using Majesty.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Majesty
{
    class Program
    {
        internal IProtocolFactory ProtocolFactory { get; } = new ProtocolFactory();
        internal INotifyUIFactory NotifyUIFactory { get; } = new NotifyUIFactory();

        static void Main(string[] args) =>
            new Program()
            .StartServerAsync()
            .GetAwaiter()
            .GetResult();

        private async Task StartServerAsync()
        {
            try
            {
                new Thread(SimpleListenerThread)
                {
                    Name = "Simple Protocol Thread"
                }.Start();

                new Thread(XmlListenerThread)
                {
                    Name = "XML Protocol Thread"
                }.Start();

                new Thread(SymmetricListenerThread)
                {
                    Name = "Symmetric Protocol Thread"
                }.Start();

                new Thread(AsymmectricListenerThread)
                {
                    Name = "Asymmetric Protocol Thread"
                }.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await Task.Delay(-1);
        }

        private void SimpleListenerThread()
        {
            try
            {
                new SocketListener().Listen(
                        ProtocolFactory
                        .Create(
                            "SimpleProtocol2"));
            }
            catch (NotSupportedException)
            {
                NotifyUIFactory
                    .Create("ConsoleNotification")
                    .SendMessageToUi(
                        "Simple Protocol Thread : Failed Initialzing.",
                        LogLevels.Critical,
                        Colors.Crimson);
            }
        }

        private void XmlListenerThread()
        {
            try
            {
                new SocketListener().Listen(
                        ProtocolFactory
                        .Create(
                            "XmlProtocol"));
            }
            catch (NotSupportedException)
            {
                NotifyUIFactory
                    .Create("ConsoleNotification")
                    .SendMessageToUi(
                        "Xml Protocol Thread : Failed Initialzing.",
                        LogLevels.Critical,
                        Colors.Crimson);
            }
        }

        private void SymmetricListenerThread()
        {
            try
            {
                new SocketListener().Listen(
                        ProtocolFactory
                        .Create(
                            "SymmetricProtocol"));
            }
            catch (NotSupportedException)
            {
                NotifyUIFactory
                    .Create("ConsoleNotification")
                    .SendMessageToUi(
                        "Symmetric Protocol Thread : Failed Initialzing.",
                        LogLevels.Critical,
                        Colors.Crimson);
            }
        }

        private void AsymmectricListenerThread()
        {
            try
            {
                new SocketListener().Listen(
                        ProtocolFactory
                        .Create(
                            "AsymmetricProtocol"));
            }
            catch (NotSupportedException)
            {
                NotifyUIFactory
                    .Create("ConsoleNotification")
                    .SendMessageToUi(
                        "Asymmetric Protocol Thread : Failed Initialzing.",
                        LogLevels.Critical,
                        Colors.Crimson);
            }
        }
    }
}
