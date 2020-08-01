using Microsoft.AppCenter.Crashes;
using MvvmCross.ViewModels;
using SoundByte.Core.Sources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Models.Core
{
    public class SoundByteCollection<TSource> : MvxObservableCollection<Media.Media> where TSource : ISource
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
        ///     no more items are available.
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
            Source = source;
        }

        #endregion Constructors

        #region Load More Items Method

        public async Task LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            // No more items to fetch
            if (Token == "eol")
                return;

            // Don't load items if currently loading
            if (IsLoading)
                return;

            // Lower limit of 10 items
            if (count <= 10)
                count = 10;

            // Upper limit of 40 items
            if (count >= 40)
                count = 40;

            try
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    // Show the loading bar
                    ShowLoadingMessage();

                    // Try get some more items
                    var response = await Task.Run(async () => await Source.GetItemsAsync((int)count, Token, cancellationToken), cancellationToken);

                    // If not successful, show error
                    // and return.
                    if (!response.Successful)
                    {
                        // This is a little cheat way to make sure the error messages don't overlap content.
                        // As long as items are displayed, don't worry about errors.
                        if (Count == 0)
                            ShowErrorMessage(response.MessageTitle, response.MessageContent);

                        Token = "eol";
                        return;
                    }

                    // Set the new token
                    Token = string.IsNullOrEmpty(response.NextToken) ? "eol" : response.NextToken;

                    // Add items to list
                    foreach (var item in response.Items)
                    {
                        Add(item);
                    }

                    HideLoadingMessage();
                }
            }
            catch (OperationCanceledException)
            {
                // The operation has been canceled using the Cancellation Token.
                HideLoadingMessage();
            }
            catch (Jint.Runtime.JavaScriptException ex)
            {
                Crashes.TrackError(ex);
                ShowErrorMessage("Error running JavaScript", ex.ToString() + " : " + ex.LineNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Crashes.TrackError(ex);
                ShowErrorMessage("Unable to load content", ex.ToString());
            }
            finally
            {
                if (_refreshOnLoad)
                {
                    _refreshOnLoad = false;
                    await RefreshItemsAsync();
                }
            }
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
                Clear();
                Token = string.Empty;

                // Force refresh load
                return LoadMoreItemsAsync(0, _cancellationToken);
            }

            return Task.CompletedTask;
        }

        public void RefreshItems()
        {
            RefreshItemsAsync();
        }

        protected void ShowErrorMessage(string heading, string description)
        {
            IsLoading = false;
            IsError = true;
            ErrorHeader = heading;
            ErrorDescription = description;
        }

        protected void ShowLoadingMessage()
        {
            IsLoading = true;
            IsError = false;
            ErrorHeader = "Loading";
            ErrorDescription = "Fetching new content...";
        }

        protected void HideLoadingMessage()
        {
            IsLoading = false;
            IsError = false;
            ErrorHeader = string.Empty;
            ErrorDescription = string.Empty;
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