using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using EarthExplorer.Core;
using Android.Content;

namespace EarthExplorer.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        List<PointOfInterest> Datasource;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            ListView listView = FindViewById<ListView>(Resource.Id.listView1);

            listView.ItemClick += ListView_ItemClick;

            Datasource = await PointOfInterest.GetGlobalListAsync();
            listView.Adapter = new POIAdapter(this, Datasource);
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var poi = Datasource[(int)e.Id];

            var geoUri = Android.Net.Uri.Parse($"geo:{poi.Latitude},{poi.Longitude}");
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            StartActivity(mapIntent);
        }
    }
}