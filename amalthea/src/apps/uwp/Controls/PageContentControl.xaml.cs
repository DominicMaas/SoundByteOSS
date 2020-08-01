using SoundByte.Core.Models.Content;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls
{
    public sealed partial class PageContentControl : UserControl
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

        public PageContentControl() => InitializeComponent();

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var m = e.ClickedItem as Core.Models.Media.Media;
            Source.OnItemClickCommand.Execute(m);
        }

        private void OnViewMoreClick(object sender, RoutedEventArgs e)
        {
            Source.OnViewAllClickCommand.Execute();
        }
    }
}