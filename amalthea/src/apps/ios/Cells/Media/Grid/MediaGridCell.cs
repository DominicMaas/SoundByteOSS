using CoreAnimation;
using CoreGraphics;
using FFImageLoading.Cross;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.Core.Helpers;
using System;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Cells.Media.Grid
{
    /// <summary>
    ///     A base media grid cell that other grid cells can extend from
    /// </summary>
    public class MediaGridCell : MvxCollectionViewCell
    {
        protected MvxCachedImageView _imageControl;
        protected UILabel _title;
        protected UILabel _subtitle;
        protected UILabel _rightFooter;
        protected UILabel _leftFooter;
        protected UIAlertController _alertController;

        private CAGradientLayer _gradientLayer;

        private readonly float _imageCornerRadius;

        public MediaGridCell(IntPtr handle, float imageCornerRadius) : base(string.Empty, handle)
        {
            _imageCornerRadius = imageCornerRadius;
            InitializeUi();
        }

        private void InitializeUi()
        {
            // Holds all the views
            var viewHolder = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            Add(viewHolder);

            // Track Thumbnail
            _imageControl = new MvxCachedImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            _imageControl.Layer.MasksToBounds = true;
            _imageControl.Layer.CornerRadius = _imageCornerRadius;

            var shadowView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            shadowView.Layer.ShadowOpacity = 0.6f;
            shadowView.Layer.ShadowColor = UIColor.Black.CGColor;
            shadowView.Layer.ShadowRadius = 4.0f;
            shadowView.Layer.ShadowOffset = new CGSize(0, 3);

            shadowView.AddSubview(_imageControl);
            viewHolder.Add(shadowView);

            shadowView.LeftAnchor.ConstraintEqualTo(viewHolder.LeftAnchor, 4).Active = true;
            shadowView.TopAnchor.ConstraintEqualTo(viewHolder.TopAnchor, 4).Active = true;
            shadowView.HeightAnchor.ConstraintEqualTo(160).Active = true;
            shadowView.WidthAnchor.ConstraintEqualTo(160).Active = true;

            _imageControl.HeightAnchor.ConstraintEqualTo(160).Active = true;
            _imageControl.WidthAnchor.ConstraintEqualTo(160).Active = true;
            _imageControl.LeftAnchor.ConstraintEqualTo(shadowView.LeftAnchor).Active = true;
            _imageControl.TopAnchor.ConstraintEqualTo(shadowView.TopAnchor).Active = true;

            // Right Footer
            _rightFooter = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(12f, UIFontWeight.Semibold)
            };

            _imageControl.Add(_rightFooter);

            // Position the right footer
            _rightFooter.RightAnchor.ConstraintEqualTo(_imageControl.RightAnchor, -8).Active = true;
            _rightFooter.BottomAnchor.ConstraintEqualTo(_imageControl.BottomAnchor, -6).Active = true;

            // Left Footer
            _leftFooter = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.FromName("FontAwesome5Pro-Light", 12)
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
                Font = UIFont.SystemFontOfSize(14f, UIFontWeight.Semibold),
            };

            viewHolder.Add(_title);

            _title.LeftAnchor.ConstraintEqualTo(viewHolder.LeftAnchor, 6).Active = true;
            _title.TopAnchor.ConstraintEqualTo(viewHolder.TopAnchor, 174).Active = true;
            _title.WidthAnchor.ConstraintEqualTo(160).Active = true;

            // Subtitle
            _subtitle = new UILabel
            {
                TextColor = ColorHelper.Text0.ToPlatformColor(),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0.8f,
                Font = UIFont.SystemFontOfSize(13f, UIFontWeight.Medium)
            };

            viewHolder.Add(_subtitle);

            _subtitle.LeftAnchor.ConstraintEqualTo(viewHolder.LeftAnchor, 6).Active = true;
            _subtitle.TopAnchor.ConstraintEqualTo(viewHolder.TopAnchor, 194).Active = true;
            _subtitle.WidthAnchor.ConstraintEqualTo(160).Active = true;

            // The controller used for extra info
            _alertController = UIAlertController.Create("", "", UIAlertControllerStyle.ActionSheet);

            // Add the default close
            _alertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Cancel, _ => _alertController.DismissViewController(true, null)));

            var gr = new UILongPressGestureRecognizer();
            gr.AddTarget(() => LongPressed(gr));
            AddGestureRecognizer(gr);
        }

        public override bool Highlighted
        {
            get => _highlighted;
            set
            {
                _highlighted = value;

                UIView.Animate(0.100f, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    Alpha = _highlighted ? 0.4f : 1.0f;
                    Layer.Transform = _highlighted ? CoreAnimation.CATransform3D.MakeScale(0.96f, 0.96f, 1.0f) : CoreAnimation.CATransform3D.Identity;
                }, () => { });

                if (_highlighted)
                {
                    var g = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
                    g.ImpactOccurred();
                }
            }
        }

        private bool _highlighted;

        private void LongPressed(UILongPressGestureRecognizer longPressGestureRecognizer)
        {
            if (longPressGestureRecognizer.State != UIGestureRecognizerState.Began) return;

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(_alertController, true, null);
        }

        protected void CreateGradientLayer()
        {
            // Gradient overlay for duration
            _gradientLayer = new CAGradientLayer
            {
                Frame = new CGRect(0, 0, 160, 160),
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