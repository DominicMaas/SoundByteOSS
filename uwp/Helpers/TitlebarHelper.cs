using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    ///     Helper methods for adjusting application title bar / status bar styles
    /// </summary>
    public static class TitlebarHelper
    {
        /// <summary>
        ///     Set the initial style of the status bar / title bar
        /// </summary>
        public static void SetInitialStyle()
        {
            Color textColor;

            if (App.Shell == null)
            {
                textColor = App.Current.RequestedTheme == ApplicationTheme.Dark
                    ? Colors.White
                    : Colors.Black;
            }
            else
            {
                textColor = (App.Shell as Page).ActualTheme == ElementTheme.Dark
                    ? Colors.White
                    : Colors.Black;
            }

            if (DeviceHelper.IsDesktop)
            {
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 20 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 60 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;
            }
        }

        /// <summary>
        ///     Refreshes the stored application accent color
        /// </summary>
        public static void UpdateTitlebarStyle()
        {
            Color textColor;

            if (App.Shell == null)
            {
                textColor = App.Current.RequestedTheme == ApplicationTheme.Dark
                    ? Colors.White
                    : Colors.Black;
            }
            else
            {
                textColor = (App.Shell as Page).ActualTheme == ElementTheme.Dark
                    ? Colors.White
                    : Colors.Black;
            }

            if (DeviceHelper.IsDesktop)
            {
                if ((App.Shell?.GetName("AppTitle") as TextBlock) != null)
                    (App.Shell?.GetName("AppTitle") as TextBlock).Foreground = new SolidColorBrush(textColor);

                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

                // Set custom titlebar
                if ((App.Shell?.GetName("Titlebar") as Border) != null)
                {
                    var appTitlebar = App.Shell?.GetName("Titlebar") as Border;
                    Window.Current.SetTitleBar(appTitlebar);
                }

                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 20 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 60 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;
            }
        }
    }
}