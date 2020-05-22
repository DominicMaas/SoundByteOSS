using SoundByte.App.Uwp.Services;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Panes
{
    public sealed partial class PremiumPaneView
    {
        public PremiumPaneView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            switch (PremiumService.Current.Status)
            {
                case PremiumService.PremiumStatus.Free:
                    PremiumStatus.Text = "Status: Free";
                    PremiumDescription.Text = "SoundByte Pro is not active. Purchase to unlock features.";
                    PurchaseOneTimeButton.IsEnabled = true;
                    PurchaseSubscriptionButton.IsEnabled = true;
                    UnsubscribeButton.Visibility = Visibility.Collapsed;
                    break;

                case PremiumService.PremiumStatus.Unknown:
                    PremiumStatus.Text = "Status: Unknown";
                    PremiumDescription.Text = "The status of SoundByte Premium is unknown, please restart the app.";
                    PurchaseOneTimeButton.IsEnabled = false;
                    PurchaseSubscriptionButton.IsEnabled = false;
                    UnsubscribeButton.Visibility = Visibility.Collapsed;
                    break;

                case PremiumService.PremiumStatus.Donated:
                    PremiumStatus.Text = "Status: Previously Donated";
                    PremiumDescription.Text = "SoundByte Premium has been unlocked via a previous application purchase.";
                    PurchaseOneTimeButton.IsEnabled = false;
                    PurchaseSubscriptionButton.IsEnabled = false;
                    UnsubscribeButton.Visibility = Visibility.Collapsed;
                    break;

                case PremiumService.PremiumStatus.Purchased:
                    PremiumStatus.Text = "Status: Purchased";
                    PremiumDescription.Text = "SoundByte Premium was unlocked via a purchase.";
                    PurchaseOneTimeButton.IsEnabled = false;
                    PurchaseSubscriptionButton.IsEnabled = false;
                    UnsubscribeButton.Visibility = Visibility.Collapsed;
                    break;

                case PremiumService.PremiumStatus.Subscription:
                    PremiumStatus.Text = "Status: Subscription";
                    PremiumDescription.Text = "SoundByte Premium was unlocked via a subscription.";
                    PurchaseOneTimeButton.IsEnabled = false;
                    PurchaseSubscriptionButton.IsEnabled = false;
                    PurchaseSubscriptionButton.Visibility = Visibility.Visible;
                    UnsubscribeButton.Visibility = Visibility.Visible;
                    break;

                case PremiumService.PremiumStatus.AccountLinked:
                    PremiumStatus.Text = "Status: Account Linked";
                    PremiumDescription.Text = "This device is logged in with a Premium linked account.";
                    PurchaseOneTimeButton.IsEnabled = false;
                    PurchaseSubscriptionButton.IsEnabled = false;
                    UnsubscribeButton.Visibility = Visibility.Collapsed;
                    break;
            }

            // One-Time
            try
            {
                var storeProduct = PremiumService.Current.Products.FirstOrDefault(x => x.Key == "9NZW466C1857").Value;
                PurchaseOneTimeButton.Content = $"One-Time ({storeProduct.Price.FormattedPrice} {storeProduct.Price.CurrencyCode})";
            }
            catch
            {
                PurchaseOneTimeButton.Content = "One-Time (price unknown)";
            }

            // Subscription
            try
            {
                var storeProduct = PremiumService.Current.Products.FirstOrDefault(x => x.Key == "9PHZ0ZS2V06Z").Value;
                PurchaseSubscriptionButton.Content = $"Subscribe ({storeProduct.Price.FormattedRecurrencePrice} {storeProduct.Price.CurrencyCode}/month)";
            }
            catch
            {
                PurchaseSubscriptionButton.Content = "Subscribe (?/month)";
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Load();
        }

        private async void PurchaseOneTime(object sender, RoutedEventArgs e)
        {
            await PremiumService.Current.PurchaseOneTimeAsync();
        }

        private async void PurchaseSubscription(object sender, RoutedEventArgs e)
        {
            await PremiumService.Current.PurchaseSubscriptionAsync();
        }

        private async void CancelSubscription(object sender, RoutedEventArgs e)
        {
            await PremiumService.Current.CancelSubscriptionAsync();
        }
    }
}