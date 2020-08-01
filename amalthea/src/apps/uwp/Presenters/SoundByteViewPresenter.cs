using MvvmCross;
using MvvmCross.Platforms.Uap.Presenters;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using SoundByte.App.UWP.Extensions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace SoundByte.App.UWP.Presenters
{
    public class SoundByteViewPresenter : MvxWindowsViewPresenter
    {
        public SoundByteViewPresenter(IMvxWindowsFrame rootFrame) : base(rootFrame)
        { }

        public override async Task<bool> Close(IMvxViewModel viewModel)
        {
            // Only run this on host pages
            if (!(_rootFrame.Content is ISoundByteHost host))
                return await base.Close(viewModel);

            // Get the view type
            var viewsContainer = Mvx.IoCProvider.Resolve<IMvxViewsContainer>();
            var viewType = viewsContainer.GetViewType(viewModel.GetType());

            // Modal frame
            if (viewType.HasModalAttribute())
            {
                host.CloseModal();
                return true;
            }

            // Standard frame
            await host.NavigateBackAsync();
            return true;
        }

        public override Task<bool> Show(MvxViewModelRequest request)
        {
            if (_rootFrame.Content is ISoundByteHost host)
            {
                var viewsContainer = Mvx.IoCProvider.Resolve<IMvxViewsContainer>();

                var requestText = GetRequestText(request);
                var viewType = viewsContainer.GetViewType(request.ViewModelType);

                // Determine the base page type
                if (viewType.HasModalAttribute())
                {
                    // Get the tag if a tab (ensure the correct tab on the top of the page is selected)
                    var title = "SoundByte";
                    if (viewType.HasModalAttribute())
                        title = viewType.GetModalAttribute().Title;

                    host.ShowModal(viewType, requestText, title);
                }
                else
                {
                    // Get the tag if a tab (ensure the correct tab on the top of the page is selected)
                    string tag = null;
                    if (viewType.HasTabAttribute())
                        tag = viewType.GetTabAttribute().Tag;

                    host.ShowPage(viewType, requestText, tag, new EntranceNavigationTransitionInfo());
                }

                return Task.FromResult(true);
            }

            base.Show(request);
            return Task.FromResult(true);
        }
    }
}