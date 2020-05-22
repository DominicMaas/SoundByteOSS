using System;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    ///     Generic exception called when trying to access resources before the SoundByte
    ///     Service has loaded.
    /// </summary>
    [Serializable]
    public class SoundByteNotLoadedException : Exception
    {
        public SoundByteNotLoadedException() : base("The SoundByte Service has not been loaded. Cannot continue.")
        { }
    }
}