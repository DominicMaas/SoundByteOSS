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
    [MvxTabLayoutPresentation(ActivityHostViewModelType = typeof(RootViewModel), ViewPagerResourceId = Resource.Id.viewPager, TabLayoutResourceId = Resource.Id.tabLayout, Title = "Podcasts")]
    [Register(nameof(PodcastsView))]
    public class PodcastsView : MvxFragment<PodcastsViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.PodcastsView, null);
            inflater.Inflate(Resource.Layout.PodcastsView, container, true);

            var set = this.CreateBindingSet<PodcastsView, PodcastsViewModel>();
            set.Apply();

            return view;
        }
    }
}