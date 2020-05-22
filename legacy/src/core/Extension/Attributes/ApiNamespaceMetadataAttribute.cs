using SoundByte.Core.Models.Extension;
using System;

namespace SoundByte.Core.Extension.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ApiNamespaceMetadataAttribute : Attribute
    {
        public ApiVersion Version { get; }

        public string Description { get; }

        public ApiNamespaceMetadataAttribute(ApiVersion version, string description)
        {
            Version = version;
            Description = description;
        }
    }
}