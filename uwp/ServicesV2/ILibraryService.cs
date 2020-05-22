using System.Threading.Tasks;
using Windows.Storage;

namespace SoundByte.App.Uwp.ServicesV2
{
    /// <summary>
    ///     Interface used for accessing the internal app library where
    ///     local music is saved.
    /// </summary>
    public interface ILibraryService
    {
        /// <summary>
        ///     Start all the media folder watchers, these watchers check
        ///     for any file changes when the app is open and update the internal
        ///     database with these new changes.
        /// </summary>
        Task StartWatchersAsync();

        /// <summary>
        ///     This should be called when the app closes, it unsubscribes from the
        ///     events and closes any watchers.
        /// </summary>
        /// <returns></returns>
        Task StopWatchersAsync();

        /// <summary>
        ///     Add a folder (and it's sub-folders) to the library
        ///     so users can see the contents within the app (and enable the
        ///     watcher to update on any changes).
        /// </summary>
        /// <param name="folder">The folder to add to the library,</param>
        Task AddFolderAsync(StorageFolder folder);

        /// <summary>
        ///     Remove a folder from the users library.
        /// </summary>
        /// <param name="folder">The folder to remove.</param>
        Task RemoveFolderAsync(StorageFolder folder);
    }
}