using CryptoMarketData.Properties;
using PusherServer;
using System;

namespace CryptoMarketData
{
    public class Publisher
    {
        /*
         *  btcusd and ethusd total messages.
         *  24 x 60 x 60 x 2 < 200,000 messags per day.
         */

        private PusherServer.Pusher pusher;

        public Publisher()
        {
            var options = new PusherOptions
            {
                Cluster = Resources.PusherCluster,
                Encrypted = bool.Parse(Resources.PusherEncryped)
            };

            this.pusher = new PusherServer.Pusher(
              Resources.PusherAppId,
              Resources.PusherAppKey,
              Resources.PusherAppSecret,
              options);
        }

        public void Send(string symbol, object[] data)
        {
            int rows = data.Length;
            //Console.WriteLine($"{DateTime.Now} - Ready to send {rows} rows.");

            var events = new Event[1];
            events[0] = new Event { Channel = symbol, EventName = "ob-data", Data = data };
            var result = pusher.TriggerAsync(events);
        }

        public void SendReset(string symbol)
        {
            Console.WriteLine($"{DateTime.Now} - Ready to send a reset message.");
            var events = new Event[1];
            events[0] = new Event { Channel = symbol, EventName = "ob-reset", Data = "" };
            var result = pusher.TriggerAsync(events);
        }

        //public void SendComplete(string symbol)
        //{
        //    Console.WriteLine($"{DateTime.Now} - Ready to send a reset message.");
        //    var events = new Event[1];
        //    events[0] = new Event { Channel = symbol, EventName = "ob-reset", Data = "" };
        //    var result = pusher.TriggerAsync(events);
        //}

        public void HelloWorld()
        {
            var events = new Event[2];
            events[0] = new Event
            {
                Channel = "btcusd",
                EventName = "ob-data",
                Data = new object[] {
                    new { price = 3801, isArb = false, bidex = "", bidqty = "", askqty = 0.12345678, askex = "bitstamp" },
                    new { price = 3800.20, isArb = false, bidex = "tidebit", bidqty = 999.12345678, askqty = "", askex = "" },
                    new { price = 3800.11, isArb = true, bidex = "ib", bidqty = 2.12345678, askqty = "9.12345678", askex = "bitstamp" },
                    new { price = 3800.11, isArb = true, bidex = "", bidqty = "", askqty = "1.234567", askex = "okex" },
                    new { price = 3800.11, isArb = true, bidex = "", bidqty = "", askqty = "82.345", askex = "huobi" },
                    new { price = 3850, isArb = true, bidex = "bitstamp", bidqty = 234.12345678, askqty = 444.12345678, askex = "gemini" }
                }
            };

            events[1] = new Event
            {
                Channel = "ob",
                EventName = "ethusd",
                Data = new object[] {
                    new { price = 131, isArb = false, bidex = "", bidqty = "", askqty = 0.12345678, askex = "bitstamp" },
                    new { price = 130.20, isArb = false, bidex = "tidebit", bidqty = 999.12345678, askqty = "", askex = "" },
                    new { price = 130.11, isArb = true, bidex = "ib", bidqty = 2.12345678, askqty = "9.12345678", askex = "bitstamp" },
                    new { price = 130.11, isArb = true, bidex = "", bidqty = "", askqty = "1.234567", askex = "okex" },
                    new { price = 130.11, isArb = true, bidex = "", bidqty = "", askqty = "82.345", askex = "huobi" },
                    new { price = 130, isArb = true, bidex = "bitstamp", bidqty = 234.12345678, askqty = 444.12345678, askex = "gemini" }
                }
            };
            var result = pusher.TriggerAsync(events);
        }
    }
}
