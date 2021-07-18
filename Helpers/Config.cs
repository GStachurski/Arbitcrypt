using ArbitCrypt.Configuration;
using System;

namespace ArbitCrypt.Helpers
{
    public static class Config
    {
        public static string GetCurrentConfig(ArbitCryptConfiguration currentConfig)
        {
            return ($"{currentConfig._defaultArbitragePercentage}|" +     // array 0
                    $"{currentConfig._defaultRefreshTimeoutInSeconds}|" + // array 1
                    $"{currentConfig._currentBtcBalance}|" +              // array 2
                    $"{currentConfig._currentEthBalance}");               // array 3
        }

        public static ArbitCryptConfiguration ResetConfiguration(ArbitCryptConfiguration currentConfig, 
            string newRefreshString)
        {
            // update the values with the new refresh string
            var configStringArray = newRefreshString.Split('|');            
            currentConfig._defaultArbitragePercentage = Convert.ToDouble(configStringArray[0]);
            currentConfig._defaultRefreshTimeoutInSeconds = Convert.ToDouble(configStringArray[1]);
            currentConfig._currentBtcBalance = Convert.ToDecimal(configStringArray[2]);
            currentConfig._currentEthBalance = Convert.ToDecimal(configStringArray[3]);
            return currentConfig;
        }
    }
}
