using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExotischNLConsoleApp.Models
{

    internal class Location
    {
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public string LocationName { get; private set; }
        public string Province { get; private set; }

        public Location(string locationName, string province)
        {
            this.Latitude = 0;
            this.Longitude = 0;
            this.LocationName = locationName;
            this.Province = province;
        }
    }
}
