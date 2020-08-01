using System;
using System.Collections.Generic;

namespace SoundByte.Core.Models.MusicProvider
{
    public class Manifest
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Publisher { get; set; }

        public string Version { get; set; }

        public double? PlatformVersion { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Script { get; set; }

        public ManifestResolvers? Resolvers { get; set; }

        public List<string> Permissions { get; set; }

        public List<string> Platforms { get; set; }

        public ManifestAssets Assets { get; set; }

        public ManifestAuthentication Authentication { get; set; }

        public ManifestPlayback Playback { get; set; }

        public List<ManifestContent> Content { get; set; }
    }

    public class ManifestResolvers
    {
        public string? Track { get; set; }
        public string? Tracks { get; set; }
        public string? Playlist { get; set; }
        public string? Playlists { get; set; }
        public string? User { get; set; }
        public string? Users { get; set; }
        public string? PlaylistTracks { get; set; }
    }

    public class ManifestAssets
    {
        public string StoreLogo { get; set; }
    }

    public class ManifestAuthentication
    {
        public string Type { get; set; }

        public string Scheme { get; set; }

        public string ClientId { get; set; }

        public string RedirectUrl { get; set; }

        public string ConnectUrl { get; set; }

        public bool? AppendState { get; set; }
    }

    public class ManifestContent
    {
        public string Title { get; set; }

        public string Area { get; set; }

        public bool IsAuthenticatedFeed { get; set; } = false;

        public List<string> Buttons { get; set; }

        public string OnGet { get; set; }

        public string OnViewMore { get; set; }
    }

    public class ManifestPlayback
    {
        public bool SupportsVideo { get; set; }

        public string OnMusicRequest { get; set; }

        public string OnVideoRequest { get; set; }
    }
}