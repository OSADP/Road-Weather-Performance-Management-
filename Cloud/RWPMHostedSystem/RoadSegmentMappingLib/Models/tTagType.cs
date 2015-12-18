namespace RoadSegmentMapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tTagType")]
    public partial class tTagType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Typ { get; set; }

        [StringLength(255)]
        public string Name { get; set; }
    }
}
