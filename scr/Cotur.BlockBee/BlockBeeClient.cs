using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cotur.BlockBee.Contracts;
using Cotur.BlockBee.Contracts.Models;
using Cotur.BlockBee.Contracts.Models.Checkouts;
using Cotur.BlockBee.Contracts.Models.Deposits;
using Cotur.BlockBee.Contracts.Models.Payment;
using Cotur.BlockBee.Contracts.Models.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Cotur.BlockBee
{
    public class BlockBeeClient : IBlockBeeClient
    {
        #region infrastructure
        
        protected static JsonSerializer JsonSerializer = new JsonSerializer
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        
        protected static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };


        protected RestClient Client = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://api.blockbee.io/"),
            FailOnDeserializationError = false,
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false
        }, configureSerialization: s => s.UseNewtonsoftJson(JsonSerializerSettings));
        
        #endregion

        protected readonly BlockBeeOptions Options;
        
        public BlockBeeClient(IOptions<BlockBeeOptions> options)
        {
            Options = options.Value;
        }
        
        public async Task<InfoDto> GetSupportedCoinsAsync()
        {
            var request = new RestRequest("info")
                .AddParameter("apikey", Options.ApiKey);
            
            var response = await Client.ExecuteGetAsync(request);

            if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(response.Content))
            {
                throw new BlockBeeClientException($"Error for {nameof(GetSupportedCoinsAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            var infoDto = new InfoDto();

            var jObject = JObject.Parse(response.Content);

            if (jObject.ContainsKey("fee_tiers") && jObject["fee_tiers"] != null)
            {
                infoDto.FeeTiers = jObject["fee_tiers"]?.ToObject<List<FeeTierInfo>>(JsonSerializer) ?? new List<FeeTierInfo>();

                jObject.Remove("fee_tiers");
            }

            foreach (var (key, value) in jObject)
            {
                if (value == null)
                {
                    continue;
                }
                    
                var val = (JObject) value;

                if (val.ContainsKey("coin") && val.ContainsKey("ticker"))
                {
                    try
                    {
                        infoDto.CryptoCurrencies.Add(key, val.ToObject<CryptoCurrencyInfo>(JsonSerializer));
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
                else
                {
                    try
                    {
                        infoDto.NetworkCurrencies.Add(key, val.ToObject<Dictionary<string, CryptoCurrencyInfo>>(JsonSerializer));
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }

            return infoDto;
        }

        public async Task<CryptoCurrencyInfo> GetCoinInformationAsync(string ticker, bool includePrices = true)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new BlockBeeClientException($"{nameof(GetCoinInformationAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }

            var request = new RestRequest($"{ticker}/info")
                .AddParameter("apikey", Options.ApiKey);

            if (!includePrices)
            {
                request = request.AddParameter("prices", "0");
            }
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(GetCoinInformationAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }

            return JsonConvert.DeserializeObject<CryptoCurrencyInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task<QrCodeInfo> GetQrCodeInfoAsync(string ticker, string address, int size = 512, int? value = null)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new BlockBeeClientException($"{nameof(GetQrCodeInfoAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            if (string.IsNullOrEmpty(address))
            {
                throw new BlockBeeClientException($"{nameof(GetQrCodeInfoAsync)} - {nameof(address)} cannot be null or empty, please provide a valid address.");
            }
            
            if(size < 64 || size > 1024)
            {
                throw new BlockBeeClientException($"{nameof(GetQrCodeInfoAsync)} - {nameof(size)} must be between 64 and 1024.");
            }
            
            var request = new RestRequest($"{ticker}/qrcode")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("address", address)
                .AddParameter("size", size);

            if (value.HasValue)
            {
                request = request.AddParameter("value", value.Value);
            }
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(GetQrCodeInfoAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<QrCodeInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task<EstimatedBlockchainFeesInfo> GetEstimatedBlockchainFeesAsync(string ticker, int? addressesCount = null, string priority = null)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new BlockBeeClientException($"{nameof(GetEstimatedBlockchainFeesAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            var request = new RestRequest($"{ticker}/estimate")
                .AddParameter("apikey", Options.ApiKey);

            if (addressesCount.HasValue)
            {
                request = request.AddParameter("addresses", addressesCount.Value);
            }
            
            if (!string.IsNullOrEmpty(priority))
            {
                request = request.AddParameter("priority", priority);
            }
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(GetEstimatedBlockchainFeesAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<EstimatedBlockchainFeesInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task<PaymentLog> GetPaymentLogsAsync(string ticker, string callbackUrl)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new BlockBeeClientException($"{nameof(GetPaymentLogsAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            if (string.IsNullOrEmpty(callbackUrl))
            {
                throw new BlockBeeClientException($"{nameof(GetPaymentLogsAsync)} - {nameof(callbackUrl)} cannot be null or empty, please provide a valid callback url.");
            }
            
            var request = new RestRequest($"{ticker}/logs")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("callback", callbackUrl);
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(GetPaymentLogsAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<PaymentLog>(response.Content!, JsonSerializerSettings);
        }

        public async Task<AddressCreationInfo> CreateNewAddressAsync(
            string ticker,
            string callbackUrl,
            string address,
            int minimumConfirmations = 1,
            bool notifyPending = false,
            string priority = null,
            bool usePost = false,
            bool multiToken = false,
            bool multiChain = false,
            bool convert = false)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new BlockBeeClientException($"{nameof(CreateNewAddressAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                throw new BlockBeeClientException($"{nameof(CreateNewAddressAsync)} - {nameof(callbackUrl)} cannot be null or empty, please provide a valid callback url.");
            }

            if (minimumConfirmations < 1)
            {
                throw new BlockBeeClientException($"{nameof(CreateNewAddressAsync)} - {nameof(minimumConfirmations)} must be a positive number.");
            }

            var request = new RestRequest($"{ticker}/create")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("callback", callbackUrl);

            if (!string.IsNullOrWhiteSpace(address))
            {
                request = request.AddParameter("address", address);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                request = request.AddParameter("priority", priority);
            }

            request = request
                .AddParameter("pending", notifyPending ? 1 : 0)
                .AddParameter("confirmations", minimumConfirmations)
                .AddParameter("post", usePost ? 1 : 0)
                .AddParameter("multi_token", multiToken ? 1 : 0)
                .AddParameter("multi_chain", multiChain ? 1 : 0)
                .AddParameter("convert", convert ? 1 : 0);

            var response = await Client.ExecuteGetAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(CreateNewAddressAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<AddressCreationInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task CretePayoutAsync(string ticker, string address, decimal value)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new BlockBeeClientException($"{nameof(CretePayoutAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new BlockBeeClientException($"{nameof(CretePayoutAsync)} - {nameof(address)} cannot be null or empty, please provide a valid address.");
            }
            
            if (value <= 0)
            {
                throw new BlockBeeClientException($"{nameof(CretePayoutAsync)} - {nameof(value)} must be a positive number.");
            }
            
            var request = new RestRequest($"{ticker}/payout")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("address", address)
                .AddParameter("value", value);
            
            var response = await Client.ExecuteGetAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(CretePayoutAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
        }

        public async Task<CheckoutPaymentInfo> CreateCheckoutPaymentAsync(
            string redirectUrl,
            decimal value,
            string itemDescription = null,
            DateTime? expireAt = null,
            string notifyUrl = null,
            bool usePost = false)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                throw new BlockBeeClientException($"{nameof(CreateCheckoutPaymentAsync)} - {nameof(redirectUrl)} cannot be null or empty, please provide a valid url.");
            }
            
            if (value <= 0)
            {
                throw new BlockBeeClientException($"{nameof(CretePayoutAsync)} - {nameof(value)} must be a positive number.");
            }
            
            var request = new RestRequest($"checkout/request/")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("redirect_url", redirectUrl)
                .AddParameter("value", value)
                .AddParameter("post", usePost ? 1 : 0);

            if (!string.IsNullOrWhiteSpace(itemDescription))
            {
                request = request.AddParameter("item_description", itemDescription);
            }
            
            if (expireAt != null)
            {
                if (expireAt.Value < DateTime.Now.AddMinutes(61))
                {
                    throw new BlockBeeClientException(
                        $"{nameof(CretePayoutAsync)} - {nameof(expireAt)} must be a future datetime at least 60 min from now.");
                }
                
                var t = expireAt.Value - new DateTime(1970, 1, 1);
                var epoch = (int) t.TotalSeconds;
                
                request = request.AddParameter("expire_at", epoch);
            }

            if (!string.IsNullOrWhiteSpace(notifyUrl))
            {
                request = request.AddParameter("notify_url", notifyUrl);
            }
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(CreateCheckoutPaymentAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<CheckoutPaymentInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task<CheckoutDepositInfo> CreateCheckoutDepositAsync(
            string notifyUrl,
            string itemDescription = null,
            bool usePost = false)
        {
            if (string.IsNullOrWhiteSpace(notifyUrl))
            {
                throw new BlockBeeClientException($"{nameof(CreateCheckoutDepositAsync)} - {nameof(notifyUrl)} cannot be null or empty, please provide a valid url.");
            }
            
            var request = new RestRequest($"deposit/request/")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("notify_url", notifyUrl)
                .AddParameter("post", usePost ? 1 : 0);

            if (!string.IsNullOrWhiteSpace(itemDescription))
            {
                request = request.AddParameter("item_description", itemDescription);
            }
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(CreateCheckoutDepositAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<CheckoutDepositInfo>(response.Content!, JsonSerializerSettings);
        }

        public async Task<ConvertPriceInfo> ConvertPriceAsync(string ticker, decimal value, string from)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new BlockBeeClientException($"{nameof(ConvertPriceAsync)} - {nameof(ticker)} cannot be null or empty, please provide a valid ticker.");
            }
            
            if (value <= 0)
            {
                throw new BlockBeeClientException($"{nameof(ConvertPriceAsync)} - {nameof(value)} must be a positive number.");
            }
            
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new BlockBeeClientException($"{nameof(ConvertPriceAsync)} - {nameof(from)} cannot be null or empty.");
            }

            var request = new RestRequest($"{ticker}/convert")
                .AddParameter("apikey", Options.ApiKey)
                .AddParameter("value", value)
                .AddParameter("from", from);
            
            var response = await Client.ExecuteGetAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new BlockBeeClientException($"Error for {nameof(ConvertPriceAsync)}, status code: {response.StatusCode}, details: {response.Content} ");
            }
            
            return JsonConvert.DeserializeObject<ConvertPriceInfo>(response.Content!, JsonSerializerSettings);
        }
    }
}