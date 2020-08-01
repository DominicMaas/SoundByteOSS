using SoundByte.Core.Services.Definitions;
using System.Threading.Tasks;
using UIKit;

namespace SoundByte.App.iOS.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowErrorMessageAsync(string title, string body)
        {
            new UIAlertView(title, body, null, "OK", null).Show();
            return Task.CompletedTask;
        }

        public Task ShowInfoMessageAsync(string title, string body)
        {
            new UIAlertView(title, body, null, "OK", null).Show();
            return Task.CompletedTask;
        }

        public Task ShowDialogAsync<T>(params object[] param)
        {
            throw new System.NotImplementedException();
        }
    }
}