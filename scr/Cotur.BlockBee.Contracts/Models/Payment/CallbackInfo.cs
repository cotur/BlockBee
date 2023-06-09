using System;
using System.Collections.Generic;
using System.Numerics;

namespace Cotur.BlockBee.Contracts.Models.Payment
{
    public class CallbackInfo
    {
        public Guid Uuid { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Result { get; set; }
        
        public int Confirmations { get; set; }
        
        public decimal FeePercent { get; set; }
        
        [Obsolete("Use FeeCoin instead")]
        public BigInteger Fee { get; set; }
        public decimal FeeCoin { get; set; }
        
        [Obsolete("Use ValueCoin instead")]
        public BigInteger Value { get; set; }
        public decimal ValueCoin { get; set; }
        
        [Obsolete("Use ValueForwardedCoin instead")]
        public BigInteger ValueForwarded { get; set; }
        public decimal ValueForwardedCoin { get; set; }

        public string Price { get; set; }
        public string Coin { get; set; }
        
        public string TxidIn { get; set; }
        public string TxidOut { get; set; }
        
        public List<CallbackLog> Logs { get; set; }
    }
}