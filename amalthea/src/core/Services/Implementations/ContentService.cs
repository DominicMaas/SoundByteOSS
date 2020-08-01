using MvvmCross.Navigation;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.Content.Buttons;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources.Core;
using SoundByte.Core.ViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundByte.Core.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly List<ContentItem> _items;

        public ContentService(IAuthenticationService authenticationService, IMvxNavigationService mvxNavigationService)
        {
            _items = new List<ContentItem>();

            // Default SoundByte Music
            AddContent(ContentArea.Home, new ContentGroup(new HistorySource(authenticationService), "Recently Played", true, new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }), 1000.0);

            AddContent(ContentArea.Home, new ContentGroup(new MostPlayedSource(authenticationService), "Most Played", true, new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }), 1000.0);

            AddContent(ContentArea.MyMusicLikes, new ContentGroup(new LikeSource(authenticationService), "SoundByte", true, new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, async parent => await mvxNavigationService.Navigate<GenericListViewModel, GenericListViewModel.Holder>(new GenericListViewModel.Holder(parent.Collection, "Likes"))), 1000.0);

            AddContent(ContentArea.DiscoverMedia, new ContentGroup(new TopMusicPlayedSource(), "Top on SoundByte", false, new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }), -1000.0);

            // Podcasts
        }

        public void AddContent(ContentArea location, ContentGroup group, double orderWeight = 0.0)
        {
            _items.Add(new ContentItem(Guid.NewGuid(), location, group, orderWeight));
        }

        public void AddContent(Guid musicProviderId, ContentArea location, ContentGroup group, double orderWeight = 0.0)
        {
            _items.Add(new ContentItem(musicProviderId, location, group, orderWeight));
        }

        public IEnumerable<ContentGroup> GetContentByLocation(ContentArea location)
        {
            return _items.Where(x => x.Location == location).OrderBy(x => x.OrderWeight).Select(x => x.Group);
        }

        public IEnumerable<ContentGroup> GetContentByMusicProvider(Guid musicProviderId)
        {
            return _items.Where(x => x.MusicProviderId == musicProviderId).OrderBy(x => x.OrderWeight).Select(x => x.Group);
        }

        public void RemoveContentByMusicProvider(Guid musicProviderId)
        {
            _items.RemoveAll(x => x.MusicProviderId == musicProviderId);
        }

        private class ContentItem
        {
            public Guid MusicProviderId { get; }

            public ContentArea Location { get; }

            public ContentGroup Group { get; }

            public double OrderWeight { get; }

            public ContentItem(Guid musicProviderId, ContentArea location, ContentGroup group, double orderWeight)
            {
                MusicProviderId = musicProviderId;
                Location = location;
                Group = group;
                OrderWeight = orderWeight;
            }
        }
    }
}