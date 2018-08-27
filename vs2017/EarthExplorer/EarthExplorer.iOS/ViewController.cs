using CoreLocation;
using EarthExplorer.Core;
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace EarthExplorer.iOS
{
    public partial class ViewController : UIViewController
    {
        CLLocationManager LocationManager;
        ManualResetEvent waitEvent = new ManualResetEvent(false);
        PointOfInterest currentPOI = new PointOfInterest();

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public async override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
            LocationManager = new CLLocationManager();

            LocationManager.RequestWhenInUseAuthorization();

            LocationManager.DistanceFilter = CLLocationDistance.FilterNone;
            LocationManager.DesiredAccuracy = 1000;
            LocationManager.LocationsUpdated += LocationManager_LocationsUpdated;
            LocationManager.StartUpdatingLocation();

            PointOfInterest.GetCurrentPOIAsync = async () =>
            {
                await Task.Run(() => waitEvent.WaitOne());
                return currentPOI;
            };

            var source = new TableSource(await PointOfInterest.GetGlobalListAsync());

            source.OnClick += Source_OnClick;

            MyTable.Source = source;
            MyTable.ReloadData();
        }

        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            LocationManager.StopUpdatingLocation();
            currentPOI.Name = "Exactly here...";
            currentPOI.Latitude = e.Locations[0].Coordinate.Latitude;
            currentPOI.Longitude = e.Locations[0].Coordinate.Longitude;
            waitEvent.Set();
        }

        private void Source_OnClick(PointOfInterest poi)
        {
            var coords = new CLLocationCoordinate2D(poi.Latitude, poi.Longitude);

            MyMap.Region = new MapKit.MKCoordinateRegion(coords, new MapKit.MKCoordinateSpan(0.1, 0.1));
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}