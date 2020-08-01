using SoundByte.Core.Services.Definitions;
using System;
using System.Threading.Tasks;

namespace SoundByte.App.Android.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowInfoMessageAsync(string title, string body)
        {
            throw new NotImplementedException();
        }

        public Task ShowDialogAsync<T>(params object[] param)
        {
            throw new NotImplementedException();
        }

        public Task ShowDialogAsync(string name, params object[] param)
        {
            throw new NotImplementedException();
        }

        public Task ShowErrorMessageAsync(string title, string body)
        {
            throw new NotImplementedException();
        }
    }
}