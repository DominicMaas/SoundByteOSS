using SoundByte.Core.Models.Media;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Service for managing windows and iOS activities (alongside any
    ///     custom SoundByte activities in the future)
    /// </summary>
    public interface IActivityService
    {
        Task UpdateActivityAsync(ISource source, Media media, IEnumerable<Media> queue, string token, TimeSpan? timeSpan, bool isShuffled);
    }
}