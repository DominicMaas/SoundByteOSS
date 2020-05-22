using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.Models.Buttons;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels.Generic;
using SoundByte.App.Uwp.Views.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.StartScreen;

namespace SoundByte.App.Uwp.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        /// <summary>
        ///     List of items to display on the page.
        /// </summary>
        public ObservableCollection<ContentGroup> PageContent { get; } = new ObservableCollection<ContentGroup>();

        #region Getters

        private string _followUserIcon = "\uE8FA";
        private string _followUserText;

        // Icon for the pin button
        private string _pinButtonIcon = "\uE718";

        // Text for the pin button
        private string _pinButtonText;

        private bool _showFollowButton = true;
        private BaseUser _user;

        public bool ShowFollowButton
        {
            get => _showFollowButton;
            set
            {
                if (value != _showFollowButton)
                {
                    _showFollowButton = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonIcon
        {
            get => _pinButtonIcon;
            set
            {
                if (value != _pinButtonIcon)
                {
                    _pinButtonIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string FollowUserIcon
        {
            get => _followUserIcon;
            set
            {
                if (value != _followUserIcon)
                {
                    _followUserIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string FollowUserText
        {
            get => _followUserText;
            set
            {
                if (value != _followUserText)
                {
                    _followUserText = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonText
        {
            get => _pinButtonText;
            set
            {
                if (value != _pinButtonText)
                {
                    _pinButtonText = value;
                    UpdateProperty();
                }
            }
        }

        public BaseUser User
        {
            get => _user;
            private set
            {
                if (value == _user) return;

                _user = value;
                UpdateProperty();
            }
        }

        #endregion Getters

        /// <summary>
        ///     Setup this viewmodel for a specific user
        /// </summary>
        /// <param name="user"></param>
        public async Task UpdateModel(BaseUser user)
        {
            // Set the new user
            User = user;

            // Clear any models
            PageContent.Clear();

            // User tracks
            PageContent.Add(new ContentGroup(new UserTrackSource { Service = user.ServiceType, UserId = user.UserId }, "Tracks", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Tracks"))));

            // User likes
            PageContent.Add(new ContentGroup(new UserLikeSource { Service = user.ServiceType, UserId = user.UserId }, "Likes", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Likes"))));

            // User Playlists
            PageContent.Add(new ContentGroup(new UserPlaylistSource { Service = user.ServiceType, UserId = user.UserId }, "Playlists", new List<ContentButton>(),
                parent => App.NavigateTo(typeof(PlaylistListView), new GenericListViewModel.Holder(parent.Collection, "Playlists"))));

            // User Followers
            PageContent.Add(new ContentGroup(new UserFollowerSource { Service = user.ServiceType, UserId = user.UserId }, "Followers", new List<ContentButton>(),
                parent => App.NavigateTo(typeof(UserListView), new GenericListViewModel.Holder(parent.Collection, "Followers"))));

            // User Followings
            PageContent.Add(new ContentGroup(new UserFollowingSource { Service = user.ServiceType, UserId = user.UserId }, "Following", new List<ContentButton>(),
                parent => App.NavigateTo(typeof(UserListView), new GenericListViewModel.Holder(parent.Collection, "Following"))));

            try
            {
                // Update with the correct values
                User = await BaseUser.GetUserAsync(User.ServiceType, User.UserId);
            }
            catch
            {
                // Do nothing, failed to update user
            }

            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();

            // Check if the tile has been pinned
            if (TileHelper.IsTilePinned("User_" + User.UserId))
            {
                PinButtonIcon = "\uE77A";
                PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
            }
            else
            {
                PinButtonIcon = "\uE718";
                PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
            }

            // Disable by default
            ShowFollowButton = false;

            // Follow by default
            FollowUserIcon = "\uE8FA";
            FollowUserText = "Follow User";

            // SoundCloud Users
            if (User.ServiceType == ServiceTypes.SoundCloud || User.ServiceType == ServiceTypes.SoundCloudV2)
            {
                // If the users account is connected and they are not viewing their own profile
                if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud) &&
                    User.UserId != SoundByteService.Current.GetConnectedUser(ServiceTypes.SoundCloud)?.UserId)
                {
                    // Show the follow button
                    ShowFollowButton = true;

                    // Check if we are following the user
                    if ((await SoundByteService.Current.ExistsAsync(ServiceTypes.SoundCloud, "/me/followings/" + User.UserId)).Response)
                    {
                        FollowUserIcon = "\uE1E0";
                        FollowUserText = "Unfollow User";
                    }
                }
            }
            // YouTube Users
            else if (User.ServiceType == ServiceTypes.YouTube)
            {
                // Show the follow button
                ShowFollowButton = true;

                // If the users account is connected and they are not viewing their own profile
                if (SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube) &&
                    User.UserId != SoundByteService.Current.GetConnectedUser(ServiceTypes.YouTube)?.UserId)
                {
                    // TODO
                }
            }
        }

        /// <summary>
        ///     Follows the requested user
        /// </summary>
        public async void FollowUser()
        {
            if (User.ServiceType != ServiceTypes.SoundCloud || User.ServiceType != ServiceTypes.SoundCloudV2)
            {
                await new MessageDialog($"This feature is not currently supported on {User.ServiceType} users.").ShowAsync();
                return;
            }

            // Show the loading ring
            await App.SetLoadingAsync(true);

            // Check if we are following the user
            if ((await SoundByteService.Current.ExistsAsync(ServiceTypes.SoundCloud, "/me/followings/" + User.UserId)).Response)
            {
                // Unfollow the user
                if ((await SoundByteService.Current.DeleteAsync(ServiceTypes.SoundCloud, "/me/followings/" + User.UserId)).Response)
                {
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Unfollow User");
                    FollowUserIcon = "\uE8FA";
                    FollowUserText = "Follow User";
                }
                else
                {
                    FollowUserIcon = "\uE1E0";
                    FollowUserText = "Unfollow User";
                }
            }
            else
            {
                // Follow the user
                if ((await SoundByteService.Current.PutAsync(ServiceTypes.SoundCloud, $"/me/followings/{User.UserId}")).Response)
                {
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Follow User");
                    FollowUserIcon = "\uE1E0";
                    FollowUserText = "Unfollow User";
                }
                else
                {
                    FollowUserIcon = "\uE8FA";
                    FollowUserText = "Follow User";
                }
            }
            // Hide the loading ring
            await App.SetLoadingAsync(false);
        }

        public async void PinUser()
        {
            // Show the loading ring
            await App.SetLoadingAsync(true);
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();

            // Check if the tile exists
            if (TileHelper.IsTilePinned("User_" + User.UserId))
            {
                // Try remove the tile
                if (await TileHelper.RemoveTileAsync("User_" + User.UserId))
                {
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Unpin User");
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
            }
            else
            {
                // Create the tile
                if (await TileHelper.CreateTileAsync("User_" + User.UserId, User.Username,
                    "soundbyte://user?id=" + User.UserId + "&service=" + User.ServiceType, new Uri(User.ThumbnailUrl),
                    ForegroundText.Light))
                {
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Pin User");
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
            }
            // Hide the loading ring
            await App.SetLoadingAsync(false);
        }

        public async void Refresh() => await UpdateModel(User);
    }
}