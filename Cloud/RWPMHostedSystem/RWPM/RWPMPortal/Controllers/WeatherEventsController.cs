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
using PagedList;

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
        public ActionResult Index(int? page)
        {
            //var w = _uow.WeatherEvents.OrderByDescending(d => d.StartTime).Take(10).ToList();
            //return View(w);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(_uow.WeatherEvents.OrderByDescending(d => d.StartTime).ToPagedList(pageNumber, pageSize));
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
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            WeatherEvent w = _uow.WeatherEvents.Where(i => i.Id == id).FirstOrDefault();
            if (w == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Id = id;
            DateTime startLocal = TimeZoneInfo.ConvertTimeFromUtc(w.StartTime, TimeZoneInfo.Local);
            ViewBag.Message = w.Name + " [ " + startLocal.ToString("MM/dd/yy hh:mm") + " - ";
            if (w.EndTime != null)
            {
                DateTime endLocal = TimeZoneInfo.ConvertTimeFromUtc((DateTime)w.EndTime, TimeZoneInfo.Local);
                ViewBag.Message += endLocal.ToString("MM/dd/yy hh:mm");
            }
            else
            {
                string EndTime = "Currently In Progress";
                ViewBag.Message += EndTime;
            }
            ViewBag.Message += " ]";

            //Get data that triggered and ended the event
            DateTime dtStart = w.StartTime;
            DateTime dtStart5 = dtStart.AddMinutes(5);
            var observationsStart = _uow.SiteObservations.Where(s => s.DateTime >= dtStart && s.DateTime < dtStart5);

            //initialize end set to an Empty set.
            var observationsEnd = Enumerable.Empty<SiteObservation>();
            if (w.EndTime != null)
            {
                //Only if this event is completed (has an End time), populate the last 5 minutes.
                DateTime dtEnd = (DateTime)w.EndTime;
                DateTime dtEnd5 = dtEnd.AddMinutes(-5);
                observationsEnd = _uow.SiteObservations.Where(s => s.DateTime > dtEnd5 && s.DateTime <= dtEnd);
            }

            WeatherEventModel model = new WeatherEventModel();
            model.TriggeringData = observationsStart.ToList();
            model.FinalData = observationsEnd.ToList();

            return View(model);

        }
        [HttpPost]
        public ActionResult LogsByDate(FormCollection collection)
        {
            var s = collection.Get("startDate").Trim();
            var e = collection.Get("endDate").Trim();
            DateTime? startDate = null;
            DateTime? endDate = null;
            DateTime date = new DateTime();
            ViewBag.startDate = s;
            ViewBag.endDate = e;
            //If the user entered valid date, use those to bound the query. Otherwise just grab latest.
            if (DateTime.TryParse(s, out date))
            {
                startDate = date;
                ViewBag.startDate = date.ToString();
            }
            if (DateTime.TryParse(e, out date))
            {
                endDate = date;
                ViewBag.endDate = date.ToString();
            }
            return LogsByDate(startDate, endDate);
        }
        /// <summary>
        /// Returns latest 50 if no dates specified.
        /// </summary>
        /// <param name="startDate">local time</param>
        /// <param name="endDate">local time</param>
        /// <returns></returns>
        public ActionResult LogsByDate(DateTime? startDate, DateTime? endDate)
        {
            if (startDate != null && endDate != null)
            {
                DateTime startDateUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)startDate, TimeZoneInfo.Local);
                DateTime endDateUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)endDate, TimeZoneInfo.Local);
                return View("Logs", _uow.WeatherLogs.Where(i => i.Time >= (DateTime)startDateUtc && i.Time <= (DateTime)endDateUtc)
                    .OrderByDescending(i => i.Time).ToList());
            }
            else if (startDate != null)
            {
                DateTime startDateUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)startDate, TimeZoneInfo.Local);
                return View("Logs", _uow.WeatherLogs.Where(i => i.Time >= startDateUtc)
                    .OrderByDescending(i => i.Time).ToList());
            }
            else if (endDate != null)
            {
                DateTime endDateUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)endDate, TimeZoneInfo.Local);
                return View("Logs", _uow.WeatherLogs.Where(i => i.Time <= endDateUtc)
                    .OrderByDescending(i => i.Time).ToList());
            }
            else
            {
                //Show top 50 if no dates specified
                return View("Logs", _uow.WeatherLogs
                    .OrderByDescending(i => i.Time).Take(50).ToList());
            }
        }

        public ActionResult Logs(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("LogsByDate");
            }
            else
            {
                return View("Logs", _uow.WeatherLogs.Where(i => i.WeatherEventId == id)
                        .OrderByDescending(i => i.Time).ToList());
            }
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

                        //Query all observations occurring during the weather event start and stop times.
                        var observations = _uow.SiteObservations.Where(s => s.DateTime >= dtSince && s.DateTime <= dtTo);
                        var siteObservations = observations.Where(o => o.SiteId == siteId);

                        det.RoadTemperature = siteObservations.Average(s => s.RoadTemp);
                        det.RoadTemperature = Math.Round(det.RoadTemperature);



                        if (siteObservations != null)
                        {
                            WeatherEventStatistics stats = new WeatherEventStatistics();
                            foreach (var siteobs in siteObservations)
                            {
                                //Add each observation
                                stats.AddToStatistics(siteobs);
                            }

                            //Now that all observations are loaded, run the calculations to characterize the set.
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

        /// <summary>
        /// Pulls stations for roadway from Configuration_TSSDetectorStations. Finds the nearest station to the MMofSite.
        /// Gets the zone mapped to that station.
        /// Grabs data from TME_TSSData_Inputs for that zone during the timestamps specified.
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="MMofSite"></param>
        /// <param name="southRoadwayId"></param>
        /// <param name="since">date range to average speeds for zone</param>
        /// <param name="to">date range to average speeds for zone</param>
        /// <returns></returns>
        public static double GetAvgSpeedForRoadSegment(IUnitOfWork uow, Nullable<double> MMofSite, string southRoadwayId,
            DateTime since, DateTime to)
        {

            if (MMofSite == null) return -1;
            double startMM, endMM;
            startMM = (double)MMofSite; //start by looking for exact match
            endMM = (double)MMofSite;

            //queyr all the nearby stations once for efficiency, grabbing only mmlocation and the zone.
            var stations = uow.Configuration_TSSDetectorStations.Where(s => s.RoadwayId == southRoadwayId
               && s.MMLocation > startMM - 5
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
                    dsid = nearestStation;
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
                //Query the zone and times from the data stored in TME_TSSData_Inputs.
                var dataDuringEvent = uow.TME_TSSData_Inputs.Where(t => zoneForStation == t.DZId
                && t.DateReceived >= since && t.DateReceived <= to);
                 List<TME_TSSData_Input> test = dataDuringEvent.ToList();
                 double accum = 0;
                 if (test.Count() > 1)
                 {
                     foreach (var t in test)
                     {
                         accum += t.AvgSpeed;
                     }
                     speed = accum / test.Count();
                     return speed;
                 }
                //Averaging done manually on full record above. doing anything else with linq keeps
                //crashing the debugger or timing out - unclear why.
                // var speeds = uow.TME_TSSData_Inputs.Where(t => zoneForStation == t.DZId
                //&& t.DateReceived >= since && t.DateReceived <= to).Select(m=>m.AvgSpeed).ToList();
                // var wtf = speeds.Sum(t=>t);
                // var wtfa = speeds.Average(t => t);
                // double? apple = dataDuringEvent.Average(a => (double?)a.AvgSpeed);
                //if (dataDuringEvent.Any())
                //{
                //    speed = dataDuringEvent.Average(t => t.AvgSpeed);
                //    return speed;
                //}
            }
            return -1;
        }
    }
}