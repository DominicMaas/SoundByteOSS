using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.System;

namespace SoundByte.App.UWP.Services
{
    public class StoreService : IStoreService
    {
        private readonly StoreContext _storeContext;
        private readonly IDialogService _dialogService;
        private readonly ITelemetryService _telemetryService;
        private readonly List<KeyValuePair<string, StoreProduct>> _products = new List<KeyValuePair<string, StoreProduct>>();

        private bool _hasPremium;

        public StoreService(IDialogService dialogService, ITelemetryService telemetryService)
        {
            _storeContext = StoreContext.GetDefault();
            _dialogService = dialogService;
            _telemetryService = telemetryService;
        }

        public async Task InitializeAsync()
        {
            return; // TODO: Remove

            try
            {
                // Clear Product List
                _products.Clear();

                // Specify the kinds of add-ons to retrieve.
                var filterList = new List<string> { "Durable", "Subscription", "Consumable" };

                // Specify the Store IDs of the products to retrieve.
                var storeIds = new[]
                {
                    "9PHZ0ZS2V06Z", // Subscription
                    "9NZW466C1857", // One Time
                    "9P3VLS5WTFT6", // Donate Loose Change
                    "9MSXRVNLNLJ7", // Donate Small Coffee
                    "9PNSD6HSKWPK", // Donate Large Coffee
                    "9NRGS6R2GRSZ" // Donate regular coffee
                };

                // Get all the products and add them to the product list
                var results = await _storeContext.GetStoreProductsAsync(filterList, storeIds);
                _products.AddRange(results.Products);

                var donateItemExists = _products.Any(x => x.Key == "9P3VLS5WTFT6"
                    || x.Key == "9MSXRVNLNLJ7"
                    || x.Key == "9PNSD6HSKWPK"
                    || x.Key == "9NRGS6R2GRSZ");

                var hasUserDonated = donateItemExists
                    && ((_products.First(x => x.Key == "9P3VLS5WTFT6").Value?.IsInUserCollection ?? false)
                    || (_products.First(x => x.Key == "9MSXRVNLNLJ7").Value?.IsInUserCollection ?? false)
                    || (_products.First(x => x.Key == "9PNSD6HSKWPK").Value?.IsInUserCollection ?? false)
                    || (_products.First(x => x.Key == "9NRGS6R2GRSZ").Value?.IsInUserCollection ?? false));

                // If the user has donated before, we unlock the app via donation
                if (hasUserDonated)
                {
                    _hasPremium = true;
                    return;
                }

                var subscriptionExists = _products.Any(x => x.Key == "9PHZ0ZS2V06Z");
                var hasUserSubscribed = subscriptionExists
                    && (_products.FirstOrDefault(x => x.Key == "9PHZ0ZS2V06Z").Value?.IsInUserCollection ?? false);

                // The user has purchased a subscription
                if (hasUserSubscribed)
                {
                    _hasPremium = true;
                    return;
                }

                var oneTimeExists = _products.Any(x => x.Key == "9NZW466C1857");
                var hasUserPurchasedOneTime = oneTimeExists
                    && (_products.FirstOrDefault(x => x.Key == "9NZW466C1857").Value?.IsInUserCollection ?? false);

                // The user bought the app
                if (hasUserPurchasedOneTime)
                {
                    _hasPremium = true;
                    return;
                }

                // By default the user has not purchased the app
                _hasPremium = false;
            }
            catch (Exception ex)
            {
                // Something went wrong, not sure what, default to no
                _hasPremium = false;
                _telemetryService.TrackException(ex);
            }
        }

        public Task<bool> HasPremiumAsync()
        {
            return Task.FromResult(_hasPremium);
        }

        public Task OpenStorePageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task PurchaseOneTimeAsync()
        {
            await PurchaseAsync("9NZW466C1857");
        }

        public async Task RequestRefundAsync()
        {
            await _dialogService.ShowInfoMessageAsync("Not yet supported.", "Please visit https://account.microsoft.com to manage your subscriptions.");
        }

        public async Task RequestReviewAsync()
        {
            if (ApiInformation.IsMethodPresent("Windows.Services.Store.StoreContext", nameof(StoreContext.RequestRateAndReviewAppAsync)))
            {
                var storeContext = StoreContext.GetDefault();
                await storeContext.RequestRateAndReviewAppAsync();
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
        }

        public async Task SubscribeToPremiumAsync()
        {
            await PurchaseAsync("9PHZ0ZS2V06Z");
        }

        public async Task UnsubscribeFromPremiumAsync()
        {
            await _dialogService.ShowInfoMessageAsync("Not yet supported.", "Please visit https://account.microsoft.com to manage your subscriptions.");
        }

        #region Helpers

        private async Task PurchaseAsync(string key)
        {
            _telemetryService.TrackEvent("Purchase Pro Attempt");

            // Get the item
            var item = _products.FirstOrDefault(x => x.Key == key).Value;

            try
            {
                if (item != null)
                {
                    // Request to purchase the item
                    var result = await item.RequestPurchaseAsync();

                    // Check if the purchase was successful
                    if (result.Status == StorePurchaseStatus.Succeeded)
                    {
                        _telemetryService.TrackEvent("Purchase Pro Success");
                        await _dialogService.ShowInfoMessageAsync("SoundByte", "Thank you for your purchase!");
                    }
                    else
                    {
                        _telemetryService.TrackEvent("Purchase Pro Fail", new Dictionary<string, string> { { "Reason", result.ExtendedError?.Message } });
                        await _dialogService.ShowInfoMessageAsync("SoundByte", "Your account has not been charged:\n" + result.ExtendedError?.Message);
                    }
                }
                else
                {
                    await _dialogService.ShowInfoMessageAsync("SoundByte", "Your account has not been charged:\n" + "Unknown Error");
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowInfoMessageAsync("SoundByte", "Your account has not been charged:\n" + e.Message);
            }
        }

        #endregion Helpers
    }
}