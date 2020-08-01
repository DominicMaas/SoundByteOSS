using SoundByte.Core.Models.Authentication;
using System;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     used to authenticate with different music provider services
    ///     and SoundByte accounts
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        ///     Exchanges a code for a token
        /// </summary>
        /// <param name="code">The code to exchange</param>
        /// <param name="id">Music provider id</param>
        /// <returns></returns>
        Task<Token> ExchangeCodeForTokenAsync(string code, Guid? id);

        /// <summary>
        ///     Refreshes a token
        /// </summary>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="id">Music provider id</param>
        /// <returns></returns>
        Task<Token> RefreshTokenAsync(string refreshToken, Guid? id);

        /// <summary>
        ///     Connect a music provider to SoundByte
        /// </summary>
        /// <param name="id">Music provider id</param>
        /// <returns></returns>
        Task ConnectAccountAsync(Guid id, Token token);

        /// <summary>
        ///     Connect the users SoundByte Account.
        /// </summary>
        /// <param name="token"></param>
        Task ConnectSoundByteAccountAsync(Token token);

        /// <summary>
        ///     Disconnect a music provider from SoundByte.
        /// </summary>
        /// <param name="id">Music provider id</param>
        Task DisconnectAccountAsync(Guid id);

        /// <summary>
        ///     Disconnect the users SoundByte Account.
        /// </summary>
        Task DisconnectSoundByteAccountAsync();

        /// <summary>
        ///     Get the stored login token for this music provider
        /// </summary>
        /// <param name="id">Music provider id</param>
        /// <returns>The login token (or null if none exists)</returns>
        Task<Token> GetTokenAsync(Guid id);

        /// <summary>
        ///     Get the SoundByte account token
        /// </summary>
        /// <returns></returns>
        Task<Token> GetSoundByteAccountTokenAsync();

        /// <summary>
        ///     Is the specified music provider account connected.
        /// </summary>
        /// <param name="id">Music provider id</param>
        /// <returns>If the music provider account is connected.</returns>
        Task<bool> IsAccountConnectedAsync(Guid id);

        /// <summary>
        ///     Is the users SoundByte account currently
        ///     connected to the application.
        /// </summary>
        /// <returns>If the users SoundByte account is connected.</returns>
        Task<bool> IsSoundByteAccountConnectedAsync();
    }
}