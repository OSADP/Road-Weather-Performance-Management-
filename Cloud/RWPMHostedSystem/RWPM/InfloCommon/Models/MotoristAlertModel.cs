using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class MotoristAlertModel
    {
        public List<QWarnAlert> QWarn { get; set; }
        public List<SpdHarmAlert> SpdHarm { get; set; }
        public List<PikalertMAW> Pik { get; set; }


        public MotoristAlertModel()
        {
            QWarn = new List<QWarnAlert>();
            SpdHarm = new List<SpdHarmAlert>();
            Pik = new List<PikalertMAW>();
        }
        public class QWarnAlert{
        public DateTime DateGenerated { get; set; }
        public string RoadwayId { get; set; }
        public double BOQMMLocation { get; set; }
        public double FOQMMLocation { get; set; }
        public int SpeedInQueue { get; set; }
        public double RateOfQueueGrowth { get; set; }
        public int? ValidityDuration { get; set; }
        public double DistanceToQueue { get; set; }
    }
        public class SpdHarmAlert
        {
            public DateTime DateGenerated { get; set; }
            public string RoadwayId { get; set; }
            public int RecommendedSpeed { get; set; }
            public double BeginMM { get; set; }
            public double EndMM { get; set; }
            public string Justification { get; set; }
            public int? ValidityDuration { get; set; }
        }
        public class PikalertMAW
        {
            public DateTime DateGenerated {get;set;}
            public string RoadwayId {get ;set;}
            public double MileMarker { get;set;}
            public string PrecipAlert {get;set;}
            public int PrecipAlertCode {get;set;}
            public DateTime AlertTime { get; set; }
            public int PavementAlertCode{get;set;}
            public string PavementAlert {get;set;}
            public int VisibilityAlertCode {get;set;}
            public string VisibilityAlert {get;set;}
            public DateTime AlertGenerationTime {get;set;}
            public int AlertActionCode { get; set; }
            public string AlertAction { get; set; }
        }
    }
}
