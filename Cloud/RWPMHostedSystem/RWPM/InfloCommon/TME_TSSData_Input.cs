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
    
    public partial class TME_TSSData_Input
    {
        public string DZId { get; set; }
        public string DSId { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
        public Nullable<short> StartInterval { get; set; }
        public Nullable<short> EndInterval { get; set; }
        public Nullable<short> IntervalLength { get; set; }
        public Nullable<System.DateTime> BeginTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public short Volume { get; set; }
        public float Occupancy { get; set; }
        public short AvgSpeed { get; set; }
        public Nullable<bool> Queued { get; set; }
        public Nullable<bool> Congested { get; set; }
        public string DZStatus { get; set; }
        public string DataType { get; set; }
        public string RoadwayId { get; set; }
        public int Id { get; set; }
    }
}
