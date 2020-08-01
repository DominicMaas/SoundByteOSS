using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Used for displaying platform specific dialogs / sheets
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        ///     Shows an error message to the user
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="body">The body of the message</param>
        Task ShowErrorMessageAsync(string title, string body);

        /// <summary>
        ///     Shows an information message to the user
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="body">The body of the message</param>
        Task ShowInfoMessageAsync(string title, string body);

        /// <summary>
        ///     Shows a dialog of a certain type
        /// </summary>
        /// <typeparam name="T">The dialog type</typeparam>
        /// <param name="param">Any parameters that this dialog requires</param>
        Task ShowDialogAsync<T>(params object[] param);
    }
}