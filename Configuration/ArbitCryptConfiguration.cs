using System;
using System.Configuration;

namespace ArbitCrypt.Configuration
{
    public class ArbitCryptConfiguration
    {
        public ArbitCryptConfiguration()
        {
            _binancePricesUrl = ConfigurationManager.AppSettings["Binance.PricesUrl"];
            _bittrexPricesUrl = ConfigurationManager.AppSettings["Bittrex.PricesUrl"];
            _currentPricesUrl = ConfigurationManager.AppSettings["Compare.PricesUrl"];

            _defaultArbitragePercentage = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultArbitragePercentage"]);
            _defaultPromptTimeoutInSeconds = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultPromptTimeoutInSeconds"]);
            _defaultRefreshTimeoutInSeconds = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultRefreshTimeoutInSeconds"]);

            _defaultColumnFormat = ConfigurationManager.AppSettings["DefaultColumnFormat"];
            _defaultSeperatorWidth = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSeperatorWidth"]);
        }

        // formatting
        public string _defaultColumnFormat { get; set; }
        public int _defaultSeperatorWidth { get; set; }

        // urls
        public string _binancePricesUrl { get; set; }
        public string _bittrexPricesUrl { get; set; }
        public string _currentPricesUrl { get; set; }

        // refresh times and percentages
        public string _currentRefreshConfigString { get; set; }
        public double _defaultArbitragePercentage { get; set; }
        public double _defaultPromptTimeoutInSeconds { get; set; }
        public double _defaultRefreshTimeoutInSeconds { get; set; }

        //console derived values
        public decimal _currentEthBalance { get; set; }
        public decimal _currentBtcBalance { get; set; }
    }
}
