using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsDetectionZoneWSDOT
    {
        //Static Members
        private string m_Identifier;
        private int m_DSIdentifier;
        private int m_RoadwayIdentifier;
        //private string m_Type;  //Detection zone type: SingleLoop, Loops SppedTrap, Radar, etc.
        private string m_DZType;
        private string m_DataType;  //Measured, Calculated
        private double m_MMLocation;
        private clsEnums.enDirection m_Direction;
        //private int m_DetectorStationNo;
        private string m_Location;  //
        private string m_Milepost;
        private string m_Lat;
        private string m_Lon;
        private int m_ArrayIndex;
        private bool m_Found;

        //Dynamic members
        private string m_DZStatus;
        private DateTime m_DateReceived;
        private DateTime m_BeginTime;
        private DateTime m_EndTime;
        private int m_StartInterval;
        private int m_EndInterval;
        private int m_IntervalLength;
        private int m_Volume;
        private double m_Occupancy;
        private int m_Speed;
        private int m_CalcSpeed;
        private double m_AvgSpeed;
        private bool m_Queued;
        private bool m_Congested;
        private int m_FlagGoodBad;
        private int m_RTLanes;
        private int m_Periods;
        private byte m_Congestion;
        private string m_TimeStamp;

        private int m_NumberNoNewDataIntervals;
        private bool m_NoNewData;

        //private string m_Name;  //
        //private int m_ID;
        private string m_Description;  //
        private string m_LaneType;
        private string m_Lanes;
        //private int m_Vol;
        //private int m_Occ;

        public string Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
        }
        public int DSIdentifier
        {
            get { return m_DSIdentifier; }
            set { m_DSIdentifier = value; }
        }
        public int RoadwayIdentifier
        {
            get { return m_RoadwayIdentifier; }
            set { m_RoadwayIdentifier = value; }
        }
        public string DZStatus
        {
            get { return m_DZStatus; }
            set { m_DZStatus = value; }
        }
        public string DZType
        {
            get { return m_DZType; }
            set { m_DZType = value; }
        }
        public string DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }
        public string Lanes
        {
            get { return m_Lanes; }
            set { m_Lanes = value; }
        }
        public string LaneType
        {
            get { return m_LaneType; }
            set { m_LaneType = value; }
        }
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        public bool Found
        {
            get { return m_Found; }
            set { m_Found = value; }
        }
        public string Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }
        public double MMLocation
        {
            get { return m_MMLocation; }
            set { m_MMLocation = value; }
        }
        public int ArrayIndex
        {
            get { return m_ArrayIndex; }
            set { m_ArrayIndex = value; }
        }
        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        //public string DZStatus
        //{
        //    get { return m_DZStatus; }
        //    set { m_DZStatus = value; }
       // }
        public int IntervalLength
        {
            get { return m_IntervalLength; }
            set { m_IntervalLength = value; }
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
        public int Speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        public int CalcSpeed
        {
            get { return m_CalcSpeed; }
            set { m_CalcSpeed = value; }
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

        public bool NoNewData
        {
            get { return m_NoNewData; }
            set { m_NoNewData = value; }
        }

        public int NumberNoNewDataIntervals
        {
            get { return m_NumberNoNewDataIntervals; }
            set { m_NumberNoNewDataIntervals = value; }
        }
        public string Milepost
        {
            get { return m_Milepost; }
            set { m_Milepost = value; }
        }
        public string Lat
        {
            get { return m_Lat; }
            set { m_Lat = value; }
        }
        public string Lon
        {
            get { return m_Lon; }
            set { m_Lon = value; }
        }
        public int FlagGoodBad
        {
            get { return m_FlagGoodBad; }
            set { m_FlagGoodBad = value; }
        }
        public int RTLanes
        {
            get { return m_RTLanes; }
            set { m_RTLanes = value; }
        }
        public int Periods
        {
            get { return m_Periods; }
            set { m_Periods = value; }
        }
        public byte Congestion
        {
            get { return m_Congestion; }
            set { m_Congestion = value; }
        }
        public string TimeStamp
        {
            get { return m_TimeStamp; }
            set { m_TimeStamp = value; }
        }


        public clsDetectionZoneWSDOT()
        {
        }

        ~clsDetectionZoneWSDOT()
        {
        }

    }
    public class clsDetectionZone
    {
        //static members
        //Modified by Hassan Charara to convert m_Identifier from int to string
        private string m_Identifier;
        private int m_DSIdentifier;
        private int m_RoadwayIdentifier;
        private string m_Type;  //Detection zone type: loops, radar, etc.
        private string m_DataType;  //Real-time data, historical, etc.
        private int m_LaneNumber;
        private string m_LaneType;
        private string m_LaneDesc;
        private double m_MMLocation;
        private clsEnums.enDirection m_Direction;

        //Dynamic members
        private string m_DZStatus;
        private DateTime m_DateReceived;
        private DateTime m_BeginTime;
        private DateTime m_EndTime;
        private int m_StartInterval;
        private int m_EndInterval;
        private int m_IntervalLength;
        private int m_Volume;
        private double m_Occupancy;
        private double m_AvgSpeed;
        private bool m_Queued;
        private bool m_Congested;

        private int m_NumberNoNewDataIntervals;
        private bool m_NoNewData;

        public string Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
        }
        public int DSIdentifier
        {
            get { return m_DSIdentifier; }
            set { m_DSIdentifier = value; }
        }
        public int RoadwayIdentifier
        {
            get { return m_RoadwayIdentifier; }
            set { m_RoadwayIdentifier = value; }
        }
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        public string DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }
        public int LaneNo
        {
            get { return m_LaneNumber; }
            set { m_LaneNumber = value; }
        }
        public string LaneType
        {
            get { return m_LaneType; }
            set { m_LaneType = value; }
        }
        public string LaneDesc
        {
            get { return m_LaneDesc; }
            set { m_LaneDesc = value; }
        }
        public double MMLocation
        {
            get { return m_MMLocation; }
            set { m_MMLocation = value; }
        }
        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public string DZStatus
        {
            get { return m_DZStatus; }
            set { m_DZStatus = value; }
        }
        public int IntervalLength
        {
            get { return m_IntervalLength; }
            set { m_IntervalLength = value; }
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

        public bool NoNewData
        {
            get { return m_NoNewData; }
            set { m_NoNewData = value; }
        }

        public int NumberNoNewDataIntervals
        {
            get { return m_NumberNoNewDataIntervals; }
            set { m_NumberNoNewDataIntervals = value; }
        }


        public clsDetectionZone()
        {
        }

        ~clsDetectionZone()
        {
        }
    }
}
