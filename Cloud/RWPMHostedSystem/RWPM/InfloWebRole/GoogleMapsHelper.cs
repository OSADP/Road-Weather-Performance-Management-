using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using InfloCommon.Models;
using InfloCommon.Models.GoogleMaps;
using InfloCommon;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Web.Http.Cors;

namespace InfloWebRole
{
    public class GoogleMapsHelper
    {
        public Feature GetPolylineFeatureForLocation(Location originLocation, Location destinationLocation)
        {
            //Get route


            var client = new RestClient("https://maps.googleapis.com/maps/api");


            var request = new RestRequest("/directions/json", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("origin", originLocation.Latitude.ToString() + "," + originLocation.Longitude.ToString());
            request.AddParameter("destination", destinationLocation.Latitude.ToString() + "," + destinationLocation.Longitude.ToString());
            request.AddParameter("key", "AIzaSyDCbYnXFSCKOLlTr3uVrLemsw6frkZurEs");

            IRestResponse<GoogleMapsDirections> response = client.Execute<GoogleMapsDirections>(request);

            if (response != null)
            {
                GoogleMapsDirections gmDirs = response.Data;
                if (gmDirs != null)
                {
                    List<Location> polylineLocationList = DecodePolylinePoints(gmDirs.routes.First().overview_polyline.points);

                    List<GeographicPosition> lineStringList = new List<GeographicPosition>();

                    foreach (Location loc in polylineLocationList)
                    {
                        GeographicPosition gp = new GeographicPosition(loc.Latitude, loc.Longitude);
                        lineStringList.Add(gp);
                    }

                    var line = new LineString(lineStringList);
                    var lineFeature = new Feature(line);

                    return lineFeature;
                }
            }

            return null;
        }

        public Feature GetFeatureForEncodedPolyline(String encodedPolyline, Dictionary<string, object> props)
        {
            List<Location> polylineLocationList = DecodePolylinePoints(encodedPolyline);

            List<GeographicPosition> lineStringList = new List<GeographicPosition>();

            foreach (Location loc in polylineLocationList)
            {
                GeographicPosition gp = new GeographicPosition(loc.Latitude, loc.Longitude);
                lineStringList.Add(gp);
            }

            var line = new LineString(lineStringList);
            var lineFeature = new Feature(line, props);

            return lineFeature;
        }

        public String GetEncodedPolylineForLocation(Location originLocation, Location destinationLocation)
        {
            //Get route


            var client = new RestClient("https://maps.googleapis.com/maps/api");


            var request = new RestRequest("/directions/json", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("origin", originLocation.Latitude.ToString() + "," + originLocation.Longitude.ToString());
            request.AddParameter("destination", destinationLocation.Latitude.ToString() + "," + destinationLocation.Longitude.ToString());
            request.AddParameter("key", "AIzaSyDCbYnXFSCKOLlTr3uVrLemsw6frkZurEs");

            IRestResponse<GoogleMapsDirections> response = client.Execute<GoogleMapsDirections>(request);

            if (response != null)
            {
                GoogleMapsDirections gmDirs = response.Data;
                if (gmDirs != null)
                {
                    return gmDirs.routes.First().overview_polyline.points;
                }
            }

            return null;
        }

        private List<Location> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Location> poly = new List<Location>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Location p = new Location();
                    p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                    p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                // logo it
            }
            return poly;
        }
    
    }
}
