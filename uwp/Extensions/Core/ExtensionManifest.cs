#nullable enable

using System;
using System.Collections.Generic;

namespace SoundByte.App.Uwp.Extensions.Core
{
    public class ExtensionManifest
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Publisher { get; set; }

        public string? Version { get; set; }

        public double? PlatformVersion { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public string? Script { get; set; }

        public List<string>? Permissions { get; set; }

        public List<string>? Platforms { get; set; }

        public ExtensionAssets? Assets { get; set; }

        public MusicProvider? Provider { get; set; }
    }

    public class ExtensionAssets
    {
        public string? StoreLogo { get; set; }
    }

    public class MusicProvider
    {
        public string? Name { get; set; }

        public int? LegacyId { get; set; }

        public MusicProviderAuthentication? Authentication { get; set; }

        public List<MusicProviderContent>? Content { get; set; }

        public MusicProviderPlayback? Playback { get; set; }
    }

    public class MusicProviderAuthentication
    {
        public string? Type { get; set; } // oauth / custom

        public string? ClientId { get; set; }

        public string? ConnectUrl { get; set; }
    }

    public class MusicProviderContent
    {
        public string? Title { get; set; }

        public string? Area { get; set; }

        public List<string>? Buttons { get; set; }

        public string? OnGet { get; set; }

        public string? OnViewMore { get; set; }
    }

    public class MusicProviderPlayback
    {
        public bool? SupportsVideo { get; set; }

        public string? OnMusicRequest { get; set; }

        public string? OnVideoRequest { get; set; }
    }
}

#nullable restore