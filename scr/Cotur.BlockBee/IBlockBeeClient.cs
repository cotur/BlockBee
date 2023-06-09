using System;
using System.Threading.Tasks;
using Cotur.BlockBee.Contracts.Models;
using Cotur.BlockBee.Contracts.Models.Checkouts;
using Cotur.BlockBee.Contracts.Models.Deposits;
using Cotur.BlockBee.Contracts.Models.Payment;
using Cotur.BlockBee.Contracts.Models.Utils;

namespace Cotur.BlockBee
{
    public interface IBlockBeeClient
    {
        Task<InfoDto> GetSupportedCoinsAsync();
        
        /// <summary>
        /// Use this method to retrieve information about the cryptocurrency you intend to use. This endpoint provides detailed information about the selected cryptocurrency.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="includePrices">If you want to receive also the coin prices, set to true to get the prices (default: false)</param>
        /// <returns></returns>
        Task<CryptoCurrencyInfo> GetCoinInformationAsync(string ticker, bool includePrices = false);

        /// <summary>
        /// This method generates a base64-encoded image for payments
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="address">The payment address {`address_in` from BlockBee system}</param>
        /// <param name="size">Size of the QR Code image in pixels. Min: 64 Max: 1024 Default: 512</param>
        /// <param name="value">Value to request the user, in the main coin (BTC, ETH, etc). Optional.
        /// Note: Some user wallets don't support this, only the address. Use at your own discretion.</param>
        /// <returns></returns>
        Task<QrCodeInfo> GetQrCodeInfoAsync(string ticker, string address, int size = 512, int? value = null);
        
        /// <summary>
        /// This method allows you to estimate blockchain fees to process a transaction.
        /// Attention: This is an estimation only, and might change significantly when the transaction is processed. BlockBee is not responsible if blockchain fees when forwarding the funds differ from this estimation.
        ///
        /// Note: Does not include BlockBee's fees.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="addressesCount">The number of addresses to forward the funds to. Default Request Parameter: 1</param>
        /// <param name="priority">Different per currency/network. Check BlockBee knowledge base for more information about this parameter.</param>
        /// <returns></returns>
        Task<EstimatedBlockchainFeesInfo> GetEstimatedBlockchainFeesAsync(string ticker, int? addressesCount = null, string priority = null);

        /// <summary>
        /// This method provides information and callbacks for addresses created through the "create" endpoint.
        /// It returns a list of callbacks made at the specified `callback`, and allows to track payment activity and troubleshoot any issues.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="callbackUrl">The URL of the callback. Must be the same URL provided when the payment was created
        /// Notice: Must be URL Encoded.</param>
        /// <returns></returns>
        Task<PaymentLog> GetPaymentLogsAsync(string ticker, string callbackUrl);

        /// <summary>
        /// This method is used to generate a new address to give your clients, where they can send payments.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="callbackUrl">A VALID and UNIQUE url the callbacks will be sent to. Must be `urlencoded`</param>
        /// <param name="address">This parameter is where you specify where BlockBee will forward your payment.</param>
        /// <param name="notifyPending">Set true if you want to be notified of pending transactions )before they're confirmed)</param>
        /// <param name="minimumConfirmations">Number of blockchain confirmations you want before receiving the calback</param>
        /// <param name="usePost">Set true if you wish to receive the callback as a POST request (default: GET)</param>
        /// <param name="priority">This parameter allows users to set the priority with which funds should be forwarded to the provided `address`.</param>
        /// <param name="multiToken">Allows customers to pay with any token supported by the system, even if the token is different from what the user initially specified.</param>
        /// <param name="multiChain">If enabled, allows you to create the same address on BEP-20, ERC-20 and POLYGON blockchains, which enables you to receive payments on BEP-20, ERC-20 and POLYGON blockchains.</param>
        /// <param name="convert">If enabled, returns the converted value converted to FIAT in the callback</param>
        /// <returns></returns>
        Task<AddressCreationInfo> CreateNewAddressAsync(
            string ticker,
            string callbackUrl,
            string address,
            int minimumConfirmations = 1,
            bool notifyPending = false,
            string priority = null,
            bool usePost = false,
            bool multiToken = false,
            bool multiChain = false,
            bool convert = false);

        /// <summary>
        /// Use this method to create a new Payout Request, which is a request for a payment you wish to send to your customers.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="address">Provide the destination address for the payment or withdrawal. This is the address where you want the funds to be sent.</param>
        /// <param name="value">Please indicate the amount you wish to send to the provided destination `address`. This is the amount that will be transferred or withdrawn from your account.</param>
        /// <returns></returns>
        Task CretePayoutAsync(string ticker, string address, decimal value);

        /// <summary>
        /// This method allows you to create a new Checkout payment.
        /// </summary>
        /// <param name="redirectUrl">URL where your customers will be redirected to after successfully completing the payment. </param>
        /// <param name="value">Value of the order in the currency set in BlockBee's Dashboard payment settings.</param>
        /// <param name="itemDescription">Description of the product or service being paid. This information will appear on the checkout page.</param>
        /// <param name="expireAt">Epoch time in seconds at which the Checkout payment will expire. Minimum is 1h. If not set, it will never expire.</param>
        /// <param name="notifyUrl">Optional URL where BlockBee system will send a payment notification (IPN) when the order is paid. Similar functionality as callback_url of the base API.</param>
        /// <param name="usePost">Set true if you wish to receive the callback as a POST request (default: GET)</param>
        /// <returns></returns>
        Task<CheckoutPaymentInfo> CreateCheckoutPaymentAsync(
            string redirectUrl,
            decimal value,
            string itemDescription = null,
            DateTime? expireAt = null,
            string notifyUrl = null,
            bool usePost = false);

        /// <summary>
        /// This method allows you to create a new Checkout deposit.
        /// </summary>
        /// <param name="notifyUrl">URL where our system will send a payment notification (IPN) when the order is paid. Similar functionality as callback_url of the base API.</param>
        /// <param name="itemDescription">Description of the product or service being paid. This information will appear on the checkout page.</param>
        /// <param name="usePost">Set true if you wish to receive the callback as a POST request (default: GET)</param>
        /// <returns></returns>
        Task<CheckoutDepositInfo> CreateCheckoutDepositAsync(
            string notifyUrl,
            string itemDescription = null,
            bool usePost = false);
        
        /// <summary>
        /// With this method, you can effortlessly convert prices between FIAT and cryptocurrencies, or even between different cryptocurrencies.
        /// </summary>
        /// <param name="ticker">Unique identifier of the cryptocurrency. Example: `btc` for Bitcoin and `trc20/usdt` for USDT over TRC-20</param>
        /// <param name="value">Value you wish to convert in the cryptocurrency/token of the ticker you are using.</param>
        /// <param name="from">Specify the currency you wish to convert from, whether it is FIAT or cryptocurrency.</param>
        /// <returns></returns>
        Task<ConvertPriceInfo> ConvertPriceAsync(string ticker, decimal value, string from);
    }
}