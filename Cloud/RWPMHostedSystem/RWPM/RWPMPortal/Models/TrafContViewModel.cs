using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfloCommon.Models;

namespace RWPMPortal.Models
{
    public class TrafContViewModel
    {
        public double MapLatitude { get; set; }
        public double MapLongitude { get; set; }
        public int MapZoom { get; set; }
        public List<MotoristAlertModel.SpdHarmAlert> SpeedHarmAlerts { get; set; }
        public List<MotoristAlertModel.QWarnAlert> QWarnAlerts { get; set; }
    }
}