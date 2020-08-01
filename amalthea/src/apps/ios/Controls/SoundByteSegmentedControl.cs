using System;
using System.Collections.Generic;
using CoreGraphics;
using SoundByte.Core.Helpers;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Controls
{
    /// <summary>
    ///     A custom segmented control for SoundByte that loosely follows the
    ///     UWP list view styling
    /// </summary>
    public class SoundByteSegmentedControl : UIView
    {
        private readonly List<string> _buttonTitles;
        private readonly List<UIButton> _buttons = new List<UIButton>();
        private UIView _selectorView;

        public delegate void ValueChangedEventHandler(object sender, int index);

        public event ValueChangedEventHandler IndexChanged;

        private nfloat LeftOffset = 20.0f;
        private nfloat Spacing = 22.0f;

        public SoundByteSegmentedControl(IEnumerable<string> buttonTitles) : base(new CGRect(0, 0, 0, 46))
        {
            _buttonTitles = new List<string>(buttonTitles);

            UpdateView();
        }

        private void ConfigStackView()
        {
            var stack = new UIStackView(_buttons.ToArray())
            {
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Fill,
                Distribution = UIStackViewDistribution.FillProportionally,
                Spacing = Spacing
            };

            AddSubview(stack);

            stack.TranslatesAutoresizingMaskIntoConstraints = false;
            stack.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            stack.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            stack.LeftAnchor.ConstraintEqualTo(LeftAnchor, LeftOffset).Active = true;
        }

        private void ConfigSelectorView()
        {
            _selectorView = new UIView(new CGRect(LeftOffset + 2.5f, 42, CalculateTextWidth(_buttonTitles[0]), 3))
            {
                BackgroundColor = ColorHelper.Accent.ToPlatformColor()
            };

            _buttons[0].SetTitleColor(ColorHelper.Accent.ToPlatformColor(), UIControlState.Normal);

            AddSubview(_selectorView);
        }

        private void CreateButtons()
        {
            _buttons.Clear();
            foreach (var v in Subviews) v.RemoveFromSuperview();

            foreach (var buttonTitle in _buttonTitles)
            {
                var button = new UIButton(UIButtonType.System);
                button.SetTitle(buttonTitle, UIControlState.Normal);
                button.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                button.TitleLabel.Font = UIFont.BoldSystemFontOfSize(20);

                var f = button.Frame;
                f.Width = CalculateTextWidth(buttonTitle);
                button.Frame = f;

                button.TouchUpInside += Button_TouchUpInside;
                _buttons.Add(button);
            }

            _buttons[0].SetTitleColor(ColorHelper.Accent.ToPlatformColor(), UIControlState.Normal);
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            var offset = LeftOffset;

            for (var i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                button.SetTitleColor(UIColor.LightGray, UIControlState.Normal);

                // Increment the offset
                var targetWidth = CalculateTextWidth(_buttonTitles[i]);

                offset += 2.5f;

                if (button == sender)
                {
                    IndexChanged?.Invoke(this, i);

                    Animate(0.3, () =>
                    {
                        // Move the frame
                        var frame = _selectorView.Frame;
                        frame.X = offset;
                        frame.Width = targetWidth;

                        _selectorView.Frame = frame;
                    });

                    // Set the color
                    button.SetTitleColor(ColorHelper.Accent.ToPlatformColor(), UIControlState.Normal);
                }

                offset += targetWidth;
                offset += Spacing;
            }
        }

        private void UpdateView()
        {
            CreateButtons();
            ConfigSelectorView();
            ConfigStackView();
        }

        private nfloat CalculateTextWidth(string text)
        {
            var label = new UILabel
            {
                Lines = 0,
                Font = UIFont.BoldSystemFontOfSize(18),
                Text = text
            };

            label.SizeToFit();

            var size = label.Frame;
            return size.Width;
        }
    }
}