using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsDetectorStation
    {
        private int m_Identifier;
        private int m_LinkIdentifier;
        private string m_RoadwayIdentifier;
        private string m_Name;
        private double m_MMLocation;  //Milemarker where located
        private int m_NumberDetectionZones;
        private int m_NumberLanes;
        private string m_DetectionZones;
        private double m_Latitude;
        private double m_Longitude;
        private clsEnums.enDirection m_Direction;
        //Modified by Hassan Charara on 11/14/2014 to convert the DetectionZonesList ID from int to string
        //private List<int> m_DetectionZonesList;
        private List<string> m_DetectionZonesList;

        private DateTime m_DateReceived;
        private int m_StartInterval;
        private int m_EndInterval;
        private DateTime m_BeginTime;
        private DateTime m_EndTime;
        private int m_IntervalLength;
        private int m_Volume;
        private double m_Occupancy;
        private double m_AvgSpeed;
        private bool m_Queued;
        private bool m_Congested;

        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }
        public int Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
        }
        public int LinkIdentifier
        {
            get { return m_LinkIdentifier; }
            set { m_LinkIdentifier = value; }
        }
        public string RoadwayIdentifier
        {
            get { return m_RoadwayIdentifier; }
            set { m_RoadwayIdentifier = value; }
        }
        public string DetectionZones
        {
            get { return m_DetectionZones; }
            set { m_DetectionZones = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public double MMLocation
        {
            get { return m_MMLocation; }
            set { m_MMLocation = value; }
        }
        public int NumberDetectionZones
        {
            get { return m_NumberDetectionZones; }
            set { m_NumberDetectionZones = value; }
        }
        public int NumberLanes
        {
            get { return m_NumberLanes; }
            set { m_NumberLanes = value; }
        }
        public double Latitude
        {
            get { return m_Latitude; }
            set { m_Latitude = value; }
        }
        public double Longitude
        {
            get { return m_Longitude; }
            set { m_Longitude = value; }
        }
        public List<string> DetectionZonesList
        {
            get { return m_DetectionZonesList; }
            set { m_DetectionZonesList = value; }
        }
        public int IntervalLength
        {
            get { return m_IntervalLength; }
            set { m_IntervalLength = value; }
        }
        public int Volume
        {
            get { return m_Volume; }
            set { m_Volume = value; }
        }
        public double Occupancy
        {
            get { return m_Occupancy; }
            set { m_Occupancy = value; }
        }
        public double AvgSpeed
        {
            get { return m_AvgSpeed; }
            set { m_AvgSpeed = value; }
        }
        public bool Queued
        {
            get { return m_Queued; }
            set { m_Queued = value; }
        }
        public bool Congested
        {
            get { return m_Congested; }
            set { m_Congested = value; }
        }
        public DateTime BeginTime
        {
            get { return m_BeginTime; }
            set { m_BeginTime = value; }
        }
        public DateTime EndTime
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }
        public DateTime DateReceived
        {
            get { return m_DateReceived; }
            set { m_DateReceived = value; }
        }
        public int StartInterval
        {
            get { return m_StartInterval; }
            set { m_StartInterval = value; }
        }
        public int EndInterval
        {
            get { return m_EndInterval; }
            set { m_EndInterval = value; }
        }

        public clsDetectorStation()
        {
        }

        ~clsDetectorStation()
        {
        }
}
}
