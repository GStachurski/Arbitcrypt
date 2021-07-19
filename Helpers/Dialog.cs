using ArbitCrypt.Classes;
using ArbitCrypt.Configuration;
using ArbitCrypt.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArbitCrypt.Helpers
{
    public static class Dialog
    {        
        public static ArbitCryptConfiguration CurrentConfig { get; set; }

        public static string GetDialog(ConsoleDialogs consoleDialog, ArbitCryptConfiguration config = null)
        {
            var dialog = string.Empty;
            switch (consoleDialog)
            {
                case ConsoleDialogs.ConsoleTimeoutDialog:
                    dialog = "Console timeout reached ({0} seconds)";
                    break;

                case ConsoleDialogs.ResetDialog:
                    dialog = "{ArbitrageGainPercentage}|{PriceRefreshInSeconds}|{CurrentBtcHoldings}|{CurrentEthHoldings}";
                    break;

                case ConsoleDialogs.LanguageWarning:
                    dialog = "This application has the ability to contain vulgar language. Would you like to use it? ('Y' or 'N')";
                    break;
                case ConsoleDialogs.ThresholdWarning:
                    dialog = $"You have a few crypto pairs that are over your predefined arbitrage threshold of {config.DefaultArbitragePercentage}%";
                    break;
                case ConsoleDialogs.RefreshPricesAsk:
                    dialog = "Hey there pal, welcome to ArbitCrypt. Tell me, how many seconds should I wait before refreshing the prices?";
                    break;
                case ConsoleDialogs.RefreshPricesAnswer:
                    dialog = $"Alright, no problem pal, I'll wait for {config.DefaultRefreshTimeoutInSeconds} seconds before refreshing the prices for you.";
                    break;
                case ConsoleDialogs.BitcoinAndEtherAsk:
                    dialog = $"For profit calculation purposes, tell me how much BTC and ETH you have. Enter them in '|' seperated decimal form, buddy!";
                    break;
                case ConsoleDialogs.BitcoinAndEtherAnswer:
                    dialog = $"Excellent, it looks like you have {config.CurrentBtcBalance} BTC, and {config.CurrentEthBalance} ETH available for some good gains.";
                    break;
                case ConsoleDialogs.ArbitrageGainAsk:
                    dialog = $"Well, one more question. At what percent arbitrage gain should I tell you that the pair is over your threshold? The default is currently to {config.DefaultArbitragePercentage}%.";
                    break;
                case ConsoleDialogs.ArbitrageGainAnswer:
                    dialog = $"Alright, that sounds great to me. I will be on the lookout for those gains at or above {config.DefaultArbitragePercentage}%.";
                    break;
            }

            return dialog;
        }

        public static string ReadConfigurationLine(int defaultRefreshTimeoutInSeconds, string currentConfigString)
        {
            try { currentConfigString = TimeableReader.ReadLine(defaultRefreshTimeoutInSeconds); }
            catch (TimeoutException timeEx) { Console.WriteLine(timeEx.Message); }
            return currentConfigString;
        }

        public static void WriteConfigurationAndDiagnosticsToConsole(Stopwatch timer, int priceReadoutCount)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Configuration and Diagnostics:");
            Console.WriteLine(new string('-', CurrentConfig.DefaultSeperatorWidth));
            Console.WriteLine($"[BTC|ETH]                     {CurrentConfig.CurrentBtcBalance}|{CurrentConfig.CurrentEthBalance}");
            Console.WriteLine($"[PriceRefreshTime]            {CurrentConfig.DefaultRefreshTimeoutInSeconds} seconds");
            Console.WriteLine($"[PriceRequestsTime]           {timer.Elapsed.Seconds}.{timer.ElapsedMilliseconds} seconds");
            Console.WriteLine($"[TimesRunSinceStart]          {priceReadoutCount}");
            Console.WriteLine($"[ArbitrageGainPercentage]     {CurrentConfig.DefaultArbitragePercentage}%");
            Console.WriteLine($"[CurrentConfigString]         {Config.GetCurrentConfig(CurrentConfig)}");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Enter new config values in the following format:");
            Console.WriteLine(GetDialog(ConsoleDialogs.ResetDialog));
        }

        public static void WritePricesToConsole(List<PriceComparison> priceComparisons, bool anyOverThreshold = false)
        {
            // warning dialog
            Console.WriteLine(Environment.NewLine);
            if (anyOverThreshold)
            {
                Console.WriteLine(GetDialog(ConsoleDialogs.ThresholdWarning, CurrentConfig));
                Console.WriteLine(Environment.NewLine);
            };

            // header
            Console.WriteLine(CurrentConfig.DefaultColumnFormat,
                "MarketName",
                "BinUsd",
                "BitUsd",
                "Binance",
                "Bittrex",
                "PriceDiscrepancy",
                "ArbtUsdPrice",
                "ArbtPercent",
                "EstProfit");
            Console.WriteLine(new string('-', CurrentConfig.DefaultSeperatorWidth));

            // TODO: final list filtering (volume, 100%ers, etc.)

            // rows
            foreach (var pc in priceComparisons.OrderByDescending(pc => 
                // removing the currency symbol so the sorting works properly
                Convert.ToDecimal(pc.EstimatedProfit.Remove(0, 1))))
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine(CurrentConfig.DefaultColumnFormat,
                    pc.MarketName,
                    PriceFormatting.ApplyUsdPriceFormats(pc.BinanceUsdPrice, pc.BestMarket, Markets.Binance),
                    PriceFormatting.ApplyUsdPriceFormats(pc.BittrexUsdPrice, pc.BestMarket, Markets.Bittrex),
                    PriceFormatting.ApplyCryptoPriceFormats(pc.BinancePrice, pc.BestMarket, Markets.Binance, pc.MarketType),
                    PriceFormatting.ApplyCryptoPriceFormats(pc.BittrexPrice, pc.BestMarket, Markets.Bittrex, pc.MarketType),
                    PriceFormatting.ApplyBaseCryptoFormat(pc.PriceDiscrepancy),
                    pc.UsdPriceDiscrepancy.ToString("C3"),
                    pc.ArbitragePercentage,
                    pc.EstimatedProfit);
            }
            Console.OutputEncoding = System.Text.Encoding.ASCII;
        }
    }
}
