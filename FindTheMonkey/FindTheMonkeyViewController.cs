using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreLocation;
using MonoTouch.CoreFoundation;
using MonoTouch.AVFoundation;

namespace FindTheMonkey
{
	public partial class FindTheMonkeyViewController : UIViewController
	{
		static readonly string uuid = "E4C8A4FC-F68B-470D-959F-29382AF72CE7";
		static readonly string monkeyId = "Monkey";

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		CBPeripheralManager peripheralMgr;
		BTPeripheralDelegate peripheralDelegate;
		CLLocationManager locationMgr;
		CLProximity previousProximity;

		float volume = 0.5f;
		float pitch = 1.0f;

		public FindTheMonkeyViewController () : base (UserInterfaceIdiomIsPhone ? "FindTheMonkeyViewController_iPhone" : "FindTheMonkeyViewController_iPad", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var monkeyUUID = new NSUuid (uuid);
			var beaconRegion = new CLBeaconRegion (monkeyUUID, monkeyId);

			beaconRegion.NotifyEntryStateOnDisplay = true;
			beaconRegion.NotifyOnEntry = true;
			beaconRegion.NotifyOnExit = true;

			if (!UserInterfaceIdiomIsPhone) {

				//power - the received signal strength indicator (RSSI) value (measured in decibels) of the beacon from one meter away
				var power = new NSNumber (-59);
				NSMutableDictionary peripheralData = beaconRegion.GetPeripheralData (power);
				peripheralDelegate = new BTPeripheralDelegate ();
				peripheralMgr = new CBPeripheralManager (peripheralDelegate, DispatchQueue.DefaultGlobalQueue);

				peripheralMgr.StartAdvertising (peripheralData);

			} else {

				InitPitchAndVolume ();

				locationMgr = new CLLocationManager ();

				locationMgr.RegionEntered += (object sender, CLRegionEventArgs e) => {
					if (e.Region.Identifier == monkeyId) {
						UILocalNotification notification = new UILocalNotification () { AlertBody = "There's a monkey hiding nearby!" };
						UIApplication.SharedApplication.PresentLocationNotificationNow (notification);
					}
				};

				locationMgr.DidRangeBeacons += (object sender, CLRegionBeaconsRangedEventArgs e) => {
					if (e.Beacons.Length > 0) {

						CLBeacon beacon = e.Beacons [0];
						string message = "";

						switch (beacon.Proximity) {
						case CLProximity.Immediate:
							message = "You found the monkey!";
							monkeyStatusLabel.Text = message;
							View.BackgroundColor = UIColor.Green;
							break;
						case CLProximity.Near:
							message = "You're getting warmer";
							monkeyStatusLabel.Text = message;
							View.BackgroundColor = UIColor.Yellow;
							break;
						case CLProximity.Far:
							message = "You're freezing cold";
							monkeyStatusLabel.Text = message;
							View.BackgroundColor = UIColor.Blue;
							break;
						case CLProximity.Unknown:
							message = "I'm not sure how close you are to the monkey";;
							monkeyStatusLabel.Text = message;
							View.BackgroundColor = UIColor.Gray;
							break;
						}

						if(previousProximity != beacon.Proximity){
							Speak (message);
						}
						previousProximity = beacon.Proximity;
					}
				};

				locationMgr.StartMonitoring (beaconRegion);
				locationMgr.StartRangingBeacons (beaconRegion);
			}
		}

		void Speak (string text)
		{
			var speechSynthesizer = new AVSpeechSynthesizer ();

//			var voices = AVSpeechSynthesisVoice.GetSpeechVoices ();

			var speechUtterance = new AVSpeechUtterance (text) {
				Rate = AVSpeechUtterance.MaximumSpeechRate/4,
				Voice = AVSpeechSynthesisVoice.FromLanguage ("en-AU"),
				Volume = volume,
				PitchMultiplier = pitch
			};

			speechSynthesizer.SpeakUtterance (speechUtterance);
		}

		void InitPitchAndVolume ()
		{
			volumeSlider.MinValue = 0;
			volumeSlider.MaxValue = 1.0f;
			volumeSlider.SetValue (volume, false);

			pitchSlider.MinValue = 0.5f;
			pitchSlider.MaxValue = 2.0f;
			pitchSlider.SetValue (pitch, false);

			volumeSlider.ValueChanged += (sender, e) => {
				volume = volumeSlider.Value;
			};

			pitchSlider.ValueChanged += (sender, e) => {
				pitch = volumeSlider.Value;
			};
		}

		class BTPeripheralDelegate : CBPeripheralManagerDelegate
		{
			public override void StateUpdated (CBPeripheralManager peripheral)
			{
				if (peripheral.State == CBPeripheralManagerState.PoweredOn) {
					Console.WriteLine ("powered on");
				}
			}
		}
	}
}

