using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.Common
{
    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

        public static async void FireAndForgetSafeAsync(this Task task)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
            }
        }
    }
}