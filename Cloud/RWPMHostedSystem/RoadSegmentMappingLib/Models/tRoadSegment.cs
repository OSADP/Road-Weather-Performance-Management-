using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace RoadSegmentMapping
{
    [Table("tRoadSegment")]
    public partial class tRoadSegment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Required]
        public string seg_name { get; set; }

        [Required]
        public int seg_id { get; set; }

        [Required]
        public string aux_id { get; set; }

        [Required]
        public double start_MileMarker { get; set; }

        [Required]
        public double end_MileMarker { get; set; }

        public string north_bound_polyline { get; set; }

        public string south_bound_polyline { get; set; }

    }
}
