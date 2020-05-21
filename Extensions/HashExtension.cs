using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Promotion.Extensions
{
    public class HashExtension
    {
        public static string Create(string password, string salt)
        {
            string hased = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            return hased;
        }

        public static bool Validate(string password, string salt, string hash)
            => Create(password, salt) == hash;
    }
}