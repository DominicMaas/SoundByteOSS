using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Views.Shell
{
    public delegate void BackEventHandler();

    /// <summary>
    ///     Methods that both shells will contain
    /// </summary>
    public interface IAppShell
    {
        /// <summary>
        ///     Link to the main frame
        /// </summary>
        Frame RootFrame { get; }

        BackEventHandler OnBack { get; set; }

        /// <summary>
        ///     Unsubscribe from events on dispose
        /// </summary>
        void Dispose();

        /// <summary>
        ///     Load the application, most work is done in the background
        /// </summary>
        Task PerformWorkAsync();

        /// <summary>
        ///     Wrapper around FindName(string).
        /// </summary>
        /// <param name="name"></param>
        object GetName(string name);
    }
}