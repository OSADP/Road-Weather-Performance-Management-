namespace RoadSegmentMapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tNode")]
    public partial class tNode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Required]
        public DbGeography location { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
