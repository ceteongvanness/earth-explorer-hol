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
