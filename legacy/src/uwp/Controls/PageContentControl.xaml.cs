using SoundByte.App.Uwp.Models;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls
{
    public sealed partial class PageContentControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ContentGroup), typeof(PageContentControl), null);

        /// <summary>
        ///     The label to show on the button
        /// </summary>
        public ContentGroup Source
        {
            get => GetValue(SourceProperty) as ContentGroup;
            set => SetValue(SourceProperty, value);
        }

        public PageContentControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     On App Button click, this is a little cheat-y, but we grab the
        ///     data context and execute the command.
        /// </summary>
        private void AppButton_OnClick(object sender, RoutedEventArgs e)
        {
            var datacontext = (sender as FrameworkElement)?.DataContext as ContentButton;
            datacontext?.ButtonClickCommand.Execute(Source);
        }

        private void ViewAllButtonClick(object sender, RoutedEventArgs e)
        {
            Source.OnViewAllClickCommand.Execute(Source);
        }
    }
}