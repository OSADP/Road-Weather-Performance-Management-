using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;

namespace RoadSegmentMapping
{
    public class RoadSegmentMapper
    {
        private string _connectionString = "";
        OsmMapModel mapModel;

        public RoadSegmentMapper(string osmMapConnectionString)
        {
            _connectionString = osmMapConnectionString;
             mapModel= new OsmMapModel(_connectionString);

             // Load SQL Server Types for RoadSegmentMatter Distance functions.
             SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
        }

        public RoadSegment GetNearestMileMarker(double lat, double lon, String roadwayId, String direction)
        {
            var myLocation = DbGeography.FromText("POINT(" + lon.ToString() + " " + lat.ToString() + ")");
            //string roadwayId = roadName + "_" + direction;
            var mileMarker = mapModel.tMileMarkers.Where(mm => mm.RoadwayId == roadwayId).OrderBy(mm => mm.Location.Distance(myLocation)).Take(1).ToList().FirstOrDefault();

            RoadSegment roadSegment = new RoadSegment();
            roadSegment.RoadwayID = roadwayId;
            roadSegment.MileMarker = mileMarker.MileMarker;
            roadSegment.RoadwayDirection = direction;

            if (direction == "N" || direction == "E")
                roadSegment.IsMileMarkersIncreasing = true;
            else
                roadSegment.IsMileMarkersIncreasing = false;

            return roadSegment;
        }

        /// <summary>
        ///  Returns only the mile marker. Pik Alert stations are valid for both directions, so we
        /// don't want to query direction. Pass in both roadway directions - for presumably more
        /// efficiency - otherwise could query like 'I35W_%'
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="roadwayIdDir1">e.g the northbound roadway id</param>
        /// <param name="roadwayIdDir2">e.g the southbound roadway id</param>
        /// <returns></returns>
        public double? GetNearestMileMarkerEitherDirection(double lat, double lon, String roadwayIdDir1, String roadwayIdDir2)
        {
            var myLocation = DbGeography.FromText("POINT(" + lon.ToString() + " " + lat.ToString() + ")");
            //string roadwayId = roadName + "_" + direction;
            var mileMarker = mapModel.tMileMarkers.Where(mm => mm.RoadwayId == roadwayIdDir1 || mm.RoadwayId == roadwayIdDir2)
                .OrderBy(mm => mm.Location.Distance(myLocation)).Take(1).ToList().FirstOrDefault();
            if (mileMarker != null)
                return mileMarker.MileMarker;
            else
                return null;
        }

        public Location GetLocationForMileMarker(string roadwayId, double mileMarker)
        {

            var mileMarkerInfo = mapModel.tMileMarkers.Where(mm => mm.RoadwayId == roadwayId && mm.MileMarker == mileMarker);

            if (mileMarkerInfo != null && mileMarkerInfo.Count()>0)
            {
                Location lc = new Location();
                lc.Latitude = mileMarkerInfo.First().Latitude;
                lc.Longitude = mileMarkerInfo.First().Longitude;

                return lc;
            }

            return null;
        }

        public RoadSegment GetNearestRoadSegment(double lat, double lon, double heading)
        {
            var myLocation = DbGeography.FromText("POINT(" + lon.ToString() + " " + lat.ToString() + ")");

            //var wayInfoList = mapModel.tWays.Select(s => new WayInfo
            //    {
            //        id = s.Id,
            //        line = s.line,
            //        Dist = s.line.Distance(myLocation).Value
            //    }).OrderBy(x => x.Dist).Take(4).ToList();


            var firstList = mapModel.tWays.Where(w=>w.line.Distance(myLocation) < 15).ToList();

            var wayInfoList = firstList.Select(s => new WayInfo
                {
                    id = s.Id,
                    line = s.line,
                    Dist = s.line.Distance(myLocation).Value
                }).OrderBy(x => x.Dist).ToList();

            WayInfo nearestWayInfo = null;
            string direction = "";
            foreach(var wayInfo in wayInfoList)
            {
                DbGeography point1 = wayInfo.line.PointAt(1);
                DbGeography point2 = wayInfo.line.PointAt(wayInfo.line.PointCount.Value);
                double bearing = CalculateBearing(point1, point2);

                double bearingDiff = Math.Abs(bearing - heading);

                if (bearingDiff >= 315)
                    bearingDiff = bearingDiff - 315;

                if(bearingDiff < 45)
                {
                    if (nearestWayInfo == null)
                    {
                        nearestWayInfo = wayInfo;

                        if (bearing >= 0 && bearing < 45)
                            direction = "N";
                        else if (bearing >= 45 && bearing < 135)
                            direction = "E";
                        else if (bearing >= 135 && bearing < 225)
                            direction = "S";
                        else if (bearing >= 225 && bearing < 315)
                            direction = "W";
                        else
                            direction = "N";
                    }
                }
            }

            if (nearestWayInfo != null)
            {
                var nameTag = mapModel.tWayTags.Where(t => t.WayId == nearestWayInfo.id && t.Typ == 1).FirstOrDefault().Info.ToString();

                if(nameTag.Contains(";"))
                {
                    //name has a suffix concatenated. remove it.
                    nameTag = nameTag.Substring(0, nameTag.IndexOf(';'));
                }
                string roadwayId = nameTag.Replace(" ", "") + "_" + direction;
                
                    var mileMarker = mapModel.tMileMarkers.Where(mm => mm.RoadwayId == roadwayId).OrderBy(mm => mm.Location.Distance(myLocation)).Take(1).ToList().FirstOrDefault();
                
                if(mileMarker==null)
                {
                    string msg = "Roadway ID " + roadwayId + " not found in mapModel.tMileMarkers in function GetNearestRoadSegment.";
                    throw new Exception(msg);
                }
                RoadSegment roadSegment = new RoadSegment();
                roadSegment.RoadwayID = roadwayId;//return the real made one.
                roadSegment.MileMarker = mileMarker.MileMarker;


                if (direction == "N" || direction == "E")
                    roadSegment.IsMileMarkersIncreasing = true;
                else
                    roadSegment.IsMileMarkersIncreasing = false;
                roadSegment.RoadwayDirection = direction;

                return roadSegment;
            }
            else
                return null;
        }

        public double CalculateBearing(DbGeography pointA, DbGeography pointB)
        {
            if (pointA.IsEmpty || pointB.IsEmpty)
                return -1;

            double dLat = ConvertToRadians(pointB.Latitude.Value - pointA.Latitude.Value);
            double dLon = ConvertToRadians(pointB.Longitude.Value - pointA.Longitude.Value);

            double rLat1 = ConvertToRadians(pointA.Latitude.Value);
            double rLat2 = ConvertToRadians(pointB.Latitude.Value);

            double y = Math.Sin(dLon) * Math.Cos(rLat2);
            double x = Math.Cos(rLat1) * Math.Sin(rLat2) - Math.Sin(rLat1) * Math.Cos(rLat2) * Math.Cos(dLon);

            if (x == 0 && y == 0)
                return -1;

            double bearing = (ConvertToDegrees(Math.Atan2(y, x)) + 360) % 360;

            return bearing;
        }

        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public double ConvertToDegrees(double angle)
        {
            return (180 / Math.PI) * angle;
        }
    }
}
