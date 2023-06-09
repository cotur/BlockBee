namespace Cotur.BlockBee.Contracts.Models.Checkouts
{
    public class CheckoutPaymentInfo
    {
        public string Status { get; set; }
        public string SuccessToken { get; set; }
        public string PaymentUrl { get; set; }
    }
}