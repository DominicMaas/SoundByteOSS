using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SoundByte.App.Uwp.Models.Session;
using Windows.UI.Popups;

namespace SoundByte.App.Uwp.ServicesV2.Implementations
{
    public class SessionService : ISessionService
    {
        private SessionStatus _sessionStatus;

        private string _connectionCode;

        private HubConnection _connection;

        public SessionService()
        {
            // Build the connection
            _connection = new HubConnectionBuilder()
                .WithUrl("https://soundbytemedia.com/api/v1/live/session")
                .Build();
        }

        public async Task ConnectAsync(string connectionCode, string displayName)
        {
            // Cannot run this code while connecting
            if (_sessionStatus == SessionStatus.Connecting)
                return;

            // Create the dialog asking if the user wants to start a connection
            var dialog = new MessageDialog($"A device ({displayName}) would like to connect to SoundByte and control features such as the current playing track. Do you want to accept this connection?", "SoundByte Connect");
            dialog.Commands.Add(new UICommand("Accept", null, 0));
            dialog.Commands.Add(new UICommand("Cancel", null, 1));

            // By default, cancel
            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 1;

            // Check to see if the user accepts this connection
            var result = await dialog.ShowAsync();
            if ((int)result.Id != 0)
                return;

            // We have started connecting
            _sessionStatus = SessionStatus.Connecting;

            // Keep track of the connection code
            _connectionCode = connectionCode;

            // Attempt to connect to signalR (5 tries)
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    // Attempt to start the connection
                    await _connection.StartAsync();

                    // This method will assign the connection code to the client server side
                    // and then update the connection status (allowing the listening phone to know
                    // the client is running).
                    await _connection.InvokeAsync("ClientStarted", connectionCode);

                    // This point marks where the client is connected (but may not be listening yet, we have to wait
                    // for another event (phone lets the client know that it's alive and requests current playback status.)
                    _sessionStatus = SessionStatus.Connected;
                }
                catch
                {
                    // Wait another 500 ms
                    await Task.Delay(500);
                }
            }

            // Could not connect
            if (_sessionStatus == SessionStatus.Connecting)
                _sessionStatus = SessionStatus.Disconnected;

            // Let the user know
            await new MessageDialog("Could not connect to server. Please try again later", "SoundByte Connect").ShowAsync();
        }

        public async Task DisconnectAsync()
        {
            await _connection.StopAsync();
            _sessionStatus = SessionStatus.Disconnected;
        }

        public SessionStatus GetSessionStatus() => _sessionStatus;

        public bool IsConnected() => _sessionStatus == SessionStatus.Connected;
    }
}