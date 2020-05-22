using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Extension;
using SoundByte.Core.Models.Content;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    public class ExtensionContent : IExtensionContent
    {
        public void AddContentGroup(ContentArea location, string title, ContentButton[] buttons, Action<ContentGroup> onClickAction, Func<int, string, Dictionary<string, object>, SourceResponse> execute)
        {
            //  var source = new ExtensionSource(execute);
            //  var contentGroup = new ContentGroup(source, title, buttons?.ToList(), onClickAction);
            //   SimpleIoc.Default.GetInstance<IContentService>().AddContent(location, contentGroup);
        }

        public void RegisterContentSection(ContentSection contentSection)
        {
            //  var source = new ExtensionSource(contentSection.OnGet);
            //   var contentGroup = new ContentGroup(source, contentSection.Title, null, null);
            //  SimpleIoc.Default.GetInstance<IContentService>().AddContent(contentSection.Area, contentGroup);
        }
    }
}