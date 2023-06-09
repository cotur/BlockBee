using System.Collections.Generic;

namespace Cotur.BlockBee.Contracts.Models.Payment
{
    public class AddressCreationInfo
    {
        public string Status { get; set; }
        public string AddressIn { get; set; }
        public string AddressOut { get; set; }
        public string CallbackUrl { get; set; }
        public string MinimumTransactionCoin { get; set; }
        public string Priority { get; set; }
        public bool MultiToken { get; set; }
        public List<string> ExtraChains { get; set; }
    }
}