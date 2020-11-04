using Majesty.Communication.Sockets;
using Majesty.Protocols;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Majesty
{
    class Program
    {
        internal IProtocolFactory ProtocolFactory { get; } = new ProtocolFactory();

        static void Main(string[] args) =>
            new Program()
            .StartServerAsync()
            .GetAwaiter()
            .GetResult();

        private async Task StartServerAsync()
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

            await Task.Delay(-1);
        }

        private void SimpleListenerThread()
        {
            try
            {
                new SocketListener().Listen(
                        ProtocolFactory
                        .Create(
                            "SimpleProtocol"));
            }
            catch (NotSupportedException)
            {
                Console.WriteLine($"Simple Protocol Thread : Failed Initialzing.");
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
                Console.WriteLine($"Xml Protocol Thread : Failed Initialzing.");
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
                Console.WriteLine($"Symmetric Protocol Thread : Failed Initialzing.");
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
                Console.WriteLine($"Asymmetric Protocol Thread : Failed Initialzing.");
            }
        }
    }
}
