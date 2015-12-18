using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace RoadSegmentMapping
{
    [Table("tMileMarker")]
    public partial class tMileMarker
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [StringLength(255)]
        public string RoadwayId { get; set; }

        public DbGeography Location { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double MileMarker { get; set; }
    }
}
