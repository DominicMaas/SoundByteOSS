using System;
using System.Collections.Generic;

namespace SoundByte.Core.Models.Local
{
    /// <summary>
    ///     Represents a local music library
    ///     on the users device
    /// </summary>
    public class LocalLibrary
    {
        /// <summary>
        ///     Unique id for this library
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The user readable/editable name of this library
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The location of this folder on the file system
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     Sand-boxed operating systems will have a handle
        ///     on the folder that lets the application access the folder
        ///     at a later time.
        /// </summary>
        public string FileSystemHandle { get; set; }

        /// <summary>
        ///     The files in this library
        /// </summary>
        public List<LocalFile> Files { get; set; }
    }
}