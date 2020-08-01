using MvvmCross.Commands;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls
{
    public sealed partial class AppButton : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AppButton), null);

        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(AppButton), null);

        public static readonly DependencyProperty IsExtendedProperty =
            DependencyProperty.Register(nameof(IsExtended), typeof(bool), typeof(AppButton), null);

        public static readonly DependencyProperty ClickCommandProperty =
           DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(AppButton), null);

        public AppButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Should more text be shown
        /// </summary>
        public bool IsExtended
        {
            get => (bool)GetValue(IsExtendedProperty);
            set => SetValue(IsExtendedProperty, value);
        }

        /// <summary>
        ///     The label to show on the button
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty) as string;
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        ///     The glyph to show on the button
        /// </summary>
        public string Glyph
        {
            get => GetValue(GlyphProperty) as string;
            set => SetValue(GlyphProperty, value);
        }

        public ICommand ClickCommand
        {
            get => GetValue(ClickCommandProperty) as ICommand;
            set => SetValue(ClickCommandProperty, value);
        }
    }
}