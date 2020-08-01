using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Converters;
using SoundByte.Core.Models.Media;
using System;
using UIKit;

namespace SoundByte.App.iOS.Cells.Media.Grid
{
    public class TrackGridCell : MediaGridCell
    {
        public static readonly NSString Key = new NSString("TrackGridCell");

        public TrackGridCell(IntPtr handle) : base(handle, 6.0f)
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<TrackGridCell, Track>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Title);
                set.Bind(_subtitle).To(vm => vm.User.Username);
                set.Bind(_rightFooter).WithConversion<ReadableDurationConverter>().To(vm => vm.Duration);
                set.Bind(_alertController).For(x => x.Title).To(vm => vm.Title);
                set.Apply();
            });

            _leftFooter.Text = "\uf8cf";

            CreateGradientLayer();

            // Context menu
            _alertController.AddAction(UIAlertAction.Create("Play track", UIAlertActionStyle.Default, null));
            _alertController.AddAction(UIAlertAction.Create("Add to queue", UIAlertActionStyle.Default, null));
            _alertController.AddAction(UIAlertAction.Create("Share track", UIAlertActionStyle.Default, null));
            _alertController.AddAction(UIAlertAction.Create("Add to playlist", UIAlertActionStyle.Default, null));
        }
    }
}