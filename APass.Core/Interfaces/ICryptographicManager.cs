
namespace APass.Core.Interfaces
{
    public interface ICryptographicManager
    {
        byte[] CryptographicRandomNumberGenerator(int length);
        byte[] DeriveKey(string password, byte[] salt, int iterations, int keySize);
        byte[] Encrypt(byte[] dataToEncrypt, byte[] key);
        byte[] Decrypt(byte[] dataToDecrypt, byte[] key);

    }
}
