using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Globalization;

namespace DAIProxy.Core
{
    public static class DataEncoder
    {

        public static string EncryptAndEncode(string rawData, string key)
        {
            byte[] encrypted;
            try
            {
                encrypted = Encrypt(rawData, key);
            }
            catch (Exception ex)
            {
                throw new EncryptionException("Encrypting the data failed", ex);
            }

            try
            {
                return Base62Encode(encrypted);
            }
            catch (Exception ex)
            {
                throw new EncodingException("Encoding the data failed", ex);
            }
        }

        private static byte[] Encrypt(string text, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // 16 byte empty IV (all zero)
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                sw.Write(text);
            }
            return ms.ToArray();
        }

        private static string Base62Encode(byte[] data)
        {
            return data.ToBase62();
        }
    }
}
