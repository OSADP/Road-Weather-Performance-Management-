using InfloCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RWPMPortal.Models
{

    public class WeatherEventModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime? EndTime { get; set; }

        public IEnumerable<SiteObservation> TriggeringData { get; set; }
        public IEnumerable<SiteObservation> FinalData { get; set; }
   
    }
   
}