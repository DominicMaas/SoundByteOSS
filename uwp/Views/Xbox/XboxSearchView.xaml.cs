using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Views.Xbox
{
    public sealed partial class XboxSearchView
    {
        public XboxSearchView() => InitializeComponent();

        private void SearchForItem(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // If the user has not typed anything, don't search
            if (string.IsNullOrEmpty(args.QueryText))
                return;

            // Navigate to search view
            App.NavigateTo(typeof(SearchView), args.QueryText);
        }
    }
}