using Shouldly;

namespace BlockBeeClientTests;

public class GetInfoTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldGetInfoAsync()
    {
        var result = await Client.GetSupportedCoinsAsync();
        
        result.CryptoCurrencies.Count.ShouldBeGreaterThan(0);
        result.NetworkCurrencies.Count.ShouldBeGreaterThan(0);
        result.FeeTiers.Count.ShouldBeGreaterThan(0);
    }
}