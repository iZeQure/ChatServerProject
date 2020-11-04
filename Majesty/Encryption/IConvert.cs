using Majesty.Messages;

namespace Majesty.Encryption
{
    interface IConvert
    {
        IMessage ConvertMessage(byte[] messageBytes);
        byte[] ConvertMessageBack(IMessage message);
    }
}
