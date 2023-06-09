using Shouldly;

namespace BlockBeeClientTests;

public class GetCoinInfoTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldGetCoinInformationWithoutPricesAsync()
    {
        var result = await Client.GetCoinInformationAsync("btc", false);
        
        result.Ticker.ShouldBe("btc");
        result.Coin.ShouldBe("Bitcoin");
        result.Logo.ShouldNotBeNullOrEmpty();
        
        result.MinimumTransaction.ShouldBeGreaterThan(0);
        result.MinimumTransactionCoin.ShouldNotBeNullOrEmpty();
        
        result.MinimumFee.ShouldBeGreaterThan(0);
        result.MinimumFeeCoin.ShouldNotBeNullOrEmpty();
        
        result.FeePercent.ShouldNotBeNullOrEmpty();
        result.NetworkFeeEstimation.ShouldNotBeNullOrEmpty();
        result.Status.ShouldBe("success");
        
        result.Prices.ShouldBeNull();
        result.PricesUpdated.ShouldBeNull();
    }
    
    [Fact]
    public async Task ShouldGetCoinInformationWithPricesAsync()
    {
        var result = await Client.GetCoinInformationAsync("btc", true);
        
        result.Ticker.ShouldBe("btc");
        result.Coin.ShouldBe("Bitcoin");
        result.Logo.ShouldNotBeNullOrEmpty();
        
        result.MinimumTransaction.ShouldBeGreaterThan(0);
        result.MinimumTransactionCoin.ShouldNotBeNullOrEmpty();
        
        result.MinimumFee.ShouldBeGreaterThan(0);
        result.MinimumFeeCoin.ShouldNotBeNullOrEmpty();
        
        result.FeePercent.ShouldNotBeNullOrEmpty();
        result.NetworkFeeEstimation.ShouldNotBeNullOrEmpty();
        result.Status.ShouldBe("success");
        
        result.Prices.ShouldNotBeNull();
        result.Prices.ShouldContainKey("USD");
        result.Prices["USD"].ShouldNotBeNull();
        result.PricesUpdated.ShouldNotBeNull();
    }
}