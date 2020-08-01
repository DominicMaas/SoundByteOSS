using AppKit;
using Foundation;
using MvvmCross.Platforms.Mac.Core;

namespace SoundByte.App.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate<Setup, Core.App>
    {
        public override void DidFinishLaunching(NSNotification notification)
        {
            MvxMacSetupSingleton.EnsureSingletonAvailable(this, MainWindow).EnsureInitialized();
            RunAppStart();
        }
    }
}