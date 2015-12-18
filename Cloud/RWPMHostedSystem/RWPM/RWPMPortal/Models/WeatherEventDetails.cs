using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RWPMPortal.Models
{
    public class WeatherEventDetails
    {
        public string SiteDescription { get; set; }
        public double RoadTemperature { get; set; }
        /// <summary>
        /// Percent of pavement observations during this weather event that were reported as "clear" conditions.
        /// </summary>
        public double PavementPcntClear { get; set; }
        public double PavementPcntWet { get; set; }
        public double PavementPcntSnow { get; set; }
        public double PavementPcntIce { get; set; }
        public double PrecipPcntClear { get; set; }
        public double PrecipPcntWet { get; set; }
        public double PrecipPcntSnow { get; set; }
        public double PrecipPcntIce { get; set; }

        public double AvgSpeedSouth { get; set; }
        public double AvgSpeedNorth { get; set; }
    }
}