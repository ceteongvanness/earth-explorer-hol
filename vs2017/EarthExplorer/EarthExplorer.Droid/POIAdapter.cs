using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using EarthExplorer.Core;

namespace EarthExplorer.Droid
{
    public class POIAdapter : BaseAdapter<PointOfInterest>
    {
        PointOfInterest[] items;
        Activity context;
        public POIAdapter(Activity context, List<PointOfInterest> items) : base()
        {
            this.context = context;
            this.items = items.ToArray();
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override PointOfInterest this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
            return view;
        }
    }

}