using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.App.iOS.Cells.Media.Grid;
using UIKit;

namespace SoundByte.App.iOS.Sources
{
    public class MediaGridViewSource : MvxCollectionViewSource
    {
        public MediaGridViewSource(UICollectionView collectionView) : base(collectionView, UserGridCell.Key)
        {
            CollectionView.RegisterClassForCell(typeof(TrackGridCell), TrackGridCell.Key);
            CollectionView.RegisterClassForCell(typeof(PlaylistGridCell), PlaylistGridCell.Key);
            CollectionView.RegisterClassForCell(typeof(UserGridCell), UserGridCell.Key);
            CollectionView.RegisterClassForCell(typeof(PodcastShowGridCell), PodcastShowGridCell.Key);
        }

        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            if (item is Core.Models.Media.Track)
                return (UICollectionViewCell)collectionView.DequeueReusableCell(TrackGridCell.Key, indexPath);

            if (item is Core.Models.Media.Playlist)
                return (UICollectionViewCell)collectionView.DequeueReusableCell(PlaylistGridCell.Key, indexPath);

            if (item is Core.Models.Media.User)
                return (UICollectionViewCell)collectionView.DequeueReusableCell(UserGridCell.Key, indexPath);

            if (item is Core.Models.Media.PodcastShow)
                return (UICollectionViewCell)collectionView.DequeueReusableCell(PodcastShowGridCell.Key, indexPath);

            return GetOrCreateCellFor(collectionView, indexPath, item);
        }
    }
}