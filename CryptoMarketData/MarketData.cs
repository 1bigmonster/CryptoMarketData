using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoMarketData
{
    public interface IOrderbook
    {
        void AddOrUpdateOrder(Order order);
        void DeleteExchange(string exchange);
    }

    public interface ITestFunctions
    {
        SortedDictionary<decimal, Dictionary<string, Order>> GetBids();
        SortedDictionary<decimal, Dictionary<string, Order>> GetOffers();
    }

    public class PriceLevel
    {
        public List<Order> Bids = new List<Order>();
        public List<Order> Offers = new List<Order>();
    }

    public class MarketData : IOrderbook, ITestFunctions
    {
        public class Quote
        {
            public decimal Bid { get; set; }
            public decimal Ask { get; set; }
        }

        private readonly Dictionary<string, Quote> quotes = new Dictionary<string, Quote>();
        private readonly SortedDictionary<decimal, PriceLevel> ladders = new SortedDictionary<decimal, PriceLevel>();
        private readonly string symbol;
        private readonly int priceDecimal;

        public MarketData(string symbol)
        {
            this.symbol = symbol;
            this.priceDecimal = 8;
        }

        public MarketData(string symbol, int priceDecimal)
        {
            this.symbol = symbol;
            this.priceDecimal = priceDecimal;
        }

        public SortedDictionary<decimal, Dictionary<string, Order>> GetBids()
        {
            var dict = new SortedDictionary<decimal, Dictionary<string, Order>>();
            foreach (var o in ladders)
            {
                if (o.Value.Bids.Count != 0)
                {
                    var d = new Dictionary<string, Order>();
                    o.Value.Bids.ForEach(x => d.Add(x.Exchange, x));
                    dict.Add(o.Key, d);
                }
            }
            return dict;
        }

        public SortedDictionary<decimal, Dictionary<string, Order>> GetOffers()
        {
            var dict = new SortedDictionary<decimal, Dictionary<string, Order>>();
            foreach (var o in ladders)
            {
                if (o.Value.Offers.Count != 0)
                {
                    var d = new Dictionary<string, Order>();
                    o.Value.Offers.ForEach(x => d.Add(x.Exchange, x));
                    dict.Add(o.Key, d);
                }
            }
            return dict;
        }

        /// <summary>
        /// upper and lower is used to trim the total data send to PUSHER:
        /// From PUSHER:
        /// The data content of this event exceeds the allowed maximum (10240 bytes).
        /// See http://pusher.com/docs/server_api_guide/server_publishing_events for more info
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object[]> ToPusherFormat()
        {
            //use the bid and offer from all exchanges to create a upper and lower boundary.
            foreach (var quote in quotes)
                Console.WriteLine($"Exchange={quote.Key}, Bid={quote.Value.Bid}, Ask={quote.Value.Ask}");

            decimal minBid = quotes.Min(x => x.Value.Bid);
            decimal maxAsk = quotes.Max(x => x.Value.Ask);
            Console.WriteLine($"{DateTime.Now} - minBid= {minBid}, maxAsk= {maxAsk}.");

            decimal rate = 0.05m;
            decimal lower = minBid * (1 - rate);
            decimal upper = maxAsk * (1 + rate);
            Console.WriteLine($"{DateTime.Now} - adjust the ladder boundary, rate= {rate}, lower= {lower}, upper= {upper}.");

            int batchSize = 100;
            var data = new object[batchSize];
            int index = 0;
            foreach (var ladder in ladders.Reverse())
            {
                var price = ladder.Key;
                if (lower <= price && price <= upper)
                {
                    var bids = ladder.Value.Bids;
                    var asks = ladder.Value.Offers;
                    var bCnt = bids.Count;
                    var aCnt = asks.Count;
                    var max = Math.Max(bCnt, aCnt);
                    var isArb = bCnt > 0 && aCnt > 0;

                    for (int i = 0; i < max; i++)
                    {
                        var bid = i < bCnt ? bids[i] : null;
                        var ask = i < aCnt ? asks[i] : null;
                        data[index] = new
                        {
                            price = price,
                            isArb = isArb,
                            bidex = bid != null ? bid.Exchange : "",
                            bidqty = bid != null ? bid.Quantity : 0,
                            askqty = ask != null ? ask.Quantity : 0,
                            askex = ask != null ? ask.Exchange : "",
                        };
                        index += 1;
                        if (index == batchSize)
                        {
                            index = 0; //reset
                            Console.WriteLine($"{DateTime.Now} - Ready to send {batchSize} rows.");
                            var copy = data;
                            yield return copy;
                            data = new object[batchSize];
                        }
                    }
                }
            }
            if (0 < index && index < batchSize)
            {
                Console.WriteLine($"{DateTime.Now} - Ready to send {index} rows.");
                yield return data;
            }
        }

        public void AddOrUpdateOrder(Order order)
        {
            var price = order.Price;
            var exchange = order.Exchange;
            var quantity = order.Quantity;

            if (!ladders.TryGetValue(price, out PriceLevel priceLevel))
            {
                priceLevel = new PriceLevel();
                ladders.Add(price, priceLevel);
            }

            var container = order.BidOfferEnum == BidOfferEnum.Bid ? priceLevel.Bids : priceLevel.Offers;
            if (quantity > 0)
            {
                var c = container.Where(x => x.Exchange == exchange).FirstOrDefault();
                if (c == null)
                    container.Add(order);
                else
                {
                    c.Price = order.Price;
                    c.Quantity = order.Quantity;
                    c.RawMessageTime = order.RawMessageTime;
                }
            }
            else
            {
                var c = container.Where(x => x.Exchange == exchange).FirstOrDefault();
                if (c != null)
                    container.Remove(c);
            }
        }

        public void DeleteExchange(string exchange)
        {
            foreach (var p in ladders.Values)
            {
                var bids = p.Bids;
                var b = bids.Where(x => x.Exchange == exchange).FirstOrDefault();
                if (b != null)
                    bids.Remove(b);
            }
            foreach (var p in ladders.Values)
            {
                var asks = p.Offers;
                var a = asks.Where(x => x.Exchange == exchange).FirstOrDefault();
                if (a != null)
                    asks.Remove(a);
            }
        }

        public void AddOrderbook(Bitstamp.Orderbook orderbook)
        {
            string exchange = Bitstamp.EXCHANGE_NAME;
            DeleteExchange(exchange); //delete all.

            var time = orderbook.LocalTime;
            Console.WriteLine($"{DateTime.Now} - {exchange} time is {time}.");

            {
                var bids = orderbook.Bids;
                for (int i = 0; i < bids.Length / 2; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Bid,
                        Exchange = exchange,
                        Price = decimal.Parse(bids[i, 0]),
                        Quantity = decimal.Parse(bids[i, 1]),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
            {
                var asks = orderbook.Asks;
                for (int i = 0; i < asks.Length / 2; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Offer,
                        Exchange = exchange,
                        Price = decimal.Parse(asks[i, 0]),
                        Quantity = decimal.Parse(asks[i, 1]),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
        }

        public void AddOrderbook(Bitfinex.Orderbook orderbook)
        {
            string exchange = Bitfinex.EXCHANGE_NAME;
            DeleteExchange(exchange); //delete all.

            var time = orderbook.Timestamp;
            Console.WriteLine($"{DateTime.Now} - {exchange} time is {time}.");

            {
                var bids = orderbook.Bids;
                foreach (var bid in bids)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Bid,
                        Exchange = exchange,
                        Price = bid.Price,
                        Quantity = bid.Quantity,
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
            {
                var asks = orderbook.Asks;
                foreach (var ask in asks)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Offer,
                        Exchange = exchange,
                        Price = ask.Price,
                        Quantity = ask.Quantity,
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
        }

        public void AddOrderbook(Tidebit.Orderbook orderbook)
        {
            string exchange = Tidebit.EXCHANGE_NAME;
            DeleteExchange(exchange); //delete all.

            var time = orderbook.Timestamp;
            Console.WriteLine($"{DateTime.Now} - {exchange} time is {time}.");

            {
                var bids = orderbook.Bids;
                for (int i = 0; i < bids.Length; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Bid,
                        Exchange = exchange,
                        Price = bids[i].PriceUsd,
                        Quantity = decimal.Parse(bids[i].RemainingVolume),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
            {
                var asks = orderbook.Asks;
                for (int i = 0; i < asks.Length; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Offer,
                        Exchange = exchange,
                        Price = asks[i].PriceUsd,
                        Quantity = decimal.Parse(asks[i].RemainingVolume),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
        }

        public void AddOrderbook(Gemini.Orderbook orderbook)
        {
            string exchange = Gemini.EXCHANGE_NAME;

            DeleteExchange(exchange); //delete all.

            var time = orderbook.Timestamp;
            Console.WriteLine($"{DateTime.Now} - {exchange} time is {time}.");

            {
                var bids = orderbook.Bids;
                for (int i = 0; i < bids.Length; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Bid,
                        Exchange = exchange,
                        Price = decimal.Parse(bids[i].Price),
                        Quantity = decimal.Parse(bids[i].Amount),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
            {
                var asks = orderbook.Asks;
                for (int i = 0; i < asks.Length; i++)
                {
                    AddOrUpdateOrder(new Order
                    {
                        BidOfferEnum = BidOfferEnum.Offer,
                        Exchange = exchange,
                        Price = decimal.Parse(asks[i].Price),
                        Quantity = decimal.Parse(asks[i].Amount),
                        RawMessageTime = time,
                        Symbol = symbol,
                    });
                }
            }
        }

        public void AddQuote(Bitfinex.Quote quote)
        {
            quotes[quote.ExchangeName] = new Quote
            {
                Bid = Convert.ToDecimal(quote.Bid),
                Ask = Convert.ToDecimal(quote.Ask),
            };
        }

        public void AddQuote(Bitstamp.Quote quote)
        {
            quotes[quote.ExchangeName] = new Quote
            {
                Bid = Convert.ToDecimal(quote.Bid),
                Ask = Convert.ToDecimal(quote.Ask),
            };
        }

        public void AddQuote(Tidebit.Quote quote)
        {
            quotes[quote.ExchangeName] = new Quote
            {
                Bid = Convert.ToDecimal(quote.Ticker.BuyUsd),
                Ask = Convert.ToDecimal(quote.Ticker.SelltUsd),
            };
        }

        public void AddQuote(Gemini.Quote quote)
        {
            quotes[quote.ExchangeName] = new Quote
            {
                Bid = Convert.ToDecimal(quote.Bid),
                Ask = Convert.ToDecimal(quote.Ask),
            };
        }
    }
}
