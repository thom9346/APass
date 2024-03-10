using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Services
{
    public static class SecureSessionService
    {
        private static byte[] _dek = null;

        public static void StoreDEK(byte[] dek)
        {
            ClearDEK(); //Ensure any existing DEK is cleared
            _dek = dek;
        }

        public static byte[] GetDEK()
        {
            if (_dek == null) throw new InvalidOperationException("DEK not set or session expired.");
            return _dek;
        }

        public static void ClearDEK()
        {
            if (_dek != null)
            {
                //erase the DEK from memory
                Array.Clear(_dek, 0, _dek.Length);
                _dek = null;
            }
        }
    }
}
