using System;
using System.Collections.Generic;
using System.Text;

namespace forekast.Models
{
    class CityInfo
    {
        public CityInfo(string name, double lon, double lat, string country)
        {
            Name = name;
            Lon1 = lon;
            Lat1 = lat;
            Country = country;
        }

        public string Name { get; set; }
        public string Country { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public double Lon1 { get; }
        public double Lat1 { get; }
    }
}
