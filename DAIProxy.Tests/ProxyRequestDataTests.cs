using NUnit.Framework;

namespace DAIProxy.Core.Tests
{
    [TestFixture()]
    public class ProxyRequestDataTests
    {
        
        [Test]
        public void TestNullInput()
        {
            Assert.Throws<ParsingException>(() => ProxyRequestDataDecoder.CreateFrom(null));
        }

        [TestCase("")]
        [TestCase("1341234")]
        [TestCase("1341234;4576457")]
        public void TestInputWithInvalidChunkCount(string arg)
        {
            Assert.Throws<ParsingException>(() => ProxyRequestDataDecoder.CreateFrom(arg));
        }

        [TestCase("2020-05-31T15:52:15.Z")]
        [TestCase("2020-05-31T15:99:15.10Z")]
        [TestCase("2020-50-31T15:52:15.Z")]
        [TestCase("2020\05-31T15:52:15.10Z")]
        public void TestInputWithInvalidTimestamp(string arg)
        {
            Assert.Throws<ParsingException>(() => ProxyRequestDataDecoder.CreateFrom(arg+ ";http%3a%2f%2fwww.heise.de%2fq%3fd%3dhello+world;192.168.2.1"));
        }

        [TestCase("http%2f%2fwww.heise.de%2fq%3fd%3dhello+world")]
        [TestCase("http%3a/fwww.heise.de%2fq%3fd%3dhello+world")]
        [TestCase("http%3a%2f%2fwww.heise.de/q%3fd%3dhello+world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq?d%3dhello+world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq%3fd:hello+world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq%3fd%3dhello world")]
        public void TestInputWithInvalidUrl(string arg)
        {
            Assert.Throws<ParsingException>(() => ProxyRequestDataDecoder.CreateFrom("2020-05-31T15:52:15.100Z;" + arg + ";192.168.2.1"));
        }

        [TestCase("hello")]
        [TestCase("134765238745")]
        [TestCase("192.168.2.300")]
        [TestCase("192.168.300.1")]
        [TestCase("192.300.2.1")]
        [TestCase("300.168.2.1")]
        public void TestInputWithInvalidIp(string arg)
        {
            Assert.Throws<ParsingException>(() => ProxyRequestDataDecoder.CreateFrom("2020-05-31T15:52:15.100Z;http%3a%2f%2fwww.heise.de%2fq%3fd%3dhello+world;" + arg));
        }

        [TestCase("http:%2f%2fwww.heise.de%2fq%3fd%3dhello+world" , "http://www.heise.de/q?d=hello world")]
        [TestCase("http%3a//www.heise.de%2fq%3fd%3dhello+world", "http://www.heise.de/q?d=hello world")]
        [TestCase("http%3a%2f%2fwww.heise.de/q%3fd%3dhello+world", "http://www.heise.de/q?d=hello world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq?d%3dhello+world", "http://www.heise.de/q?d=hello world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq%3fd=hello+world", "http://www.heise.de/q?d=hello world")]
        [TestCase("http%3a%2f%2fwww.heise.de%2fq%3fd%3dhello world", "http://www.heise.de/q?d=hello world")]
        public void TestInputWithCorrectUrl(string arg, string expected)
        {
            var result = ProxyRequestDataDecoder.CreateFrom("2020-05-31T15:52:15.100Z;" + arg + ";192.168.2.1");
            Assert.AreEqual(expected, result.Url);
        }


    }
}