using SoundByte.Core.Models.Extension;
using System;

namespace SoundByte.Core.Extension.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ApiMetadataAttribute : Attribute
    {
        public ApiVersion Version { get; }

        public string Description { get; }

        public ApiMetadataAttribute(ApiVersion version, string description)
        {
            Version = version;
            Description = description;
        }
    }
}