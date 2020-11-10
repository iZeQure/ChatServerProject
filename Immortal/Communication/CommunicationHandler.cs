using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Immortal.Communication
{
    class CommunicationHandler
    {
        private static List<SocketClient> SocketClients = new List<SocketClient>();
        private readonly Logger _logger = new Logger();

        public CommunicationHandler()
        {
            var simpleProtocolThread = new Thread(StartListeningOnSimpleProtocol_Thread)
            {
                Name = "Simple Protocol Thread",
            };

            var xmlProtocolThread = new Thread(StartListeningOnXmlProtocol_Thread)
            {
                Name = "XML Protocol Thread"
            };

            _logger.Log(LogSeverity.System, "Protocols is starting..");

            simpleProtocolThread.Start();
        }

        private protected void StartListeningOnSimpleProtocol_Thread()
        {
            _logger.Log(LogSeverity.System, $"{Thread.CurrentThread.Name} is ready!");

            // Initialzie new endpoint.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.16.2.30"), 50001);

            // Define socket listener from the endpoint.
            Socket socketListener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the endpoint to the socket.
            socketListener.Bind(endPoint);

            // Listen on communication on the socket.
            socketListener.Listen(10);

            // Listen for connections..
            while (true)
            {
                // Handle incoming connection.
                Socket incomingSocketConnection = socketListener.Accept();                

                // Run new thread on the current socket connection.
                Task.Run(() =>
               {
                   bool isClientConnected = false;

                   // Check whether a client is connected to the endpoint socket.
                   if (incomingSocketConnection.Connected)
                   {
                       // Log the client who has connected.
                       _logger.Log(LogSeverity.Info, $"Client Connected: {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");
                       isClientConnected = true;

                       // Check ifthe client connected exists.
                       if (SocketClients.Any(ip => ip.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString()))
                       {
                           // Get the client, and set their state to connected is true.
                           var getClient = SocketClients.FirstOrDefault(u => u.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString());
                           if (getClient != null) getClient.IsConnected = true;
                       }
                       // Add the client to the socket clients.
                       else
                           SocketClients.Add(new SocketClient(((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString(), incomingSocketConnection, true));
                   }

                   while (true)
                   {
                       // Initialize a buffer to hold data.
                       byte[] dataBuffer = new byte[1024];

                       // Define an object to store the data in.
                       string readableData = null;

                       // Read incoming data from the connected client.
                       while (isClientConnected)
                       {
                           int bytesReceived = 0;

                           try
                           {
                               // Get the number of bytes received from the socket.
                               bytesReceived = incomingSocketConnection.Receive(dataBuffer);
                           }
                           catch (SocketException)
                           {
                               // Set the current socket user to disconnected.
                               var getClient = SocketClients.FirstOrDefault(u => u.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString());
                               if (getClient != null) SocketClients.Remove(getClient);
                           }

                           // Check if the bytes received isn't zero.
                           if (bytesReceived != 0)
                           {
                               // Append the data with the length of bytes received on the specific index.
                               readableData += Encoding.UTF8.GetString(dataBuffer, 0, bytesReceived);

                               // Check if data contains end of file.
                               if (readableData.IndexOf("{END}") > -1)
                               {
                                   // Remove end of file data.
                                   readableData = readableData.Replace("{END}", string.Empty);
                                   break;
                               }
                           }
                           // Client is disconnected.
                           else
                           {
                               isClientConnected = false;
                               _logger.Log(LogSeverity.Info, $"Client Disconnected: {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");

                               // Set the current socket user to disconnected.
                               var getClient = SocketClients.FirstOrDefault(u => u.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString());
                               if (getClient != null) getClient.IsConnected = false;
                           }
                       }

                       // Check on data if client is connected.
                       if (isClientConnected)
                       {
                           try
                           {
                               // Store the message sent from the client, if fails, the format was incorrect.
                               var userMessage = FormatMessage(readableData);

                               // Log the received message.
                               _logger.Log(LogSeverity.Info, $"Message Received: {userMessage}");

                               try
                               {
                                   // Check if any clients exists and is online, with the provided information sent from a client.
                                   if (SocketClients.Any(user => user.IpAddress == userMessage.ReceiverIpAddress && user.IsConnected))
                                   {
                                       // Get the client to communicate with.
                                       var getClient = SocketClients.FirstOrDefault(user => user.IpAddress == userMessage.ReceiverIpAddress);

                                       // Check that the client isn't null.
                                       if (getClient != null)
                                       {
                                           // Deliver message to receiver client.
                                           getClient.ClientSocket.Send(Encoding.UTF8.GetBytes($"{userMessage.NickName} : {userMessage.ChatMessage}"));

                                           // Notify client that their message has been delivered.
                                           incomingSocketConnection.Send(Encoding.UTF8.GetBytes("Message has been delivered."));
                                       }
                                       // Notify the client, that their message couldn't be delivered.
                                       else
                                           incomingSocketConnection.Send(Encoding.UTF8.GetBytes("Message couldn't be delivered."));

                                   }
                                   // Notify client with a message if the client isn't online or found.
                                   else
                                       incomingSocketConnection.Send(Encoding.UTF8.GetBytes("Client is not online."));

                                   // Send data to client.
                                   //incomingSocketConnection.Send(Encoding.UTF8.GetBytes(readableData));
                               }
                               catch (Exception e)
                               {
                                   Console.WriteLine(e.Message);
                               }
                           }
                           catch (Exception)
                           {
                               incomingSocketConnection.Send(Encoding.UTF8.GetBytes("Message was in wrong format."));
                           }
                       }
                   }
               });
            }
        }

        private protected void StartListeningOnXmlProtocol_Thread()
        {

        }

        private protected SocketMessage FormatMessage(string rawData)
        {
            try
            {
                // Decodes Data.
                //string data = Encoding.UTF8.GetString(rawData);

                // Define separator.
                char separator = ':';

                // Formatted data.
                string[] formattedData = rawData.Split(separator);

                // Return new User.
                return new SocketMessage(formattedData[0], formattedData[1], formattedData[2], formattedData[3], formattedData[4], formattedData[5]);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

    class SocketMessage
    {
        private string _nickName;
        private string _senderHostName;
        private string _senderIpAddress;
        private string _receiverHostName;
        private string _receiverIpAddress;
        private string _chatMessage;

        public string NickName { get => _nickName; set => _nickName = value; }
        public string SenderHostName { get => _senderHostName; set => _senderHostName = value; }
        public string SenderIpAddress { get => _senderIpAddress; set => _senderIpAddress = value; }
        public string ReceiverHostName { get => _receiverHostName; set => _receiverHostName = value; }
        public string ReceiverIpAddress { get => _receiverIpAddress; set => _receiverIpAddress = value; }
        public string ChatMessage { get => _chatMessage; set => _chatMessage = value; }

        public SocketMessage()
        {

        }

        public SocketMessage(string nickName, string senderHostName, string senderIpAddress, string receiverHostName, string receiverIpAddress, string chatMessage)
        {
            _nickName = nickName;
            _senderHostName = senderHostName;
            _senderIpAddress = senderIpAddress;
            _receiverHostName = receiverHostName;
            _receiverIpAddress = receiverIpAddress;
            _chatMessage = chatMessage;
        }

        public override string ToString()
        {
            return $"{NickName} : {SenderHostName} : {SenderIpAddress} : {ReceiverHostName} : {ReceiverIpAddress} : {ChatMessage}";
        }
    }

    class SocketClient
    {
        private string _ipAddress;
        private Socket _userSocket;
        private bool _isConnected;

        public string IpAddress { get => _ipAddress; set => _ipAddress = value; }
        public Socket ClientSocket { get => _userSocket; set => _userSocket = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }

        public SocketClient()
        {

        }

        public SocketClient(string ipAddress, Socket clientSocket, bool isConnected)
        {
            _ipAddress = ipAddress;
            _userSocket = clientSocket;
            _isConnected = isConnected;
        }
    }

    class Logger
    {
        public void Log(LogSeverity logSeverity, string message)
        {
            switch (logSeverity)
            {
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[INF]");
                    break;
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[CRI]");
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[WAR]");
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[VER]");
                    break;
                case LogSeverity.System:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[SYS]");
                    break;
            }
            Console.WriteLine($"[{DateTimeOffset.UtcNow:hh:mm:ss}] {message}");
            Console.ResetColor();
        }
    }

    public enum LogSeverity
    {
        Info,
        Critical,
        Warning,
        Verbose,
        System
    }
}
