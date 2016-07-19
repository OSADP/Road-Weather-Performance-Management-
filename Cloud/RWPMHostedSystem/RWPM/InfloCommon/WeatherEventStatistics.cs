using InfloCommon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfloCommon.Models;

namespace InfloCommon
{
    public enum WeatherCondition
    {
        Clear = 0,
        Ice = 1,
        Snow = 2,
        Wet = 3
    }
    public class WeatherEventStatistics
    {
        public static int WAITING_HOURS_BTW_EVENTS_THRESH = 1;
        public static int ROADTEMP_WEATHEREVENT_THRESH = 35;
        public static double PERCENT_CLEAR_MIN_END_EVENT = 90.0;
        public static double PERCENT_CLEAR_MAX_START_EVENT = 10.0;

        public short[] TrackPavementCondCnts = new short[4] { 0, 0, 0, 0 };
        public short[] TrackPrecipCondCnts = new short[4] { 0, 0, 0, 0 };
        /// <summary>
        /// Running sum of road temperatures for site observations.
        /// </summary>
        private double SummedRoadTemp = 0;
        /// <summary>
        /// Total number of site observations loaded for statistics.
        /// </summary>
        private short TotalSitesReporting = 0;

        /// <summary>
        /// Earliest date time of the data added to the statistics set.  This earliest time
        /// will be used as the weather start event time.
        /// </summary>
        public DateTime? EarliestDataPoint { get; set; }
        /// <summary>
        /// Latest date time of the data added to the statistics set.  This latest time
        /// will be used as the weather end event time.
        /// </summary>
        public DateTime? LatestDataPoint { get; set; }
        public WeatherEventStatistics()
        {

            EarliestDataPoint = null;
            LatestDataPoint = null;
        }

        public void AddToStatistics(SiteObservation siteObs)
        {
            //Check date of data and see if it is earlier than the earliest we've stored.
            if (EarliestDataPoint == null || siteObs.DateTime < EarliestDataPoint)
            {
                //save earlier time
                EarliestDataPoint = siteObs.DateTime;
            }
            //Check date of data and see if it is earlier than the earliest we've stored.
            if (LatestDataPoint == null || siteObs.DateTime > LatestDataPoint)
            {
                //save earlier time
                LatestDataPoint = siteObs.DateTime;
            }
            //PikAlert strings have already been parsed into the appropriate "weather bucket".
            //Check each bucket and record the counts for pavement in TrackPavementCondCnts.
            if (siteObs.PaveValWet == 1) TrackPavementCondCnts[(int)WeatherCondition.Wet]++;
            else if (siteObs.PaveValSnow == 1) TrackPavementCondCnts[(int)WeatherCondition.Snow]++;
            else if (siteObs.PaveValIce == 1) TrackPavementCondCnts[(int)WeatherCondition.Ice]++;
            else TrackPavementCondCnts[(int)WeatherCondition.Clear]++;
            //Check each bucket and record the counts for pavement in TrackPrecipCondCnts.
            if (siteObs.PrecipValWet == 1) TrackPrecipCondCnts[(int)WeatherCondition.Wet]++;
            else if (siteObs.PrecipValSnow == 1) TrackPrecipCondCnts[(int)WeatherCondition.Snow]++;
            else if (siteObs.PrecipValIce == 1) TrackPrecipCondCnts[(int)WeatherCondition.Ice]++;
            else TrackPrecipCondCnts[(int)WeatherCondition.Clear]++;

            //Increment total number of data points (pik alerts sites) that are being added to the dataset
            TotalSitesReporting++;
            //Add on road temp so it can be averaged when done.
            SummedRoadTemp = +siteObs.RoadTemp;

        }

        /// <summary>
        /// String parse the provided pik Alert field to determine the predominant weather condition.
        /// Preferences first icy, then snowy, then wet based on text in field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public WeatherCondition ParseWeatherField(string field)
        {
            string pikField = field.Trim().ToLower();
            if (pikField.Contains("ice") || pikField.Contains("icy") || pikField.Contains("slick") || pikField.Contains("freez"))
            {
                return WeatherCondition.Ice;
            }
            else if (pikField.Contains("snow") || pikField.Contains("mixed"))
            {
                return WeatherCondition.Snow;
            }
            else if (pikField.Contains("rain") || pikField.Contains("wet"))
            {
                return WeatherCondition.Wet;
            }
            return WeatherCondition.Clear;
        }
        /// <summary>
        /// Updates uow to start or end a weather event based on the loaded statistics.
        /// Caller should Commit uow changes.
        /// </summary>
        /// <param name="uow"></param>
        public void ProcessWeatherEvents(IUnitOfWork uow)
        {
            double clear, wet, snow, ice;
            double totalPave = CalculatePavementPercentages(out ice, out snow, out wet, out clear);
            double PavementPcntClear = Math.Round(clear);
            double PavementPcntWet = Math.Round(wet);
            double PavementPcntSnow = Math.Round(snow);
            double PavementPcntIce = Math.Round(ice);
            double totalPrecip = CalculatePrecipPercentages(out ice, out snow, out wet, out clear);
            double PrecipPcntClear = Math.Round(clear);
            double PrecipPcntWet = Math.Round(wet);
            double PrecipPcntSnow = Math.Round(snow);
            double PrecipPcntIce = Math.Round(ice);

            //PikAlert may have not called stats.AddToStatistics on fresh data.
            //As long as we got valid data, check for weather events.
            if (totalPave > 0 && totalPrecip > 0)
            {
                //Create a weatherLog entry every time we run processing on weather.
                WeatherLog log = new WeatherLog();
                log.PaveCntIce = TrackPavementCondCnts[(int)WeatherCondition.Ice];
                log.PaveCntSnow = TrackPavementCondCnts[(int)WeatherCondition.Snow];
                log.PaveCntWet = TrackPavementCondCnts[(int)WeatherCondition.Wet];
                log.PaveCntClear = TrackPavementCondCnts[(int)WeatherCondition.Clear];
                log.PrecipCntIce = TrackPrecipCondCnts[(int)WeatherCondition.Ice];
                log.PrecipCntSnow = TrackPrecipCondCnts[(int)WeatherCondition.Snow];
                log.PrecipCntWet = TrackPrecipCondCnts[(int)WeatherCondition.Wet];
                log.PrecipCntClear = TrackPrecipCondCnts[(int)WeatherCondition.Clear];
                log.TotalReportedSites = TotalSitesReporting;
                log.RoadTempAvg = CalculateAvgRoadTemp();
                log.Time = DateTime.UtcNow;
                log.WeatherEventId = null;
                log.Message = "";
                //Technically, we should probably have Roadway Id in the Weather Events table. And
                //then query for this roadway with site info for this roadway. Since we are doing
                //north/south for the same road, we are fine for now but if we ever did  a separate
                //location this would break down.
                var activeEvent = uow.WeatherEvents.Where(w => w.EndTime == null).FirstOrDefault();
                if (activeEvent != null)
                {
                    log.WeatherEventId = activeEvent.Id;
                    log.Message = "In a weather event.  ";
                    //We have an active weather event, see if it is time to End it.
                    if (PavementPcntClear > PERCENT_CLEAR_MIN_END_EVENT
                         && PrecipPcntClear > PERCENT_CLEAR_MIN_END_EVENT)
                    {
                        log.Message += "  Weather is clear to end event.  ";
                        bool wasClearConsec = WasLastCycleClearToo(uow);
                        if (wasClearConsec)
                        {
                            //Can only Auto end the events that were generated Auto
                            if (activeEvent.Mode == (int)WeatherEventMode.Auto)
                            {
                                log.Message += "  Ending auto-generated event.";
                                activeEvent.EndTime = LatestDataPoint;
                                uow.SaveChanges();
                            }
                            else
                            {
                                log.Message += "  Cannot end manual event.";
                            }
                        }
                        else
                        {
                            //PikAlert sometimes give one set of all dry/clear data and then goes back to snow.
                            //This must be bad data.  So we need to "debounce" all clear.
                            //If this is the first all clear we've gotten, we don't end the event just yet until
                            //the second consecutive one is clear too.
                            log.Message += "Waiting for 2nd clear.";
                        }
                    }
                }
                else
                {
                    //We are not in an active weather event, see if it is time to start one.
                    if (PavementPcntSnow + PavementPcntIce >= PERCENT_CLEAR_MAX_START_EVENT ||
                       PrecipPcntSnow + PrecipPcntIce >= PERCENT_CLEAR_MAX_START_EVENT)
                    {
                        //Query most recent event
                        var latestEvent = uow.WeatherEvents.OrderByDescending(w => w.EndTime).FirstOrDefault();
                        if (latestEvent == null || latestEvent.EndTime < DateTime.UtcNow.AddHours(-1 * WAITING_HOURS_BTW_EVENTS_THRESH))
                        {
                            //it has been at least an hour since the last event ended,
                            //so it is okay to start a new one.
                            WeatherEvent we = new WeatherEvent();
                            
                            if (EarliestDataPoint != null)
                            {
                                //Record start time of weather event as the time from which we got the data.
                                we.StartTime = (DateTime)EarliestDataPoint;
                            }
                            else
                            {
                                we.StartTime = DateTime.UtcNow;
                            }
                            we.Name = "AutoGen " + we.StartTime.ToString();
                            we.Mode = (int)WeatherEventMode.Auto;

                            uow.WeatherEvents.Add(we);
                            uow.SaveChanges();

                            log.Message = "Starting weather event.";
                            log.WeatherEventId = we.Id;
                        }
                        else
                        {
                            //Note in log that we would start one if we were allowed.
                            log.Message = "Cannot start weather event - too soon since last event.";
                        }

                    }
                    else
                    {
                        log.Message = "No weather event detected.";
                    }
                }
                uow.WeatherLogs.Add(log);
                uow.SaveChanges();
            }

        }

        /// <summary>
        /// Checks the last saved weather log to see if the results indicated Clear weather conditions
        /// that would qualify to end weather event.
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        private static bool WasLastCycleClearToo(IUnitOfWork uow)
        {
            //Get most recent log.
            var wPast = uow.WeatherLogs.OrderByDescending(w => w.Time).First();
            //Calculate percentages. and see if it is clear.
            double PavementPcntClear = (double)wPast.PaveCntClear / wPast.TotalReportedSites * 100;
            double PrecipPcntClear = (double)wPast.PrecipCntClear / wPast.TotalReportedSites * 100;
            if (PavementPcntClear > PERCENT_CLEAR_MIN_END_EVENT
                     && PrecipPcntClear > PERCENT_CLEAR_MIN_END_EVENT)
            {
                //Last log result indicated clear conditions
                return true;
            }
            return false;
        }
        public double CalculatePavementPercentages(out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
            return CalculatePercentages(TrackPavementCondCnts, out icePer, out  snowPer, out  wetPer, out  clearPer);
        }
        public double CalculatePrecipPercentages(out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
            return CalculatePercentages(TrackPrecipCondCnts, out icePer, out  snowPer, out  wetPer, out  clearPer);
        }
        /// <summary>
        /// Returns the percentage of loaded statistics that were classified to various conditions.
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="icePer">returns the percentage of loaded statistics that were icy.</param>
        /// <param name="snowPer">returns the percentage of loaded statistics that were snowy.</param>
        /// <param name="wetPer">returns the percentage of loaded statistics that were rainy.</param>
        /// <param name="clearPer">returns the percentage of loaded statistics that were clear.</param>
        public double CalculatePercentages(short[] conditions, out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
            //double total = (double)(conditions[(int)WeatherCondition.Ice] +
            //    conditions[(int)WeatherCondition.Snow] +
            //    conditions[(int)WeatherCondition.Rain] +
            //    conditions[(int)WeatherCondition.Clear]);
            double total = TotalSitesReporting;
            if (total != 0)
            {
                icePer = (double)conditions[(int)WeatherCondition.Ice] / total * 100;
                snowPer = (double)conditions[(int)WeatherCondition.Snow] / total * 100;
                wetPer = (double)conditions[(int)WeatherCondition.Wet] / total * 100;
                clearPer = (double)conditions[(int)WeatherCondition.Clear] / total * 100;
            }
            else
            {
                //There are no loaded datapoint. Assume clear.
                icePer = 0;
                snowPer = 0;
                wetPer = 0;
                clearPer = (double)100;
            }
            return total;
        }
        /// <summary>
        /// Returns the average road temp calculated from loaded statistics.
        /// </summary>
        /// <returns></returns>
        public double CalculateAvgRoadTemp()
        {
            double temp;

            temp = SummedRoadTemp / TotalSitesReporting;

            return temp;
        }

    }
}
