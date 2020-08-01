using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;
using System;

namespace SoundByte.App.iOS.Cells.Media.Grid
{
    public class UserGridCell : MediaGridCell
    {
        public static readonly NSString Key = new NSString("UserGridCell");

        public UserGridCell(IntPtr handle) : base(handle, 80.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<UserGridCell, User>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Username);
                set.Bind(_subtitle).To(vm => vm.Username);
                set.Bind(_alertController).For(x => x.Title).To(vm => vm.Username);
                set.Apply();
            });
        }
    }
}