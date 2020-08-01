using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;
using System;

namespace SoundByte.App.iOS.Cells.Media.Grid
{
    public class PodcastShowGridCell : MediaGridCell
    {
        public static readonly NSString Key = new NSString("PodcastShowGridCell");

        public PodcastShowGridCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<PodcastShowGridCell, PodcastShow>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ImageUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_subtitle).To(vm => vm.Author);
                set.Bind(_alertController).For(x => x.Title).To(vm => vm.Title);
                set.Apply();
            });

            _leftFooter.Text = "\uf2ce";

            CreateGradientLayer();
        }
    }
}