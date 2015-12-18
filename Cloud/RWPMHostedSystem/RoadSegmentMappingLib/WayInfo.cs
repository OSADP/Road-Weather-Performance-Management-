using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;

namespace RoadSegmentMapping
{
    class WayInfo
    {
        public long id;
        public DbGeography line;
        public double Dist;
    }
}
