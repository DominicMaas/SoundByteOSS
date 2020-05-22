using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoundByte.Core.Items
{
    /// <summary>
    ///     A base items that all base{type}s extend off. e.g BaseTrack.cs or BaseUser.cs
    /// </summary>
    public class BaseItem : INotifyPropertyChanged
    {
        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Property Changed Event Handlers
    }
}