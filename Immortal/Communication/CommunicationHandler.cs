﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;

namespace Immortal.Communication
{
    class CommunicationHandler
    {
        private static List<SocketClient> SocketClients = new List<SocketClient>();
        private readonly object _socketClientLocker = new object();
        private readonly Logger _logger = new Logger();
        private readonly IConfiguration _configuration;
        private readonly SymmetricCrypto _symmetricCrypto;

        public CommunicationHandler(IConfiguration configuration)
        {
            // Set configuration
            _configuration = configuration;

            _symmetricCrypto = new SymmetricCrypto(_configuration["CryptoConfig:Symmetrical:key"], _configuration["CryptoConfig:Symmetrical:iv"]);

            var simpleProtocolThread = new Thread(StartListeningOnSimpleProtocol_Thread)
            {
                Name = "Simple Protocol Thread",
            };

            var xmlProtocolThread = new Thread(StartListeningOnXmlProtocol_Thread)
            {
                Name = "XML Protocol Thread"
            };
            
            var symetricProtocolThread = new Thread(StartListeningOnSymetricProtocol_Thread)
            {
                Name = "Symtetric Protocol Thread"
            };

            _logger.Log(LogSeverity.System, "Protocols is starting..");

            simpleProtocolThread.Start();
            xmlProtocolThread.Start();
            symetricProtocolThread.Start();
        }

        private protected void StartListeningOnSimpleProtocol_Thread()
        {
            _logger.Log(LogSeverity.System, $"{Thread.CurrentThread.Name} is ready!");

            // Initialzie new endpoint.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_configuration["SimpleConfig:server_ip"]), int.Parse(_configuration["SimpleConfig:server_port"]));

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

                // When the user connects set clientEndPointIpAddress to the ip as a string
                var clientEndPointIpAddress =
                    ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString();

                // Run new thread on the current socket connection.
                Task.Run(async () =>
                {
                    bool isClientConnected = false;

                    // Check whether a client is connected to the endpoint socket.
                    if (incomingSocketConnection.Connected)
                    {
                        // Log the client who has connected.
                        _logger.Log(LogSeverity.Info, $"Client Connected: {clientEndPointIpAddress}");
                        isClientConnected = true;

                        // Broadcast client states.
                        await BroadcastClientState(clientEndPointIpAddress);

                        // Check ifthe client connected exists.
                        lock (_socketClientLocker)
                        {
                            if (SocketClients.Any(ip => ip.IpAddress == clientEndPointIpAddress))
                            {
                                // Get the client, and set their state to connected is true.
                                var getClient =
                                    SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                if (getClient != null)
                                {
                                    getClient.ClientSocket.Disconnect(false);
                                    getClient.IsConnected = true;
                                    getClient.ClientSocket = incomingSocketConnection;
                                }
                            }
                            // Add the client to the socket clients.
                            else
                                SocketClients.Add(new SocketClient(clientEndPointIpAddress, incomingSocketConnection,
                                    true));
                        }                        
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
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
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
                                _logger.Log(LogSeverity.Info, $"Client Disconnected: {clientEndPointIpAddress}");
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
                            }
                        }

                        // Check on data if client is connected.
                        if (isClientConnected)
                        {
                            try
                            {
                                // Store the message sent from the client, if fails, the format was incorrect.
                                var clientMessage = await SplitMessage(readableData);

                                // Log the received message.
                                _logger.Log(LogSeverity.Info, $"Message Received: {clientMessage}");

                                try
                                {
                                    lock (_socketClientLocker)
                                    {
                                        // Check if any clients exists and is online, with the provided information sent from a client.
                                        if (SocketClients.Any(user =>
                                            user.IpAddress == clientMessage.ReceiverIpAddress && user.IsConnected))
                                        {
                                            // Get the client to communicate with.
                                            var getClient = SocketClients.FirstOrDefault(user =>
                                                user.IpAddress == clientMessage.ReceiverIpAddress);

                                            // Check that the client isn't null.
                                            if (getClient != null)
                                            {
                                                // Deliver message to receiver client.
                                                getClient.ClientSocket.Send(
                                                    Encoding.UTF8.GetBytes(
                                                        $"{clientMessage.NickName} : {clientMessage.ChatMessage}"));

                                                // Notify client that their message has been delivered.
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message has been delivered."));
                                            }
                                            // Notify the client, that their message couldn't be delivered.
                                            else
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message couldn't be delivered."));
                                        }
                                        // Notify client with a message if the client isn't online or found.
                                        else
                                            incomingSocketConnection.Send(
                                                Encoding.UTF8.GetBytes("Client is not online."));
                                    }

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
            _logger.Log(LogSeverity.System, $"{Thread.CurrentThread.Name} is ready!");

            // Initialzie new endpoint.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_configuration["XmlConfig:server_ip"]), int.Parse(_configuration["XmlConfig:server_port"]));

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

                // When the user connects set clientEndPointIpAddress to the ip as a string
                var clientEndPointIpAddress =
                    ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString();

                // Run new thread on the current socket connection.
                Task.Run(async () =>
                {
                    bool isClientConnected = false;

                    // Check whether a client is connected to the endpoint socket.
                    if (incomingSocketConnection.Connected)
                    {
                        // Check ifthe client connected exists.
                        lock (_socketClientLocker)
                        {
                            if (SocketClients.Any(ip => ip.IpAddress == clientEndPointIpAddress))
                            {
                                // Get the client, and set their state to connected is true.
                                var getClient =
                                    SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                if (getClient != null)
                                {
                                    getClient.ClientSocket.Disconnect(false);
                                    getClient.IsConnected = true;
                                    getClient.ClientSocket = incomingSocketConnection;
                                }
                            }
                            // Add the client to the socket clients.
                            else
                                SocketClients.Add(new SocketClient(clientEndPointIpAddress, incomingSocketConnection,
                                    true));
                        }

                        // Log the client who has connected.
                        _logger.Log(LogSeverity.Info, $"Client Connected: {clientEndPointIpAddress}");
                        isClientConnected = true;

                        // Broadcast client states.
                        await BroadcastClientState(clientEndPointIpAddress);
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
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
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
                                _logger.Log(LogSeverity.Info, $"Client Disconnected: {clientEndPointIpAddress}");
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
                            }
                        }

                        // Check on data if client is connected.
                        if (isClientConnected)
                        {
                            try
                            {
                                // Store the message sent from the client, if fails, the format was incorrect.
                                var clientMessage = await FormatMessageFromXml(readableData);

                                // Log the received message.
                                _logger.Log(LogSeverity.Info, $"Message Received: {clientMessage}");

                                try
                                {
                                    lock (_socketClientLocker)
                                    {
                                        // Check if any clients exists and is online, with the provided information sent from a client.
                                        if (SocketClients.Any(user =>
                                            user.IpAddress == clientMessage.ReceiverIpAddress && user.IsConnected))
                                        {
                                            // Get the client to communicate with.
                                            var getClient = SocketClients.FirstOrDefault(user =>
                                                user.IpAddress == clientMessage.ReceiverIpAddress);

                                            // Check that the client isn't null.
                                            if (getClient != null)
                                            {
                                                // Deliver message to receiver client.
                                                getClient.ClientSocket.Send(
                                                    Encoding.UTF8.GetBytes(
                                                         FormatMessageToXml(clientMessage).Result));

                                                // Notify client that their message has been delivered.
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message has been delivered."));
                                            }
                                            // Notify the client, that their message couldn't be delivered.
                                            else
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message couldn't be delivered."));
                                        }
                                        // Notify client with a message if the client isn't online or found.
                                        else
                                            incomingSocketConnection.Send(
                                                Encoding.UTF8.GetBytes("Client is not online."));
                                    }

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

        private protected void StartListeningOnSymetricProtocol_Thread()
        {
            _logger.Log(LogSeverity.System, $"{Thread.CurrentThread.Name} is ready!");
            
            // Initialzie new endpoint.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_configuration["CryptoConfig:server_ip"]), int.Parse(_configuration["CryptoConfig:server_port"]));

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

                // When the user connects set clientEndPointIpAddress to the ip as a string
                var clientEndPointIpAddress =
                    ((IPEndPoint)incomingSocketConnection.RemoteEndPoint).Address.ToString();

                // Run new thread on the current socket connection.
                Task.Run(async () =>
                {
                    bool isClientConnected = false;

                    // Check whether a client is connected to the endpoint socket.
                    if (incomingSocketConnection.Connected)
                    {
                        try
                        {
                            string xmlCryptoInfo = FormatCryptoInfo(new CryptoInfo("AES",
                                new[]
                                {
                                    _configuration["CryptoConfig:Symmetrical:key"],
                                    _configuration["CryptoConfig:Symmetrical:iv"]
                                })).GetAwaiter().GetResult();
                            // Send cryptoinfo
                            incomingSocketConnection.Send(Encoding.UTF8.GetBytes(xmlCryptoInfo));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        
                        // Check ifthe client connected exists.
                        lock (_socketClientLocker)
                        {
                            if (SocketClients.Any(ip => ip.IpAddress == clientEndPointIpAddress))
                            {
                                // Get the client, and set their state to connected is true.
                                var getClient =
                                    SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                if (getClient != null)
                                {
                                    getClient.ClientSocket.Disconnect(false);
                                    getClient.IsConnected = true;
                                    getClient.ClientSocket = incomingSocketConnection;
                                }
                            }
                            // Add the client to the socket clients.
                            else
                                SocketClients.Add(new SocketClient(clientEndPointIpAddress, incomingSocketConnection,
                                    true));
                        }

                        // Log the client who has connected.
                        _logger.Log(LogSeverity.Info, $"Client Connected: {clientEndPointIpAddress}");
                        isClientConnected = true;

                        // Broadcast client states.
                        await BroadcastClientState(clientEndPointIpAddress);
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
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
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
                                _logger.Log(LogSeverity.Info, $"Client Disconnected: {clientEndPointIpAddress}");
                                lock (_socketClientLocker)
                                {
                                    // Set the current socket user to disconnected.
                                    var getClient =
                                        SocketClients.FirstOrDefault(u => u.IpAddress == clientEndPointIpAddress);
                                    if (getClient != null) getClient.IsConnected = false;
                                }
                            }
                        }

                        // Check on data if client is connected.
                        if (isClientConnected)
                        {
                            try
                            {
                                // Store the message sent from the client, if fails, the format was incorrect.
                                var clientMessage = await FormatMessageFromXml(_symmetricCrypto.Decrypt(readableData).Result);

                                // Log the received message.
                                _logger.Log(LogSeverity.Info, $"Message Received: {clientMessage}");

                                try
                                {
                                    lock (_socketClientLocker)
                                    {
                                        // Check if any clients exists and is online, with the provided information sent from a client.
                                        if (SocketClients.Any(user =>
                                            user.IpAddress == clientMessage.ReceiverIpAddress && user.IsConnected))
                                        {
                                            // Get the client to communicate with.
                                            var getClient = SocketClients.FirstOrDefault(user =>
                                                user.IpAddress == clientMessage.ReceiverIpAddress);

                                            // Check that the client isn't null.
                                            if (getClient != null)
                                            {
                                                var xmlMessage = FormatMessageToXml(clientMessage).GetAwaiter().GetResult();
                                                var encryptedMessage = _symmetricCrypto.Encrypt(xmlMessage).GetAwaiter().GetResult();
                                                
                                                // Deliver message to receiver client.
                                                getClient.ClientSocket.Send(Encoding.UTF8.GetBytes(encryptedMessage));

                                                // Notify client that their message has been delivered.
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message has been delivered."));
                                            }
                                            // Notify the client, that their message couldn't be delivered.
                                            else
                                                incomingSocketConnection.Send(
                                                    Encoding.UTF8.GetBytes("Message couldn't be delivered."));
                                        }
                                        // Notify client with a message if the client isn't online or found.
                                        else
                                            incomingSocketConnection.Send(
                                                Encoding.UTF8.GetBytes("Client is not online."));
                                    }

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

        private protected Task BroadcastClientState(string currentClientIpAddress)
        {
            Task.Run(() =>
            {
                lock (_socketClientLocker)
                {
                    foreach (var client in SocketClients)
                    {
                        if (client.IsConnected)
                        {
                            client.ClientSocket.Send(Encoding.UTF8.GetBytes($"{currentClientIpAddress} is online!"));
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        private protected Task<String> FormatCryptoInfo(CryptoInfo cryptoInfo)
        {
            try
            {
                // Use a new string writer to hold the xml from the serializer
                using (var sw = new StringWriter())
                {
                    // Create a new serializer with the type of CryptoInfo
                    XmlSerializer ser = new XmlSerializer(typeof(CryptoInfo));
                    // Serialize the object as xml to the string writer
                    ser.Serialize(sw, cryptoInfo);
                    // Return the string held in string writer
                    return Task.FromResult(sw.ToString());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        private protected Task<String> FormatMessageToXml(SocketMessage socketMessage)
        {
            try
            {
                // Use a new string writer to hold the xml from the serializer
                using (var sw = new StringWriter())
                {
                    // Create a new serializer with the type of SocketMessage
                    XmlSerializer ser = new XmlSerializer(typeof(SocketMessage));
                    // Serialize the object as xml to the string writer
                    ser.Serialize(sw, socketMessage);
                    // Return the string held in string writer
                    return Task.FromResult(sw.ToString());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private protected Task<SocketMessage> FormatMessageFromXml(string rawData)
        {
            try
            { 
                // Use a new String Reader to hold the rawData
                using (var sr = new StringReader(rawData))
                {
                    // Create a new XML Serializer of the SocketMessage type
                    XmlSerializer ser = new XmlSerializer(typeof(SocketMessage));
                    // Return the socket message object using the serializer with the string reader
                    return Task.FromResult((SocketMessage)ser.Deserialize(sr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private protected Task<SocketMessage> SplitMessage(string rawData)
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
                return Task.FromResult(new SocketMessage(formattedData[0], formattedData[1], formattedData[2], formattedData[3],
                    formattedData[4], formattedData[5]));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class SocketMessage
    {
        private string _nickName;
        private string _senderHostName;
        private string _senderIpAddress;
        private string _receiverHostName;
        private string _receiverIpAddress;
        private string _chatMessage;

        public string NickName
        {
            get => _nickName;
            set => _nickName = value;
        }

        public string SenderHostName
        {
            get => _senderHostName;
            set => _senderHostName = value;
        }

        public string SenderIpAddress
        {
            get => _senderIpAddress;
            set => _senderIpAddress = value;
        }

        public string ReceiverHostName
        {
            get => _receiverHostName;
            set => _receiverHostName = value;
        }

        public string ReceiverIpAddress
        {
            get => _receiverIpAddress;
            set => _receiverIpAddress = value;
        }

        public string ChatMessage
        {
            get => _chatMessage;
            set => _chatMessage = value;
        }

        public SocketMessage()
        {
        }

        public SocketMessage(string nickName, string senderHostName, string senderIpAddress, string receiverHostName,
            string receiverIpAddress, string chatMessage)
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
            return
                $"{NickName} : {SenderHostName} : {SenderIpAddress} : {ReceiverHostName} : {ReceiverIpAddress} : {ChatMessage}";
        }
    }

    class SocketClient
    {
        private string _ipAddress;
        private Socket _userSocket;
        private bool _isConnected;

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public Socket ClientSocket
        {
            get => _userSocket;
            set => _userSocket = value;
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => _isConnected = value;
        }

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
        private protected readonly object _logLocker = new object();

        public void Log(LogSeverity logSeverity, string message)
        {
            // Lock while priting on the console to avoid doing multiple console actions at once and getting wrong output
            lock (_logLocker)
            {
                // Set the console color based on the logSeverity
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
                // Write the message to the console with time prefix
                Console.WriteLine($"[{DateTimeOffset.UtcNow:hh:mm:ss}] {message}");
                Console.ResetColor();
            }
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

    public class CryptoInfo
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string[] Keys
        {
            get => _keys;
            set => _keys = value;
        }

        private string _name;
        private string[] _keys;
         
        public CryptoInfo(string name, string[] keys)
        {
            _name = name;
            _keys = keys;
        }

        public CryptoInfo()
        {
        }
    }
    
    class SymmetricCrypto
    {
        private string _key;
        private string _iv;
        private SymmetricAlgorithm _symmetricAlgorithm;
        
        public SymmetricCrypto(string key, string iv)
        {
            _key = key;
            _iv = iv;
            
            _symmetricAlgorithm = Aes.Create();
            
            _symmetricAlgorithm.Key = Convert.FromBase64String(_key);
            _symmetricAlgorithm.IV = Convert.FromBase64String(_iv);
        }

        public Task<string> Decrypt(string rawDataToDecrypt)
        {
            byte[] rawDataToDecryptBytes = Convert.FromBase64String(rawDataToDecrypt);
            
            using (var memoryStream = new MemoryStream())
            {
                var cryptoStream = new CryptoStream(memoryStream, _symmetricAlgorithm.CreateDecryptor(),
                    CryptoStreamMode.Write);
                cryptoStream.Write(rawDataToDecryptBytes, 0, rawDataToDecryptBytes.Length);
                cryptoStream.FlushFinalBlock();

                return Task.FromResult(Encoding.UTF8.GetString(memoryStream.ToArray()));
            }
        }
        
        public Task<string> Encrypt(string rawDataToEncrypt)
        {
            byte[] rawDataToEncryptBytes = Encoding.UTF8.GetBytes(rawDataToEncrypt);
            
            using (var memoryStream = new MemoryStream())
            {
                var cryptoStream = new CryptoStream(memoryStream, _symmetricAlgorithm.CreateEncryptor(),
                    CryptoStreamMode.Write);
                cryptoStream.Write(rawDataToEncryptBytes, 0, rawDataToEncryptBytes.Length);
                cryptoStream.FlushFinalBlock();

                return Task.FromResult(Convert.ToBase64String(memoryStream.ToArray()));
            }
        }
    }
}