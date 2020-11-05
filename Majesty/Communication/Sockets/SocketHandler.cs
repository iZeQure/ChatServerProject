﻿using Majesty.Messages;
using System;
using System.Net.Sockets;
using System.Security;
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
                        try
                        {
                            int bytesReceived = _socket.Receive(buffer);

                            data += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                            if (data.IndexOf("<EOF>") > -1)
                                break;
                        }
                        catch (ArgumentNullException argNullE)
                        {
                            //Console.WriteLine($"SocketHandler Exception : Buffer was empty.");
                            throw argNullE;
                        }
                        catch (SocketException socE)
                        {
                            //Console.WriteLine($"SocketHandler Exception : Error occurred when attempting to access the socket.");
                            throw socE;
                        }
                        catch (ObjectDisposedException objDisE)
                        {
                            //Console.WriteLine($"SocketHandler Exception : Socket has been closed.");
                            throw objDisE;
                        }
                        catch (SecurityException secE)
                        {
                            //Console.WriteLine($"SocketHandler Exception : A caller in the call stack does not have the required permissions.");
                            throw secE;
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
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
