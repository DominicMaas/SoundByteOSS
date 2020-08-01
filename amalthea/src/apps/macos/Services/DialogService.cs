using System.Threading.Tasks;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.macOS.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowErrorMessageAsync(string title, string body)
        {
            throw new System.NotImplementedException();
        }

        public async Task ShowInfoMessageAsync(string title, string body)
        {
            throw new System.NotImplementedException();
        }

        public async Task ShowDialogAsync<T>(params object[] param)
        {
            throw new System.NotImplementedException();
        }
    }
}