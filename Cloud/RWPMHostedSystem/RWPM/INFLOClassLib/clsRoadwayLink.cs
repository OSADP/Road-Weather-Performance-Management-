using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsRoadwayLink
    {
        private string m_RoadwayID;
        private int m_Identifier;
        private double m_BeginMM;
        private double m_EndMM;
        private double m_SpeedLimit;
        private int m_NumberDetectionStations;
        private string m_DetectionStations;
        private string m_StartCrossStreetName;
        private string m_EndCrossStreetName;
        private int m_UpstreamLinkID;
        private int m_DownStreamLinkID;
        private int m_NumberLanes;
        private int m_VSLSignID;
        private string m_VSLSignaName;
        private int m_DMSID;
        private string m_DMSName;
        private clsEnums.enDirection m_Direction;
        private int m_RSEID;
        private string m_RSEName;
        private DateTime m_DateProcessed;
        private int m_StartInterval;
        private int m_EndInterval;
        private bool m_Queued;
        private bool m_Congested;
        private double m_TSSAvgSpeed;
        private double m_WRTMSpeed;
        private int m_Volume;
        private double m_Occupancy;
        private List<int> m_DSList;
        private double m_RecommendedSpeed;
        private clsEnums.enRecommendedSpeedSource m_RecommendedSpeedSource;

        public clsRoadwayLink()
        {
        }

        public List<int> DSList
        {
            get { return m_DSList; }
            set { m_DSList = value; }
        }

        public DateTime DateProcessed
        {
            get { return m_DateProcessed; }
            set { m_DateProcessed = value; }
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

        public int NumberDetectionStations
        {
            get { return m_NumberDetectionStations; }
            set { m_NumberDetectionStations = value; }
        }

        public string DetectionStations
        {
            get { return m_DetectionStations; }
            set { m_DetectionStations = value; }
        }

        public string RoadwayID
        {
            get { return m_RoadwayID; }
            set { m_RoadwayID = value; }
        }
        public int Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
        }
        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }
        public double BeginMM
        {
            get { return m_BeginMM; }
            set { m_BeginMM = value; }
        }
        public double EndMM
        {
            get { return m_EndMM; }
            set { m_EndMM = value; }
        }

        public string BeginCrossStreetName
        {
            get { return m_StartCrossStreetName; }
            set { m_StartCrossStreetName = value; }
        }
        public string EndCrossStreetName
        {
            get { return m_EndCrossStreetName; }
            set { m_EndCrossStreetName = value; }
        }
        public int UpstreamLinkID
        {
            get { return m_UpstreamLinkID; }
            set { m_UpstreamLinkID = value; }
        }
        public int DownStreamLinkID
        {
            get { return m_DownStreamLinkID; }
            set { m_DownStreamLinkID = value; }
        }
        public int NumberLanes
        {
            get { return m_NumberLanes; }
            set { m_NumberLanes = value; }
        }
        public double SpeedLimit
        {
            get { return m_SpeedLimit; }
            set { m_SpeedLimit = value; }
        }
        public int VSLSignID
        {
            get { return m_VSLSignID; }
            set { m_VSLSignID = value; }
        }
        public string VSLSignaName
        {
            get { return m_VSLSignaName; }
            set { m_VSLSignaName = value; }
        }
        public int DMSID
        {
            get { return m_DMSID; }
            set { m_DMSID = value; }
        }
        public string DMSName
        {
            get { return m_DMSName; }
            set { m_DMSName = value; }
        }
        public int RSEID
        {
            get { return m_RSEID; }
            set { m_RSEID = value; }
        }
        public string RSEName
        {
            get { return m_RSEName; }
            set { m_RSEName = value; }
        }
        public int Volume
        {
            get { return m_Volume; }
            set { m_Volume = value; }
        }
        public double TSSAvgSpeed
        {
            get { return m_TSSAvgSpeed; }
            set { m_TSSAvgSpeed = value; }
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
        public double Occupancy
        {
            get { return m_Occupancy; }
            set { m_Occupancy = value; }
        }

        public double WRTMSpeed
        {
            get { return m_WRTMSpeed; }
            set { m_WRTMSpeed = value; }
        }
        public double RecommendedSpeed 
        {
            get { return m_RecommendedSpeed; }
            set { m_RecommendedSpeed = value; }
        }
        public clsEnums.enRecommendedSpeedSource RecommendedSpeedSource
        {
            get { return m_RecommendedSpeedSource; }
            set { m_RecommendedSpeedSource = value; }
        }

        ~clsRoadwayLink()
        {
        }
    }
}
