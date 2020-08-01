using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.Converters;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.Content;
using System;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Cells
{
    public partial class ContentGroupCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ContentGroupCell");
        public static readonly UINib Nib;
        public static readonly float CellHeight = 290.0f;

        static ContentGroupCell()
        {
            Nib = UINib.FromName("ContentGroupCell", NSBundle.MainBundle);
        }

        protected ContentGroupCell(IntPtr handle) : base(handle)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.DelayBind(() =>
            {
                var source = new MediaGridViewSource(CollectionView);

                var set = (this).CreateBindingSet<ContentGroupCell, ContentGroup>();
                set.Bind(TitleLabel).To(vm => vm.Title);
                set.Bind(ViewAllButton).To(vm => vm.OnViewAllClickCommand);
                set.Bind(source).For(x => x.ItemsSource).To(vm => vm.Collection);
                set.Bind(source).For(x => x.SelectionChangedCommand).To(vm => vm.OnItemClickCommand);
                set.Bind(MessageTitleLabel).For(x => x.Text).To(vm => vm.Collection.ErrorHeader);
                set.Bind(MessageDescriptionLabel).For(x => x.Text).To(vm => vm.Collection.ErrorDescription);
                set.Bind(IsLoadingIndicator).For(x => x.Hidden).WithConversion<InverseBooleanConverter>().To(vm => vm.Collection.IsLoading);
                set.Bind(IsLoadingIndicator).For(x => x.IsAnimating).To(vm => vm.Collection.IsLoading);
                set.Apply();

                ViewAllButton.SetTitleColor(ColorHelper.Accent.ToPlatformColor(), UIControlState.Normal);
                TitleLabel.TextColor = ColorHelper.Text0.ToPlatformColor();
                IsLoadingIndicator.Color = ColorHelper.Text0.ToPlatformColor();

                CollectionView.Source = source;
                CollectionView.ReloadData();
            });
        }
    }
}