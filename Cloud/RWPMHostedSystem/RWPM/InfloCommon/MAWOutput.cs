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
    
    public partial class MAWOutput
    {
        public int Id { get; set; }
        public System.DateTime AlertRequestTime { get; set; }
        public System.DateTime AlertTime { get; set; }
        public System.DateTime AlertGenTime { get; set; }
        public int PrecipitationCode { get; set; }
        public int PavementCode { get; set; }
        public int VisibilityCode { get; set; }
        public int ActionCode { get; set; }
        public int SiteId { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
