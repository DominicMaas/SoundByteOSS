using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Services.Definitions;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowErrorMessageAsync(string title, string body)
        {
            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await new MessageDialog(body, title).ShowAsync();
                });
            }
            catch (Exception)
            {
                // Crashes if another dialog is open
            }
        }

        public async Task ShowInfoMessageAsync(string title, string body)
        {
            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await new MessageDialog(body, title).ShowAsync();
                });
            }
            catch (Exception)
            {
                // Crashes if another dialog is open
            }
        }

        public async Task ShowDialogAsync<T>(params object[] param)
        {
            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    var instance = Activator.CreateInstance(typeof(T), param);
                    await ((ContentDialog)instance).ShowAsync();
                });
            }
            catch (Exception)
            {
                // Crashes if another dialog is open
            }
        }
    }
}