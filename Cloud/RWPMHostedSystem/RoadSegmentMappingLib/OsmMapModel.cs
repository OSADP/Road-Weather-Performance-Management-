
namespace RoadSegmentMapping
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class OsmMapModel : DbContext
    {
        public OsmMapModel(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<tMemberRole> tMemberRoles { get; set; }
        public virtual DbSet<tMemberType> tMemberTypes { get; set; }
        public virtual DbSet<tNode> tNodes { get; set; }
        public virtual DbSet<tRelation> tRelations { get; set; }
        public virtual DbSet<tRelationCreation> tRelationCreations { get; set; }
        public virtual DbSet<tWay> tWays { get; set; }
        public virtual DbSet<tWayCreation> tWayCreations { get; set; }
        public virtual DbSet<tNodeTag> tNodeTags { get; set; }
        public virtual DbSet<tRelationTag> tRelationTags { get; set; }
        public virtual DbSet<tTagType> tTagTypes { get; set; }
        public virtual DbSet<tWayTag> tWayTags { get; set; }
        public virtual DbSet<tMileMarker> tMileMarkers { get; set; }
        public virtual DbSet<tRoadSegment> tRoadSegments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
