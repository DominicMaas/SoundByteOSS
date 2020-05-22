namespace SoundByte.Core.Items.User
{
    /// <summary>
    ///     Extend custom service user classes
    ///     off of this interface.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        ///     Convert the service specific track implementation to a
        ///     universal implementation. Overide this method and provide
        ///     the correct mapping between the service specific and universal
        ///     classes.
        /// </summary>
        /// <returns>A base user item.</returns>
        BaseUser ToBaseUser();
    }
}