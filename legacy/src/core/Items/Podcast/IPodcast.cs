namespace SoundByte.Core.Items.Podcast
{
    /// <summary>
    ///     Extend custom service podcast classes
    ///     off of this interface.
    /// </summary>
    public interface IPodcast
    {
        /// <summary>
        ///     Convert the service specific podcast implementation to a
        ///     universal implementation. Overide this method and provide
        ///     the correct mapping between the service specific and universal
        ///     classes.
        ///     </summary>
        /// <returns>A base podcast item.</returns>
        BasePodcast ToBasePodcast();
    }
}