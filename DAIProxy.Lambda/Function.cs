using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using DAIProxy.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DAIProxy
{
    public class Function
    {
        /// <summary>
        /// The Handler function triggered by the AWS APIGateway.
        /// </summary>
        /// <param name="gwEvent">Data passed from the APIGateway to the handler. The Handler only checks for one QueryParameter withe the name "d" and then tries to decode its value.</param>
        /// <returns>The response that will beconverted to a HTTP response by the API Gateway</returns>
        /// <example>Parameter d structure: content of the parameter is aes encrypted and base62 encoded. Format: "[optional salt];[validuntil ISO 8601];[target url urlencoded];[target ip];[salt]*"</example>
        public APIGatewayHttpApiV2ProxyResponse Handler(APIGatewayHttpApiV2ProxyRequest gwEvent)
        {
            var key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            var dataok = gwEvent.QueryStringParameters.TryGetValue("d", out var data);
            if (!dataok)
            {
                return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = "Missing Data" };
            }

            var decodeddataok = TryDecodeBase62String(data, out var decodeddata);
            if (!decodeddataok)
            {
                return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = "Decoding failed" };
            }

            var decrypteddataok = TryDecryptData(decodeddata, key);
            if (!decrypteddataok)
            {
                return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = "Decrypting failed" };
            }

            var response = new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 200, Body="MyFunc" };
            return response;
        }


        private bool TryDecodeBase62String(string data, out byte[] decodeddata)
        {
            decodeddata = data.FromBase62();
            return true;
        }

        private bool TryDecryptData(byte[] data, string key)
        {
            throw new NotImplementedException();
        }
    }
}
