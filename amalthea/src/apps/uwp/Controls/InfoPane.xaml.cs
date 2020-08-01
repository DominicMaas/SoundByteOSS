using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls
{
    public sealed partial class InfoPane : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(InfoPane), null);
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(InfoPane), null);
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(InfoPane), null);
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(nameof(IsError), typeof(bool), typeof(InfoPane), null);

        public InfoPane() => InitializeComponent();

        /// <summary>
        ///     The title to show on the error control
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        ///     The text to show on the error control
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);

                if (value)
                {
                    ProgressRing.Visibility = Visibility.Visible;
                    Header = "Loading";
                    Text = "Fetching new content...";
                    Visibility = Visibility.Visible;
                }
                else
                {
                    ProgressRing.Visibility = Visibility.Collapsed;

                    if (!IsError)
                        Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool IsError
        {
            get => (bool)GetValue(IsErrorProperty);
            set
            {
                SetValue(IsErrorProperty, value);

                if (value)
                {
                    Visibility = Visibility.Visible;
                    ProgressRing.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (!IsLoading)
                        Visibility = Visibility.Collapsed;
                }
            }
        }

        public void ClosePaneButtonClick()
        {
            IsError = false;
        }
    }
}