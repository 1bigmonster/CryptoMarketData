using CryptoMarketData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Add_bid_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(2, bids[100].Values.Count);
                Assert.AreEqual(true, bids[100].ContainsKey("gemini"));
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));

                var offers = ob.GetOffers();
                Assert.AreEqual(0, offers.Count);
            }
        }

        [TestMethod]
        public void Add_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(2, offers[100].Values.Count);
                Assert.AreEqual(true, offers[100].ContainsKey("gemini"));
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));

                var bids = ob.GetBids();
                Assert.AreEqual(0, bids.Count);
            }
        }

        [TestMethod]
        public void Add_bid_and_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });

                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(2, bids[100].Values.Count);
                Assert.AreEqual(true, bids[100].ContainsKey("gemini"));
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));

                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(2, offers[100].Values.Count);
                Assert.AreEqual(true, offers[100].ContainsKey("gemini"));
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));
            }
        }

        [TestMethod]
        public void Change_bid_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 3, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(2, bids[100].Values.Count);
                Assert.AreEqual(true, bids[100].ContainsKey("gemini"));
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));
                Assert.AreEqual(4, bids[100].Values.Sum(x => x.Quantity)); //should be 4, but not 90.

                var offers = ob.GetOffers();
                Assert.AreEqual(0, offers.Count);
            }
        }

        [TestMethod]
        public void Change_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 3, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(2, offers[100].Values.Count);
                Assert.AreEqual(true, offers[100].ContainsKey("gemini"));
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));
                Assert.AreEqual(4, offers[100].Values.Sum(x => x.Quantity)); //should be 4, but not 90.

                var bids = ob.GetBids();
                Assert.AreEqual(0, bids.Count);
            }
        }

        [TestMethod]
        public void Change_bid_and_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 3, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 3, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(2, bids[100].Values.Count);
                Assert.AreEqual(true, bids[100].ContainsKey("gemini"));
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));
                Assert.AreEqual(4, bids[100].Values.Sum(x => x.Quantity)); //should be 4, but not 90.
                               
                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(2, offers[100].Values.Count);
                Assert.AreEqual(true, offers[100].ContainsKey("gemini"));
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));
                Assert.AreEqual(4, offers[100].Values.Sum(x => x.Quantity)); //should be 4, but not 90.
            }
        }

        [TestMethod]
        public void Delete_bid_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 3, Symbol = "btc" });
                //delete
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 0, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(1, bids[100].Values.Count);  // changed
                Assert.AreEqual(false, bids[100].ContainsKey("gemini"));  // changed
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));
                Assert.AreEqual(1, bids[100].Values.Sum(x => x.Quantity));  // changed

                var offers = ob.GetOffers();
                Assert.AreEqual(0, offers.Count);
            }
        }

        [TestMethod]
        public void Delete_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 3, Symbol = "btc" });
                //delete
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 0, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(1, offers[100].Values.Count);  // changed
                Assert.AreEqual(false, offers[100].ContainsKey("gemini"));  // changed
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));
                Assert.AreEqual(1, offers[100].Values.Sum(x => x.Quantity)); // changed

                var bids = ob.GetBids();
                Assert.AreEqual(0, bids.Count);
            }
        }

        [TestMethod]
        public void Delete_bid_and_offer_queue()
        {
            var orderbook = new MarketData("btc", 8);

            {
                IOrderbook ob = orderbook as IOrderbook;
                //add:
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Bid, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 90, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "bitstamp", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 1, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "binance", BidOfferEnum = BidOfferEnum.Offer, Price = 99, Quantity = 2, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 90, Symbol = "btc" });

                //change qty from 90 to 3                
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 3, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 3, Symbol = "btc" });
                //delete
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Bid, Price = 100, Quantity = 0, Symbol = "btc" });
                ob.AddOrUpdateOrder(new Order { Exchange = "gemini", BidOfferEnum = BidOfferEnum.Offer, Price = 100, Quantity = 0, Symbol = "btc" });
            }

            {
                ITestFunctions ob = orderbook as ITestFunctions;
                var bids = ob.GetBids();
                Assert.AreEqual(true, bids.ContainsKey(99));
                Assert.AreEqual(true, bids.ContainsKey(100));
                Assert.AreEqual(true, bids[99].ContainsKey("binance"));
                Assert.AreEqual(1, bids[100].Values.Count);  // changed
                Assert.AreEqual(false, bids[100].ContainsKey("gemini"));  // changed
                Assert.AreEqual(true, bids[100].ContainsKey("bitstamp"));
                Assert.AreEqual(1, bids[100].Values.Sum(x => x.Quantity));   // changed

                var offers = ob.GetOffers();
                Assert.AreEqual(true, offers.ContainsKey(99));
                Assert.AreEqual(true, offers.ContainsKey(100));
                Assert.AreEqual(true, offers[99].ContainsKey("binance"));
                Assert.AreEqual(1, offers[100].Values.Count);  // changed
                Assert.AreEqual(false, offers[100].ContainsKey("gemini"));  // changed
                Assert.AreEqual(true, offers[100].ContainsKey("bitstamp"));
                Assert.AreEqual(1, offers[100].Values.Sum(x => x.Quantity));  // changed
            }
        }
    }
}
