using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Models.Extension;
using SoundByte.Core.Models.Settings;

namespace SoundByte.Core.Extension
{
    public interface IExtensionSettings
    {
        [ApiMetadata(ApiVersion.V1, "Add a toggle switch to the settings page.")]
        [ApiParameterMetadata("toggleSwitch", "The toggle switch to add.")]
        void AddToggleSwitch(SettingsToggleSwitch toggleSwitch);

        [ApiMetadata(ApiVersion.V1, "Add a button to the settings page.")]
        [ApiParameterMetadata("button", "The button to add.")]
        void AddButton(SettingsButton button);
    }
}