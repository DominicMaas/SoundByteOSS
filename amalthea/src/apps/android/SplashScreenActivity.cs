using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace SoundByte.App.Android
{
    [Activity(Label = "SoundByte", MainLauncher = true, NoHistory = true, Theme = "@style/SplashTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreenActivity : MvxSplashScreenAppCompatActivity<Setup, Core.App>
    {
        public SplashScreenActivity() : base(Resource.Layout.SplashScreen)
        { }
    }
}