using SoundByte.App.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using SoundByte.Core.Models.Extension;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    /// <summary>
    ///     This class handles bootstrapping extensions. It makes
    ///     sure extensions can only use certain permissions and saves
    ///     memory by reusing classes.
    /// </summary>
    public class ExtensionBootstrapper
    {
        #region Main APIs

        public ExtensionNavigation Navigation => _permissions.Contains("navigation") ? new ExtensionNavigation() : null;

        public ExtensionNetwork Network => _permissions.Contains("network") ? new ExtensionNetwork() : null;

        public ExtensionPlayback Playback => _permissions.Contains("playback") ? new ExtensionPlayback() : null;

        public ExtensionUtils Utils => new ExtensionUtils();

        #endregion Main APIs

        #region Properties

        public string AppVersion => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        public static double ApiVersion => 1.0;

        public static ApiPlatform Platform => ApiPlatform.Uwp;

        public string Id { get; }

        #endregion Properties

        #region Private Variables

        private readonly List<string> _permissions;

        #endregion Private Variables

        #region Generic Methods

        public async Task ShowMessageDialog(string content, string title)
        {
            await NavigationService.Current.CallMessageDialogAsync(content, title);
        }

        public async Task ShowMessageDialog(string content)
        {
            await NavigationService.Current.CallMessageDialogAsync(content);
        }

        public async Task ShowDialog(string name, object[] param)
        {
            await NavigationService.Current.CallDialogAsync(name, param);
        }

        public async Task ShowDialog(string name)
        {
            await NavigationService.Current.CallDialogAsync(name);
        }

        public void Log(object message)
        {
            Debug.WriteLine(message);
        }

        #endregion Generic Methods

        #region Constructor

        /// <summary>
        ///     Setup the extension bootstrapper with the provided permissions.
        /// </summary>
        /// <param name="permissions">A list of permissions dictating what the extension can do.</param>
        /// <param name="id">The extension id.</param>
        public ExtensionBootstrapper(List<string> permissions, Guid id)
        {
            // Handle for when the permission list is null
            if (permissions == null)
                permissions = new List<string>();

            _permissions = permissions;

            Id = id.ToString();
        }

        #endregion Constructor
    }
}