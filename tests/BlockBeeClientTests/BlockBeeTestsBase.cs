using Cotur.BlockBee;
using Microsoft.Extensions.Options;

namespace BlockBeeClientTests;

public abstract class BlockBeeTestsBase
{
    protected IBlockBeeClient Client = new BlockBeeClient(Options.Create(new BlockBeeOptions
    {
        ApiKey = "ADD_YOUR_API_KEY_TO_RUN_TESTS"
    }));
}