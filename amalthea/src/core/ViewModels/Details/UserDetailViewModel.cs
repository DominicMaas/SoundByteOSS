using Flurl;
using Flurl.Http;
using MvvmCross.ViewModels;
using SoundByte.Core.Extensions;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.LastFm;
using SoundByte.Core.Models.Media;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Details
{
    /// <summary>
    ///     View more details about a certain user in the application (such as description,
    ///     following, viewing content etc.
    /// </summary>
    public class UserDetailViewModel : MvxViewModel<User>
    {
        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> PageContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     The user that is currently being displayed on the screen
        /// </summary>
        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private User _user;

        /// <summary>
        ///     If detailed information is available to show
        /// </summary>
        public bool ShowDetailedInformation
        {
            get => _showDetailedInformation;
            set => SetProperty(ref _showDetailedInformation, value);
        }

        private bool _showDetailedInformation;

        public ArtistRoot? DetailedInformation
        {
            get => _detailedInformation;
            set => SetProperty(ref _detailedInformation, value);
        }

        private ArtistRoot? _detailedInformation;

        /// <summary>
        ///     Prepare the view model for display
        /// </summary>
        /// <param name="parameter">The parameter that the user wants to view</param>
        public override void Prepare(User parameter)
        {
            User = parameter;
        }

        public override async Task Initialize()
        {
            await GetDetailUserInformationAsync();
        }

        private async Task GetDetailUserInformationAsync()
        {
            // Clean up the username so last fm picks up a good resource
            var cleanUsername = User.Username;

            // Fixes common YouTube issues
            if (cleanUsername.ToUpperInvariant().Contains("VEVO"))
                cleanUsername = cleanUsername.TrimEnd("VEVO", System.StringComparison.OrdinalIgnoreCase);

            try
            {
                DetailedInformation = await "https://ws.audioscrobbler.com/2.0"
                    .SetQueryParams(new
                    {
                        method = "artist.getinfo",
                        artist = cleanUsername,
                        api_key = Constants.LastFmApiKey,
                        format = "json",
                        autocorrect = 1
                    })
                    .GetJsonAsync<ArtistRoot>();
            }
            catch (Exception e)
            {
                DetailedInformation = null;
                ShowDetailedInformation = false;
            }
        }
    }
}