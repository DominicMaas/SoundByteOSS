using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;
using System;
using SoundByte.Core.Converters;

namespace SoundByte.App.iOS.Cells.Media.List
{
    public class PlaylistListCell : MediaListCell
    {
        public static readonly NSString Key = new NSString("PlaylistListCell");

        public PlaylistListCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<PlaylistListCell, Playlist>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_rightFooter).WithConversion<ReadableDurationConverter>().To(vm => vm.Duration);
                set.Bind(_subtitle).To(vm => vm.User.Username);
                set.Apply();
            });

            _leftFooter.Text = "\uf89f";

            CreateGradientLayer();
        }
    }
}