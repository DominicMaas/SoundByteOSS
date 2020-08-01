using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.App.UWP.Extensions;

namespace SoundByte.App.UWP.Controls.Media.Grid
{
    public class MediaItem
    {
        public static void ShowHoverAnimation(DropShadowPanel shadowPanel, Border hoverArea)
        {
            shadowPanel.DropShadow.StartAnimation("Offset.Y",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.DropShadow.StartAnimation("BlurRadius",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 22.0f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.DropShadow.StartAnimation("Opacity",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.8f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.Offset(0, -3.0f, 250, 0).Start();
            hoverArea.Fade(0.0f, 200).Start();
        }

        public static void HideHoverAnimation(DropShadowPanel shadowPanel, Border hoverArea)
        {
            shadowPanel.DropShadow.StartAnimation("Offset.Y",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 3.0f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.DropShadow.StartAnimation("BlurRadius",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 8.0f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.DropShadow.StartAnimation("Opacity",
                shadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.6f, TimeSpan.FromMilliseconds(250), null));

            shadowPanel.Offset(0, 0, 250, 0).Start();
            hoverArea.Fade(0.3f, 200).Start();
        }
    }
}
