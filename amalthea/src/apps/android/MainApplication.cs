using Android.App;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Droid.Support.V7.AppCompat;
using SoundByte.Core;
using System;

namespace SoundByte.App.Android
{
    [Application]
    public class MainApplication : MvxAppCompatApplication<Setup, Core.App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Xamarin.Essentials.Platform.Init(this);

            // Start AppCenter
            AppCenter.Start(Constants.AppCenterAndroidKey, typeof(Analytics), typeof(Crashes));
        }
    }
}