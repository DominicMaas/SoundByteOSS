namespace SoundByte.Core.Models.Authentication
{
    /// <summary>
    ///     Wrapper around token objects
    /// </summary>
    public class Token
    {
        /// <summary>
        ///     Used to access a secure resource
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        ///     Used to refresh the access token
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}