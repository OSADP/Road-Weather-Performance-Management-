using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models.GoogleMaps
{
    public class GoogleMapsDirections
    {
        public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }

    public class GeocodedWaypoint
    {
        public string geocoder_status { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class Bounds
    {
        public GMLocation northeast { get; set; }
        public GMLocation southwest { get; set; }
    }

    public class KeyValuePair
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class GMLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Polyline
    {
        public string points { get; set; }
    }

    public class Step
    {
        public KeyValuePair distance { get; set; }
        public KeyValuePair duration { get; set; }
        public GMLocation end_location { get; set; }
        public string html_instructions { get; set; }
        public Polyline polyline { get; set; }
        public GMLocation start_location { get; set; }
        public string travel_mode { get; set; }
        public string maneuver { get; set; }
    }

    public class Leg
    {
        public KeyValuePair distance { get; set; }
        public KeyValuePair duration { get; set; }
        public string end_address { get; set; }
        public GMLocation end_location { get; set; }
        public string start_address { get; set; }
        public GMLocation start_location { get; set; }
        public List<Step> steps { get; set; }
        public List<object> via_waypoint { get; set; }
    }

    public class OverviewPolyline
    {
        public string points { get; set; }
    }

    public class Route
    {
        public Bounds bounds { get; set; }
        public string copyrights { get; set; }
        public List<Leg> legs { get; set; }
        public OverviewPolyline overview_polyline { get; set; }
        public string summary { get; set; }
        public List<object> warnings { get; set; }
        public List<object> waypoint_order { get; set; }
    }
}
