using InfloCommon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfloCommon.Models;

namespace InfloCommon
{
    public class WeatherEventStatistics
    {
        public static int WAITING_HOURS_BTW_EVENTS_THRESH = 1;
        public static int ROADTEMP_WEATHEREVENT_THRESH = 35;
        public static double PERCENT_CLEAR_MIN_END_EVENT = 90.0;
        public static double PERCENT_CLEAR_MAX_START_EVENT = 10.0;

       // public int ClearCnt { get; set; }
       // public int UnclearCnt { get; set; }

        public int[] TrackPavementCondCnts = new int[4]{0,0,0,0};
        public int[] TrackPrecipCondCnts = new int[4] { 0, 0, 0, 0 };
  
        public WeatherEventStatistics()
        {
         //   ClearCnt = 0;
         //   UnclearCnt = 0;
        }
        /// <summary>
        /// Call for each SiteObservation in order to Process weather events.
        /// </summary>
        /// <param name="siteObs"></param>
        //public void AddToStatistics(SiteObservation siteObs)
        //{
        //    bool unclear = false;
        //    string pavement = siteObs.Pavement.Trim().ToLower();
        //    if (pavement.Contains("wet")) unclear = true;
        //    string precip = siteObs.Precipitation.Trim().ToLower();
        //    if (precip.Contains("rain")) unclear = true;

        //    if (siteObs.RoadTemp < ROADTEMP_WEATHEREVENT_THRESH
        //        && unclear)
        //    {
        //        UnclearCnt++;
        //    }
        //    else
        //    {
        //        ClearCnt++;
        //    }
        //}
        public enum WeatherCondition
        {
            Clear = 0,
            Ice=1,
            Snow=2,
            Rain=3
        }
        public void AddToStatistics(SiteObservation siteObs)
        {
          
            //Parse pikAlert strings for pavement and precipitation. Then track result counts.
            WeatherCondition paveCond = ParseWeatherField(siteObs.Pavement);
            TrackPavementCondCnts[(int)paveCond]++;
            WeatherCondition precipCond = ParseWeatherField(siteObs.Precipitation);
            TrackPrecipCondCnts[(int)precipCond]++;
        
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
                return WeatherCondition.Rain;
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
                //Technically, we should probably have Roadway Id in the Weather Events table. And
                //then query for this roadway with site info for this roadway. Since we are doing
                //north/south for the same road, we are fine for now but if we ever did  a separate
                //location this would break down.
                var activeEvent = uow.WeatherEvents.Where(w => w.EndTime == null).FirstOrDefault();
                if (activeEvent != null)
                {
                    //We have an active weather event, see if it is time to End it.
                    //Can only Auto end the events that were generated Auto
                    if (activeEvent.Mode == (int)WeatherEventMode.Auto
                        && PavementPcntClear > PERCENT_CLEAR_MIN_END_EVENT
                        && PrecipPcntClear > PERCENT_CLEAR_MIN_END_EVENT)
                    {
                        activeEvent.EndTime = DateTime.UtcNow;
                        uow.SaveChanges();
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
                            we.Name = "AutoGen " + DateTime.UtcNow.ToString();
                            we.StartTime = DateTime.UtcNow;
                            we.Mode = (int)WeatherEventMode.Auto;

                            uow.WeatherEvents.Add(we);
                            uow.SaveChanges();
                        }

                    }
                }
            }

        }
        public double CalculatePavementPercentages(out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
           return CalculatePercentages( TrackPavementCondCnts,out icePer,out  snowPer, out  wetPer, out  clearPer);
        }
        public double CalculatePrecipPercentages(out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
            return CalculatePercentages(TrackPrecipCondCnts, out icePer, out  snowPer, out  wetPer, out  clearPer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="icePer">returns the percentage of loaded statistics that were icy.</param>
        /// <param name="snowPer">returns the percentage of loaded statistics that were snowy.</param>
        /// <param name="wetPer">returns the percentage of loaded statistics that were rainy.</param>
        /// <param name="clearPer">returns the percentage of loaded statistics that were clear.</param>
        public double CalculatePercentages(int[] conditions, out double icePer, out double snowPer, out double wetPer, out double clearPer)
        {
            double total = (double)(conditions[(int)WeatherCondition.Ice] +
                conditions[(int)WeatherCondition.Snow] +
                conditions[(int)WeatherCondition.Rain] +
                conditions[(int)WeatherCondition.Clear]);
            if (total != 0)
            {
                icePer = (double)conditions[(int)WeatherCondition.Ice] / total * 100;
                snowPer = (double)conditions[(int)WeatherCondition.Snow] / total * 100;
                wetPer = (double)conditions[(int)WeatherCondition.Rain] / total * 100;
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
        //public double CalculatePercentClear()
        //{
        //    double total = (double)(ClearCnt + UnclearCnt);
        //    if (total != 0)
        //    {
        //        double percentClear = (double)ClearCnt / total * 100;
        //        return percentClear;
        //    }
        //    else
        //    {
        //        //There are no loaded datapoint. Assume clear.
        //        return 100.0;
        //    }
        //}
    }
}
