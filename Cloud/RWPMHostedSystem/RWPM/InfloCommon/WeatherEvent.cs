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
    
    public partial class WeatherEvent
    {
        public WeatherEvent()
        {
            this.Mode = 1;
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public short Mode { get; set; }
    }
}
