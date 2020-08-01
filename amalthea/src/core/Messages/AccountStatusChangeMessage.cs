using MvvmCross.Plugin.Messenger;

namespace SoundByte.Core.Messages
{
    /// <summary>
    ///     A message that is sent when an account status changed
    ///     e.g. connecting or disconnecting accounts
    /// </summary>
    public class AccountStatusChangeMessage : MvxMessage
    {
        public AccountStatusChangeMessage(object sender) : base(sender)
        { }
    }
}