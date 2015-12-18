using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class VisualDisplayModels
    {
        public class SpdHarmAlert : MotoristAlertModel.SpdHarmAlert
        {
            public double BeginLatitude { get; set; }
            public double BeginLongitude { get; set; }
            public double EndLatitude { get; set; }
            public double EndLongitude { get; set; }
        }

        public class QWarnAlert : MotoristAlertModel.QWarnAlert
        {
            public double BOQLatitude { get; set; }
            public double BOQLongitude { get; set; }
            public double FOQLatitude { get; set; }
            public double FOQLongitude { get; set; }
        }
    }
}
