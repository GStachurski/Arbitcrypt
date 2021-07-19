using ArbitCrypt.Classes;
using ArbitCrypt.Enums;
using System;

namespace ArbitCrypt.Helpers
{
    public static class PriceFormatting
    {
        public const char _upArrow = '\u2191';
        public const char _downArrow = '\u2193';
        public const string _eightDigitFormat = "0.00000000";
        public const int _priceLength = 10;

        public static void FormatBittrexPrices(BittrexPrices bittrexPrices)
        {
            foreach (var price in bittrexPrices.Result)
            {
                // remove the dash from the market name, and flip the symbols
                if (price.MarketName.Contains("-"))
                {
                    string[] result = price.MarketName.Split('-');
                    price.MarketName = result[1] + result[0];
                }
            }
        }

        public static string ApplyCryptoPriceFormats(decimal price, Markets highestMarket, Markets baseMarketType, MarketType marketType)
        {
            // format to 3 decimal places for the TetherUsd prices
            if (marketType == MarketType.Usdt) price.ToString("C3");

            // append an appropriate arrow for the high or low market price
            if (highestMarket == baseMarketType) return ApplyBaseCryptoFormat(price) + _upArrow;
            return ApplyBaseCryptoFormat(price) + _downArrow;
        }

        public static string ApplyUsdPriceFormats(decimal price, Markets highestMarket, Markets baseMarketType)
        {
            // append an appropriate arrow for the high or low market price
            if (highestMarket == baseMarketType) return price.ToString("C3") + _upArrow;
            return price.ToString("C3") + _downArrow;
        }

        public static string ApplyBaseCryptoFormat(decimal price)
        {
            return price.ToString(_eightDigitFormat).Truncate(_priceLength);
        }

        public static string Truncate(this string str, int length)
        {
            int maxLength = Math.Min(str.Length, length);
            return str.Substring(0, maxLength);
        }
    }
}
