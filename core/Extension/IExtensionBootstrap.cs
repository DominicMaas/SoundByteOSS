using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Models.Extension;
using System.Threading.Tasks;

namespace SoundByte.Core.Extension
{
    public interface IExtensionBootstrap
    {
        #region Main APIs

        [ApiNamespaceMetadata(ApiVersion.V1, "Manage SoundByte content.")]
        IExtensionContent Content { get; }

        [ApiNamespaceMetadata(ApiVersion.V1, "Navigate across the app.")]
        IExtensionNavigation Navigation { get; }

        [ApiNamespaceMetadata(ApiVersion.V1, "Networking APIs.")]
        IExtensionNetwork Network { get; }

        [ApiNamespaceMetadata(ApiVersion.V1, "Manage playing tracks.")]
        IExtensionPlayback Playback { get; }

        [ApiNamespaceMetadata(ApiVersion.V1, "Manage extension settings.")]
        IExtensionSettings Settings { get; }

        IExtensionUtils Utils { get; }

        #endregion Main APIs

        #region Properties

        [ApiMetadata(ApiVersion.V1, "The version of SoundByte that this extension is running on.")]
        string AppVersion { get; }

        [ApiMetadata(ApiVersion.V1, "The API version that this SoundByte client supports, use this to determine which API features you can use.")]
        ApiVersion ApiVersion { get; }

        [ApiMetadata(ApiVersion.V1, "The platform this extension is running on, you can use this to customize the experience between different platforms. Either Platform_UWP or Platform_Android.")]
        ApiPlatform Platform { get; }

        [ApiMetadata(ApiVersion.V1, "The id of the extension that was loaded. This will return the id specified in the manifest, or a randomly generated id if none is specified.")]
        string Id { get; }

        #endregion Properties

        #region Generic Methods

        [ApiMetadata(ApiVersion.V1, "Show a message to the user that grabs their attention.")]
        [ApiParameterMetadata("content", "The message to display.")]
        [ApiParameterMetadata("title", "The title of the message.")]
        Task ShowMessageDialog(string content, string title);

        [ApiMetadata(ApiVersion.V1, "Show a message to the user that grabs their attention.")]
        [ApiParameterMetadata("content", "The message to display.")]
        Task ShowMessageDialog(string content);

        [ApiMetadata(ApiVersion.V1, "Shows a built in SoundByte dialog. Some of these dialogs are platform dependent, make sure you read the related documentation.")]
        [ApiParameterMetadata("name", "The name of the dialog to show.")]
        [ApiParameterMetadata("params", "A list of parameters that this dialog requires.")]
        Task ShowDialog(string name, object[] param);

        [ApiMetadata(ApiVersion.V1, "Shows a built in SoundByte dialog. Some of these dialogs are platform dependent, make sure you read the related documentation.")]
        [ApiParameterMetadata("name", "The name of the dialog to show.")]
        Task ShowDialog(string name);

        [ApiMetadata(ApiVersion.V1, "Logs a message to the console. Currently this is only seen when debugging SoundByte in Visual Studio, so it's not useful for general extension development.")]
        [ApiParameterMetadata("message", "The message to log.")]
        void Log(object message);

        #endregion Generic Methods
    }
}