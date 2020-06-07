using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DAIProxy.Core;
using EasyConsole;

namespace DAIProxy.Sample
{
    class Program
    {
        private static ProxyRequestData _tokenData = null;
        private static string _token;
        private static string _key = "b14ca5898a4e4133bbce2ea2315a1916";
        private static Menu _mainMenu;
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide the encryption key as command line parameter.");
                Console.WriteLine("For the moment the default key is used.");
            }
            else
            {
                _key = args[1];
            }
            CreateMenu();
            ShowMenu();
        }

        private static void DisplayHeader()
        {
            Console.Clear();
            Console.WriteLine("DAIProxy Sample App");
            Console.WriteLine("===================");
            Console.WriteLine();
        }

        private static void DisplaySubheader(string header)
        {
            Console.WriteLine(header);
            Console.WriteLine("----------------");
            Console.WriteLine();
        }

    private static void ShowMenu()
        {
            while (true)
            {
                DisplayHeader();
                Console.WriteLine($"Key:         {_key}");
                Console.WriteLine();
                DisplaySubheader("Menu");
                _mainMenu.Display();
            }
        }

        private static void CreateMenu()
        {
            _mainMenu = new EasyConsole.Menu()
                .Add("Generate random key", () => GenerateKey())
                .Add("Enter key", () => SetKey())
                .Add("Generate encrypted token", () => GenerateToken())
                .Add("Decode encrypted token", () => DecodeToken())
                .Add("Send Get request", () => SendRequest().Wait())
                .Add("Exit", () => DoExit());
        }

        private static void DisplayToken()
        {
            Console.WriteLine("Token Details:");
            Console.WriteLine($"    Valid Until: {_tokenData.ValidUntil:O}");
            Console.WriteLine($"    Target URL:  {_tokenData.Url}");
            Console.WriteLine($"    Source IP:   {_tokenData.IP}");
            Console.WriteLine($"    Salted:      {_tokenData.Salted}");
            Console.WriteLine($"    Debug Flag:  {_tokenData.Debug}");
            Console.WriteLine($"    Encrypted Token:  {_token}");
            Console.WriteLine();
        }

        private static void GenerateToken()
        {
            DisplayHeader();
            DisplaySubheader("Generate Token");

            var validseconds = EasyConsole.Input.ReadInt("Token validity in seconds (10-3000): ", 10, 3000);
            var targeturl = EasyConsole.Input.ReadString("Target url: ");
            var sourceIP = EasyConsole.Input.ReadString("Source IP (no syntax check): ");
            var salted = EasyConsole.Input.ReadKeyYesOrNo("Add salt?");
            Console.WriteLine();
            var debug = EasyConsole.Input.ReadKeyYesOrNo("Add debug flag?");

            _tokenData = new ProxyRequestData() { ValidUntil = DateTime.Now.AddSeconds(validseconds), Url = targeturl, IP = IPAddress.Parse(sourceIP), Debug = debug, Salted = salted };
            _token = ProxyRequestDataEncoder.EncodeAndEncrypt(_tokenData, _key);

            Console.WriteLine();
            Console.WriteLine();
            DisplayToken();

            ContinueWithEnter();
        }

        private static void DecodeToken()
        {
            DisplayHeader();
            Console.WriteLine($"Key:         {_key}");
            Console.WriteLine();
            DisplaySubheader("Decode Token");
            var token = EasyConsole.Input.ReadString("Encoded & Encrypted token:");
            try
            {
                _tokenData = ProxyRequestDataDecoder.CreateFromEncodedAndEncrypted(token, _key);
                _token = token;
                Console.WriteLine();
                DisplayToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decoding token failed: {ex.Message}");
            }
            ContinueWithEnter();
        }

        private static async Task SendRequest()
        {
            DisplayHeader();
            DisplaySubheader("Send Request");
            if (String.IsNullOrEmpty(_token))
            {
                Console.WriteLine("Token is empty");
                ContinueWithEnter();
                return;
            }

            DisplayToken();
            Console.WriteLine("Sending ....");

            var httpclient = new HttpClient();
            var result = await httpclient.GetAsync("https://6bk90lpvbe.execute-api.eu-north-1.amazonaws.com/default/DAIProxyFunc?d=" + _token);

            Console.WriteLine($"Sen result code: {result.StatusCode}");
            ContinueWithEnter();
        }

        private static void ContinueWithEnter()
        {
            Console.WriteLine("Continue with <Enter>");
            Console.ReadLine();
        }

        private static void DoExit()
        {
            Environment.Exit(0);
        }

        private static void GenerateKey()
        {
            DisplayHeader();
            DisplaySubheader("Generate Key");
            _key = RandomString(32, true);
            Console.WriteLine($"New Key:     {_key}");
            Console.WriteLine();
            ContinueWithEnter();
        }

        private static void SetKey()
        {
            bool keyok = false;
            while(!keyok)
            {
                DisplayHeader();
                DisplaySubheader("Generate Key");
                var key = EasyConsole.Input.ReadString("New key (32 characters): ");
                keyok = key.Length == 32;
                _key = keyok ? key : _key;
                if (key.Length == 0)
                {
                    Console.WriteLine("Key not changed");
                    break;
                }
                if (key.Length != 32)
                {
                    Console.WriteLine("invalid key length. 16 characters required (128 bit aes key)");
                    ContinueWithEnter();
                }
            }
            ContinueWithEnter();
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
