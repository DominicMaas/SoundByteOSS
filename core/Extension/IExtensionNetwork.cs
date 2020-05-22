using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Models.Extension;

namespace SoundByte.Core.Extension
{
    public interface IExtensionNetwork
    {
        [ApiMetadata(ApiVersion.V1, "Get a url and return the body as a simple string.")]
        [ApiParameterMetadata("url", "The url to fetch.")]
        string GetString(string url);

        dynamic Get(string url);
    }
}