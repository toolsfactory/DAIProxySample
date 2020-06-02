using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Globalization;

namespace DAIProxy.Core
{
    public class DataEncoder
    {
        private string _rawData;
        private string _key;

        public DataEncoder(string rawData, string key)
        {
            _rawData = rawData;
            _key = key;
        }

        public string EncryptAndEncode()
        {
            byte[] encrypted;
            try
            {
                encrypted = Encrypt(_rawData);
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

        private byte[] Encrypt(string text)
        {
            var key = Encoding.UTF8.GetBytes(_key);
            byte[] iv = new byte[16];

            using var aes = Aes.Create();
            using var encryptor = aes.CreateEncryptor(key, iv);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                sw.Write(text);
            }
            return ms.ToArray();
        }

        private string Base62Encode(byte[] data)
        {
            return data.ToBase62();
        }
    }
}
