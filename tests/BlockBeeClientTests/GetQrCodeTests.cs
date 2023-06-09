using Shouldly;

namespace BlockBeeClientTests;

public class GetQrCodeTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldGetQrCodeAsync()
    {
        var result = await Client.GetQrCodeInfoAsync("bep20/usdt", "0x000000000");
        
        result.Status.ShouldBe("success");
        result.PaymentUri.ShouldBe("0x000000000");
        result.QrCode.ShouldNotBeNullOrEmpty();
    }
}