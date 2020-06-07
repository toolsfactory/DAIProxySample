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
            return EncodeAndEncrypt(data.ValidUntil, data.Url, data.IP, key, data.Salted, data.Debug);
        }

        public static string EncodeAndEncrypt(DateTime time, string url, IPAddress ip, string key, bool salted = false,  bool debug = false)
        {
            var data = CreateParamterString(time, url, ip, salted, debug);
            return DataEncoder.EncryptAndEncode(data, key);
        }

        private static string CreateParamterString(DateTime time, string url, IPAddress ip, bool salted, bool debug)
        {
            var builder = new StringBuilder();
            if (salted)
            {
                builder.Append(RandomString(16, true));
                builder.Append(";");
            }
            builder.Append(time.ToUniversalTime().ToString("O"));
            builder.Append(";");
            builder.Append(System.Web.HttpUtility.UrlEncode(url));
            builder.Append(";");
            builder.Append(ip.ToString());
            if (debug)
            {
                builder.Append("debug");
            }
            if (salted)
            {
                builder.Append(";");
                builder.Append(RandomString(16, true));
                builder.Append(";");
                builder.Append(RandomString(16, true));
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
