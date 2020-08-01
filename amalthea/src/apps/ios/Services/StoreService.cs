using SoundByte.Core.Services.Definitions;
using StoreKit;
using System;
using System.Threading.Tasks;

namespace SoundByte.App.iOS.Services
{
    public class StoreService : IStoreService
    {
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> HasPremiumAsync()
        {
            return Task.FromResult(true);
        }

        public Task OpenStorePageAsync()
        {
            throw new NotImplementedException();
        }

        public Task PurchaseOneTimeAsync()
        {
            throw new NotImplementedException();
        }

        public Task RequestRefundAsync()
        {
            throw new NotImplementedException();
        }

        public Task RequestReviewAsync()
        {
            SKStoreReviewController.RequestReview();
            return Task.CompletedTask;
        }

        public Task SubscribeToPremiumAsync()
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeFromPremiumAsync()
        {
            throw new NotImplementedException();
        }
    }
}