using System.Collections.Generic;

namespace Cotur.BlockBee.Contracts.Models.Payment
{
    public class PaymentLog
    {
        public string Status { get; set; }
        public string CallbackUrl { get; set; }
        public string AddressIn { get; set; }
        public string AddressOut { get; set; }
        public bool NotifyPending { get; set; }
        public int NotifyConfirmations { get; set; }
        public string Priority { get; set; }
        public List<CallbackInfo> Callbacks { get; set; }
    }
}