using System;
using System.Collections.Generic;
using System.Text;

namespace forekast.Models
{

    public class AirPollution
    {
        public List[] List { get; set; }
    }

    public class List
    {
        public int Dt { get; set; }
        public Main Main { get; set; }
        public Components Components { get; set; }
    }

    public class Main
    {
        public int Aqi { get; set; }
    }

    public class Components
    {
        public float co { get; set; }
        public float no { get; set; }
        public float no2 { get; set; }
        public float o3 { get; set; }
        public float so2 { get; set; }
        public float pm2_5 { get; set; }
        public float pm10 { get; set; }
        public float nh3 { get; set; }
    }
}