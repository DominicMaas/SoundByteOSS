using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
using SoundByte.Core.ViewModels.Details;
using SoundByte.Core.ViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Helpers;

namespace SoundByte.Core.Models.Content
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
        ///     Is this feed requires the user to be logged in
        /// </summary>
        public bool IsAuthenticatedFeed { get; }

        /// <summary>
        ///     A list of buttons to appear on the content group.
        /// </summary>
        public List<ContentButton> Buttons { get; }

        /// <summary>
        ///     Code that is run when the item is clicked on
        /// </summary>
        public MvxCommand<Media.Media> OnItemClickCommand { get; }

        /// <summary>
        ///     The command to run when the user clicks on the view all button
        /// </summary>
        public IMvxCommand OnViewAllClickCommand { get; }

        public ContentGroup(ISource source, string title, bool isAuthenticatedFeed, List<ContentButton> buttons, Action<ContentGroup>? onClickAction = null, Action<Media.Media>? onItemClickCommand = null)
        {
            Collection = new SoundByteCollection<ISource>(source);
            Title = title;
            IsAuthenticatedFeed = isAuthenticatedFeed;
            Buttons = buttons;

            if (onClickAction != null)
            {
                // Bind to the custom click event
                OnViewAllClickCommand = new MvxCommand(() => onClickAction?.Invoke(this));
            }
            else
            {
                // Nothing was passed through, use the default navigation
                OnViewAllClickCommand = new MvxAsyncCommand(async () =>
                {
                    var navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                    await navigationService.Navigate<GenericListViewModel, GenericListViewModel.Holder>(new GenericListViewModel.Holder(Collection, Title));
                });
            }

            OnItemClickCommand = onItemClickCommand == null
                ? new MvxCommand<Media.Media>(async item => await ItemInvokeHelper.InvokeItem(Collection, item))
                : new MvxCommand<Media.Media>(onItemClickCommand);
        }

        /// <summary>
        ///     Refresh the inner content. Wraps around
        ///     the collection refresh event.
        /// </summary>
        public void Refresh()
        {
            Collection.RefreshItems();
        }
    }
}