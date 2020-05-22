using System;

namespace SoundByte.Core.Extension.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ApiParameterMetadataAttribute : Attribute
    {
        public string Name { get; }

        public string Description { get; }

        public string[] ChildrenNames { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="childrenNames">Used for actions and functions to give the parameters names in the typescript definition file.</param>
        public ApiParameterMetadataAttribute(string name, string description, string[] childrenNames = null)
        {
            Name = name;
            Description = description;
            ChildrenNames = childrenNames;
        }
    }
}