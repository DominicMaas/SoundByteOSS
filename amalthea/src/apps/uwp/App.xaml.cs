using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Platforms.Uap.Views;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Constants = SoundByte.Core.Constants;

namespace SoundByte.App.UWP
{
    public sealed partial class App
    {
        public App() => InitializeComponent();
    }

    public abstract class SoundByteApp : MvxApplication<Setup, Core.App>
    {
        protected override void OnLaunched(LaunchActivatedEventArgs activationArgs)
        {
            // Use Reveal Focus on 1803+
            if (ApiInformation.IsEnumNamedValuePresent(nameof(Windows.UI.Xaml.FocusVisualKind), nameof(FocusVisualKind.Reveal)))
                FocusVisualKind = FocusVisualKind.Reveal;

            // Start AppCenter
            AppCenter.Start(Constants.AppCenterUwpKey, typeof(Analytics), typeof(Crashes));

            base.OnLaunched(activationArgs);
        }
    }
}