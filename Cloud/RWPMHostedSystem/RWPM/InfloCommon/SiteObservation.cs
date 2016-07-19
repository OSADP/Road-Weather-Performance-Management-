//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InfloCommon
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiteObservation
    {
        public SiteObservation()
        {
            this.PaveValClear = 0;
            this.PaveValWet = 0;
            this.PaveValSnow = 0;
            this.PaveValIce = 0;
            this.PrecipValClear = 0;
            this.PrecipValWet = 0;
            this.PrecipValSnow = 0;
            this.PrecipValIce = 0;
        }
    
        public int Id { get; set; }
        public System.DateTime DateTime { get; set; }
        public string AlertCode { get; set; }
        public string Chemical { get; set; }
        public string Pavement { get; set; }
        public string Plow { get; set; }
        public string Precipitation { get; set; }
        public double RoadTemp { get; set; }
        public string TreatmentAlertCode { get; set; }
        public string Visibility { get; set; }
        public int SiteId { get; set; }
        public short PaveValClear { get; set; }
        public short PaveValWet { get; set; }
        public short PaveValSnow { get; set; }
        public short PaveValIce { get; set; }
        public short PrecipValClear { get; set; }
        public short PrecipValWet { get; set; }
        public short PrecipValSnow { get; set; }
        public short PrecipValIce { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
