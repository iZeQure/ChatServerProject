using Majesty.Communication;
using Majesty.Communication.Sockets;
using Majesty.Messages;
using Majesty.Protocols;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            new SocketListener().Listen(
                ProtocolFactory
                .Create(
                    "SimpleProtocol"));
        }

        private void XmlListenerThread()
        {
            new SocketListener().Listen(
                ProtocolFactory
                .Create(
                    "XmlProtocol"));
        }

        private void SymmetricListenerThread()
        {
            new SocketListener().Listen(
                ProtocolFactory
                .Create(
                    "SymmetricProtocol"));
        }

        private void AsymmectricListenerThread()
        {
            new SocketListener().Listen(
                ProtocolFactory
                .Create(
                    "AsymmetricProtocol"));
        }
    }
}
