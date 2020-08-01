using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;
using System;
using SoundByte.Core.Converters;

namespace SoundByte.App.iOS.Cells.Media.Grid
{
    public class PlaylistGridCell : MediaGridCell
    {
        public static readonly NSString Key = new NSString("PlaylistGridCell");

        public PlaylistGridCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<PlaylistGridCell, Playlist>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_subtitle).To(vm => vm.User.Username);
                set.Bind(_rightFooter).WithConversion<ReadableDurationConverter>().To(vm => vm.Duration);
                set.Bind(_alertController).For(x => x.Title).To(vm => vm.Title);
                set.Apply();
            });

            _leftFooter.Text = "\uf89f";

            CreateGradientLayer();
        }
    }
}