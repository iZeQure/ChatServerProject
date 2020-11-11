using Majesty.Packages;

namespace Majesty.Encryption
{
    interface IConvert
    {
        IPackage ConvertMessage(byte[] messageBytes);
        byte[] ConvertMessageBack(IPackage package);
    }
}
