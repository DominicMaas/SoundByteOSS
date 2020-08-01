using System.Threading.Tasks;
using AppKit;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.macOS.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowErrorMessageAsync(string title, string body)
        {
            var alert = new NSAlert {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = body,
                MessageText = title,
            };
            alert.RunModal ();

            return Task.CompletedTask;
        }

        public Task ShowInfoMessageAsync(string title, string body)
        {
            var alert = new NSAlert {
                AlertStyle = NSAlertStyle.Informational,
                InformativeText = body,
                MessageText = title,
            };
            alert.RunModal ();

            return Task.CompletedTask;
        }

        public async Task ShowDialogAsync<T>(params object[] param)
        {
            throw new System.NotImplementedException();
        }
    }
}