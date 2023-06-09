using Shouldly;

namespace BlockBeeClientTests;

public class CreateNewAddressTests : BlockBeeTestsBase
{
    //[Fact]
    public async Task ShouldCreateAddressAsync()
    {
        var callbackUrl = "https://ctr-blockbee.free.beeceptor.com/123456";
        
        var result = await Client.CreateNewAddressAsync(
            "bep20/usdt",
            callbackUrl,
            address: null,
            minimumConfirmations: 3,
            notifyPending: true,
            priority: null,
            usePost: true,
            multiToken: true,
            multiChain: true,
            convert: true);
        
        result.Status.ShouldBe("success");
        result.AddressIn.ShouldNotBeNullOrEmpty();
        result.AddressOut.ShouldNotBeNullOrEmpty();
        result.CallbackUrl.ShouldBe(callbackUrl);
        result.MinimumTransactionCoin.ShouldNotBeNullOrEmpty();
        result.ExtraChains.ShouldNotBeNull();
        result.ExtraChains.Count.ShouldBeGreaterThanOrEqualTo(1);
    }
}