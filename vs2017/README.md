## Earth explorer hands-on lab

In this hands-on lab, we will be building a [Xamarin](http://xamarin.com) application that will display a list of interest locations and its location on a map. This app will target iOS and Android.

### Get Started

1. Open **Visual Studio 2017**

2. Go to **File** > **New** > **Project** 

3. Navigate to **Other Project Types** > **Visual Studio Solutions** and create a **Blank Solution** called **EarthExplorer**

![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/6157f9da-a9dc-44ec-9870-2150f6975bd1/blank-solution.JPG)

4. We are going to add 3 projects into this solution. 

5. In the Solution Explorer, right click on **Earth Explorer** > **Add** > **New Project**

6. Navigate to **Visual C#** > **.NET Standard** > **Class Library (.NET Standard)**

7. Name this project **EarthExplorer.Core** and click **OK**
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/0d36dba3-5b66-4110-a588-ccc385803a4e/core-project.jpg)

8. In the Solution Explorer right click on **Earth Explorer** > **Add** > **New Project**

9. Navigate to **Visual C#** > **Android** > **Android App (Xamarin)**

10. Name this project **EarthExplorer.Droid** and click **OK**

11. Select **Blank App** and click **OK**

![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/a60d077a-82f8-45e6-a29a-64e7693a4035/create%20android.JPG)

12. Add another project by going to the Solution Explorer. Right click on **Earth Explorer** > **Add** > **New Project**

13. Navigate to **Visual C#** > **iPhone & iPad** > **iOS App(Xamarin)**

14. Name this project **EarthExplorer.iOS** and click **OK**

15. Select **Single View App** and click **OK**

![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/1318ec60-2fc0-4469-b22a-e7a5d6b3b906/create-ios.JPG)

You should see a solution that contains 3 projects as such

* EarthExplorer.Core - Portable Class Library that will have all shared code (model, views, and view models).
* EarthExplorer.Droid - Xamarin.Droid application
* EarthExplorer.iOS - Xamarin.iOS application

![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/1987419b-3471-4421-a534-8605cb33dd6b/solution-explorer.JPG)

### Building the shared logic

In the **EarthExplorer (Portable)** project, a default class **MyClass** has been created

Open the class, delete **everything**, and use this code

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace EarthExplorer.Core
{
    public class PointOfInterest
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static async Task<List<PointOfInterest>> GetGlobalListAsync()
        {
            var list = new List<PointOfInterest>();

            list.Add(new PointOfInterest() { Name = "Paris", Latitude = 48.86206, Longitude = 2.343179 });
            list.Add(new PointOfInterest() { Name = "Seattle", Latitude = 47.59978, Longitude = -122.3346 });

            if (GetCurrentPOIAsync != null)
            {
                list.Add(await GetCurrentPOIAsync());
            }

            return list;
        }

        public static Func<Task<PointOfInterest>> GetCurrentPOIAsync { get; set; }
    }
}

```


In the **solution explorer**, rename **MyClass.cs** to **PointOfInterest.cs** as a form of good practice

We're now done with our business logic!

### EarthExplorer.Android

We're now going to implement the functionality for the Android app

1. Right click on **EarthExplorer.Anroid** and select **Set as StartUp project**

2. Right click on **References** > **Add Reference**
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/2aa55c98-b7a1-4f88-a78b-e379b9996c8f/android-add-reference.png)

3. Check **EarthExplorer.Core** and click **OK**
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/8a98f65f-7422-4f1f-8543-3fcb697eb953/android-add-core-reference.JPG)

4. In the **solution explorer**, go to **EarthExplorer.Anroid** > **Resources** > **layout** and open **activity_main.axml**

5. Go to **View** > **Toolbox**
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/a3c17eec-f24a-4a16-8e98-ab40120f5130/android-view-toolbox.png)

6. In the **Toolbox**, search for the **ListView** control, and drag it onto the Android designer
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/4682961c-9349-480b-b7dd-d344339eb4f7/android-search-listview.png)

7. Click on **File** > **Save** all to save changes 

#### Implementing the MainActivity

8. In the **Solution Explorer** open **MainActivity.cs**

9. At the top of the file, replace all using statements with the following
```csharp
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using EarthExplorer.Core;
using System.Collections.Generic;
```

10. Within the class, declare a list of PointOfInterest
```csharp
List<PointOfInterest> Datasource;
```
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/82f1101a-2e49-408f-a4aa-8d920eb94dbf/android-declare-datasource.png)

11. Initialise, and wire up the list view such that your **OnCreate** method looks like this

```csharp
protected override void OnCreate (Bundle bundle)
{
    base.OnCreate (bundle);

    // Set our view from the "main" layout resource
    SetContentView (Resource.Layout.Main);

    ListView listView = FindViewById<ListView>(Resource.Id.listView1);

    listView.ItemClick += ListView_ItemClick;

    Datasource = await PointOfInterest.GetGlobalListAsync();
    listView.Adapter = new POIAdapter(this, Datasource);
}
```

12. Since there's awaitable code here, change the method signature of **OnCreate** from

```csharp
protected override void OnCreate (Bundle bundle)
```

to

```csharp
protected async override void OnCreate (Bundle bundle)
```

13. Now, let's implement the **ItemClick** handler of the **ListView**

```csharp
private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
{
    var poi = Datasource[(int)e.Id];

    var geoUri = Android.Net.Uri.Parse($"geo:{poi.Latitude},{poi.Longitude}");
    var mapIntent = new Intent(Intent.ActionView, geoUri);
    StartActivity(mapIntent);
}
```

#### Creating the adapter class

In Android, an [Adapter](https://developer.android.com/reference/android/widget/Adapter.html) object acts as a bridge between an AdapterView and the underlying data for that view. The Adapter provides access to the data items. The Adapter is also responsible for making a View for each item in the data set
   
14. Let's add a new class by **right clicking** on the **EarthExplorer.Droid** project, go to **Add** and select **class**

15. Create a new class **POIAdapter.cs**

16. Delete everything in the file and replace it with


```csharp
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
```

17. Save the changes made to **POIAdapter.cs** by going to **File** > **Save all**

#### Configure the app for build and deployment
18. Right click on the main **Solution** and select **Properties**

19. Navigate to **Configuration Properties** > **Configuration**

20. Ensure that **Build** and **Deploy** is checked against **EarthExplorer.Droid**
![Visual Studio blank solution](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/1987419b-3471-4421-a534-8605cb33dd6b/solution-explorer.JPG)

### Running the app

You can run the app in the android emulator by click on the green arrow with the following settings

![Running the app in emulator](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/9e0e7df0-a1c2-4bfe-8c3a-6717251caa72/Capture.PNG)

### EarthExplorer.iOS

1. Right click on the **EarthExplorer.iOS** project and select **Set as StartUp Project**

2. Add the **EarthExplorer.Core** project as a reference (refer to step 2 & 3 in the Android section, except right click on the **References** item under **EarthExplorer.iOS**)

3. Right click on the main solution **EarthExplorer** project and select **Properties**

4. Uncheck **Build** and **Deploy** for Android and select **Build** for EarthExplorer.iOS. 

![Confguration properties](http://content.screencast.com/users/louisleong/folders/earthexplorer-2017/media/6fe10405-7fe4-40a8-8705-af2b7f9abda9/ios-config.png)

5. Click **OK**

6. In the Solution Explorer, open **Main.storyboard**

7. Delete the **Hello World, Click Me!** button

8. From the Toolbox, drag a **Map View** and **Table View** control onto the storyboard, and set their constraints

![Storyboard](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/e0c86e42-aa4f-4f69-b5ce-0038199e5cb7/iOS%20storyboard.PNG)

9. Set the name of the controls to **MyMap** and **MyTable** respectively in the **Properties** windows

![Setting properties name](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/5460c5f5-6708-4eb0-857f-1ac1f5a0bcfd/iOS%20control%20names.png)

#### Viewing controls from the document outline windows

10. Go to **View** > **Other Windows** > **Document outline**

11. Remove the nested **Table Cell** from the **Table View** until you see the following view:

![Removing the table cell from the Table View](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/cccbd987-6f15-4dfc-b462-68a3899fb401/iOS%20document%20outline.PNG)

#### Creating the TableSource class

12. Right click on the **EarthExplorer.iOS** project > **Add** > **Add New Item** > **Class**

13. Set the name as **TableSource.cs** and click **Add**

![TableSource class](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/1308e04f-dc4b-4fdd-b2f5-b6b0fe2a51f1/iOS%20tablesource.png)

#### TableSource.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EarthExplore.Core;
using Foundation;
using UIKit;

namespace EarthExplore.iOS
{
    public class TableSource : UITableViewSource
    {
        List<PointOfInterest> TableItems;
        string CellIdentifier = "TableCell";
        public event Action<PointOfInterest> OnClick;

        public TableSource(List<PointOfInterest> items)
        {
            TableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OnClick?.Invoke(TableItems[indexPath.Row]);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            PointOfInterest item = TableItems[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            }

            cell.TextLabel.Text = item.Name;
            return cell;
        }
    }
}
```

#### Open ViewController.cs

14. Remove the code related to the Button control that was deleted and add the following snippet in the **ViewDidLoad** method

Remember to mark the method as **async**

```csharp
public async override void ViewDidLoad ()
{
    base.ViewDidLoad ();
    // Perform any additional setup after loading the view, typically from a nib.

    var source = new TableSource(await PointOfInterest.GetGlobalListAsync());

    source.OnClick += Source_OnClick;

    MyTable.Source = source;
    MyTable.ReloadData();
}
```

#### Implementing the OnClick event handler

```csharp
private void Source_OnClick(PointOfInterest poi)
{
    var coords = new CLLocationCoordinate2D(poi.Latitude, poi.Longitude);

    MyMap.Region = new MapKit.MKCoordinateRegion(coords, new MapKit.MKCoordinateSpan(0.1, 0.1));
}
```

Import the following namespace

```csharp
using CoreLocation;
using EarthExplorer.Core;
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
````

#### Implementing the Location manager

Implemenet the LocationManager within **ViewDidLoad**. The method should now look like this

```csharp
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
```

Declare the following members at the class level
```csharp
CLLocationManager LocationManager;
ManualResetEvent waitEvent = new ManualResetEvent(false);
PointOfInterest currentPOI = new PointOfInterest();
```

#### Implement the LocationsUpdated event handler

```csharp
private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
{
    LocationManager.StopUpdatingLocation();
    currentPOI.Name = "Exactly here...";
    currentPOI.Latitude = e.Locations[0].Coordinate.Latitude;
    currentPOI.Longitude = e.Locations[0].Coordinate.Longitude;
    waitEvent.Set();
}
```

#### Modify the plist to allow locations to be used
In the **Solution Explorer**, right click on **Info.plist** > **Open With** > **XML (Text) Editor**

Add the following snippet right before ```</dict>```

```xml
  <key>NSLocationAlwaysUsageDescription</key>
  <string>This will be called if location is used behind the scenes</string>
  <key>NSLocationWhenInUseUsageDescription</key>
  <string>You are about to use location!</string>
</dict>
</plist>

```

### Build and run the app
![TableSource class](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/7018e48b-045e-4df3-933b-0d7ed43fcb9b/iOS%20run%20simulator.PNG)


