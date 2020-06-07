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
        public DateTime ValidUntil { get; set; }
        public string Url { get; set; }
        public IPAddress IP { get; set; }
        public bool Salted { get; set; }
    }
}
