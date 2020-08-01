using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;

namespace SoundByte.App.iOS.Cells.Media.List
{
    public class PodcastShowListCell : MediaListCell
    {
        public static readonly NSString Key = new NSString("PodcastShowListCell");

        public PodcastShowListCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<PodcastShowListCell, PodcastShow>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ImageUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_subtitle).To(vm => vm.Author);
                set.Apply();
            });

            _leftFooter.Text = "\uf2ce";

            CreateGradientLayer();
        }
    }
}