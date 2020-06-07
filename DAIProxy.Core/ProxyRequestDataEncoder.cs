using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace DAIProxy.Core
{
    public static class ProxyRequestDataEncoder
    {
        public static string EncodeAndEncrypt(ProxyRequestData data, string key)
        {
            return EncodeAndEncrypt(data.ValidUntil, data.Url, data.IP, key, data.Debug);
        }

        public static string EncodeAndEncrypt(DateTime time, string url, IPAddress ip, string key,  bool debug = false)
        {
            var data = CreateParamterString(time, url, ip, debug);
            return DataEncoder.EncryptAndEncode(data, key);
        }

        private static string CreateParamterString(DateTime time, string url, IPAddress ip, bool debug)
        {
            var builder = new StringBuilder();
            // Salt
            builder.Append(RandomString(16, true));
            builder.Append(";");
            // Validity
            builder.Append(time.ToUniversalTime().ToString("O"));
            builder.Append(";");
            // URL
            builder.Append(System.Web.HttpUtility.UrlEncode(url));
            builder.Append(";");
            // IP
            builder.Append(ip.ToString());
            // Debug
            if (debug)
            {
                builder.Append(";");
                builder.Append("debug");
            }
            return builder.ToString();
        }

        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
