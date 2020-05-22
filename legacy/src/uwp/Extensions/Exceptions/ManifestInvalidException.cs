using SoundByte.App.Uwp.Extensions.Core;
using System;

namespace SoundByte.App.Uwp.Extensions.Exceptions
{
    public class ManifestInvalidException : Exception
    {
        public InvalidManifestError Error { get; private set; }

        public ManifestInvalidException(InvalidManifestError error, Exception ex = null) : base(error.ToString(), ex)
        {
            Error = error;
        }
    }
}