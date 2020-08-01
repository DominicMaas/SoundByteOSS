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
    [MvxTabLayoutPresentation(ActivityHostViewModelType = typeof(RootViewModel), ViewPagerResourceId = Resource.Id.viewPager, TabLayoutResourceId = Resource.Id.tabLayout, Title = "Me")]
    [Register(nameof(MeView))]
    public class MeView : MvxFragment<MeViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.MeView, null);
            inflater.Inflate(Resource.Layout.MeView, container, true);

            var set = this.CreateBindingSet<MeView, MeViewModel>();
            set.Apply();

            return view;
        }
    }
}