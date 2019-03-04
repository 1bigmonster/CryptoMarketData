using RestSharp;
using System;

namespace CryptoMarketData
{
    public class ExchangeBase
    {
        protected string GetContent(string url)
        {
            Console.WriteLine($"{DateTime.Now} - Connecting {url}");
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string
            return content;
        }
    }
}
