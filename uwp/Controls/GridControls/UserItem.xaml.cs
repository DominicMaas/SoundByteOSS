using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using SoundByte.Core.Items.User;
using System;
using UICompositionAnimations.Composition;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.App.Uwp.Controls.GridControls
{
    public sealed partial class UserItem
    {
        /// <summary>
        /// Identifies the Track dependency property.
        /// </summary>
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register(nameof(User), typeof(BaseUser), typeof(UserItem), null);

        /// <summary>
        /// Gets or sets the user for this object
        /// </summary>
        public BaseUser User
        {
            get => (BaseUser)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public UserItem()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var gridViewItemElement = this.FindAscendant<GridViewItem>();
            if (gridViewItemElement != null)
            {
                gridViewItemElement.GotFocus += GridViewItemElement_GotFocus;
                gridViewItemElement.LostFocus += GridViewItemElement_LostFocus;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var gridViewItemElement = this.FindAscendant<GridViewItem>();
            if (gridViewItemElement != null)
            {
                gridViewItemElement.GotFocus -= GridViewItemElement_GotFocus;
                gridViewItemElement.LostFocus -= GridViewItemElement_LostFocus;
            }
        }

        private void GridViewItemElement_LostFocus(object sender, RoutedEventArgs e)
        {
            HideHoverAnimation(sender, null);
        }

        private void GridViewItemElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowHoverAnimation(sender, null);
        }

        private void ShowHoverAnimation(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 22.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
               ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.8f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.Offset(0, -3.0f, 250, 0).Start();
            HoverArea.Fade(0.0f, 200).Start();
        }

        private void HideHoverAnimation(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 2.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 8.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
              ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.2f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.Offset(0, 0, 250, 0).Start();
            HoverArea.Fade(0.3f, 200).Start();
        }

        private async void OpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(User.Link));
        }
    }
}