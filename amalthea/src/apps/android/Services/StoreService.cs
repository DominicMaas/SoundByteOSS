using SoundByte.Core.Services.Definitions;
using System;
using System.Threading.Tasks;

namespace SoundByte.App.Android.Services
{
    public class StoreService : IStoreService
    {
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> HasPremiumAsync()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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