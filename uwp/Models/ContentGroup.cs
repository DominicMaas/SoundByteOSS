using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Commands;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.Views;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Models
{
    /// <summary>
    ///     Represents a content group within SoundByte. Content groups provide
    ///     a title, error checking view, buttons and a grid view for a certain type
    ///     of content (e.g users likes, SoundCloud stream).
    /// </summary>
    public class ContentGroup
    {
        /// <summary>
        ///     Source of data to display
        /// </summary>
        public SoundByteCollection<ISource> Collection { get; }

        /// <summary>
        ///     The title to display on the UI.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     A list of buttons to appear on the content group.
        /// </summary>
        public List<ContentButton> Buttons { get; }

        /// <summary>
        ///     An optional command to perform custom code on click.
        /// </summary>
        public ICommand OnItemClickCommand { get; }

        /// <summary>
        ///     The command to run when the user clicks on the view all button
        /// </summary>
        public ICommand OnViewAllClickCommand { get; }

        public ContentGroup(ISource source, string title, List<ContentButton> buttons, Action<ContentGroup> onClickAction, Action<BaseSoundByteItem> onItemClickCommand = null)
        {
            Collection = new SoundByteCollection<ISource>(source);
            Title = title;
            Buttons = buttons;

            if (onClickAction != null)
                OnViewAllClickCommand = new DelegateCommand<ContentGroup>(onClickAction);

            if (onItemClickCommand != null)
                OnItemClickCommand = new DelegateCommand<BaseSoundByteItem>(onItemClickCommand);
        }

        #region Methods

        /// <summary>
        ///     Refresh the inner content. Wraps around
        ///     the collection refresh event.
        /// </summary>
        public void Refresh()
        {
            Collection.RefreshItems();
        }

        public async void PlaySingleItem(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            // Get the item
            var item = (BaseSoundByteItem)e.ClickedItem;

            // If the on click command is set, run that code instead
            if (OnItemClickCommand != null)
            {
                OnItemClickCommand.Execute(item);
                return;
            }

            // Loop through all the item types
            switch (item.Type)
            {
                case ItemType.Track:
                    await BaseViewModel.PlayAllTracksAsync(Collection, item.Track);
                    break;

                case ItemType.User:
                    App.NavigateTo(typeof(UserView), item.User);
                    break;

                case ItemType.Playlist:
                    App.NavigateTo(typeof(PlaylistView), item.Playlist);
                    break;

                case ItemType.Podcast:
                    App.NavigateTo(typeof(PodcastShowView), item.Podcast);
                    break;
            }
        }

        #endregion Methods
    }
}