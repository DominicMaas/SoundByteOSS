using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Methods for access store services
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        ///     Loads everything required for the store service
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        ///     Open the review dialog for the application
        /// </summary>
        Task RequestReviewAsync();

        /// <summary>
        ///     Opens the store page in the users default
        ///     browser / the store application
        /// </summary>
        Task OpenStorePageAsync();

        /// <summary>
        ///     Returns if this user has premium
        /// </summary>
        /// <returns>If the user has bought premium</returns>
        Task<bool> HasPremiumAsync();

        /// <summary>
        ///     Subscribe the user to SoundByte Premium
        /// </summary>
        Task SubscribeToPremiumAsync();

        /// <summary>
        ///     Unsubscribe the user from SoundByte Premium
        /// </summary>
        Task UnsubscribeFromPremiumAsync();

        /// <summary>
        ///     Purchase the one time premium pass
        ///     for SoundByte.
        /// </summary>
        Task PurchaseOneTimeAsync();

        /// <summary>
        ///     Handles logic for requesting refunds
        ///     depending on the platform.
        /// </summary>
        Task RequestRefundAsync();
    }
}