using SoundByte.Core.Models.Navigation;
using SoundByte.App.Uwp.Views;
using SoundByte.App.Uwp.Views.Generic;
using System;
using SoundByte.Core.Extension;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    public class ExtensionNavigation : IExtensionNavigation
    {
        public void NavigateTo(PageName name, object param)
        {
            switch (name)
            {
                case PageName.TrackListView:
                    App.NavigateTo(typeof(TrackListView), param);
                    break;

                case PageName.PlaylistListView:
                    App.NavigateTo(typeof(PlaylistListView), param);
                    break;

                case PageName.UserListView:
                    App.NavigateTo(typeof(UserListView), param);
                    break;

                case PageName.MixedListView:
                    App.NavigateTo(typeof(MixedListView), param);
                    break;

                case PageName.FilteredListView:
                    App.NavigateTo(typeof(FilteredListView), param);
                    break;

                default:
                    throw new ArgumentException("Invalid page name.", nameof(name));
            }
        }

        public void NavigateTo(PageName name)
        {
            NavigateTo(name, null);
        }
    }
}