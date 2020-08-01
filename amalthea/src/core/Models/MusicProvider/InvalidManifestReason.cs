namespace SoundByte.Core.Models.MusicProvider
{
    /// <summary>
    ///     A list of invalid manifest reasons
    /// </summary>
    public enum InvalidManifestReason
    {
        Missing,
        Invalid,
        UnsupportedPlatformVersion,
        MissingScript,
        MissingId,
        MissingName,
        MissingPublisher,
        MissingDescription,
        MissingPlatformVersion,
        MissingVersion,
        MissingAssets,
        PlatformUnsupported
    }
}