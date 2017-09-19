## Earth explorer hands-on lab

In this hands-on lab, we will be building a [Xamarin](http://xamarin.com) application that will display a list of interest locations and its location on a map. This app will target iOS, Android and UWP.

### Get Started

Open **Visual Studio 2015**

Go to **File** > **New** > **Project** 

Navigate to **Visual C#** > **Cross-platform** and create **Cross Platform App (Xamarin)** a project called **EarthExplorer**

![Visual Studio new project](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/0774d4fc-f48b-477b-b471-07471950321c/new%20project.PNG)

Select **Blank App**, **Native** for the UI Technology, and **Portable Class Library (PCL)** for the Code Sharing Strategy
![Visual Studio new project](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/49dc5eea-3e42-4a2c-888d-95f2a5e21e3b/new%20project%20options.PNG)


This solution contains 4 projects

* EarthExplorer (Portable) - Portable Class Library that will have all shared code (model, views, and view models).
* EarthExplorer.Droid - Xamarin.Android application
* EarthExplorer.iOS - Xamarin.iOS application
* EarthExplorer.WinPhone (Windows Phone 8.1)  - Windows 10 UWP application (can only be run from VS 2015 on Windows 10)

At the time of this demo, the UWP project is not created by default. We will be replacing the **WinPhone** project with a **UWP** version


### EarthExplorer.UWP

Remove the **EarthExplorer.WinPhone (Windows Phone 8.1)** from the solution by **right clicking** on the project, and select **Remove**

This can be done by **Right-clicking** on the **Solution** and clicking on **Restore NuGet packages...**

![Remove project](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/ab259842-465c-4fef-b0ad-b36c0b9ceaee/Capture.PNG)

We will now add a new UWP project into the solution

Right click on the **Solution** > **Add** > **New project**

Navigate to **Visual C#** > **Windows** > **Universal** and create **Blank App (Universal Windows)** a project called **EarthExplorer.UWP**

![New UWP project](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/e035ce03-a5cb-47f5-abeb-2ff6c9453a59/Capture.PNG)

Accept the default selections for the **minimum** and **target** versions and click **OK**

![UWP target versions](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/8b63d7a6-7fb8-41a6-bd4c-5c19a92daff9/Capture.PNG)

In the **EarthExplorer.UWP (Universal Windows)** project, right click on **References* and select **Add Reference**

![Add reference](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/9cbce3c9-4045-4c61-9308-9d3e0e7e1483/Capture.PNG)

Under **Projects** > **Solution**, select **Earth Explorer** project and click **OK**

![Select](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/3f430408-3f0a-4ffc-9910-f9042496eb23/Capture.PNG)




### Building the shared logic in the EarthExplorer (Portable) project

In the **EarthExplorer (Portable)** project, a default class **MyClass** has been created

Open the class, and replace the empty class with the following code

```csharp
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
```

Add in the following using statements

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
```

In the **solution explorer**, rename **MyClass.cs** to **PointOfInterest.cs** as a form of good practice

We're now done with our business logic!

### EarthExplorer.UWP

We will now look at how to build UWP version of our app

Right click on the **EarthExplorer.UWP (Universal Windows)** project, and select **Set as StartUp project**

In the **solution explorer**, open **MainPage.xaml**

In the **XAML** view, add the following grid definition code within the **Grid** tags

```xml
 <Grid.RowDefinitions>
    <RowDefinition Height="Auto"></RowDefinition>
    <RowDefinition></RowDefinition>
</Grid.RowDefinitions>
<ListView Grid.Row="0" x:Name="POIList" Margin="10,20" IsItemClickEnabled="True">
    <ListView.ItemTemplate>
        <DataTemplate>
            <Grid>
                <TextBlock VerticalAlignment="Center" Text="{Binding Name}" FontSize="25"/>
            </Grid>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
<my:MapControl Grid.Row="1" x:Name="Map" ZoomLevel="12"/>
```

Add the following XML namespace in the **Page** tag 

```xml
xmlns:my="using:Windows.UI.Xaml.Controls.Maps"
```

### Code behind

Navigate to the code behind file **MainPage.xaml.cs**

#### Overriding OnNavigatedTo
**OnNavigatedTo** is invoked when the [Page](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.page.aspx) is loaded and becomes the current source of a parent [Frame](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.frame.aspx).

```csharp
protected override void OnNavigatedTo(NavigationEventArgs e)
{
    base.OnNavigatedTo(e);

    //implement other stuff
}
```
Now, replace 

```csharp
//implement other stuff
```

with

```csharp
PointOfInterest.GetCurrentPOIAsync = async () =>
{
    var currentPOI = new PointOfInterest();
    var geolocator = new Geolocator();

    var position = await geolocator.GetGeopositionAsync();

    currentPOI.Name = "Just where I am";
    currentPOI.Latitude = position.Coordinate.Point.Position.Latitude;
    currentPOI.Longitude = position.Coordinate.Point.Position.Longitude;

    return currentPOI;
};

POIList.ItemClick += POIList_ItemClick;

var dataSource = await PointOfInterest.GetGlobalListAsync();

POIList.ItemsSource = dataSource;
MoveTo(dataSource[0]);

Map.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads;
```

Since there's awaitable code here, change the method signature of **OnNavigatedTo** from

```csharp
protected override void OnNavigatedTo(NavigationEventArgs e)
```

to

```csharp
protected async override void OnNavigatedTo(NavigationEventArgs e)
```

Add the following using statements

```csharp
using Windows.Devices.Geolocation;
```

#### Implementing the event handlers


Add the following methods to implement the event handlers

```csharp
private void POIList_ItemClick(object sender, ItemClickEventArgs e)
{
    var poi = e.ClickedItem as PointOfInterest;
    MoveTo(poi);
}
```

```csharp
private async void MoveTo(PointOfInterest poi)
{
    var position = new BasicGeoposition();

    position.Latitude = poi.Latitude;
    position.Longitude = poi.Longitude;

    await Map.TrySetViewAsync(new Geopoint(position));
}
```

We're almost done!

#### Configuring capabilities
Right click on the **EarthExplorer.UWP (Universal Windows)** project, and select **Properties**

In the **Application** tab, click on **Package Manifest**

Navigate to the **Capabilities** tab, and check **Location**

![UWP capabilities](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/841e06d9-6a61-416b-b21e-2c66d67b6bd0/Capture.PNG)

Save your changes to the project by going to **File** > **Save All**

### Configure the app for build and deployment
Right click on the main **Solution** and select **Properties**

Navigate to **Configuration Properties** > **Configuration**

Check **Build** and **Deploy** against **EarthExplorer.UWP** and click **OK**. You may uncheck **Build** and **Deploy** for **iOS** and **Android** if you want to cut down time on the build

![UWP configuration manager](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/3535bdcf-07f7-47b6-9f78-efb2e38e687b/Capture.PNG)

### Run your app!

To run your app in its desktop form, select **Local Machine**

![Run desktop version](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/12df8264-ee27-441a-9f62-696e58b2faea/Capture.PNG)

Or you could select the other emulators 

![Run on emulator](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/69981122-b0eb-499f-a184-7b6894f0b018/Capture.PNG)

#### App running on the emulator

![UWP app on emulator](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/6448abeb-17b6-457d-a81b-2182ab3b17d2/Capture.PNG)

### EarthExplorer.Droid

We're now going to implement the functionality for the Android app

Right click on **EarthExplorer.Droid** and select **Set as StartUp project**

In the **solution explorer**, go to **Resources** > **layout** and open **Main.axml**

Select the **HELLO WORLD, CLICK ME** button in the designer, and press **delete**

In the **Toolbox**, look for the **ListView** control, and drag it onto the Android designer

![Drag listview to designer](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/f6e98f34-6a87-4f26-81fa-0dc02e3aa268/Capture.PNG)

Click on **File** > **Save** all to save changes 

#### Implementing the MainActivity

Open **MainActivity.cs**

In the **OnCreate** method, remove the code related to the Button as we have removed it from the UI earlier

Initialise, and wire up the list view such that your **OnCreate** method looks like this

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

Above the **OnCreate** method, within the class, add the Datasource field:

```csharp
List<PointOfInterest> Datasource;
```

Add the following using statement

```csharp
using System.Collections.Generic;
```

Since there's awaitable code here, change the method signature of **OnCreate** from

```csharp
protected override void OnCreate (Bundle bundle)
```

to

```csharp
protected async override void OnCreate (Bundle bundle)
```

Now, let's implement the **ItemClick** handler of the **ListView**

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
   
Let's add a new class by **right clicking** on the **EarthExplorer.Droid** project, go to **Add** and select **class**

Create a new class **POIAdapter.cs**

Replace

```csharp
class POIAdapter
{
}
```

with

```csharp
public class POIAdapter : BaseAdapter<PointOfInterest>
{
    PointOfInterest[] items;
    Activity context;
    public POIAdapter(Activity context, List<PointOfInterest> items) : base() {
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
```

Save the changes made to **POIAdapter.cs** by going to **File** > **Save all**

#### Configure the app for build and deployment
Right click on the main **Solution** and select **Properties**

Navigate to **Configuration Properties** > **Configuration**

Ensure that **Build** and **Deploy** is checked against **EarthExplorer.Droid**

### Running the app

You can run the app in the android emulator by click on the green arrow with the following settings

![Running the app in emulator](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/9e0e7df0-a1c2-4bfe-8c3a-6717251caa72/Capture.PNG)

### EarthExplorer.iOS

Right click on the **EarthExplorer.iOS** project and select **Set as StartUp Project**

Right click on the main solution **EarthExplorer** project and select **Properties**

Uncheck **Build** and **Deploy** for Android and select **Build** for EarthExplorer.iOS. 

![Confguration properties](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/b0b9ddbc-e861-4e16-aa79-b392aff6368f/iOS%20configuration%20manager.png)

Click **OK**

In the Solution Explorer, open **Main.storyboard**

Delete the **Hello World, Click Me!** button

From the Toolbox, drag a **Map View** and **Table View** control onto the storyboard, and set their constraints

![Storyboard](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/e0c86e42-aa4f-4f69-b5ce-0038199e5cb7/iOS%20storyboard.PNG)

Set the name of the controls to **MyMap** and **MyTable** respectively in the **Properties** windows

![Setting properties name](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/5460c5f5-6708-4eb0-857f-1ac1f5a0bcfd/iOS%20control%20names.png)

#### Viewing controls from the document outline windows

Go to **View** > **Other Windows** > **Document outline**

Remove the nested **Table Cell** from the **Table View** until you see the following view:

![Removing the table cell from the Table View](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/cccbd987-6f15-4dfc-b462-68a3899fb401/iOS%20document%20outline.PNG)

#### Creating the TableSource class

Right click on the **EarthExplorer.iOS** project > **Add** > **Add New Item** > **Class**

Set the name as **TableSource.cs** and click **Add**

![TableSource class](http://content.screencast.com/users/louisleong/folders/earthexplorer-hol/media/1308e04f-dc4b-4fdd-b2f5-b6b0fe2a51f1/iOS%20tablesource.png)

#### TableSource.cs

```csharp
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
```

#### Open ViewController.cs

Remove the code related to the Button control that was deleted and add the following snippet in the **ViewDidLoad** method

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
    CLLocationCoordinate2D coords = new CLLocationCoordinate2D(poi.Latitude, poi.Longitude);

    MyMap.Region = new MapKit.MKCoordinateRegion(coords, new MapKit.MKCoordinateSpan(0.1, 0.1));
}
```

Import the following namespace

```csharp
using CoreLocation;
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


