// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SoundByte.App.iOS.Cells
{
    [Register ("ContentGroupCell")]
    partial class ContentGroupCell
    {
        [Outlet]
        UIKit.UICollectionView CollectionView { get; set; }


        [Outlet]
        UIKit.UIButton ViewAllButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView IsLoadingIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MessageDescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MessageTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (IsLoadingIndicator != null) {
                IsLoadingIndicator.Dispose ();
                IsLoadingIndicator = null;
            }

            if (MessageDescriptionLabel != null) {
                MessageDescriptionLabel.Dispose ();
                MessageDescriptionLabel = null;
            }

            if (MessageTitleLabel != null) {
                MessageTitleLabel.Dispose ();
                MessageTitleLabel = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}