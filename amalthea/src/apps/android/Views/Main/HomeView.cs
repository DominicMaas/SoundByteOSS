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
    [MvxTabLayoutPresentation(ActivityHostViewModelType = typeof(RootViewModel), ViewPagerResourceId = Resource.Id.viewPager, TabLayoutResourceId = Resource.Id.tabLayout, Title = "Home")]
    [Register(nameof(HomeView))]
    public class HomeView : MvxFragment<HomeViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.HomeView, null);
            inflater.Inflate(Resource.Layout.HomeView, container, true);

            var set = this.CreateBindingSet<HomeView, HomeViewModel>();
            set.Apply();

            return view;
        }
    }
}