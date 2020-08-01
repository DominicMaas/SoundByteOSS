using SoundByte.Core.Models.Content;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     This service provides application content throughout the entire application.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        ///     Add a content group to the application, takes in where the content group
        ///     should be located and the actual content group it's self.
        /// </summary>
        /// <param name="location">The location where this content group should be in the app.</param>
        /// <param name="group">The content group to add.</param>
        void AddContent(ContentArea location, ContentGroup group, double orderWeight = 0.0);

        /// <summary>
        ///     Add content to the application that is specified by a certain music provider.
        /// </summary>
        /// <param name="musicProviderId"></param>
        /// <param name="location"></param>
        /// <param name="group"></param>
        void AddContent(Guid musicProviderId, ContentArea location, ContentGroup group, double orderWeight = 0.0);

        /// <summary>
        ///     Gets a list of content groups for a specified location.
        /// </summary>
        /// <param name="location">The location to search.</param>
        /// <returns>A list of content groups matching this location.</returns>
        IEnumerable<ContentGroup> GetContentByLocation(ContentArea location);

        /// <summary>
        ///     Get all content groups that register by a specified music provider.
        /// </summary>
        /// <param name="musicProviderId">The if of the music provider.</param>
        /// <returns></returns>
        IEnumerable<ContentGroup> GetContentByMusicProvider(Guid musicProviderId);

        /// <summary>
        ///     Remove all registered content groups that are registered to a
        ///     specific music provider.
        /// </summary>
        /// <param name="musicProviderId">The id of the music provider.</param>
        void RemoveContentByMusicProvider(Guid musicProviderId);
    }
}