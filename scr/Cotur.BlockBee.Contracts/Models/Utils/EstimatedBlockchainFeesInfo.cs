namespace Cotur.BlockBee.Contracts.Models.Utils
{
    public class EstimatedBlockchainFeesInfo
    {
        public string Status { get; set; }

        public string EstimatedCost { get; set; }

        public PriceInfo EstimatedCostCurrency { get; set; }
    }
}