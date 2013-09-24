using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreLocation;
using MonoTouch.CoreFoundation;

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
			NSUuid monkeyUUID = new NSUuid (uuid);
			CLBeaconRegion beaconRegion = new CLBeaconRegion (monkeyUUID, monkeyId);

			beaconRegion.NotifyEntryStateOnDisplay = true;
			beaconRegion.NotifyOnEntry = true;
			beaconRegion.NotifyOnExit = true;

			if (!UserInterfaceIdiomIsPhone) {

				//power - the received signal strength indicator (RSSI) value (measured in decibels) of the beacon from one meter away
				NSNumber power = new NSNumber (-59);
				NSMutableDictionary peripheralData = beaconRegion.GetPeripheralData (power);
				peripheralDelegate = new BTPeripheralDelegate ();
				peripheralMgr = new CBPeripheralManager (peripheralDelegate, DispatchQueue.DefaultGlobalQueue);

				peripheralMgr.StartAdvertising (peripheralData);

			} else {

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

						switch (beacon.Proximity) {
						case CLProximity.Immediate:
							monkeyStatusLabel.Text = "You found the monkey!";
							View.BackgroundColor = UIColor.Green;
							break;
						case CLProximity.Near:
							monkeyStatusLabel.Text = "You're getting warmer";
							View.BackgroundColor = UIColor.Yellow;
							break;
						case CLProximity.Far:
							monkeyStatusLabel.Text = "You're freezing cold";
							View.BackgroundColor = UIColor.Blue;
							break;
						case CLProximity.Unknown:
							monkeyStatusLabel.Text = "I'm not sure how close you are to the monkey";
							View.BackgroundColor = UIColor.Gray;
							break;
						}
					}
				};

				locationMgr.StartMonitoring (beaconRegion);
				locationMgr.StartRangingBeacons (beaconRegion);
			}
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

