using ArbitCrypt.Classes;
using ArbitCrypt.Configuration;
using ArbitCrypt.Enums;
using ArbitCrypt.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArbitCrypt.Helpers
{
    public class Calculation
    {
        private static CurrentPrices _currentPrices { get; set; }
        private static ArbitCryptConfiguration _config { get; set; }

        public Calculation(CurrentPrices currentPrices, ArbitCryptConfiguration config)
        {
            _currentPrices = currentPrices ?? throw new ArgumentNullException();
            _config = config ?? throw new ArgumentNullException();
        }

        public static string CalculateEstimatedProfit(MarketType marketType, Markets bestMarket, decimal binancePrice, decimal bittrexPrice)
        {
            var balance = marketType == MarketType.Btc ? _config._currentBtcBalance : _config._currentEthBalance;
            var currentPrice = marketType == MarketType.Btc ? _currentPrices.BTCUSD : _currentPrices.ETHUSD;

            if (bestMarket == Markets.Binance)
            {
                // go to bittrex, purchase at the current price with 100% of BTC holdings
                var bittrexTotalCoinPurchase = SafeDivision(balance, bittrexPrice);

                // send to binance, sell 100% of coins at current binance price
                var boughtAtBittrexPrice = balance;
                var soldAtBinancePrice = bittrexTotalCoinPurchase * binancePrice;
                return (Math.Abs(soldAtBinancePrice - boughtAtBittrexPrice) * currentPrice).ToString("C3");
            }

            // go to bittrex, purchase at the current price with 100% of BTC holdings
            var binanceTotalCoinPurchase = SafeDivision(balance, binancePrice);

            // send to binance, sell 100% of coins at current binance price
            var boughtAtBinancePrice = balance;
            var soldAtBittrexPrice = binanceTotalCoinPurchase * bittrexPrice;
            return (Math.Abs(soldAtBittrexPrice - boughtAtBinancePrice) * currentPrice).ToString("C3");
        }

        public static decimal CalculateArbitragePercentage(Markets bestPriceMarket, decimal binancePrice, decimal bittrexPrice)
        {
            decimal percentage;
            switch (bestPriceMarket)
            {
                case Markets.Binance:
                    percentage = SafeDivision(binancePrice - bittrexPrice, Math.Abs(binancePrice));
                    break;
                case Markets.Bittrex:
                    percentage = SafeDivision(bittrexPrice - binancePrice, Math.Abs(bittrexPrice));
                    break;
                default:
                    percentage = decimal.Zero;
                    break;
            }

            return percentage;
        }

        public static decimal CalculateCurrentUsdPrice(MarketType marketType, decimal cryptoPrice)
        {
            if (marketType == MarketType.Eth)
            {
                return cryptoPrice * _currentPrices.ETHUSD;
            }
            else if (marketType == MarketType.Btc)
            {
                return cryptoPrice * _currentPrices.BTCUSD;
            }
            else return cryptoPrice * _currentPrices.USDTUSD;
        }

        public static decimal CalculateUsdPriceDiscrepency(MarketType marketType, decimal priceDiscrepency)
        {
            switch (marketType)
            {
                case MarketType.Eth:
                    return priceDiscrepency * _currentPrices.ETHUSD;
                case MarketType.Usdt:
                    return priceDiscrepency * _currentPrices.USDTUSD;
            }
            return priceDiscrepency * _currentPrices.BTCUSD;
        }

        public static List<PriceComparison> CalculatePriceDeltas(List<BinancePrices> binancePrices, BittrexPrices bittrexPrices)
        {
            // typically the Bittrex market is much larger, we'll use it as our starting point
            var priceDiffList = new List<PriceComparison>();
            foreach (var bittrexPrice in bittrexPrices.result)
            {
                // get the market type for price calculation
                var marketType = DiscernMarketType(bittrexPrice.MarketName);

                // let's see if we have a match
                var binancePrice =
                    binancePrices.FirstOrDefault(
                        bip => string.Equals(bip.symbol, bittrexPrice.MarketName, StringComparison.OrdinalIgnoreCase));

                // check if we found a match - if so, do some simple math to calculate the arbitrage differential
                if (binancePrice != null)
                {
                    // calculate the price discrepency and get the best market price
                    var priceDiscrepency = Price.CalculatePriceDiscrepency(binancePrice.price, bittrexPrice.Last, out var bestPriceMarket);
                    var priceComparison = new PriceComparison
                    {
                        BestMarket = bestPriceMarket,
                        MarketType = marketType,
                        MarketName = bittrexPrice.MarketName,
                        BinancePrice = binancePrice.price,
                        BinanceUsdPrice = CalculateCurrentUsdPrice(marketType, binancePrice.price),
                        BittrexPrice = bittrexPrice.Last,
                        BittrexUsdPrice = CalculateCurrentUsdPrice(marketType, bittrexPrice.Last),
                        PriceDiscrepancy = priceDiscrepency,
                        UsdPriceDiscrepancy = CalculateUsdPriceDiscrepency(marketType, priceDiscrepency),
                        ArbitragePercentage = bestPriceMarket != Markets.EvenMarkets
                                                ? CalculateArbitragePercentage(bestPriceMarket, binancePrice.price, bittrexPrice.Last).ToString("P2")
                                                : decimal.Zero.ToString("P3"),
                        EstimatedProfit = CalculateEstimatedProfit(marketType, bestPriceMarket, binancePrice.price, bittrexPrice.Last)
                    };
                    priceDiffList.Add(priceComparison);
                }
            }

            return priceDiffList;
        }

        private static MarketType DiscernMarketType(string marketName)
        {
            if (marketName.EndsWith(Resources.ETH, StringComparison.OrdinalIgnoreCase))
            { return MarketType.Eth; };
            if (marketName.EndsWith(Resources.USDT, StringComparison.OrdinalIgnoreCase))
            { return MarketType.Usdt; };
            return MarketType.Btc;
        }

        public static decimal SafeDivision(decimal Numerator, decimal Denominator)
        {
            return (Denominator <= 0) ? 0 : Numerator / Denominator;
        }
    }
}
