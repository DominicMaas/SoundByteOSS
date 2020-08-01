using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Converters;
using SoundByte.Core.Models.Media;
using System;
using UIKit;

namespace SoundByte.App.iOS.Cells.Media.List
{
    public class TrackListCell : MediaListCell
    {
        public static readonly NSString Key = new NSString("TrackListCell");

        public TrackListCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<TrackListCell, Track>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_subtitle).To(vm => vm.User.Username);
                set.Bind(_rightFooter).WithConversion<ReadableDurationConverter>().To(vm => vm.Duration);
                set.Apply();
            });

            _leftFooter.Text = "\uf8cf";

            CreateGradientLayer();
        }
    }
}