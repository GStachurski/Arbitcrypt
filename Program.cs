using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using ArbitCrypt.Classes;
using ArbitCrypt.Enums;
using ArbitCrypt.Helpers;
using ArbitCrypt.Configuration;
using ArbitCrypt.Properties;

namespace ArbitCrypt
{
    public class Program
    {
        // variables, helpers, and configuration
        private static int _priceReadoutCount = 1;
        private static Calculation _calcHelper;
        private static CurrentPrices _currentPrices;
        private static ArbitCryptConfiguration _config;

        public static void Main()
        {
            // initialize the url values, gathering the refreshing time and percentage alert from the user, and setting the prompt timer
            GatherConfigAndSessionValues();            

            while (true)
            {
                // 1. call the binance API and get the current price pairs (also set the performance timer)           
                var binancePrices = new List<BinancePrices>();
                var bittrexPrices = new BittrexPrices();
                var timer = new Stopwatch(); timer.Start();

                // 2. make all your web requests for current exchange prices and market data  
                using (var client = new WebClient())
                {                    
                    binancePrices = JsonConvert.DeserializeObject<List<BinancePrices>>(
                        client.DownloadString(_config._binancePricesUrl));
                    bittrexPrices = JsonConvert.DeserializeObject<BittrexPrices>(
                        client.DownloadString(_config._bittrexPricesUrl));
                    _currentPrices.BTCUSD  = JsonConvert.DeserializeObject<CurrentPrices>(
                        client.DownloadString(_config._currentPricesUrl + Price.GetCryptoFiatPair(Resources.BTC))).USD;
                    _currentPrices.ETHUSD  = JsonConvert.DeserializeObject<CurrentPrices>(
                        client.DownloadString(_config._currentPricesUrl + Price.GetCryptoFiatPair(Resources.ETH))).USD;
                    _currentPrices.USDTUSD = JsonConvert.DeserializeObject<CurrentPrices>(
                        client.DownloadString(_config._currentPricesUrl + Price.GetCryptoFiatPair(Resources.USDT))).USD;
                }

                // 3. stop the timer, set the calc helper up with the current prices and most recent config
                //    and format the bittrex prices for market name matching
                timer.Stop(); PriceFormatting.FormatBittrexPrices(bittrexPrices);
                _calcHelper = new Calculation(_currentPrices, _config);

                // 4. compare the prices, find the price deltas, and write it to console
                var priceComparisons = Calculation.CalculatePriceDeltas(binancePrices, bittrexPrices);

                // 5. output the prices and crypto-pairs for an overall review 
                Dialog.CurrentConfig = _config;
                Dialog.WritePricesToConsole(priceComparisons);

                // 6. list the pairs that are over the predefined threshold
                var overThresholdPairs = priceComparisons.Where(pc => 
                                            Convert.ToDouble(pc.ArbitragePercentage.Replace("%", string.Empty)) > 
                                                _config._defaultArbitragePercentage);

                //6a. check if there are any to begin with
                if (overThresholdPairs != null && overThresholdPairs.Any())
                    Dialog.WritePricesToConsole(overThresholdPairs.ToList(), true);

                // 7. wait for the provided refresh time before running again, out the current\default config and symbol keys
                //    and prompt for any refresh config variables
                Dialog.WriteConfigurationAndDiagnosticsToConsole(timer, _priceReadoutCount++);
                var configurationstring = Dialog.ReadConfigurationLine(
                    Convert.ToInt32(_config._defaultRefreshTimeoutInSeconds), 
                    Config.GetCurrentConfig(_config));
                _config = Config.ResetConfiguration(_config, configurationstring);
            }
        }

        private static void GatherConfigAndSessionValues()
        {
            // new up the config, prices, and string resources 
            // also set the tls ver. for the pricing urls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _config = new ArbitCryptConfiguration();
            _currentPrices = new CurrentPrices();

            // show the current version and copyright
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"{assembly.GetName().Name} - {assembly.GetName().Version} © {DateTime.Now.Year} {Resources.CompanyName}");

            // prompt for all the information we need
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.RefreshPricesAsk, _config));
            _config._defaultRefreshTimeoutInSeconds = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.RefreshPricesAnswer, _config));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.BitcoinAndEtherAsk, _config));
            string[] currentHoldings = Console.ReadLine().Split('|');
            _config._currentBtcBalance = Convert.ToDecimal(currentHoldings[0]);
            _config._currentEthBalance = Convert.ToDecimal(currentHoldings[1]);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.BitcoinAndEtherAnswer, _config));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.ArbitrageGainAsk, _config));
            _config._defaultArbitragePercentage = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Dialog.GetDialog(ConsoleDialogs.ArbitrageGainAnswer, _config));
            Thread.Sleep(1800);
        }
    }
}