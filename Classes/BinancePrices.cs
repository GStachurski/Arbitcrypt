using ArbitCrypt.Enums;

namespace ArbitCrypt.Classes
{
    public class BinancePrices
    {
        public string symbol { get; set; }
        public decimal price { get; set; }
        public MarketType MarketType { get; set; }
    }
}
