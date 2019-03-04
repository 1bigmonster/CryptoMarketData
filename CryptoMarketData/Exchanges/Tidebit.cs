using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace CryptoMarketData
{
    public class Tidebit : ExchangeBase
    {
        [DataContract]
        public class Orderbook
        {
            public DateTime Timestamp { get; set; }
            [DataMember(Name = "bids")]
            public Order[] Bids { get; set; }
            [DataMember(Name = "asks")]
            public Order[] Asks { get; set; }
        }

        [DataContract]
        public class Order
        {
            [DataMember(Name = "id")]
            public long Id { get; set; }
            [DataMember(Name = "side")]
            public string Side { get; set; } //"side": "sell",
            [DataMember(Name = "ord_type")]
            public string OrdType { get; set; } // "ord_type": "limit",
            [DataMember(Name = "ask")]
            public string Crypto { get; set; } // "ask": "btc",
            [DataMember(Name = "kind")]
            public string Kind { get; set; } // "kind": "ask",
            [DataMember(Name = "price")]
            public decimal Price { get; set; }
            [DataMember(Name = "avg_price")]
            public decimal AvgPrice { get; set; }
            [DataMember(Name = "state")]
            public string State { get; set; } //"state": "wait",
            [DataMember(Name = "market")]
            public string Market { get; set; } //"market": "btchkd",
            [DataMember(Name = "created_at")]
            public string CreatedAt { get; set; } //"created_at": "2019-03-02T15:33:20+08:00",
            [DataMember(Name = "volume")]
            public string Volume { get; set; }
            [DataMember(Name = "remaining_volume")]
            public string RemainingVolume { get; set; } //"remaining_volume": "0.06",
            [DataMember(Name = "executed_volume")]
            public string ExecutedVolume { get; set; }
            [DataMember(Name = "trades_count")]
            public long TradesCount { get; set; }

            public decimal PriceUsd { get { return Math.Round(Price * HKDUSD, 2); } }
        }

        [DataContract]
        public class Ticker
        {
            [DataMember(Name = "buy")]
            public decimal Buy { get; set; }
            [DataMember(Name = "sell")]
            public decimal Sell { get; set; }
            [DataMember(Name = "low")]
            public decimal Low { get; set; }
            [DataMember(Name = "high")]
            public decimal High { get; set; }
            [DataMember(Name = "last")]
            public decimal Last { get; set; }
            [DataMember(Name = "open")]
            public decimal Open { get; set; }
            [DataMember(Name = "vol")]
            public decimal Volume { get; set; }

            public decimal LastUsd { get { return Math.Round(Last * HKDUSD, 2); } }
            public decimal BuyUsd { get { return Math.Round(Buy * HKDUSD, 2); } }
            public decimal SelltUsd { get { return Math.Round(Sell * HKDUSD, 2); } }
        }

        [DataContract]
        public class Quote
        {
            [DataMember(Name = "at")]
            public string Timestamp { get; set; }
            [DataMember(Name = "ticker")]
            public Ticker Ticker { get; set; }
            public string ExchangeName { get { return Tidebit.EXCHANGE_NAME; } }
        }

        //TODO convert hkd to usd.
        public const decimal HKDUSD = 0.127407m;
        public const string EXCHANGE_NAME = "tidebit";

        public Orderbook GetOrderbook(string symbol)
        {
            string url = "https://www.tidebit.com//api/v2/order_book.json?market=" + symbol + "";
            string content = GetContent(url);
            var ob = JsonConvert.DeserializeObject<Orderbook>(content);
            ob.Timestamp = DateTime.Now;
            return ob;
        }

        public Quote GetQuote(string symbol)
        {
            string url = "https://www.tidebit.com//api/v2/tickers/" + symbol + ".json";
            string content = GetContent(url);
            var quote = JsonConvert.DeserializeObject<Quote>(content);
            return quote;
        }
    }
}
