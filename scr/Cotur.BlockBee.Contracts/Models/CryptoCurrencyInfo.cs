using System;
using System.Numerics;

namespace Cotur.BlockBee.Contracts.Models
{
    public class CryptoCurrencyInfo
    {
        public string Coin { get; set; }
        public string Logo { get; set; }
        public string Ticker { get; set; }
        public BigInteger MinimumTransaction { get; set; }
        public string MinimumTransactionCoin { get; set; }
        public BigInteger MinimumFee { get; set; }
        public string MinimumFeeCoin { get; set; }
        public string FeePercent { get; set; }
        public string NetworkFeeEstimation { get; set; }
        public PriceInfo Prices { get; set; }
        public DateTime? PricesUpdated { get; set; }
        public string Status { get; set; }
    }
}

