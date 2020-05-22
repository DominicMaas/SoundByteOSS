using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.Extension;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Extension
{
    public interface IExtensionContent
    {
        void RegisterContentSection(ContentSection contentSection);
    }
}