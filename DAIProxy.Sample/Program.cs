using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DAIProxy.Core;

namespace DAIProxy.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowMenu();
        }

        private static void ShowMenu()
        {
            bool exit = false;
            var menu = new EasyConsole.Menu()
                .Add("Generate & Save RSA Key", () => GenerateKey())
                .Add("Load RSA Key", () => LoadKey())
                .Add("Run signing sample", () => RunSigningSample())
                .Add("Exit", () => exit = true);

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("DAIProxy Sample App");
                Console.WriteLine("===================");
                Console.WriteLine();
                menu.Display();
            }
        }


        private static void LoadKey()
        {
            
            throw new NotImplementedException();
        }

        private static void RunSigningSample()
        {
            var input = EasyConsole.Input.ReadString("Url to encrypt: ");
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = CreateParametersString(DateTime.UtcNow, input, IPAddress.Parse("172.168.2.1"));
            var encryptedString = AesOperation.EncryptString(key, data);
            Console.WriteLine($"encrypted string = {encryptedString}");
            var decryptedString = AesOperation.DecryptString(key, encryptedString);
            Console.WriteLine($"decrypted string = {decryptedString}");
//            var sampleString = Convert.ToBase64String(new byte[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37, 39 });
//            var decryptedSample = AesOperation.DecryptString(key, sampleString);
//            Console.WriteLine($"decrypted sample = {decryptedSample}");
            var result = new ProxyRequestData(decryptedString);
            Console.WriteLine($"ValidUntil: {result.ValidUntil}");
            Console.WriteLine($"Url: {result.Url}");
            Console.WriteLine($"IP: {result.IP}");
            Console.ReadKey();
            }

        private static void GenerateKey()
        {
        }

        private static string CreateParametersString(DateTime time, string url, IPAddress ip)
        {
            var datestr = time.ToUniversalTime().ToString("O");
            var encurl = System.Web.HttpUtility.UrlEncode(url);
            var ipstr = ip.ToString();
            return datestr + ";" + encurl + ";" + ipstr;
        }

        private static (DateTime time, string url, string ip) ParseParameterString(string data)
        {
            var parts = data.Split(";");
            foreach (var item in parts)
            {
                Console.WriteLine($"Item : {item}");
            }
            if (parts.Count()==3)
            {
                var ts_ok = DateTime.TryParse(parts[0], out var time);
                var url = System.Web.HttpUtility.UrlDecode(parts[1]);
                var ip_ok = IPAddress.TryParse(parts[2], out var ip);
                if (!ts_ok || !ip_ok)
                    throw new Exception("parsing failed");

                return (time, url, parts[2]);
            }
            throw new Exception("parsing failed");
        }

    }

    public class AesOperation
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
