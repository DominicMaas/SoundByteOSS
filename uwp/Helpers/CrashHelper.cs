using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    ///     Handle app crashes and report the exception without closing
    ///     the app.
    /// </summary>
    public static class CrashHelper
    {
        public static void HandleAppCrashes(Application currentApplication)
        {
            // Log when the app crashes
            CoreApplication.UnhandledErrorDetected += async (sender, args) =>
            {
                try
                {
                    args.UnhandledError.Propagate();
                }
                catch (Exception e)
                {
                    await HandleAppCrashAsync(e);
                }
            };

            currentApplication.UnhandledException += async (sender, args) =>
            {
                args.Handled = true;
                await HandleAppCrashAsync(args.Exception);
            };

            TaskScheduler.UnobservedTaskException += async (sender, args) =>
            {
                args.SetObserved();
                await HandleAppCrashAsync(args.Exception);
            };
        }

        private static Task HandleAppCrashAsync(Exception ex)
        {
            try
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);

                if (!DeviceHelper.IsBackground)
                {
                    App.NotificationManager?.Show("An error occurred: " + ex.Message, 5000);
                }
            }
            catch
            {
                // Do nothing
            }

            return Task.CompletedTask;
        }
    }
}