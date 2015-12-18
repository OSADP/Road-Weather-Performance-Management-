using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsCVData
    {
        private string m_NomadicDeviceID;
        private string m_RoadwayID;
        private string m_LinkID;
        private string m_SublinkID;
        private clsEnums.enDirection m_Direction;
        private DateTime m_DateGenerated;
        private double m_Speed;
        private double m_Heading;
        private double m_Latitude;
        private double m_Longitude;
        private double m_LateralAcceleration;
        private double m_LongitudinalAcceleration;
        private double m_MMLocation;
        private bool m_Queued;
        private double m_CoefficientFriction;
        private double m_Temperature;

        public string NomadicDeviceID
        {
            get { return m_NomadicDeviceID; }
            set { m_NomadicDeviceID = value; }
        }
        public string RoadwayID
        {
            get { return m_RoadwayID; }
            set { m_RoadwayID = value; }
        }
        public string LinkID
        {
            get { return m_LinkID; }
            set { m_LinkID = value; }
        }
        public string SublinkID
        {
            get { return m_SublinkID; }
            set { m_SublinkID = value; }
        }
        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public bool Queued
        {
            get { return m_Queued; }
            set { m_Queued = value; }
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
        public double MMLocation
        {
            get { return m_MMLocation; }
            set { m_MMLocation = value; }
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
        public DateTime DateGenerated
        {
            get { return m_DateGenerated; }
            set { m_DateGenerated = value; }
        }

        public clsCVData()
        {
        }

        ~clsCVData()
        {
        }
    }
}
