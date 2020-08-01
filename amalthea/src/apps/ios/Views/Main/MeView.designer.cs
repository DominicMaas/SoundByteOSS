// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SoundByte.App.iOS.Views.Main
{
    [Register ("MeView")]
    partial class MeView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BuildDateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SoundByteAccountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel VersionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BuildDateLabel != null) {
                BuildDateLabel.Dispose ();
                BuildDateLabel = null;
            }

            if (SoundByteAccountLabel != null) {
                SoundByteAccountLabel.Dispose ();
                SoundByteAccountLabel = null;
            }

            if (VersionLabel != null) {
                VersionLabel.Dispose ();
                VersionLabel = null;
            }
        }
    }
}