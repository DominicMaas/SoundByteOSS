using System;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    ///     Used for exception handling within the app. Supports an
    ///     error title and message.
    /// </summary>
    public class SoundByteException : Exception
    {
        public SoundByteException(string title, string description, Exception innerException = null) : base(
            string.Format("Title: {0}, Description: {1}", title, description), innerException)
        {
            ErrorTitle = title;
            ErrorDescription = description;
        }

        /// <summary>
        ///     Title of the error message
        /// </summary>
        public string ErrorTitle { get; }

        /// <summary>
        ///     A description of the error message
        /// </summary>
        public string ErrorDescription { get; }
    }
}