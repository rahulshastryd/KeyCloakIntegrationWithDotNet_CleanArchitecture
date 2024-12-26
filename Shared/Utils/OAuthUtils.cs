using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Shared.Utils
{
    public static class OAuthUtils
    {
        public static string GenerateRandomState(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new char[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var buffer = new byte[length];
                rng.GetBytes(buffer);
                for (int i = 0; i < length; i++)
                {
                    random[i] = chars[buffer[i] % chars.Length];
                }
            }
            return new string(random);
        }
    }
}
