using System;

namespace CryptoMarketData
{
    public class Order
    {
        /// <summary>
        /// btcusd, ethusd
        /// </summary>
        public string Symbol { get; set; }
        public BidOfferEnum BidOfferEnum { get; set; }
        public string Exchange { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }        
        public DateTime RawMessageTime { get; set; }
    }
}
