using Shouldly;

namespace BlockBeeClientTests;

public class CreatePayoutTests : BlockBeeTestsBase
{
    [Fact]
    public async Task ShouldCreatePayoutAsync()
    {
        await Should.NotThrowAsync(async () =>
            await Client.CretePayoutAsync("bep20/usdt", "0x13483d3fbd5e7cc23c9879e544fc0b13354a4b5f", 11)
            );
    }
}