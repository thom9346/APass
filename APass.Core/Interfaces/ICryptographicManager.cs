using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Interfaces
{
    public interface ICryptographicManager
    {
        byte[] CryptographicRandomNumberGenerator(int length);
    }
}
