using Jint.Native;
using SoundByte.App.Uwp.Extensions.Definitions;
using SoundByte.App.Uwp.Services;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.Navigation;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace SoundByte.App.Uwp.Extensions.Core
{
    public class Platform
    {
        public string Host { get; } = "UWP";

        public string AppVersion => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        public double ApiVersion => 1.0;

        public object InvokeNativePlatform(string method, params object[] args)
        {
            var task = Task.Run(async () => await InvokeNativePlatformAsync(method, args));
            return task.Result;
        }

        public async Task<object> InvokeNativePlatformAsync(string method, params object[] args)
        {
            switch (method)
            {
                case "httpGetString":
                    try
                    {
                        using var client = new HttpClient();
                        using var request = await client.GetAsync((string)args[0]);

                        var str = await request.Content.ReadAsStringAsync();
                        return str;
                    }
                    catch (Exception ex)
                    {
                        return JsValue.Undefined;
                    }
                case "log":
                    await NavigationService.Current.CallMessageDialogAsync((string)args[0]);
                    break;

                case "navigateTo":
                    new ExtensionNavigation().NavigateTo(Enum.Parse<PageName>(args[0].ToString()), args[1]);
                    break;

                case "timeFromMilliseconds":
                    return new ExtensionUtils().TimeFromMilliseconds(double.Parse(args[0].ToString()));

                default:
                    throw new Exception("Method not supported: " + method);
            }

            return JsValue.Undefined;
        }
    }
}