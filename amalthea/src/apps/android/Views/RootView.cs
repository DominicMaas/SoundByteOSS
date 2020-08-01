using Android.App;
using Android.OS;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.Android.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class RootView : MvxAppCompatActivity<RootViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.RootView);

            var set = this.CreateBindingSet<RootView, RootViewModel>();
            set.Apply();

            if (bundle == null)
            {
                //ViewModel.Show.Execute();
            }
        }
    }
}