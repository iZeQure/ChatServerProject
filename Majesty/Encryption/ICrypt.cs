namespace Majesty.Encryption
{
    interface ICrypt
    {
        byte[] Decrypt(byte[] toBeDecrypted);
        byte[] Encrypt(byte[] toBeEncrypted);
    }
}
