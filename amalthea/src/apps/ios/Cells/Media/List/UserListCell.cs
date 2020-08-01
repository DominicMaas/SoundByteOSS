using Foundation;
using MvvmCross.Binding.BindingContext;
using SoundByte.Core.Models.Media;
using System;

namespace SoundByte.App.iOS.Cells.Media.List
{
    public class UserListCell : MediaListCell
    {
        public static readonly NSString Key = new NSString("UserListCell");

        public UserListCell(IntPtr handle) : base(handle, 80.0f)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var set = (this).CreateBindingSet<UserListCell, User>();
                set.Bind(_imageControl).For(x => x.ImagePath).To(vm => vm.ArtworkUrl);
                set.Bind(_title).To(vm => vm.Username);
                set.Bind(_subtitle).To(vm => vm.Username);
                set.Apply();
            });
        }
    }
}