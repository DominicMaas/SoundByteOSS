using MvvmCross.Presenters.Attributes;
using System;

namespace SoundByte.App.UWP.Presenters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoundByteTabAttribute : MvxBasePresentationAttribute
    {
        public SoundByteTabAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; set; }
    }
}