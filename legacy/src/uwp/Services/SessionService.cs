using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.RemoteSystems;
using Windows.UI.Popups;

namespace SoundByte.App.Uwp.Services
{
    public class SessionService
    {
        #region Class Setup

        public static SessionService Current => InstanceHolder.Value;
        private static readonly Lazy<SessionService> InstanceHolder = new Lazy<SessionService>(() => new SessionService());

        #endregion Class Setup

        #region Private Variables

        private RemoteSystemSessionController _manager;
        private RemoteSystemSession _currentSession;

        #endregion Private Variables

        /// <summary>
        ///     Start a session for this device/app. This will always run in the background,
        ///     allowing connecting from other devices, later if needed.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            // Check if we are aloud access
            var accessStatus = await RemoteSystem.RequestAccessAsync();
            if (accessStatus != RemoteSystemAccessStatus.Allowed)
                return;

            // Get device info
            var deviceInfo = new EasClientDeviceInformation();

            // Create new session of name "SoundByte on Dominic-PC" for example.
            _manager = new RemoteSystemSessionController($"SoundByte on {deviceInfo.FriendlyName}");

            // Handle joining
            _manager.JoinRequested += async (sender, args) =>
            {
                // Get the deferral
                var deferral = args.GetDeferral();

                // Run on UI thread
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    var dialog = new MessageDialog($"A device ({args.JoinRequest.Participant.RemoteSystem.DisplayName}) would like to connect to SoundByte and control features such as the current playing track. Do you want to accept this connection?", "SoundByte Connect");
                    dialog.Commands.Add(new UICommand("Accept", null, 0));
                    dialog.Commands.Add(new UICommand("Cancel", null, 1));

                    dialog.CancelCommandIndex = 1;
                    dialog.DefaultCommandIndex = 1;

                    var result = await dialog.ShowAsync();
                    if ((int)result.Id == 0)
                    {
                        args.JoinRequest.Accept();
                    }
                });

                deferral.Complete();
            };

            // Create and start the session
            var creationResult = await _manager.CreateSessionAsync();

            // Check if success, not too worried about failure
            if (creationResult.Status == RemoteSystemSessionCreationStatus.Success)
            {
                _currentSession = creationResult.Session;
                _currentSession.Disconnected += async (sender, args) =>
                {
                    await NavigationService.Current.CallMessageDialogAsync("Device disconnected: " + args.Reason, "SoundByte Connect");
                };
            }
            else
            {
                await NavigationService.Current.CallMessageDialogAsync("Failed to create session: " + creationResult.Status, "SoundByte Connect");
            }
        }
    }
}