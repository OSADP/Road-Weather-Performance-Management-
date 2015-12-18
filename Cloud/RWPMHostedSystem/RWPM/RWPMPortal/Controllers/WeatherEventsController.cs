using InfloCommon;
using InfloCommon.Models;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using RWPMPortal.Common;
using RWPMPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RWPMPortal.Controllers
{
    /// <summary>
    /// For shared admin functionality avl to more than one of the 3 'website' sections.
    /// </summary>
    [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_TRAFCONT_ADMIN_STR + "," + UIConstants.ROLE_RWMAINT_ADMIN_STR))]
    public class WeatherEventsController : Controller
    {
        IUnitOfWork _uow;
        OsmMapModel _mapModel;
        public WeatherEventsController(IUnitOfWork uow, OsmMapModel mapModel)
        {
            _uow = uow;
            _mapModel = mapModel;
        }
        // GET: WeatherEvents
        public ActionResult Index()
        {
            var w = _uow.WeatherEvents.OrderByDescending(d => d.StartTime).Take(10).ToList();
            return View(w);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var we = new Models.WeatherEventModel();
            return View(we);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            WeatherEvent dbEvent = new WeatherEvent();
            dbEvent.Name = collection.Get("EventName").Trim();
            if (dbEvent.Name == "")
            {
                dbEvent.Name = "Manual " + DateTime.UtcNow.ToString();
            }
            dbEvent.Mode = (int)WeatherEventMode.Manual;
            if (dbEvent.StartTime == default(DateTime))
            {
                dbEvent.StartTime = DateTime.UtcNow;
            }
            else
            {
                // dbEvent.StartTime = we.StartTime;
            }
            if (dbEvent.EndTime != null)
            {
                //   dbEvent.StartTime = we.StartTime;
            }
            _uow.WeatherEvents.Add(dbEvent);
            _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult EndEvent(int id)
        {
            WeatherEvent dbEvent = _uow.WeatherEvents.Where(d => d.Id == id).First();


            dbEvent.EndTime = DateTime.UtcNow;


            // _uow.WeatherEvents.s(dbEvent);
            _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var w = _uow.WeatherEvents.Where(i => i.Id == id).FirstOrDefault();
            if(w==null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Id = id;
            ViewBag.Message = w.Name + " [ " + w.StartTime.ToString() + " - ";
            if(w.EndTime!=null)
            {
                ViewBag.Message += w.EndTime.ToString();
            }
            else
            {
                string EndTime = "Currently In Progress";
                ViewBag.Message += EndTime;
            }
            ViewBag.Message += " ]"; ;
            return View();
          
        }


        public JsonResult Get(int weatherEventId, int siteId)
        {

            WeatherEventDetails det = new WeatherEventDetails();
            try
            {

                var district = _uow.Districts.FirstOrDefault();
                if (district != null)
                {
                    WeatherEvent weatherEv = _uow.WeatherEvents.Where(w => w.Id == weatherEventId).FirstOrDefault();
                    Site site = district.Sites.Where(o => o.Id == siteId).FirstOrDefault();
                    if (weatherEv != null && site != null)
                    {
                        det.SiteDescription = site.Description;

                        DateTime dtSince = weatherEv.StartTime;
                        DateTime dtTo = DateTime.UtcNow;
                        if (weatherEv.EndTime != null) dtTo = (DateTime)weatherEv.EndTime;





                        //var siteObservations = observations.Where(o => o.SiteId == site.Id);
                        var observations = _uow.SiteObservations.Where(s => s.DateTime > dtSince && s.DateTime <= dtTo);
                        var siteObservations = observations.Where(o => o.SiteId == siteId);

                        det.RoadTemperature = siteObservations.Average(s => s.RoadTemp);
                        det.RoadTemperature = Math.Round(det.RoadTemperature);



                        if (siteObservations != null)
                        {
                            WeatherEventStatistics stats = new WeatherEventStatistics();
                            foreach (var siteobs in siteObservations)
                            {
                                stats.AddToStatistics(siteobs);
                            }

                            //det.PercentObsReportedClear = Math.Round(stats.CalculatePercentClear());
                            double clear, wet, snow, ice;
                            stats.CalculatePavementPercentages(out ice, out snow, out wet, out clear);
                            det.PavementPcntClear = Math.Round(clear);
                            det.PavementPcntWet = Math.Round(wet);
                            det.PavementPcntSnow = Math.Round(snow);
                            det.PavementPcntIce = Math.Round(ice);
                            stats.CalculatePrecipPercentages(out ice, out snow, out wet, out clear);
                            det.PrecipPcntClear = Math.Round(clear);
                            det.PrecipPcntWet = Math.Round(wet);
                            det.PrecipPcntSnow = Math.Round(snow);
                            det.PrecipPcntIce = Math.Round(ice);
                            ////Grab the location for this site
                            //string siteIdName = siteObservations.First().Site.SiteIdName;
                            //var segs = _mapModel.tRoadSegments.Where(d => d.Id > 1).ToList();
                            //tRoadSegment roadSegment =  _mapModel.tRoadSegments.Where(rs => rs.aux_id == siteIdName).FirstOrDefault();
                            //if (roadSegment != null)
                            //{
                            //    //RoadSegmentMapper rsMapper = new RoadSegmentMapper(strOsmMapModelDbConnectionString);
                            //    string southRoadwayId = "I35W_S";
                            //    string northRoadwayId = "I35W_N";

                            //    //Query detector stations for this milemarkers
                            //    det.AvgSpeedSouth = GetAvgSpeedForRoadSegment(_uow, roadSegment, southRoadwayId);
                            //    det.AvgSpeedNorth = GetAvgSpeedForRoadSegment(_uow, roadSegment, northRoadwayId);

                            //    det.AvgSpeedSouth = Math.Round(det.AvgSpeedSouth);
                            //    det.AvgSpeedNorth = Math.Round(det.AvgSpeedNorth);
                            //}

                            //Grab the location for this site
                   
                                //RoadSegmentMapper rsMapper = new RoadSegmentMapper(strOsmMapModelDbConnectionString);
                                string southRoadwayId = "I35W_S";
                                string northRoadwayId = "I35W_N";

                                //Query detector stations for this milemarkers
                                det.AvgSpeedSouth = GetAvgSpeedForRoadSegment(_uow, site.MileMarker, southRoadwayId, dtSince, dtTo);
                                det.AvgSpeedNorth = GetAvgSpeedForRoadSegment(_uow, site.MileMarker, northRoadwayId, dtSince, dtTo);

                                det.AvgSpeedSouth = Math.Round(det.AvgSpeedSouth);
                                det.AvgSpeedNorth = Math.Round(det.AvgSpeedNorth);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            //JsonRequestBehavior.AllowGet is required, else you get the error at the client of:This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request.
            return Json(det, JsonRequestBehavior.AllowGet);
            
        }
        //private static double GetAvgSpeedForRoadSegment(IUnitOfWork uow, tRoadSegment roadSegment, string southRoadwayId)
        //{
        //    var southStations = uow.Configuration_TSSDetectorStations.Where(s => s.MMLocation > roadSegment.start_MileMarker
        //        && s.MMLocation <= roadSegment.end_MileMarker
        //        && s.RoadwayId == southRoadwayId).Select(s => s.DSId);

        //    var southZones = uow.Configuration_TSSDetectionZones.Where(z => southStations.Contains(z.DSId)).Select(s => s.DZId);
        //    if (uow.TME_TSSData_Inputs.Where(t => southZones.Contains(t.DZId)).FirstOrDefault() == null)
        //    {
        //        //These stations don't have any data.
        //        return -1;
        //    }
        //    return uow.TME_TSSData_Inputs.Where(t => southZones.Contains(t.DZId)).Average(t => t.AvgSpeed);
        //}
        public static double GetAvgSpeedForRoadSegment(IUnitOfWork uow, Nullable<double> MMofSite, string southRoadwayId,
            DateTime since, DateTime to)
        {

            if (MMofSite == null) return -1;
            double startMM, endMM;
            startMM = (double)MMofSite; //start by looking for exact match
            endMM = (double)MMofSite;

            //queyr all the nearby stations once for efficiency, grabbing only mmlocation and the zone.
            var stations = uow.Configuration_TSSDetectorStations.Where(s => s.RoadwayId == southRoadwayId
               && s.MMLocation > startMM -5
               && s.MMLocation < endMM + 5)
                .Select(s => new { s.MMLocation, s.DSId }).ToList();

            string dsid = "";
            for (int i = 0; i < 10; i++) // Span about 10 milemarkers in range looking for a matching site.
            {
                //Find the station at the ~same location as the site MM.
                var nearestStation = stations.Where(s => s.MMLocation > startMM
               && s.MMLocation < endMM).Select(s => s.DSId).FirstOrDefault(); 

                if (nearestStation != null)
                {
                    dsid= nearestStation;
                    break;
                }
                //Match not found. Expand search and see if we find one
                startMM -= 0.5;
                endMM += 0.5;
            }

            //Get zone linked to our station (1-1 relationship)

            var zoneForStation = uow.Configuration_TSSDetectionZones.Where(z => z.DSId == dsid).Select(s => s.DZId).FirstOrDefault();
            double speed;
            if (zoneForStation != null)
            {

                var dataDuringEvent = uow.TME_TSSData_Inputs.Where(t => zoneForStation == t.DZId
                && t.DateReceived >= since && t.DateReceived <= to);
                if (dataDuringEvent.Any())
                {

                    speed = dataDuringEvent.Average(t => t.AvgSpeed);
                    return speed;
                }
            }
            return -1;


        }
    }
}