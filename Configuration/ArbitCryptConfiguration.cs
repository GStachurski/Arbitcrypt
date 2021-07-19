using System;
using System.Configuration;

namespace ArbitCrypt.Configuration
{
    public class ArbitCryptConfiguration
    {
        public ArbitCryptConfiguration()
        {
            BinancePricesUrl = ConfigurationManager.AppSettings["Binance.PricesUrl"];
            BittrexPricesUrl = ConfigurationManager.AppSettings["Bittrex.PricesUrl"];
            CurrentPricesUrl = ConfigurationManager.AppSettings["Compare.PricesUrl"];

            DefaultArbitragePercentage = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultArbitragePercentage"]);
            DefaultPromptTimeoutInSeconds = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultPromptTimeoutInSeconds"]);
            DefaultRefreshTimeoutInSeconds = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultRefreshTimeoutInSeconds"]);

            DefaultColumnFormat = ConfigurationManager.AppSettings["DefaultColumnFormat"];
            DefaultSeperatorWidth = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSeperatorWidth"]);
        }

        // formatting
        public string DefaultColumnFormat { get; set; }
        public int DefaultSeperatorWidth { get; set; }

        // urls
        public string BinancePricesUrl { get; set; }
        public string BittrexPricesUrl { get; set; }
        public string CurrentPricesUrl { get; set; }

        // refresh times and percentages
        public string CurrentRefreshConfigString { get; set; }
        public double DefaultArbitragePercentage { get; set; }
        public double DefaultPromptTimeoutInSeconds { get; set; }
        public double DefaultRefreshTimeoutInSeconds { get; set; }

        //console derived values
        public decimal CurrentEthBalance { get; set; }
        public decimal CurrentBtcBalance { get; set; }
    }
}
