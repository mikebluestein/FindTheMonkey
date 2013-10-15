// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace FindTheMonkey
{
	[Register ("FindTheMonkeyViewController")]
	partial class FindTheMonkeyViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView monkeyImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel monkeyStatusLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton openMultipeerBrowser { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider pitchSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider volumeSlider { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (monkeyImageView != null) {
				monkeyImageView.Dispose ();
				monkeyImageView = null;
			}

			if (monkeyStatusLabel != null) {
				monkeyStatusLabel.Dispose ();
				monkeyStatusLabel = null;
			}

			if (pitchSlider != null) {
				pitchSlider.Dispose ();
				pitchSlider = null;
			}

			if (volumeSlider != null) {
				volumeSlider.Dispose ();
				volumeSlider = null;
			}

			if (openMultipeerBrowser != null) {
				openMultipeerBrowser.Dispose ();
				openMultipeerBrowser = null;
			}
		}
	}
}
