using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Items.User;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using System;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views
{
    /// <summary>
    ///     Display user information
    /// </summary>
    public sealed partial class UserView
    {
        /// <summary>
        ///     Setup XAML page
        /// </summary>
        public UserView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        ///     The main view model for this page
        /// </summary>
        public UserViewModel ViewModel { get; } = new UserViewModel();

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the target user (may be null)
            var targetUser = e.Parameter as BaseUser;

            // If we have both objects and they equal, do
            // nothing and return (we are navigating to the
            // same page.
            if (ViewModel.User?.UserId == targetUser?.UserId)
                return;

            // If both of these are null, we have a problem.
            // In the future we would try load the user ID from
            // a stored file. For now through an exception.
            if (targetUser == null && ViewModel.User == null)
                throw new ArgumentNullException(nameof(e),
                    "Both the view model and target user are null. UserView cannot continue");

            // If the target user is not null, we can setup the
            // the view model.
            if (targetUser != null)
            {
                // Clear description
                Description.Blocks.Clear();

                // Create the model with a new user object
                await ViewModel.UpdateModel(targetUser);

                if (!string.IsNullOrEmpty(ViewModel.User.Description))
                {
                    TextHelper.ConvertTextToFormattedTextBlock(ViewModel.User.Description, ref Description);
                }
            }

            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("User View");
        }
    }
}