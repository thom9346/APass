using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Services
{
    //public static class SessionManager
    //{
    //    public static string CurrentSessionToken { get; private set; }

    //    public static void CreateNewSession()
    //    {
    //        // Generate a secure, random session token
    //        using (var rng = new RNGCryptoServiceProvider())
    //        {
    //            byte[] tokenData = new byte[32]; // 256 bits
    //            rng.GetBytes(tokenData);
    //            CurrentSessionToken = Convert.ToBase64String(tokenData);
    //        }
    //    }

    //    public static bool ValidateSession(string token)
    //    {
    //        return CurrentSessionToken == token;
    //    }

    //    public static void EndSession()
    //    {
    //        CurrentSessionToken = null;
    //    }
    //}
}
