using System;
using System.Linq;
using System.Net;

namespace DAIProxy.Core
{
    public static class ProxyRequestDataDecoder
    {
        /// <summary>
        /// The function triggering decoding, decryption and deserializationof the provided data.
        /// </summary>
        /// <param name="data">The data to parse as string. Format: "[optional salt];[validuntil ISO 8601];[target url urlencoded];[target ip];[salt]*". If the string has more than three chunks, it is expected that exactly one salt at the beginning exists. Number of salt chunks at the end are not limited.</param>
        /// <param name="key">The key used to decrypt</param>
        public static ProxyRequestData CreateFromEncodedAndEncrypted(string rawData, string key)
        {
            if (String.IsNullOrWhiteSpace(rawData))
                throw new ParsingException("No data provided");

            var data = DataDecoder.DecodeAndDecrypt(rawData, key);

            return CreateFrom(data);
        }

        public static ProxyRequestData CreateFrom(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
                throw new ParsingException("No data provided");
            var result = SplitString(data);
            var start = result.salt ? 1 : 0;
            var validuntil = ParseTime(result.parts[start]);
            var url = ParseUrl(result.parts[start + 1]);
            var ip = ParseIP(result.parts[start + 2]);

            return new ProxyRequestData() { ValidUntil = validuntil, IP = ip, Url = url , Salted = result.salt};
        }

        private static (string[] parts, bool salt) SplitString(string data)
        {
            var parts = data.Split(";");
            var cnt = (parts == null) ? 0 : parts.Count();
            return (cnt >= 3) ? (parts, cnt>3) : throw new ParsingException("Number of data segments invalid");
        }

        private static DateTime ParseTime(string data)
        {
            var ok = DateTime.TryParse(data, out var time);
            return  (ok) ? time : throw new ParsingException("Part 1 could not be parsed");
        }

        private static string ParseUrl(string data)
        {
            var url = System.Web.HttpUtility.UrlDecode(data);
            return (CheckUrl(url)) ? url : throw new ParsingException("Part 2 could not be parsed");

        }

        private static IPAddress ParseIP(string data)
        {
            var ok = IPAddress.TryParse(data, out var ip);
            return  (ok) ? ip : throw new ParsingException("Part 3 could not be parsed");
        }

        private static bool CheckUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
