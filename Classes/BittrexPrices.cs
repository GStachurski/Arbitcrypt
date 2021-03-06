using System;
using System.Collections.Generic;
using ArbitCrypt.Enums;

namespace ArbitCrypt.Classes
{
    public class BittrexPrices
    {
        public List<BittrexPrice> Result { get; set; }
    }

    public class BittrexPrice
    {
        public string MarketName { get; set; }
        public MarketType MarketType { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public double? Volume { get; set; }
        public decimal? last { get; set; }
        public double? BaseVolume { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }
        public int? OpenBuyOrders { get; set; }
        public int? OpenSellOrders { get; set; }
        public decimal? PrevDay { get; set; }
        public DateTime Created { get; set; }
    }
}
