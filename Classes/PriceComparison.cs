using ArbitCrypt.Enums;

namespace ArbitCrypt.Classes
{
    public class PriceComparison
    {
        public Markets BestMarket { get; set; }
        public MarketType MarketType { get; set; }
        public string MarketName { get; set; }
        public decimal BittrexPrice { get; set; }
        public decimal BinancePrice { get; set; }
        public decimal BittrexUsdPrice { get; set; }
        public decimal BinanceUsdPrice { get; set; }
        public decimal PriceDiscrepancy { get; set; }
        public decimal UsdPriceDiscrepancy { get; set; }
        public string ArbitrageGrade { get; set; }
        public string ArbitragePercentage { get; set; }
        public string EstimatedProfit { get; set; }
    }
}
