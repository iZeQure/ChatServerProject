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
        private static List<User> SocketUsers = new List<User>();

        public CommunicationHandler()
        {
            new Thread(StartListeningOnSimpleProtocol_Thread)
            {
                Name = "Simple Protocol Thread",
            }.Start();

            Console.WriteLine("Waiting for protocol to start..");
        }

        private protected void StartListeningOnSimpleProtocol_Thread()
        {
            Console.WriteLine("{0} is now ready.", Thread.CurrentThread.Name);

            // Initialzie new endpoint.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.16.21.50"), 50001);

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

                bool isClientConnected = false;

                if (incomingSocketConnection.Connected)
                {
                    Console.WriteLine($"Client Connected : {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");
                    isClientConnected = true;

                    if (SocketUsers.Any(ip => ip.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString()))
                    {
                        var getClient = SocketUsers.FirstOrDefault(u => u.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString());
                        if (getClient != null) getClient.IsConnected = true;
                    }
                    else
                        SocketUsers.Add(new User(((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString(), incomingSocketConnection, true));
                }

                // Run new thread on the current socket connection.
                Task.Run(() =>
               {
                   while (true)
                   {
                       // Initialize a buffer to hold data.
                       byte[] dataBuffer = new byte[1024];

                       // Define an object to store the data in.
                       string readableData = null;

                       // Read incoming data from the connected client.
                       while (isClientConnected)
                       {
                           // Get the number of bytes received from the socket.
                           int bytesReceived = incomingSocketConnection.Receive(dataBuffer);

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
                               Console.WriteLine($"Client Disconnected : {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");

                               // Set the current socket user to disconnected.
                               var getClient = SocketUsers.FirstOrDefault(u => u.IpAddress == ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString());
                               if (getClient != null) getClient.IsConnected = false;
                           }
                       }

                       // Check on data if client is connected.
                       if (isClientConnected)
                       {
                           Console.ForegroundColor = ConsoleColor.Yellow;
                           Console.WriteLine($"[INF] {DateTimeOffset.UtcNow:hh:mm:ss} Received: {FormatMessage(readableData)}");
                           Console.ResetColor();
                           //Console.WriteLine($"Data Received : {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");

                           // Send data to client.
                           incomingSocketConnection.Send(Encoding.UTF8.GetBytes(readableData));
                           Console.WriteLine($"Data Sent : {((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address}");
                       }
                   }
               });
            }
        }

        private protected SocketMessage FormatMessage(string rawData)
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

    class User
    {
        private string _ipAddress;
        private Socket _userSocket;
        private bool _isConnected;

        public string IpAddress { get => _ipAddress; set => _ipAddress = value; }
        public Socket UserSocket { get => _userSocket; set => _userSocket = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }

        public User()
        {

        }

        public User(string ipAddress, Socket userSocket, bool isConnected)
        {
            _ipAddress = ipAddress;
            _userSocket = userSocket;
            _isConnected = isConnected;
        }
    }
}
