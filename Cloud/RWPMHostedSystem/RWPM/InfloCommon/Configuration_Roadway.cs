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
    
    public partial class Configuration_Roadway
    {
        public string RoadwayId { get; set; }
        public string Name { get; set; }
        public string Direction { get; set; }
        public Nullable<double> Grade { get; set; }
        public double BeginMM { get; set; }
        public double EndMM { get; set; }
        public string MMIncreasingDirection { get; set; }
        public Nullable<double> LowerHeading { get; set; }
        public Nullable<double> UpperHeading { get; set; }
        public Nullable<double> RecurringCongestionMMLocation { get; set; }
    }
}
