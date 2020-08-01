using MvvmCross.Presenters.Attributes;
using System;

namespace SoundByte.App.UWP.Presenters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoundByteModalAttribute : MvxBasePresentationAttribute
    {
        public SoundByteModalAttribute(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
    }
}