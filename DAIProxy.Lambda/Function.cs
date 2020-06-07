using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using DAIProxy.Core;
using System.Net.Http;

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
        public async Task<APIGatewayHttpApiV2ProxyResponse> Handler(APIGatewayHttpApiV2ProxyRequest gwEvent)
        {
            var key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            var dataok = gwEvent.QueryStringParameters.TryGetValue("d", out var data);
            if (!dataok)
            {
                return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = "Missing Data" };
            }

            try
            {
                var prd = ProxyRequestDataDecoder.CreateFromEncodedAndEncrypted(data, key);
                var body = $"ValidUntil: {prd.ValidUntil:O}  -  SourceIP: {prd.IP}  -  Url: {prd.Url}";
                if (prd.ValidUntil < DateTime.Now)
                {
                    LambdaLogger.Log($"Token outdated : {body}");
                    return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = "Token not valid anymore." };
                }

                if (!prd.Debug)
                {
                    var cli = new HttpClient();
                    LambdaLogger.Log($"Triggering Request: {body}");
                    var result = await cli.SendAsync(CreateRequest(prd));
                    LambdaLogger.Log($"Request result: {result.StatusCode}");
                    return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 200, Body = "MyFunc" };
                }
                else
                {
                    LambdaLogger.Log($"Debug Request: {body}");
                    return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 200, Body = body };
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Exception while processing: {ex.Message}");
                LambdaLogger.Log(ex.StackTrace);
                return new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 400, Body = ex.Message };
            }

        }

        private HttpRequestMessage CreateRequest(ProxyRequestData data)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, data.Url);
            req.Headers.Add("X-Forwarded-For", data.IP.ToString());
            return req;
        }

    }
}
