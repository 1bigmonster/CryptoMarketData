using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CryptoMarketData
{
    public class Bitfinex : ExchangeBase
    {
        public class Quote
        {
            public decimal Bid { get; set; }
            public decimal Ask { get; set; }
            public DateTime Timestamp { get; private set; }
            public string ExchangeName { get { return Bitfinex.EXCHANGE_NAME; } }

            public Quote(string[] array)
            {
                Bid = decimal.Parse(array[0]);
                Ask = decimal.Parse(array[2]);
                Timestamp = DateTime.Now;
            }
        }

        public const string EXCHANGE_NAME = "bitfinex";

        public class Orderbook
        {
            public List<Order> Bids { get; set; }
            public List<Order> Asks { get; set; }
            public DateTime Timestamp { get; set; }

            public Orderbook()
            {
                Bids = new List<Order>();
                Asks = new List<Order>();
                Timestamp = DateTime.Now;
            }
        }

        public class Order
        {
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }
        }

        public Quote GetQuote(string symbol)
        {
            string url = "https://api-pub.bitfinex.com/v2/ticker/t" + symbol.ToUpper() + "";
            string content = GetContent(url);
            var array = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(content);
            return new Quote(array);
        }

        public Orderbook GetOrderbook(string symbol)
        {
            string url = "https://api-pub.bitfinex.com/v2/book/t" + symbol.ToUpper() + "/P0?len=100";
            string content = GetContent(url);
            var array = JsonConvert.DeserializeObject<string[,]>(content);
            var orderbook = new Orderbook();
            for (int i = 0; i < 50; i++) //bids
            {
                orderbook.Bids.Add(new Order
                {
                    Price = decimal.Parse(array[i, 0]),
                    Quantity = Math.Abs(decimal.Parse(array[i, 2])),
                });
            }
            for (int i = 25; i < 100; i++) //asks
            {
                orderbook.Asks.Add(new Order
                {
                    Price = decimal.Parse(array[i, 0]),
                    Quantity = Math.Abs(decimal.Parse(array[i, 2])),
                });
            }
            return orderbook;
        }
    }
}
