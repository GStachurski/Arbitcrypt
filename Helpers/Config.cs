using ArbitCrypt.Configuration;
using System;

namespace ArbitCrypt.Helpers
{
    public static class Config
    {
        public static string GetCurrentConfig(ArbitCryptConfiguration currentConfig)
        {
            return ($"{currentConfig.DefaultArbitragePercentage}|" +     // array 0
                    $"{currentConfig.DefaultRefreshTimeoutInSeconds}|" + // array 1
                    $"{currentConfig.CurrentBtcBalance}|" +              // array 2
                    $"{currentConfig.CurrentEthBalance}");               // array 3
        }

        public static ArbitCryptConfiguration ResetConfiguration(ArbitCryptConfiguration currentConfig, 
            string newRefreshString)
        {
            // update the values with the new refresh string
            var configStringArray = newRefreshString.Split('|');            
            currentConfig.DefaultArbitragePercentage = Convert.ToDouble(configStringArray[0]);
            currentConfig.DefaultRefreshTimeoutInSeconds = Convert.ToDouble(configStringArray[1]);
            currentConfig.CurrentBtcBalance = Convert.ToDecimal(configStringArray[2]);
            currentConfig.CurrentEthBalance = Convert.ToDecimal(configStringArray[3]);
            return currentConfig;
        }
    }
}
