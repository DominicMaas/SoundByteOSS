using System;

namespace SoundByte.Core.Models.Settings
{
    public class SettingsButton
    {
        public string Content { get; }

        public Action OnClick { get; }

        public SettingsButton(string content, Action onClick)
        {
            Content = content;
            OnClick = onClick;
        }
    }
}