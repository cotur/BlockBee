using System.Collections.Generic;

namespace Cotur.BlockBee.Contracts.Models
{
    public class SupportedCoinsInfo
    {
        public Dictionary<string, CryptoCurrencyInfo> CryptoCurrencies { get; set; } = new Dictionary<string, CryptoCurrencyInfo>();

        public Dictionary<string, Dictionary<string, CryptoCurrencyInfo>> NetworkCurrencies { get; set; } = new Dictionary<string, Dictionary<string, CryptoCurrencyInfo>>();
        
        public List<FeeTierInfo> FeeTiers { get; set; }
    }
}