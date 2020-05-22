using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Models.Content
{
    public class ContentSection
    {
        public string Title { get; set; }

        public ContentArea Area { get; set; }

        public Func<int, string, Dictionary<string, object>, SourceResponse> OnGet { get; set; }

        public ContentSection(ContentArea area, string title)
        {
            Area = area;
            Title = title;
        }
    }
}