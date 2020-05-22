using SoundByte.App.Uwp.Models;
using SoundByte.Core.Models.Content;
using System;
using System.Collections.Generic;

namespace SoundByte.App.Uwp.ServicesV2
{
    /// <summary>
    ///     This service provides app content throughout the entire application. At
    ///     the moment these sources are provided through the app, but will be updated over time
    ///     to load the sources via extensions.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        ///     Add a content group to the app, takes in where the content group
        ///     should be located and the actual content group it's self.
        /// </summary>
        /// <param name="location">The location where this content group should be in the app.</param>
        /// <param name="group">The content group to add.</param>
        void AddContent(ContentArea location, ContentGroup group);

        void AddContent(Guid musicProviderId, ContentArea location, ContentGroup group);

        /// <summary>
        ///     Gets a list of content groups for a specified location.
        /// </summary>
        /// <param name="location">The location to search.</param>
        /// <returns>A list of content groups matching this location.</returns>
        IEnumerable<ContentGroup> GetContentGroups(ContentArea location);

        IEnumerable<ContentGroup> GetContentByMusicProvider(Guid musicProviderId);

        void RemoveContentByMusicProvider(Guid musicProviderId);
    }
}