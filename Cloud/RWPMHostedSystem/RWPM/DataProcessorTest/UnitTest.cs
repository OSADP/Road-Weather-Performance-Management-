using System;
using NUnit.Framework;
using DataProcessorWorkerRole;
using System.IO;
using Ninject;
using InfloCommon.Repositories;
using InfloCommon;
using INFLOClassLib;
using System.Data;
using System.Collections.Generic;
using System.Linq;


namespace DataProcessorTest
{
    [TestFixture]
    public class UnitTest
    {
        string strStorageAccountConnectionString = "DefaultEndpointsProtocol=http;AccountName=inflostorage;AccountKey=m3F1+Kg0N5UiaRZG8Sr6GysssCna/IxQDRXu1MuY7CbPbsuTPA54pgJu75eBd7CXQM6dg7O/5E7g/LhvthApIQ==";
        string dbConnectionString = "metadata=res://*/InfloDb.csdl|res://*/InfloDb.ssdl|res://*/InfloDb.msl;provider=System.Data.SqlClient;provider connection string='data source=hguz1d8jjq.database.windows.net,1433;initial catalog=inflo;user id=inflo;password=B@ttelle;MultipleActiveResultSets=True;App=EntityFramework'";
        //  public static IKernel Kernel = new StandardKernel();

        private void SetupData()
        {
            //UnitOfWork uow = new UnitOfWork(dbConnectionString);

            //TME_CVData_Input input = new TME_CVData_Input();

            //uow.TME_CVData_Inputs.Add(input);
            //uow.Commit();
        }

        [Test]
        public void TestGetNearestSiteId()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\RWPM_InfloConfigFile.xml"))//I35W_N
            {
                xmlConfig = sr.ReadToEnd();
            }

            UnitOfWork uow = new UnitOfWork(dbConnectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);


            var links = uow.Configuration_RoadwayLinks.Where(l => l.RoadwayId == "I35W_N").OrderBy(l => l.BeginMM).ToList();
            var roadway = uow.Configuration_Roadways.ToList();
            DataProcessing dp = new DataProcessing(xmlConfig);

            List<string> r = new List<string>();
            //Query all sites from the database, need only site id and corresponding milemarker.
            //Not all roadway links will align with a site
            List<DataProcessorWorkerRole.DataProcessing.SitesByMileMarker> sites = uow.Sites.Select(m => new DataProcessorWorkerRole.DataProcessing.SitesByMileMarker { Id = m.Id, MileMarker = m.MileMarker }).OrderBy(m => m.MileMarker).ToList();

            foreach (var l in links)
            {
                clsRoadwayLink cl = new clsRoadwayLink();
                cl.BeginMM = l.BeginMM;
                cl.EndMM = l.EndMM;
                cl.RoadwayID = l.RoadwayId;
                int? siteid = dp.GetNearestSiteId(sites, cl);

                r.Add(siteid == null ? "  " : (int)siteid + "," + l.BeginMM + "," + l.EndMM);
            }

            //Currently, the sites loaded from Pik alert are like:
            //Id	MileMarker	Description
            //39	9.5	MN ROAD SEGMENT Interstate 35w 2
            //40	12	MN ROAD SEGMENT Interstate 35w 1
            //27	18.4	MN ROAD SEGMENT Interstate 35w 20

            //And our links are
            //RoadwayId	LinkId	BeginMM	EndMM
            //I35W_N	27	10.9	11.6
            //I35W_N	28	11.6	12
            //I35W_N	29	12	12.6
            //I35W_N	30	12.6	13
            //I35W_N	31	13	13.5
            //I35W_N	32	13.5	13.8
            //I35W_N	33	13.8	14.2
            //I35W_N	34	14.2	14.8
            //I35W_N	35	14.8	15.1
            //I35W_N	36	15.1	15.4
            //I35W_N	37	15.4	15.8

            //So obviously our 29th link should be assigned to site 40, because it starts at MM12 and the site is at MM12.
            //But the links surrounding site 40 should also be assigned to 40, since that is their closest site:

            //The next lower site is at MM9.5.  The midpoint between MM9.5 and MM12 is 10.75, thus links under 10.75 should be
            //assigned to MM9.5, and links over 10.75 should be assigned to MM12.

            Assert.AreEqual(r[28], "40,12,12.6");//this link is exactly at the MM of the site.
            Assert.AreEqual(r[25], "39,10.7,10.9");//This link is just out of bounds to be close enough to site 40, and is instead assigned to another site.
            Assert.AreEqual(r[26], "40,10.9,11.6");//This link is closer to site 40 than any other sites.
            Assert.AreEqual(r[34], "40,14.8,15.1");//This link is the farthest on the other side to be closes to site 40.
        }

        [Test]
        public void TestGetNearestSiteIdSouth()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\..\..\..\Docs\RWPM_SB_InfloConfigFile.xml"))//I35W_S
            {
                xmlConfig = sr.ReadToEnd();
            }

            UnitOfWork uow = new UnitOfWork(dbConnectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);


            var links = uow.Configuration_RoadwayLinks.Where(l => l.RoadwayId == "I35W_S").OrderBy(l => l.BeginMM).ToList();
            var roadway = uow.Configuration_Roadways.ToList();
            DataProcessing dp = new DataProcessing(xmlConfig);

            List<string> r = new List<string>();
            //Query all sites from the database, need only site id and corresponding milemarker.
            //Not all roadway links will align with a site
            List<DataProcessorWorkerRole.DataProcessing.SitesByMileMarker> sites = uow.Sites.Select(m => new DataProcessorWorkerRole.DataProcessing.SitesByMileMarker { Id = m.Id, MileMarker = m.MileMarker }).OrderBy(m => m.MileMarker).ToList();

            foreach (var l in links)
            {
                clsRoadwayLink cl = new clsRoadwayLink();
                cl.BeginMM = l.BeginMM;
                cl.EndMM = l.EndMM;
                cl.RoadwayID = l.RoadwayId;
                int? siteid = dp.GetNearestSiteId(sites, cl);

                r.Add(siteid == null ? "  " : (int)siteid + "," + l.BeginMM + "," + l.EndMM);
            }

            //Currently, the sites loaded from Pik alert are like:
            //Id	MileMarker	Description
            //39	9.5	MN ROAD SEGMENT Interstate 35w 2
            //40	12	MN ROAD SEGMENT Interstate 35w 1
            //27	18.4	MN ROAD SEGMENT Interstate 35w 20

            //And our links are
            //RoadwayId	LinkId	BeginMM	EndMM
            //RoadwayId	LinkId	BeginMM	EndMM
            //I35W_S	39	11.6	11
            //I35W_S	38	12	11.6
            //I35W_S	37	12.1	12
            //I35W_S	36	12.5	12.1
            //I35W_S	35	13	12.5

            //The next lower site is at MM9.5.  The midpoint between MM9.5 and MM12 is 10.75, thus links under 10.75 should be
            //assigned to MM9.5, and links over 10.75 should be assigned to MM12.
            //*Note: Because the GetNearestSiteId reverses the begin/end milemarkers to do the comparison if the roadway
            //is in the decreasing direction, the comparision thresholds are actually based on the end MM for South.
            //This is okay since we are finding these for weather, which really should not be very variable from one site to the next
            //so it is not a big deal on the boundary which gets it.

            //Begin and End are reversed from above because we are travelling the other way on the roadway
            Assert.AreEqual(r[28], "40,12,11.6");//this link is exactly at the MM of the site.
            //This link is just out of bounds to be close enough to site 40, and is instead assigned to another site.
            Assert.AreEqual(r[25], "39,10.7,10.2");
            //11, in this case, is compared to the 10.75 and it is greater so it gets assigned to 40 not 39.
            Assert.AreEqual(r[26], "40,11,10.7");//This link is closer to site 40 than any other sites.
            Assert.AreEqual(r[37], "40,15.1,14.8");//This link is the farthest on the other side to be closes to site 40.
        }


        [Test]
        public void TestWeatherEvent_Array()
        {
            WeatherEventStatistics stats = new WeatherEventStatistics();

            stats.TrackPavementCondCnts[(int)WeatherCondition.Clear] = 16;
            stats.TrackPavementCondCnts[(int)WeatherCondition.Ice] = 4;
            stats.TrackPavementCondCnts[(int)WeatherCondition.Snow] = 2;
            stats.TrackPavementCondCnts[(int)WeatherCondition.Wet] = 0;

            double clear, wet, snow, ice;
            stats.CalculatePavementPercentages(out ice, out snow, out wet, out clear);

            Assert.GreaterOrEqual(ice, 18);
        }
                [Test]
        public void TestWeatherEvent_Add()
        {
            WeatherEventStatistics stats = new WeatherEventStatistics();

			SiteObservation siteObs = new SiteObservation();
			siteObs.Pavement = "icy";
            siteObs.Precipitation = "rain";
            stats.AddToStatistics(siteObs);
            siteObs.Pavement = "clear";
            siteObs.Precipitation = "clear";
			stats.AddToStatistics(siteObs);
			
            double clear, wet, snow, ice;
            stats.CalculatePavementPercentages(out ice, out snow, out wet, out clear);

            Assert.GreaterOrEqual(ice, 50);
        }

    }
}
