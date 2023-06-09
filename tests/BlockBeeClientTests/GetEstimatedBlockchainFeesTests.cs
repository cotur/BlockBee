using Shouldly;

namespace BlockBeeClientTests;

public class GetEstimatedBlockchainFeesTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldGetEstimatedBlockchainFeesAsync()
    {
        var result = await Client.GetEstimatedBlockchainFeesAsync("btc");
        
        result.Status.ShouldBe("success");
        result.EstimatedCost.ShouldNotBeNullOrEmpty();
        result.EstimatedCostCurrency.ShouldNotBeNull();
        result.EstimatedCostCurrency.Count.ShouldBeGreaterThan(0);
        result.EstimatedCostCurrency.ShouldContainKey("USD");
        result.EstimatedCostCurrency["USD"].ShouldNotBeNullOrEmpty();
    }
}