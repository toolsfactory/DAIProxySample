using System;
using System.Linq;
using System.Net;

namespace DAIProxy.Core
{
    /// <summary>
    /// The ProxyRequestData class parses a string into the relevant information pieces
    /// </summary>
    public class ProxyRequestData
    {
        public DateTime ValidUntil { get; private set; }
        public string Url { get; private set; }
        public IPAddress IP { get; private set; }

        public bool Salted {  get { return _saltAdd >= 1; } }

        private string[] _parts;
        private int _saltAdd = 0;
        /// <summary>
        /// The constructor triggers parsing the provided data.
        /// </summary>
        /// <param name="data">The data to parse as string. Format: "[optional salt];[validuntil ISO 8601];[target url urlencoded];[target ip];[salt]*". If the string has more than three chunks, it is expected that exactly one salt at the beginning exists. Number of salt chunks at the end are not limited.</param>
        /// <param name="hasSalt">Specifies if data contains three parts or additionally a leading salt part.</param>
        public ProxyRequestData(string data)
        {
            if(String.IsNullOrWhiteSpace(data))
                throw new ParsingException("No data provided");
            SplitString(data);
            ParseTime();
            ParseUrl();
            ParseIP();
        }

        private void SplitString(string data)
        {
            _parts = data.Split(";");
            var cnt = (_parts == null) ? 0 : _parts.Count();
            if (cnt<3)
                throw new ParsingException("Number of data segments invalid");
            _saltAdd = cnt - 3;
        }

        private void ParseTime()
        {
            var ok = DateTime.TryParse(_parts[0 + _saltAdd], out var time);
            ValidUntil = (ok) ? time : throw new ParsingException("Part 1 could not be parsed");
        }

        private void ParseUrl()
        {
            Url = System.Web.HttpUtility.UrlDecode(_parts[1 + _saltAdd]);
            if (!CheckUrl(Url))
                throw new ParsingException("Part 2 could not be parsed");
        }

        private void ParseIP()
        {
            var ok = IPAddress.TryParse(_parts[2 + _saltAdd], out var ip);
            IP = (ok) ? ip : throw new ParsingException("Part 3 could not be parsed");
        }

        private bool CheckUrl(string url)
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
