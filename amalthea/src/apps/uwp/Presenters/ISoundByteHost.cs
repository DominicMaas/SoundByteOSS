using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace SoundByte.App.UWP.Presenters
{
    public interface ISoundByteHost
    {
        void ShowModal(Type pageType, object parameter, string title);

        void CloseModal();

        void ShowPage(Type pageType, object parameter, string tag, NavigationTransitionInfo navigationTransitionInfo);

        Task NavigateBackAsync();
    }
}