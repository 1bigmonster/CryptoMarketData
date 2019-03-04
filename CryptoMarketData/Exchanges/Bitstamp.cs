using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace CryptoMarketData
{
    public class Bitstamp : ExchangeBase
    {
        [DataContract]
        public class Orderbook
        {
            [DataMember(Name = "timestamp")]
            public string Timestamp { get; set; }
            [DataMember(Name = "bids")]
            public string[,] Bids { get; set; }
            [DataMember(Name = "asks")]
            public string[,] Asks { get; set; }

            public DateTime LocalTime
            {
                get
                {
                    return Utils.UnixTimeStampToLocalTime(Double.Parse(Timestamp));
                }
            }
        }

        [DataContract]
        public class Quote
        {
            [DataMember(Name = "high")]
            public string High { get; set; }
            [DataMember(Name = "last")]
            public string Last { get; set; }
            [DataMember(Name = "timestamp")]
            public string Timestamp { get; set; }
            [DataMember(Name = "bid")]
            public string Bid { get; set; }
            [DataMember(Name = "vwap")]
            public string Vwap { get; set; }
            [DataMember(Name = "volume")]
            public string Volume { get; set; }
            [DataMember(Name = "low")]
            public string Low { get; set; }
            [DataMember(Name = "ask")]
            public string Ask { get; set; }
            [DataMember(Name = "open")]
            public string Open { get; set; }

            public string ExchangeName { get { return Bitstamp.EXCHANGE_NAME; } }
            public DateTime LocalTime
            {
                get
                {
                    return Utils.UnixTimeStampToLocalTime(Double.Parse(Timestamp));
                }
            }
        }

        public const string EXCHANGE_NAME = "bitstamp";

        public Orderbook GetOrderbook(string symbol)
        {
            string url = "https://www.bitstamp.net/api/v2/order_book/" + symbol + "/";
            string content = GetContent(url);
            var ob = JsonConvert.DeserializeObject<Orderbook>(content);
            return ob;
        }

        public Quote GetQuote(string symbol)
        {
            string url = "https://www.bitstamp.net/api/v2/ticker/" + symbol + "/";
            string content = GetContent(url);
            var quote = JsonConvert.DeserializeObject<Quote>(content);
            return quote;
        }
    }
}
