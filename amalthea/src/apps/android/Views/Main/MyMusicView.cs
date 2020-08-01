using Android.OS;
using Android.Runtime;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SoundByte.Core.ViewModels;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.Android.Views.Main
{
    [MvxTabLayoutPresentation(ActivityHostViewModelType = typeof(RootViewModel), ViewPagerResourceId = Resource.Id.viewPager, TabLayoutResourceId = Resource.Id.tabLayout, Title = "My Music")]
    [Register(nameof(MyMusicView))]
    public class MyMusicView : MvxFragment<MyMusicViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.MyMusicView, null);
            inflater.Inflate(Resource.Layout.MyMusicView, container, true);

            var set = this.CreateBindingSet<MyMusicView, MyMusicViewModel>();
            set.Apply();

            return view;
        }
    }
}