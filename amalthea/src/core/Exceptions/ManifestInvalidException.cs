using SoundByte.Core.Models.MusicProvider;
using System;

namespace SoundByte.Core.Exceptions
{
    public class ManifestInvalidException : Exception
    {
        public InvalidManifestReason Error { get; private set; }

        public ManifestInvalidException(InvalidManifestReason error, Exception ex = null) : base(error.ToString(), ex)
        {
            Error = error;
        }
    }
}