using CoreAnimation;
using CoreGraphics;
using FFImageLoading.Cross;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.Core.Helpers;
using System;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Cells.Media.List
{
    /// <summary>
    ///     A base media grid cell that other grid cells can extend from
    /// </summary>
    public class MediaListCell : MvxTableViewCell
    {
        public static float CellHeight = 94.0f;

        protected MvxCachedImageView _imageControl;

        protected UILabel _title;
        protected UILabel _subtitle;
        protected UILabel _rightFooter;
        protected UILabel _leftFooter;

        private CAGradientLayer _gradientLayer;

        private readonly float _imageCornerRadius;

        public MediaListCell(IntPtr handle, float imageCornerRadius) : base(string.Empty, handle)
        {
            _imageCornerRadius = imageCornerRadius;
            InitializeUi();
        }

        private void InitializeUi()
        {
            // Root View with the shadow
            var rootView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            Add(rootView);

            // Root view constraints
            rootView.HeightAnchor.ConstraintEqualTo(CellHeight).Active = true;
            rootView.LeftAnchor.ConstraintEqualTo(LeftAnchor, 18).Active = true;
            rootView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            rootView.RightAnchor.ConstraintEqualTo(RightAnchor, -18).Active = true;

            // Thumbnail
            _imageControl = new MvxCachedImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            _imageControl.Layer.MasksToBounds = true;
            _imageControl.Layer.CornerRadius = 6.0f;

            var shadowView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            shadowView.Layer.ShadowOpacity = 0.6f;
            shadowView.Layer.ShadowColor = UIColor.Black.CGColor;
            shadowView.Layer.ShadowRadius = 4.0f;
            shadowView.Layer.ShadowOffset = new CGSize(0, 3);

            shadowView.AddSubview(_imageControl);
            rootView.Add(shadowView);

            shadowView.HeightAnchor.ConstraintEqualTo(CellHeight - 18).Active = true;
            shadowView.WidthAnchor.ConstraintEqualTo(CellHeight - 18).Active = true;
            shadowView.LeftAnchor.ConstraintEqualTo(rootView.LeftAnchor).Active = true;
            shadowView.TopAnchor.ConstraintEqualTo(rootView.TopAnchor, 18).Active = true;

            // Make sure the image is correctly sized
            _imageControl.HeightAnchor.ConstraintEqualTo(shadowView.HeightAnchor).Active = true;
            _imageControl.WidthAnchor.ConstraintEqualTo(shadowView.WidthAnchor).Active = true;
            _imageControl.LeftAnchor.ConstraintEqualTo(shadowView.LeftAnchor).Active = true;
            _imageControl.TopAnchor.ConstraintEqualTo(shadowView.TopAnchor).Active = true;

            // Right Footer
            _rightFooter = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(11, UIFontWeight.Semibold)
            };

            _imageControl.Add(_rightFooter);

            // Position the right footer
            _rightFooter.RightAnchor.ConstraintEqualTo(_imageControl.RightAnchor, -6).Active = true;
            _rightFooter.BottomAnchor.ConstraintEqualTo(_imageControl.BottomAnchor, -4).Active = true;

            // Left Footer
            _leftFooter = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.FromName("FontAwesome5Pro-Light", 11)
            };

            _imageControl.Add(_leftFooter);

            // Position the left footer
            _leftFooter.LeftAnchor.ConstraintEqualTo(_imageControl.LeftAnchor, 8).Active = true;
            _leftFooter.BottomAnchor.ConstraintEqualTo(_imageControl.BottomAnchor, -6).Active = true;

            // Title
            _title = new UILabel
            {
                TextColor = ColorHelper.Text0.ToPlatformColor(),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(16f, UIFontWeight.Bold),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 1,
            };

            rootView.Add(_title);

            _title.LeftAnchor.ConstraintEqualTo(shadowView.RightAnchor, 12).Active = true;
            _title.RightAnchor.ConstraintEqualTo(rootView.RightAnchor).Active = true;
            _title.TopAnchor.ConstraintEqualTo(rootView.TopAnchor, 34).Active = true;

            // Ensure the label is fully set
            _title.PreferredMaxLayoutWidth = _title.Frame.Size.Width;
            _title.SizeToFit();

            // Subtitle
            _subtitle = new UILabel
            {
                TextColor = ColorHelper.Text0.ToPlatformColor(),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0.7f,
                Font = UIFont.SystemFontOfSize(15f, UIFontWeight.Regular),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 1,
            };

            rootView.Add(_subtitle);

            _subtitle.LeftAnchor.ConstraintEqualTo(shadowView.RightAnchor, 12).Active = true;
            _subtitle.RightAnchor.ConstraintEqualTo(rootView.RightAnchor).Active = true;
            _subtitle.TopAnchor.ConstraintEqualTo(_title.BottomAnchor, 4).Active = true;

            // Ensure the label is fully set
            _subtitle.PreferredMaxLayoutWidth = _subtitle.Frame.Size.Width;
            _subtitle.SizeToFit();
        }

        protected void CreateGradientLayer()
        {
            // Gradient overlay for duration
            _gradientLayer = new CAGradientLayer
            {
                Frame = new CGRect(0, 0, CellHeight - 18, CellHeight - 18),
                Locations = new[]
                {
                    new NSNumber(0.0f),
                    new NSNumber(1.0f)
                },
                Colors = new[] { UIColor.Clear.CGColor, CGColor.CreateSrgb(0, 0, 0, 0.6f) }
            };

            // Add the gradient view and bring it to the front over the image
            _imageControl.Layer.InsertSublayer(_gradientLayer, 0);
            _imageControl.BringSubviewToFront(_rightFooter);
            _imageControl.BringSubviewToFront(_leftFooter);
        }
    }
}