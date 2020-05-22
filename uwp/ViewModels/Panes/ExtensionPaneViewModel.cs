using System;
using SoundByte.App.Uwp.Extensions.Exceptions;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using Windows.Storage.Pickers;

namespace SoundByte.App.Uwp.ViewModels.Panes
{
    public class ExtensionPaneViewModel : BaseViewModel
    {
        public IExtensionService ExtensionService;

        public ExtensionPaneViewModel(IExtensionService extensionService)
        {
            ExtensionService = extensionService;
        }

        public async void LoadUnpackedQuestion()
        {
            try
            {
                // Open the folder picker
                var openPicker = new FolderPicker();
                openPicker.FileTypeFilter.Add(".json");

                // get the extension folder
                var extensionFolder = await openPicker.PickSingleFolderAsync();
                if (extensionFolder == null)
                    return;

                //await ExtensionService.InstallExtensionAsync(extensionFolder);
                throw new NotImplementedException("Platform binding is not yet enabled. This feature is unavailable");
            }
            catch (ManifestInvalidException mex)
            {
                await NavigationService.Current.CallMessageDialogAsync("Error: " + mex.Error.ToString(), "Error loading the extension");
            }
        }
    }
}