using System;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using CoreText;
using FFImageLoading.Cross;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.ViewModels;
using UIKit;

namespace SoundByte.App.iOS.Views
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.Popover, WrapInNavigationController = false)]
    public class PlaybackView : MvxViewController<PlaybackViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // ---------- BACKGROUND IMAGE --------- //
            var backgroundImage = new MvxCachedImageView { ContentMode = UIViewContentMode.ScaleAspectFill };
            Add(backgroundImage);

            // Blur layer
            var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemThickMaterialDark);
            var blurredEffectView = new UIVisualEffectView(blurEffect);

            // Add the blur infront of the image
            Add(blurredEffectView);

            // ---------- TRACK IMAGE --------- //
            var trackImage = new MvxCachedImageView { ContentMode = UIViewContentMode.ScaleAspectFill, TranslatesAutoresizingMaskIntoConstraints = false };
            trackImage.Layer.MasksToBounds = true;
            trackImage.Layer.CornerRadius = 6.0f;

            var shadowView = new UIView();
            shadowView.Layer.ShadowOpacity = 0.4f;
            shadowView.Layer.ShadowColor = UIColor.Black.CGColor;
            shadowView.Layer.ShadowRadius = 26.0f;
            shadowView.Layer.ShadowOffset = new CGSize(0, 18);

            shadowView.AddSubview(trackImage);
            View.AddSubview(shadowView);

            // ---------- TITLE AND SUBTITLE --------- //
            var title = new UILabel
            {
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(22f, UIFontWeight.Bold),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 2
            };

            var subTitle = new UILabel
            {
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(18f, UIFontWeight.Regular),
                LineBreakMode = UILineBreakMode.TailTruncation,
                Lines = 1,
                Alpha = 0.7f,
            };

            Add(title);
            Add(subTitle);

            static (UIButton, UILabel) BuildButton(string icon, nfloat size)
            {
                var label = new UILabel
                {
                    TextColor = UIColor.White,
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = UIFont.FromName("FontAwesome5Pro-Light", size),
                    Text = icon,
                    UserInteractionEnabled = false,
                    TextAlignment = UITextAlignment.Center
                };

                var button = new UIButton
                {
                    ContentEdgeInsets = new UIEdgeInsets(14, 24, 14, 24)
                };
                button.AddSubview(label);

                label.CenterXAnchor.ConstraintEqualTo(button.CenterXAnchor).Active = true;
                label.CenterYAnchor.ConstraintEqualTo(button.CenterYAnchor).Active = true;

                return (button, label);
            }

            // Playback Controls

            (UIButton shuffleButton, UILabel shuffleButtonLabel) = BuildButton("\uf074", 22);
            (UIButton previousMediaButton, _) = BuildButton("\uf048", 28);
            (UIButton playPauseButton, UILabel playPauseButtonLabel) = BuildButton("\uf04c", 44);
            (UIButton nextMediaButton, _) = BuildButton("\uf051", 28);
            (UIButton repeatButton, UILabel repeatButtonLabel) = BuildButton("\uf363", 22);

            var stackView = new UIStackView(new UIView[]
            {
                shuffleButton,
                previousMediaButton,
                playPauseButton,
                nextMediaButton,
                repeatButton
            })
            {
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.EqualSpacing,
                Spacing = 5
            };

            Add(stackView);

            // Setup the constraints
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                backgroundImage.AtTopOf(View),
                backgroundImage.AtLeftOf(View),
                backgroundImage.WithSameWidth(View),
                backgroundImage.WithSameHeight(View));

            View.AddConstraints(
                blurredEffectView.AtTopOf(View),
                blurredEffectView.AtLeftOf(View),
                blurredEffectView.WithSameWidth(View),
                blurredEffectView.WithSameHeight(View));

            shadowView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 48).Active = true;
            shadowView.RightAnchor.ConstraintEqualTo(View.RightAnchor, -48).Active = true;
            shadowView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 48).Active = true;
            shadowView.HeightAnchor.ConstraintEqualTo(shadowView.WidthAnchor).Active = true;

            View.AddConstraints(
                trackImage.AtTopOf(shadowView),
                trackImage.AtLeftOf(shadowView),
                trackImage.WithSameWidth(shadowView),
                trackImage.WithSameHeight(shadowView));

            View.AddConstraints(
                title.Below(shadowView, 38));

            View.AddConstraints(
                subTitle.Below(title, 8));

            title.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 22).Active = true;
            title.RightAnchor.ConstraintEqualTo(View.RightAnchor, -22).Active = true;

            subTitle.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 22).Active = true;
            subTitle.RightAnchor.ConstraintEqualTo(View.RightAnchor, -22).Active = true;

            View.AddConstraints(stackView.AtBottomOf(View, 48));

            stackView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 28).Active = true;
            stackView.RightAnchor.ConstraintEqualTo(View.RightAnchor, -28).Active = true;

            // ---------- BINDING --------- //
            var set = this.CreateBindingSet<PlaybackView, PlaybackViewModel>();
            set.Bind(trackImage).For(x => x.ImagePath).To(vm => vm.CurrentTrack.ArtworkUrl);
            set.Bind(backgroundImage).For(x => x.ImagePath).To(vm => vm.CurrentTrack.ArtworkUrl);
            set.Bind(title).To(vm => vm.CurrentTrack.Title);
            set.Bind(subTitle).To(vm => vm.CurrentTrack.User.Username);

            set.Bind(shuffleButton).To(vm => vm.ToggleShuffleCommand);
            set.Bind(previousMediaButton).To(vm => vm.SkipPreviousCommand);
            set.Bind(playPauseButton).To(vm => vm.ToggleMediaStateCommand);
            set.Bind(nextMediaButton).To(vm => vm.SkipNextCommand);
            set.Bind(repeatButton).To(vm => vm.ToggleRepeatCommand);

            set.Bind(playPauseButtonLabel).For(x => x.Text).To(vm => vm.PlayButtonIcon);

            set.Apply();
        }

        public override async void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            await ViewModel.CloseCommand.ExecuteAsync();
        }
    }
}