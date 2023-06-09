# Cotur.BlockBee

 [BlockBee](https://blockbee.io) api .NET wrappper.
 
 ## Features
 
 This client covers all API endpoints explained at [https://docs.blockbee.io/](https://docs.blockbee.io/)
 
 * Receive a payment
   * Create New Address
   * Check Payment Logs
 * Utils
   * Check Coin Information
   * Generate QR Code
   * Estimate Blockchain Feed
   * Convert Prices
 * Checkout
   * Create a Checkout Payments
   * Create a Checkout Deposits
 * Payouts
   * Create a Payout Request

## Installation

You can download the source code and add it your your project.

Alternatively, you can use NuGet to install the package.

### .NET CLI

```bash
dotnet add package Cotur.BlockBee
```

### Package Manager

```bash
Install-Package Cotur.BlockBee
```

## Usage

### Without DI

```csharp
var options = Options.Create(new BlockBeeOptions
    {
        ApiKey = "ADD_YOUR_API_KEY"
    };

IBlockBeeClient client = new BlockBeeClient(options);
```

### With DI

```csharp
// Register 
services.AddTransient<IBlockBeeClient, BlockBeeClient>();

// Inject
class MyService
{
    private readonly IBlockBeeClient _blockBeeClient;

    public MyService(IBlockBeeClient blockBeeClient)
    {
       _blockBeeClient = blockBeeClient;
    }
}
```





