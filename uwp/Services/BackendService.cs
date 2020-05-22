using Microsoft.AspNetCore.SignalR.Client;
using SoundByte.Core.Items;
using System;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.Services
{
    public class BackendService
    {
        #region Public Variables

        /// <summary>
        ///     Is connected to the web server
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Connection to the server
        /// </summary>
        public HubConnection Connection { get; }

        /// <summary>
        ///     Get current instance of the background service
        /// </summary>
        public static BackendService Instance => InstanceHolder.Value;

        #endregion Public Variables

        #region Private Variables

        private static readonly Lazy<BackendService> InstanceHolder = new Lazy<BackendService>(() => new BackendService());

        #endregion Private Variables

        private BackendService()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("https://soundbytemedia.com/api/v1/live/session")
                .Build();
        }

        public async Task<bool> TryConnectAsync(int tries = 5)
        {
            for (var i = 0; i < tries; i++)
            {
                try
                {
                    await Connection.StartAsync();
                    IsConnected = true;
                    return true;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }

            IsConnected = false;
            return false;
        }

        public async Task LoginXboxConnect(string code)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (IsConnected)
                    {
                        await Connection.InvokeAsync("Connect", code);
                    }
                    else
                    {
                        await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Attempting to reconnect. Please try again.");
                        await TryConnectAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Error: " + ex.Message);
            }
        }

        public async Task LoginXboxDisconnect(string code)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (IsConnected)
                    {
                        await Connection.InvokeAsync("Disconnect", code);
                    }
                    else
                    {
                        await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Attempting to reconnect. Please try again.");
                        await TryConnectAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Error: " + ex.Message);
            }
        }

        public async Task<string> LoginSendInfoAsync(LoginToken info)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (IsConnected)
                    {
                        await Connection.InvokeAsync("SendLoginInfo", info);
                    }
                    else
                    {
                        await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Attempting to reconnect. Please try again.");
                        await TryConnectAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                await NavigationService.Current.CallMessageDialogAsync("SoundByte was not connected to soundbytemedia.com (GE-1). Error: " + ex.Message);
            }

            return string.Empty;
        }
    }
}