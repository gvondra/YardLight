using YardLight.Authorization.Core.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core
{
    public sealed class ClientSecretProcessor : IClientSecretProcessor
    {
        private const string PADDING = "aqua-flaim-auth:";

        public string Create()
        {
            return string.Concat(
                Guid.NewGuid().ToString("N"),
                Guid.NewGuid().ToString("N")
                );
        }

        public static byte[] Hash(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            using (HashAlgorithm hashAlgorithm = SHA512.Create())
            {
                return hashAlgorithm.ComputeHash(
                    Encoding.Unicode.GetBytes(string.Concat(PADDING, secret.Trim()))
                    );
            }
        }

        public static bool Verify(string secret, byte[] hash)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(secret) && hash != null)
            {
                byte[] secretHash = Hash(secret);
                if (secretHash.Length == hash.Length)
                {
                    bool isMatch = true;
                    int i = 0;
                    while (isMatch && i < hash.Length)
                    {
                        if (secretHash[i] != hash[i])
                        {
                            isMatch = false;
                        }
                        i += 1;
                    }
                    result = isMatch;
                }
            }
            return result;
        }
    }
}
