using System;
using System.Linq;
using System.Net;

namespace DAIProxy.Core
{
    public class DataParser
    {
        public DateTime ValidUntil { get; private set; }
        public string Url { get; private set; }
        public IPAddress IP { get; private set; }

        private string[] _parts;
        public DataParser(string data)
        {
            if(String.IsNullOrWhiteSpace(data))
                throw new ParsingException("No data provided");
            SplitString(data);
            ParseTime();
            ParseUrl();
            ParseIP();
        }

        private void ParseTime()
        {
            var ok = DateTime.TryParse(_parts[0], out var time);
            ValidUntil = (ok) ? time : throw new ParsingException("Part 1 could not be parsed");
        }

        private void ParseUrl()
        {
            var url = System.Web.HttpUtility.UrlDecode(_parts[1]);
            Url = url;
        }

        private void ParseIP()
        {
            var ok = IPAddress.TryParse(_parts[2], out var ip);
            IP = (ok) ? ip : throw new ParsingException("Part 2 could not be parsed");
        }

        private void SplitString(string data)
        {
            _parts = data.Split(";");
            if ((_parts == null) || (_parts.Count() != 3))
                throw new ParsingException("Number of data segments invalid");
        }
    }
}
