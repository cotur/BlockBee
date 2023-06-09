using Shouldly;

namespace BlockBeeClientTests;

public class CreateCheckoutTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldCreatCheckoutPaymentAsync()
    {
        var response = await Client.CreateCheckoutPaymentAsync(
            "https://ctr-blockbee.free.beeceptor.com/order/211",
            100,
            "Test item",
            DateTime.Now.AddHours(1).AddMinutes(1));
        
        response.Status.ShouldBe("success");
        response.SuccessToken.ShouldNotBeNullOrEmpty();
        response.PaymentUrl.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldCreateCheckoutDepositAsync()
    {
        var response = await Client.CreateCheckoutDepositAsync(
            "https://ctr-blockbee.free.beeceptor.com/checkout/deposit/1",
            "Deposit Item",
            true);
        
        response.Status.ShouldBe("success");
        response.PaymentUrl.ShouldNotBeNullOrEmpty();
    }
}