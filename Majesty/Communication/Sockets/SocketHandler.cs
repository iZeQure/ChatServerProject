using Majesty.Messages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Majesty.Communication.Sockets
{
    class SocketHandler : IConnectionHandler
    {
        private protected readonly Socket _socket;

        public SocketHandler(Socket socket)
        {
            _socket = socket;

            var buffer = new byte[2048];
            string data = null;

            while (true)
            {
                _ = Task.Run(() =>
                {
                    while (true)
                    {
                        int bytesReceived = _socket.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    Console.WriteLine($"Message Received : {data}");
                });
            }
        }

        public void ReceivedMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
