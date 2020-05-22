namespace SoundByte.Core.Items.Playlist
{
    /// <summary>
    ///     Extend custom service playlist classes
    ///     off of this interface.
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        ///     Convert the service specific playlist implementation to a
        ///     universal implementation. Overide this method and provide
        ///     the correct mapping between the service specific and universal
        ///     classes.
        ///     </summary>
        /// <returns>A base playlist item.</returns>
        BasePlaylist ToBasePlaylist();
    }
}