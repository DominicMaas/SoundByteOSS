using CoreGraphics;
using FFImageLoading.Cross;
using Foundation;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Controls
{
    public class NowPlayingBar : MvxView
    {
        public const int Height = 64;

        public NowPlayingBar()
        {
            CreateView();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            if (touches.AnyObject is UITouch touch)
            {
                var navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                navigationService.Navigate<PlaybackViewModel>();
            }
        }

        private void CreateView()
        {
            // Root View with the shadow
            var rootView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = ColorHelper.Background4DP.ToPlatformColor(), UserInteractionEnabled = true };
            rootView.Layer.ShadowOpacity = 0.6f;
            rootView.Layer.ShadowColor = UIColor.Black.CGColor;
            rootView.Layer.ShadowRadius = 4.0f;
            rootView.Layer.ShadowOffset = new CGSize(0, 2);
            rootView.Layer.CornerRadius = 6.0f;

            // Thumbnail
            var imageControl = new MvxCachedImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            imageControl.Layer.MasksToBounds = true;
            imageControl.Layer.CornerRadius = 6.0f;

            rootView.AddSubview(imageControl);

            imageControl.HeightAnchor.ConstraintEqualTo(Height - 8).Active = true;
            imageControl.WidthAnchor.ConstraintEqualTo(Height - 8).Active = true;
            imageControl.LeftAnchor.ConstraintEqualTo(rootView.LeftAnchor, 4).Active = true;
            imageControl.TopAnchor.ConstraintEqualTo(rootView.TopAnchor, 4).Active = true;

            // Title
            var title = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorHelper.Text0.ToPlatformColor(),
                Font = UIFont.SystemFontOfSize(14f, UIFontWeight.Bold),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 1
            };

            // Subtitle
            var subTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorHelper.Text0.ToPlatformColor(),
                Alpha = 0.7f,
                Font = UIFont.SystemFontOfSize(13f, UIFontWeight.Regular),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 1
            };

            rootView.AddSubview(title);
            rootView.AddSubview(subTitle);

            // Constraints
            title.TopAnchor.ConstraintEqualTo(rootView.TopAnchor, 14).Active = true;
            title.LeftAnchor.ConstraintEqualTo(imageControl.RightAnchor, 10).Active = true;
            title.RightAnchor.ConstraintEqualTo(rootView.RightAnchor, 0);

            // Ensure the label is fully set
            title.PreferredMaxLayoutWidth = title.Frame.Size.Width;
            title.SizeToFit();

            subTitle.TopAnchor.ConstraintEqualTo(title.BottomAnchor, 3).Active = true;
            subTitle.LeftAnchor.ConstraintEqualTo(imageControl.RightAnchor, 10).Active = true;
            subTitle.RightAnchor.ConstraintEqualTo(rootView.RightAnchor, 0);

            // Ensure the label is fully set
            subTitle.PreferredMaxLayoutWidth = subTitle.Frame.Size.Width;
            subTitle.SizeToFit();

            // Add root
            Add(rootView);

            // Root view constraints
            rootView.TranslatesAutoresizingMaskIntoConstraints = false;
            rootView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            rootView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            rootView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            rootView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<NowPlayingBar, PlaybackViewModel>();
                set.Bind(title).For(x => x.Text).To(vm => vm.CurrentTrack.Title);
                set.Bind(subTitle).For(x => x.Text).To(vm => vm.CurrentTrack.User.Username);
                set.Bind(imageControl).For(x => x.ImagePath).To(vm => vm.CurrentTrack.ArtworkUrl);
                set.Apply();
            });
        }
    }
}