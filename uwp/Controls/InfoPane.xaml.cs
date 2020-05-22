using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls
{
    /// <summary>
    ///     This control is used to show friendly messages
    ///     within the app
    /// </summary>
    public sealed partial class InfoPane
    {
        #region Page Setup

        /// <summary>
        ///     Load the XAML part of the user control
        /// </summary>
        public InfoPane()
        {
            InitializeComponent();
        }

        #endregion Page Setup

        #region Binding Variables

        private static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InfoPane), null);

        private static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(InfoPane), null);

        private static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(InfoPane), null);

        private static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(InfoPane), null);

        #endregion Binding Variables

        #region Getters and Setters

        /// <summary>
        ///     The title to show on the error control
        /// </summary>
        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            set
            {
                SetValue(HeaderProperty, value);
                HeaderTextBlock.Text = value;
            }
        }

        /// <summary>
        ///     The text to show on the error control
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty) as string;
            set
            {
                SetValue(TextProperty, value);
                TextTextBlock.Text = value;
            }
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

        #endregion Getters and Setters

        #region Methods

        public void ClosePaneButtonClick()
        {
            IsError = false;
        }

        #endregion Methods
    }
}