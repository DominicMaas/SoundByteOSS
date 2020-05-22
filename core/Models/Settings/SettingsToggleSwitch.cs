using System;

namespace SoundByte.Core.Models.Settings
{
    public class SettingsToggleSwitch
    {
        public string Header { get; }

        public bool DefaultValue { get; }

        public Action<bool> OnToggle { get; }

        public SettingsToggleSwitch(string header, bool defaultValue, Action<bool> onToggle)
        {
            Header = header;
            DefaultValue = defaultValue;
            OnToggle = onToggle;
        }
    }
}