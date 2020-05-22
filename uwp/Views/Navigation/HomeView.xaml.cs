using SoundByte.App.Uwp.ViewModels.Navigation;

namespace SoundByte.App.Uwp.Views.Navigation
{
    /// <summary>
    ///     Page that displays the home content for the user
    /// </summary>
    public sealed partial class HomeView
    {
        public HomeViewModel ViewModel => (HomeViewModel)DataContext;

        public HomeView() => InitializeComponent();
    }
}