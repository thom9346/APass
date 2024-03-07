using APass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Services
{
    public class CryptographicManager : ICryptographicManager
    {
        public byte[] CryptographicRandomNumberGenerator(int length)
        {
            byte[] randomBytes = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // fill the array with cryptographically secure random bytes
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
