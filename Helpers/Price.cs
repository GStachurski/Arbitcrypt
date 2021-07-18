using ArbitCrypt.Enums;
using System.Linq;

namespace ArbitCrypt.Helpers
{
    public static class Price
    {
        private const string CryptoFiatPairFormat = "fsym={0}&tsyms={1}";
        private const string CryptoDefaultReturnCurrency = "USD,BTC,USDT";

        public static string GetCryptoFiatPair(string cryptoSymbol, string[] exchangeCurrencySymbol = null)
        {
            var exchangeParam = (exchangeCurrencySymbol != null && exchangeCurrencySymbol.Any()) ? 
                                string.Join(",", exchangeCurrencySymbol) : 
                                CryptoDefaultReturnCurrency;
            var formattedPairQuery = string.Format(CryptoFiatPairFormat, cryptoSymbol, exchangeParam);
            return formattedPairQuery;
        }

        public static decimal CalculatePriceDiscrepency(decimal binancePrice, decimal bittrexPrice, out Markets bestPriceMarket)
        {
            // find out which price is greater, and subtract to difference
            var priceDiscrepency = decimal.Zero;
            switch (decimal.Compare(binancePrice, bittrexPrice))
            {
                // binance is less than bittrex
                case -1:
                    priceDiscrepency = bittrexPrice - binancePrice;
                    bestPriceMarket = Markets.Bittrex;
                    break;
                // prices are equal - no opportunity here!
                case 0:
                    priceDiscrepency = 0;
                    bestPriceMarket = Markets.EvenMarkets;
                    break;
                // binance is greater than bittrex
                case 1:
                    priceDiscrepency = binancePrice - bittrexPrice;
                    bestPriceMarket = Markets.Binance;
                    break;
                // couldn't calculate, default to zero
                default:
                    bestPriceMarket = Markets.EvenMarkets;
                    break;
            }

            return priceDiscrepency;
        }
    }
}
