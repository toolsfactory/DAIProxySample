using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;

namespace DAIProxy.Core
{
    public static class DataDecoder
    {
        public static string DecodeAndDecrypt(string rawData, string key)
        {
            byte[] decoded;

            try
            {
                decoded = Base62Decode(rawData);
            }
            catch (Exception ex)
            {
                throw new DecodingException("Decoding the data failed", ex);
            }

            try
            {
                return Decrypt(decoded, key);
            }
            catch (Exception ex)
            {
                throw new DecryptionException("Encrypting the data failed", ex);
            }

        }

        private static byte[] Base62Decode(string text)
        {
            return text.FromBase62();
        }

        private static string Decrypt(byte[] data, string key)
        {
            var iv = new byte[16];

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                var result = string.Empty;
                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(data))
                {
                    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    using var sr = new StreamReader(cs);
                    result = sr.ReadToEnd();
                }
                return result;
            }
        }

    }
}
