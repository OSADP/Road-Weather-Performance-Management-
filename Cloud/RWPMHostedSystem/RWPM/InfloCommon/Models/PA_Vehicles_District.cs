using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class PA_Vehicles_District
    {
        public string display_name { get; set; }
        public string district_name { get; set; }
        public double max_lat { get; set; }
        public double max_lon { get; set; }
        public double min_lat { get; set; }
        public double min_lon { get; set; }
        public List<PA_Vehicles_Vehicle> vehicles { get; set; }
    }
}
