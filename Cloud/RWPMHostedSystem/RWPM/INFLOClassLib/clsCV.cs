using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    class clsCV
    {
        private int m_NomadicDeviceID;
        private DateTime TimeGenerated;
        private DateTime TimeReceived;
        private DateTime TimeProcessed;
        private double m_Speed;
        private double m_Heading;
        private double m_MMLocation;  //Milemarker location
        private double m_Latitude;
        private double m_Longitude;
        private double m_LateralAcceleration;
        private double m_LongitudinalAcceleration;
        private bool m_Queued;
        private string m_RoadwayName;
        private double m_CoefficientFriction;
        private double m_Temperature;
        private clsEnums.enDirection m_Direction;

        public int NomadicDeviceID
        {
            get { return m_NomadicDeviceID; }
            set { m_NomadicDeviceID = value; }
        }
        public double Speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        public double Heading
        {
            get { return m_Heading; }
            set { m_Heading = value; }
        }
        public double MMLocation
        {
            get { return m_MMLocation; }
            set { m_MMLocation = value; }
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
        public double LateralAcceleration
        {
            get { return m_LateralAcceleration; }
            set { m_LateralAcceleration = value; }
        }
        public double LongitudinalAcceleration
        {
            get { return m_LongitudinalAcceleration; }
            set { m_LongitudinalAcceleration = value; }
        }
        public bool Queued
        {
            get { return m_Queued; }
            set { m_Queued = value; }
        }
        public string RoadwayName
        {
            get { return m_RoadwayName; }
            set { m_RoadwayName = value; }
        }
        public double CoefficientFriction
        {
            get { return m_CoefficientFriction; }
            set { m_CoefficientFriction = value; }
        }
        public double Temperature
        {
            get { return m_Temperature; }
            set { m_Temperature = value; }
        }

        public clsCV()
        {
            m_Speed = 0;
            m_Heading = 0;
            m_MMLocation = 0;
            m_Latitude = 0;
            m_Longitude = 0;
            m_Queued = false;
            m_RoadwayName = string.Empty;
            m_CoefficientFriction = 0;
            m_Temperature = 0;
        }

        ~clsCV()
        {
        }
    }
}
