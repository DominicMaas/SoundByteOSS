using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Sources;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Helpers
{
    public class SoundByteCollection<TSource> : ObservableCollection<BaseSoundByteItem>, ISupportIncrementalLoading where TSource : ISource
    {
        #region Private Methods

        private CancellationToken _cancellationToken;
        private bool _refreshOnLoad;

        #endregion Private Methods

        #region Getters and Setters

        /// <summary>
        ///     Source object telling the collection how to get the next
        ///     set of items.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        ///     Current token for the next request. Will be eol if
        ///     no more items are avaliable.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Are there any more items in the collection
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                if (_cancellationToken.IsCancellationRequested)
                    return false;

                return Token != "eol";
            }
        }

        #endregion Getters and Setters

        #region UI Bindings

        /// <summary>
        /// Is this model currently loading new items
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                UpdateProperty();
            }
        }

        private bool _isLoading;

        /// <summary>
        /// Does this model have an error
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set
            {
                if (_isError == value)
                    return;

                _isError = value;
                UpdateProperty();
            }
        }

        private bool _isError;

        /// <summary>
        /// If there is an error, this is the title of the error
        /// </summary>
        public string ErrorHeader
        {
            get => _errorHeader;
            set
            {
                if (_errorHeader == value)
                    return;

                _errorHeader = value;
                UpdateProperty();
            }
        }

        private string _errorHeader;

        /// <summary>
        /// If there is an error, this is the description of the error
        /// </summary>
        public string ErrorDescription
        {
            get => _errorDescription;
            set
            {
                if (_errorDescription == value)
                    return;

                _errorDescription = value;
                UpdateProperty();
            }
        }

        private string _errorDescription;

        #endregion UI Bindings

        #region Constructors

        public SoundByteCollection() : this(Activator.CreateInstance<TSource>())
        { }

        public SoundByteCollection(TSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        #endregion Constructors

        #region Load More Items Method

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            // Don't load items if currently loading
            if (IsLoading)
                return new LoadMoreItemsResult();

            // Lower limit of 10 items
            if (count <= 10)
                count = 10;

            // Upper limit of 50 items
            if (count >= 50)
                count = 50;

            // The amount of new items added
            var addedCount = 0;

            try
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    // Show the loading bar
                    IsError = false;              

                    // Force the UI to update if the first items in the list
                    if (Count == 0)
                        IsLoading = true;
                    else
                        _isLoading = true;

                    // Try get some more items
                    var response = await Task.Run(async () => await Source.GetItemsAsync((int)count, Token, cancellationToken), cancellationToken);

                    // If not successful, show error
                    // and return.
                    if (!response.IsSuccess)
                    {
                        // This is a little cheat way to make sure the error messages don't overlap content.
                        // As long as items are displayed, don't worry about errors.
                        if (Count == 0)
                            await ShowErrorMessageAsync(response.Messages?.MessageTitle, response.Messages?.MessageContent);

                        Token = "eol";
                        return new LoadMoreItemsResult { Count = 0 };
                    }

                    // Set the new token
                    Token = string.IsNullOrEmpty(response.Token) ? "eol" : response.Token;

                    // Set the amount of items added
                    addedCount = response.Items.Count();

                    // Add items to list
                    foreach (var item in response.Items)
                    {
                        Add(item);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // The operation has been canceled using the Cancellation Token.
            }
            catch (SoundByteException ex)
            {
                Token = "eol";

                await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);
            }
            finally
            {
                IsLoading = false;

                if (_refreshOnLoad)
                {
                    _refreshOnLoad = false;
                    await RefreshItemsAsync();
                }
            }

            return new LoadMoreItemsResult { Count = (uint)addedCount };
        }

        #endregion Load More Items Method

        #region Methods

        /// <summary>
        ///     Refresh the list by removing any
        ///     existing items and reseting the token.
        /// </summary>
        public Task RefreshItemsAsync()
        {
            if (IsLoading)
            {
                _refreshOnLoad = true;
            }
            else
            {
                var previousCount = Count;
                Clear();
                Token = string.Empty;

                if (previousCount == 0)
                {
                    // When the list was empty before clearing, the automatic reload isn't fired, so force a reload.
                    return LoadMoreItemsAsync(0).AsTask();
                }
            }

            return Task.CompletedTask;
        }

        public void RefreshItems()
        {
            RefreshItemsAsync();
        }

        protected async Task ShowErrorMessageAsync(string title, string description)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                IsError = true;
                ErrorHeader = title;
                ErrorDescription = description;
            });
        }

        #endregion Methods

        #region Internal Methods

        private void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion Internal Methods
    }
}