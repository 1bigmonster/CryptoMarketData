using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace CryptoMarketData
{
    public class Gemini : ExchangeBase
    {
        [DataContract]
        public class Orderbook
        {
            [DataMember(Name = "bids")]
            public Order[] Bids { get; set; }
            [DataMember(Name = "asks")]
            public Order[] Asks { get; set; }

            public DateTime Timestamp { get; set; }
        }

        [DataContract]
        public class Order
        {
            [DataMember(Name = "price")]
            public string Price { get; set; }
            [DataMember(Name = "amount")]
            public string Amount { get; set; }
            /// <summary>
            /// DO NOT USE - this field is included for compatibility reasons only and is just populated with a dummy value.
            /// https://docs.gemini.com/rest-api/?python#current-order-book
            /// </summary>
            [DataMember(Name = "timestamp")]
            public string Timestamp { get; set; }
        }

        [DataContract]
        public class Quote
        {
            [DataMember(Name = "last")]
            public string Last { get; set; }
            [DataMember(Name = "bid")]
            public string Bid { get; set; }
            [DataMember(Name = "ask")]
            public string Ask { get; set; }
            [DataMember(Name = "volume")]
            public Volume Volume { get; set; }
            public string ExchangeName { get { return Gemini.EXCHANGE_NAME; } }
        }

        [DataContract]
        public class Volume
        {
            [DataMember(Name = "timestamp")]
            public long Timestamp { get; set; }

            public DateTime LocalTime { get { return Utils.ToLocalTime(Timestamp); } }
        }

        public const string EXCHANGE_NAME = "gemini";

        public Orderbook GetOrderbook(string symbol = "btcusd")
        {
            string url = "https://api.gemini.com/v1/book/" + symbol + "";
            string content = GetContent(url);
            var ob = JsonConvert.DeserializeObject<Orderbook>(content);
            ob.Timestamp = DateTime.Now;
            return ob;
        }

        public Quote GetQuote(string symbol = "btcusd")
        {
            string url = "https://api.gemini.com/v1/pubticker/" + symbol + "";
            string content = GetContent(url);
            var quote = JsonConvert.DeserializeObject<Quote>(content);
            return quote;
        }
    }
}
