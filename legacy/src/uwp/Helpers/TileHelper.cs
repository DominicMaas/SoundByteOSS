using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    /// Common methods to aid in live tile creation
    /// </summary>
    public static class TileHelper
    {
        /// <summary>
        /// Setup the tile updater
        /// </summary>
        public static void Init()
        {
            if (DeviceHelper.IsXbox)
                return;

            // Setup the tile updaters
            TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication("App");
            TileUpdater.EnableNotificationQueue(true);
        }

        public static TileUpdater TileUpdater { get; set; }

        public static async Task<bool> RemoveTileAsync(string tileId)
        {
            var task = (await SecondaryTile.FindAllAsync()).FirstOrDefault(x => x.TileId == tileId)?.RequestDeleteAsync();

            if (task != null)
                return await task;

            return false;
        }

        /// <summary>
        ///     Removes all live tiles from the start menu
        ///     or start screen
        /// </summary>
        public static async Task RemoveAllTilesAsync()
        {
            // Find all the tiles and loop though them all
            foreach (var tile in await SecondaryTile.FindAllAsync())
                // Request a tile delete
                await tile.RequestDeleteAsync();
        }

        /// <summary>
        /// Is the tile pinned to the start menu
        /// </summary>
        /// <param name="tileId"></param>
        /// <returns></returns>
        public static bool IsTilePinned(string tileId)
        {
            // We do not support tile pinning on Xbox.
            if (DeviceHelper.IsXbox)
                return false;

            return SecondaryTile.Exists(tileId);
        }

        /// <summary>
        ///     Creates a tile and pins it to the users screen
        /// </summary>
        /// <param name="tileId">The ID for the tile</param>
        /// <param name="tileTitle">The title that will appear on the tile</param>
        /// <param name="tileParam">Any params that will be passed to the app on launch</param>
        /// <param name="tileImage">Uri to image for the background</param>
        /// <param name="tileForeground">Text to display on tile</param>
        /// <returns></returns>
        public static async Task<bool> CreateTileAsync(string tileId, string tileTitle, string tileParam, Uri tileImage,
            ForegroundText tileForeground)
        {
            // Check if the tile already exists
            if (IsTilePinned(tileId))
                return false;

            await NavigationService.Current.CallDialogAsync<PinTileDialog>(tileId, tileTitle, tileParam, tileImage);
            return true;
        }
    }
}