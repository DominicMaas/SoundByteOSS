using SoundByte.Core.Models.Media;
using System;

namespace SoundByte.Core.Models.Local
{
    public class LocalFile
    {
        public Guid Id { get; set; }

        /// <summary>
        ///     Where in the library this file is located, useful
        ///     for updating the library at a later stage.
        /// </summary>
        public string Location { get; set; }

        public Track Track { get; set; }
    }
}