#nullable enable

using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace SoundByte.App.Uwp.Services
{
    public partial class PremiumService
    {
        #region Variables

        private readonly StoreContext _storeContext;
        public readonly List<KeyValuePair<string, StoreProduct>> Products = new List<KeyValuePair<string, StoreProduct>>();

        public PremiumStatus Status { get; private set; } = PremiumStatus.Unknown;

        /// <summary>
        ///     Is the app in premium mode
        /// </summary>
        public bool IsPremium => Status == PremiumStatus.Donated
            || Status == PremiumStatus.Purchased
            || Status == PremiumStatus.Subscription
            || Status == PremiumStatus.AccountLinked;

        #endregion Variables

        #region Constructor

        private PremiumService()
        {
            _storeContext = StoreContext.GetDefault();
        }

        #endregion Constructor

        #region Status

        public enum PremiumStatus
        {
            Unknown,
            Free,
            Donated,
            Purchased,
            Subscription,
            AccountLinked
        }

        #endregion Status

        /// <summary>
        ///     Get all the IAPs for this app
        /// </summary>
        /// <returns></returns>
        public async Task InitProductInfoAsync()
        {
            try
            {
                // Clear Product List
                Products.Clear();

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
                Products.AddRange(results.Products);

                var donateItemExists = Products.Any(x => x.Key == "9P3VLS5WTFT6"
                    || x.Key == "9MSXRVNLNLJ7"
                    || x.Key == "9PNSD6HSKWPK"
                    || x.Key == "9NRGS6R2GRSZ");

                var hasUserDonated = donateItemExists
                    && ((Products.First(x => x.Key == "9P3VLS5WTFT6").Value?.IsInUserCollection ?? false)
                    || (Products.First(x => x.Key == "9MSXRVNLNLJ7").Value?.IsInUserCollection ?? false)
                    || (Products.First(x => x.Key == "9PNSD6HSKWPK").Value?.IsInUserCollection ?? false)
                    || (Products.First(x => x.Key == "9NRGS6R2GRSZ").Value?.IsInUserCollection ?? false));

                // If the user has donated before, we unlock the app via donation
                if (hasUserDonated)
                {
                    Status = PremiumStatus.Donated;
                    return;
                }

                var subscriptionExists = Products.Any(x => x.Key == "9PHZ0ZS2V06Z");

                var hasUserSubscribed = subscriptionExists
                    && (Products.FirstOrDefault(x => x.Key == "9PHZ0ZS2V06Z").Value?.IsInUserCollection ?? false);

                // The user has purchased a subscription
                if (hasUserSubscribed)
                {
                    Status = PremiumStatus.Subscription;
                    return;
                }

                var oneTimeExists = Products.Any(x => x.Key == "9NZW466C1857");

                var hasUserPurchasedOneTime = oneTimeExists
                    && (Products.FirstOrDefault(x => x.Key == "9NZW466C1857").Value?.IsInUserCollection ?? false);

                // The user bought the app
                if (hasUserPurchasedOneTime)
                {
                    Status = PremiumStatus.Purchased;
                    return;
                }

                // By default the user has not purchased the app
                Status = PremiumStatus.Free;
            }
            catch
            {
                // Something went wrong, not sure what
                Status = PremiumStatus.Unknown;
            }
        }

        public async Task PurchaseOneTimeAsync()
        {
            await PurchaseAsync("9NZW466C1857");
        }

        public async Task PurchaseSubscriptionAsync()
        {
            await PurchaseAsync("9PHZ0ZS2V06Z");
        }

        public async Task CancelSubscriptionAsync()
        {
            await NavigationService.Current.CallMessageDialogAsync("Not yet supported. Please visit https://account.microsoft.com to manage your subscriptions.");
        }

        private async Task PurchaseAsync(string key)
        {
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Purchase Pro Attempt");

            // Get the item
            var item = Products.FirstOrDefault(x => x.Key == key).Value;

            try
            {
                if (item != null)
                {
                    // Request to purchase the item
                    var result = await item.RequestPurchaseAsync();

                    // Check if the purchase was successful
                    if (result.Status == StorePurchaseStatus.Succeeded)
                    {
                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Purchase Pro Success");

                        await new MessageDialog("Thank you for your purchase!", "SoundByte").ShowAsync();
                    }
                    else
                    {
                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Purchase Pro Fail",
                            new Dictionary<string, string> { { "Reason", result.ExtendedError?.Message } });

                        await new MessageDialog("Your account has not been charged:\n" + result.ExtendedError?.Message,
                            "SoundByte").ShowAsync();
                    }
                }
                else
                {
                    await new MessageDialog("Your account has not been charged:\n" + "Unknown Error",
                        "SoundByte").ShowAsync();
                }
            }
            catch (Exception e)
            {
                await new MessageDialog("Your account has not been charged:\n" + e.Message,
                    "SoundByte").ShowAsync();
            }
        }
    }

    public partial class PremiumService
    {
        private static readonly Lazy<PremiumService> InstanceHolder =
            new Lazy<PremiumService>(() => new PremiumService());

        public static PremiumService Current => InstanceHolder.Value;
    }
}