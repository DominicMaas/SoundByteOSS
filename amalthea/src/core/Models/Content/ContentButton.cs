using MvvmCross.Commands;

namespace SoundByte.Core.Models.Content
{
    /// <summary>
    ///     A button that appears on a content group
    /// </summary>
    public class ContentButton
    {
        /// <summary>
        ///     Is the button extended to show the label alongside the icon.
        /// </summary>
        public bool IsExtended { get; }

        /// <summary>
        ///     Text to appear on the button (if IsExtended is true).
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     The icon to appear on the button.
        /// </summary>
        public string Glyph { get; }

        /// <summary>
        ///     Command that is run on button click
        /// </summary>
        public IMvxAsyncCommand<ContentGroup> ButtonClickCommand { get; }

        /// <summary>
        ///     Initialize a ContentButton
        /// </summary>
        /// <param name="label">The label to appear on the button</param>
        /// <param name="glyph">The button icon</param>
        /// <param name="isExtended">Should the icon be extended or not</param>
        /// <param name="clickCommand">Command to run on click</param>
        public ContentButton(string label, string glyph, bool isExtended, IMvxAsyncCommand<ContentGroup> clickCommand)
        {
            Label = label;
            Glyph = glyph;
            IsExtended = isExtended;
            ButtonClickCommand = clickCommand;
        }
    }
}