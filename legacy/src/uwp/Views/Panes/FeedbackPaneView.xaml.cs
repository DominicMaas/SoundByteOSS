using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Views.Shell;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Views.Panes
{
    /// <summary>
    ///     Feedback page
    /// </summary>
    public sealed partial class FeedbackPaneView : Page
    {
        public FeedbackPaneView() => InitializeComponent();

        private void SendResponse(object sender, RoutedEventArgs e)
        {
            var improveText = ImproveText.Text;
            var isSame = SameBox.IsChecked;
            var isWorse = WorseBox.IsChecked;

            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Send Feedback", new Dictionary<string, string>
            {
                { "Is Same", isSame.ToString() },
                { "Is Worse", isWorse.ToString() },
                { "Improvements", improveText },
            });

            (App.Shell as DesktopShell)?.CloseSidePane();
        }
    }
}