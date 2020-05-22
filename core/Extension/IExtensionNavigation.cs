using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Models.Extension;
using SoundByte.Core.Models.Navigation;

namespace SoundByte.Core.Extension
{
    public interface IExtensionNavigation
    {
        [ApiMetadata(ApiVersion.V1, "Navigate the user to a page.")]
        [ApiParameterMetadata("name", "The name of the page.")]
        [ApiParameterMetadata("param", "The parameter that the page expects.")]
        void NavigateTo(PageName name, object param);

        [ApiMetadata(ApiVersion.V1, "Navigate the user to a page.")]
        [ApiParameterMetadata("name", "The name of the page.")]
        void NavigateTo(PageName name);
    }
}