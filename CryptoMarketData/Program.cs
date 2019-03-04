using CryptoMarketData;
using CryptoMarketData.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoMarketData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now} - started.");

            Publisher publisher = new Publisher();

            try
            {
                bool keepRunning = true;
                var exitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    keepRunning = false;
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                    Console.WriteLine($"{DateTime.Now} - receive a cancel event.");
                };

                while (keepRunning)
                {
                    var btcusd = new MarketData("btcusd");

                    if (bool.Parse(Resources.Bitfinex))
                    {
                        Bitfinex bitfinex = new Bitfinex();
                        btcusd.AddQuote(bitfinex.GetQuote("btcusd"));
                        btcusd.AddOrderbook(bitfinex.GetOrderbook("btcusd"));
                    }
                    if (bool.Parse(Resources.Gemini))
                    {
                        Gemini gemini = new Gemini();
                        btcusd.AddQuote(gemini.GetQuote("btcusd"));
                        btcusd.AddOrderbook(gemini.GetOrderbook("btcusd"));
                    }
                    if (bool.Parse(Resources.Bitstamp))
                    {
                        Bitstamp bitstamp = new Bitstamp();
                        btcusd.AddQuote(bitstamp.GetQuote("btcusd"));
                        btcusd.AddOrderbook(bitstamp.GetOrderbook("btcusd"));
                    }
                    if (bool.Parse(Resources.Tidebit))
                    {
                        Tidebit tidebit = new Tidebit();
                        btcusd.AddQuote(tidebit.GetQuote("btchkd"));
                        btcusd.AddOrderbook(tidebit.GetOrderbook("btchkd"));
                    }

                    publisher = new Publisher();
                    //publisher.HelloWorld();

                    publisher.SendReset("btcusd");
                    Thread.Sleep(100);
                    foreach (var cluster in btcusd.ToPusherFormat())
                    {
                        publisher.Send("btcusd", cluster);
                        Thread.Sleep(100);
                        //foreach (var i in cluster)
                        //    Console.WriteLine(i);
                    }
                    {
                        int sleep = int.Parse(Resources.RefreshInterval);
                        Console.WriteLine($"{DateTime.Now} - sleep {sleep} ms. Press Ctrl + C to quit...");
                        Thread.Sleep(sleep);
                    }
                }

                exitEvent.WaitOne();
                Console.WriteLine($"{DateTime.Now} - stopped.");
                Console.WriteLine("Press any key(s) to quit...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
                publisher = null;
            }
        }
    }
}
