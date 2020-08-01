using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoundByte.Core.Models
{
    /// <summary>
    ///     Class that all UI aware models extend from. Allows binding updates within
    ///     models on platforms such as UWP.
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            UpdateProperty(propertyName);
            return true;
        }

        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessors of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Property Changed Event Handlers
    }
}