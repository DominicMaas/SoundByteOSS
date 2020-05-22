using SoundByte.App.Uwp.Models.Session;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.ServicesV2
{
    public interface ISessionService
    {
        bool IsConnected();

        SessionStatus GetSessionStatus();

        /// <summary>
        ///     Ask the user if they want to let the device control their media playback, if
        ///     allowed, start a signal R connection so the app will listen to commands
        ///     sent to it by other devices. Will only respond to commands sent by
        ///     authorized devices.
        /// </summary>
        Task ConnectAsync(string connectionCode, string displayName);

        Task DisconnectAsync();
    }
}