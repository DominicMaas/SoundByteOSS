using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.Core.Converters;
using SoundByte.Core.Models.Content;
using System;
using System.ComponentModel;
using UIKit;

namespace SoundByte.App.iOS.Controls
{
    [Register("InfoPane"), DesignTimeVisible(true)]
    public class InfoPane : MvxView
    {
        public InfoPane(IntPtr handle) : base(handle)
        { }

        public InfoPane()
        {
            // Called when created from code
            Initialize();
        }

        public override void AwakeFromNib()
        {
            // Salled when laoded from xib or storyboard
            Initialize();
        }

        private void Initialize()
        {
            // Header
            var header = new UITextField();
            header.Text = "Default Header";
            Add(header);

            // Body
            var body = new UITextField();
            Add(body);

            // Activity Indicator
            var activityIndicator = new UIActivityIndicatorView();
            Add(activityIndicator);

            // Bindings
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<InfoPane, ContentGroup>();
                set.Bind(header).For(x => x.Text).To(vm => vm.Collection.ErrorHeader);
                set.Bind(body).For(x => x.Text).To(vm => vm.Collection.ErrorDescription);
                set.Bind(activityIndicator).For(x => x.Hidden).WithConversion<InverseBooleanConverter>().To(vm => vm.Collection.IsLoading);
            });
        }
    }
}