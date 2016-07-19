using Excel;
using INFLOClassLib;
using InfloCommon;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private static string osmMapConnectionString = "data source=hguz1d8jjq.database.windows.net;initial catalog=osm_map;user id=inflo;password=B@ttelle;MultipleActiveResultSets=True;App=EntityFramework";
        private static string InfloDatabaseConnectionString = "metadata=res://*/InfloDb.csdl|res://*/InfloDb.ssdl|res://*/InfloDb.msl;provider=System.Data.SqlClient;provider connection string='data source=hguz1d8jjq.database.windows.net,1433;initial catalog=inflo;user id=inflo;password=B@ttelle;MultipleActiveResultSets=True;App=EntityFramework'";


        //for debug/dev
        List<Configuration_Roadway> roadways = new List<Configuration_Roadway>();
        List<Configuration_TSSDetectorStation> stations = new List<Configuration_TSSDetectorStation>();
        List<Configuration_TSSDetectionZone> zones = new List<Configuration_TSSDetectionZone>();
        List<Configuration_RoadwayLinks> links = new List<Configuration_RoadwayLinks>();
        List<Configuration_RoadwaySubLinks> sublinks = new List<Configuration_RoadwaySubLinks>();

        List<string> skipped = new List<string>();

        private void process_configuration_sensor_data_Click(object sender, RoutedEventArgs e)
        {
            try {

            FileStream stream = File.Open("I-35W_Sensors.xlsx", FileMode.Open, FileAccess.Read);
            //Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            List<SensorDataXlsxInput> inputs = new List<SensorDataXlsxInput>();
            foreach (DataRow row in result.Tables[0].Rows)
            {
                SensorDataXlsxInput input = new SensorDataXlsxInput();
                input.OBJECTID = Convert.ToInt32(row.ItemArray[0].ToString());
                input.Latitude = Convert.ToDouble(row.ItemArray[1].ToString());
                input.Longitude = Convert.ToDouble(row.ItemArray[2].ToString());
                input.Roadname = row.ItemArray[3].ToString();
                input.Direction = row.ItemArray[4].ToString();
                input.SensorID = row.ItemArray[5].ToString().Trim();
                if (input.SensorID.Length > 0)//some sensor id's in the file are blank
                {
                    inputs.Add(input);
                }
                else
                {
                    skipped.Add("Sensor Id blank: OBJECTID = " + input.OBJECTID.ToString());
                }
            }
            //Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();


            RoadSegmentMapper rsMapper = new RoadSegmentMapper(osmMapConnectionString);
            UnitOfWork uow = new UnitOfWork(InfloDatabaseConnectionString);


            //Create Roadways
            Configuration_Roadway northboundRoadway = new Configuration_Roadway();
            northboundRoadway.RoadwayId = "I35W_N";
            northboundRoadway.Name = "I35W_N";
            northboundRoadway.Direction = "NB";
            northboundRoadway.Grade = null;//guess from db - all null in db.
            northboundRoadway.BeginMM = 0;
            northboundRoadway.EndMM = 41.4;
            northboundRoadway.MMIncreasingDirection = "NB";
            northboundRoadway.LowerHeading = null;//guess from db - all null in db.
            northboundRoadway.UpperHeading = null;//guess from db - all null in db.
            northboundRoadway.RecurringCongestionMMLocation = 41.4;
            Configuration_Roadway southboundRoadway = new Configuration_Roadway();
            southboundRoadway.RoadwayId = "I35W_S";
            southboundRoadway.Name = "I35W_S";
            southboundRoadway.Direction = "SB";
            southboundRoadway.Grade = null;//guess from db - all null in db.
            southboundRoadway.BeginMM = 41.5;
            southboundRoadway.EndMM = 0;
            southboundRoadway.MMIncreasingDirection = "NB";
            southboundRoadway.LowerHeading = null;//guess from db - all null in db.
            southboundRoadway.UpperHeading = null;//guess from db - all null in db.
            southboundRoadway.RecurringCongestionMMLocation = 0;
            roadways.Add(northboundRoadway);
            roadways.Add(southboundRoadway);

            DeletePreviousConfigurationForRoadway(uow, northboundRoadway);
            DeletePreviousConfigurationForRoadway(uow, southboundRoadway);
            foreach (SensorDataXlsxInput s in inputs)
            {

                //Look up milemarker
                string roadway = s.Direction == "N" ? northboundRoadway.RoadwayId : southboundRoadway.RoadwayId;
                RoadSegment roadSegment;
                try
                {
                    roadSegment = rsMapper.GetNearestMileMarker(s.Latitude, s.Longitude, roadway, s.Direction);
                }
                catch(Exception ex)
                {
                    skipped.Add(string.Format("Road segment threw exception: Lat = {0},Long = {1},Roadway = {2}. {3}", s.Latitude, s.Longitude, roadway, ex.Message));
                    continue;
                }
                if (roadSegment == null)
                {
                   skipped.Add(string.Format("Road segment not found: OBJECTID = {0}", s.OBJECTID));
                    continue;
                }

                double mileMarker = roadSegment.MileMarker;
                string RoadwayID = roadSegment.RoadwayID;
                bool increasingDir = roadSegment.IsMileMarkersIncreasing;

               
                //Sensores.xlsx spreadsheet has "N" instead of "NB" as direction, etc.
                string direction = s.Direction;
                if (direction == "N") direction = "NB";
                else if (direction == "S") direction = "SB";

                //Create detector station for this sensor
                Configuration_TSSDetectorStation station = new Configuration_TSSDetectorStation();
                station.LinkId = s.OBJECTID.ToString();  //I think these can be anything
                station.DSId = s.SensorID;    //I think these can be anything
                station.DSName = s.SensorID;
                station.MMLocation = roadSegment.MileMarker;
                station.NumberLanes = 2;//?
                station.NumberDetectionZones = 1; //We are using a 1to1 relationship between zones and stations
                station.DetectionZones = s.SensorID;
                station.Latitude = s.Latitude;
                station.Longitude = s.Longitude;
                station.Direction = direction;
                station.RoadwayId = roadSegment.RoadwayID;

                //We have too many stations for the number of milemarkers that we have. If we already have found a station mapped
                //to this milemarker, we don't need another.
                if(stations.Where(c=>c.MMLocation==station.MMLocation && c.RoadwayId==station.RoadwayId).FirstOrDefault() != null)
                {
                    //don't add. redundant.
                    continue;
                }

                stations.Add(station);

           
                //Create matching detection zone for this sensor and detector station.
                Configuration_TSSDetectionZone zone = new Configuration_TSSDetectionZone();
                zone.DSId = s.SensorID; //matches station.DSId
                zone.DZId = s.SensorID; //matches station.DetectionZones
                zone.DZType = "";//?GENERAL PURPOSE ?
                zone.LaneNumber = 1;//?
                zone.LaneType = "THROUGH";//guess from db 
                zone.LaneDescription = "";
                zone.DataType = "";//guess from db - all empty in db.
                zone.DZStatus = null;//guess from db - all null in db.
                zone.Direction = direction;
                zone.RoadwayId = roadSegment.RoadwayID;

                zones.Add(zone);
            }

            //Find links
            List<Configuration_TSSDetectorStation> northboundStations = stations.Where(d => d.Direction == "NB")
                  .OrderBy(d => d.MMLocation)//Sort Ascending, milemarkers increase northbound as you go downstream.
                .ToList();
            List<Configuration_TSSDetectorStation> southboundStations = stations.Where(d => d.Direction == "SB")
                .OrderByDescending(d => d.MMLocation)//Sort DEscending, milemarkers decrease southbound as you go downstream.
                .ToList();
            AddRoadwayLinks(uow, southboundRoadway, southboundStations);
            AddRoadwayLinks(uow, northboundRoadway, northboundStations);

            //Add sublinks

            //Northbound starts at 0 and ends at 42, so each bump should be positive .1.
            AddRoadwaySublinks(uow, northboundRoadway, 0.1);
            //Northbound starts at 42 and ends at 0, so each bump should be subtracting .1.
            AddRoadwaySublinks(uow, southboundRoadway, -0.1);

            AddConfigurationToDb(uow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void AddConfigurationToDb(UnitOfWork uow)
        {
            uow.Configuration_Roadways.AddRange(roadways);
            uow.Configuration_TSSDetectorStations.AddRange(stations);
            uow.Configuration_TSSDetectionZones.AddRange(zones);
            uow.Configuration_RoadwayLinks.AddRange(links);
            uow.Configuration_RoadwaySubLinks.AddRange(sublinks);
            uow.SaveChanges();

        }

        private void DeletePreviousConfigurationForRoadway(UnitOfWork uow, Configuration_Roadway roadway)
        {
            var rsl = uow.Configuration_RoadwaySubLinks.Where(d => d.RoadwayId == roadway.RoadwayId);
            uow.Configuration_RoadwaySubLinks.RemoveRange(rsl);

            var rl = uow.Configuration_RoadwayLinks.Where(d => d.RoadwayId == roadway.RoadwayId);
            uow.Configuration_RoadwayLinks.RemoveRange(rl);

            var z = uow.Configuration_TSSDetectionZones.Where(d => d.RoadwayId == roadway.RoadwayId);
            uow.Configuration_TSSDetectionZones.RemoveRange(z);

            var s = uow.Configuration_TSSDetectorStations.Where(d => d.RoadwayId == roadway.RoadwayId);
            uow.Configuration_TSSDetectorStations.RemoveRange(s);

            var r = uow.Configuration_Roadways.Where(d => d.RoadwayId == roadway.RoadwayId);
            uow.Configuration_Roadways.RemoveRange(r);

            uow.SaveChanges();
        }

        private void AddRoadwaySublinks(UnitOfWork uow, Configuration_Roadway roadway, double subLinkStepFactor)
        {
            int subLinkId = 1;

            double startSubLinkMM = roadway.BeginMM;
            double endSubLinkMM = Math.Round(startSubLinkMM + subLinkStepFactor,1);
            //Divide the roadway into pieces subLinkStepFactor long.
            //Start at BeginMM, stop when the last piece is at EndMM.
            //Northbound starts at 0, 0.1, 0.2  and goes to 42.
            //Southbound starts at 42, 41.9, 41.8 and goes to 0.
            bool mmIncreasing = (roadway.EndMM - roadway.BeginMM > 0) ? true : false;
            for (int s = 0; (mmIncreasing ? endSubLinkMM <= roadway.EndMM : endSubLinkMM >= roadway.EndMM); s++)
            {
                Configuration_RoadwaySubLinks sublink = new Configuration_RoadwaySubLinks();
                sublink.RoadwayId = roadway.RoadwayId;
                sublink.Direction = roadway.Direction;
                sublink.SubLinkId = (subLinkId++).ToString();
                sublink.BeginMM = startSubLinkMM;
                sublink.EndMM = endSubLinkMM;

                startSubLinkMM = endSubLinkMM;
                endSubLinkMM = Math.Round(endSubLinkMM + subLinkStepFactor,1);

                sublinks.Add(sublink);
            }
        }

        private void AddRoadwayLinks(UnitOfWork uow, Configuration_Roadway roadway, List<Configuration_TSSDetectorStation> stationsForDirection)
        {

            int linkId = 1;
            for (int i = 0; i < stationsForDirection.Count - 1; i++)//milemarkers decreasing as we go through this for loop.
            {
                //For example "southbound i-35 W"
                //Sensor 1: south - ID'6483'
                //Sensor 2: south - ID'182'
                //Heading south, so south is downstream, norther is upstream.
                //Upstream detector station is the one applied to that link.

                //First link is from '6483' to '182'. Station '6483' assigned to Link 1
                Configuration_RoadwayLinks link = new Configuration_RoadwayLinks();
                link.BeginMM = stationsForDirection[i].MMLocation;
                link.EndMM = stationsForDirection[i + 1].MMLocation;

                link.DetectorStations = stationsForDirection[i].DSId;//?
                link.Direction = roadway.Direction;
                link.DMSId = null;//guess from db - all null in db.
                link.DownstreamLinkId = null;//guess from db - all null in db.
                link.EndCrossStreetName = null;//guess from db - all null in db.
                link.ESS = null;//guess from db - all null in db.
                link.LinkId = (linkId++).ToString();
                link.NumberDetectorStations = 1;//?
                link.NumberLanes = 2;//?
                link.RoadwayId = roadway.RoadwayId;
                link.RSEId = null;//guess from db - all null in db.
                link.SpeedLimit = null;//guess from db - all null in db.
                link.StartCrossStreetName = null;//guess from db - all null in db.
                link.UpstreamLinkId = null;//guess from db - all null in db.
                link.VSLSignId = null;//guess from db - all null in db.

                //uow.Configuration_RoadwayLinks.Add(link);

                links.Add(link);
            }
        }

        class SensorDataXlsxInput
        {
            public int OBJECTID { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Roadname { get; set; }
            public string Direction { get; set; }
            public string SensorID { get; set; }
        }
    }
}
