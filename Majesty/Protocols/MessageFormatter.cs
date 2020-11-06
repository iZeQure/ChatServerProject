using System;
using System.Net;
using System.Text;
using Majesty.Packages;
using Majesty.Users;

namespace Majesty.Protocols
{
    class MessageFormatter
    {
        private readonly IUserBaseFactory _userFactory = new UserFactory();
        private readonly IPackageFactory _packageFactory = new PackageFactory();


        public IUserBase FormatMessage(byte[] messageBytes)
        {
            /*
            Format
            Nick Name / Account Name :
            Sender Hostname :
            Sender IP :
            Receiver Hostname :
            Receiver IP :
            User Message :
            */
            
            // Call the SocketUser factory to create a new IUserBase and cast that to a SocketUser
            var user = _userFactory.Create("SocketUser") as SocketUser;
            user.UserPackage = _packageFactory.Create("UserPackage") as UserPackage;

            // Set the user.UserMessage.Message to the original message for when we forward the message
            try
            {
                user.UserPackage.PackageBytes = messageBytes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            // Split the message so we get the fields in the message
            string[] messageFields = Encoding.UTF8.GetString(messageBytes).Split(":");
            user.NickName = messageFields[0];
            user.SenderHostname = messageFields[1];
            user.DestinationFrom = IPEndPoint.Parse(messageFields[2]);
            user.ReceiverHostname = messageFields[3];
            user.DestinationTo = IPEndPoint.Parse(messageFields[4]);
            
            return user;
        }
    }
}