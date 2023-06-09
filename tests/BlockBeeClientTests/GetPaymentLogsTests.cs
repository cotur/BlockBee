using Shouldly;

namespace BlockBeeClientTests;

public class GetPaymentLogsTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldGetPaymentLogsAsync()
    {
        var result = await Client.GetPaymentLogsAsync("bep20/usdt", "https://ctr-blockbee.free.beeceptor.com");
        
        result.Status.ShouldNotBeNullOrEmpty();
        result.CallbackUrl.ShouldNotBeNullOrEmpty();
        result.AddressIn.ShouldNotBeNullOrEmpty();
        result.AddressOut.ShouldNotBeNullOrEmpty();
        result.NotifyConfirmations.ShouldBePositive();
        result.Priority.ShouldNotBeNullOrEmpty();
        
        result.Callbacks.ShouldNotBeNull();
        result.Callbacks.Count.ShouldBePositive();
    }
}