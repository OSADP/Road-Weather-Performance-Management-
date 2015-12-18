using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsMileMarker
    {
        private string m_RoadwayId;
        private double m_MileMarker;
        private clsEnums.enDirection m_Direction;
        private double m_Latitude1;
        private double m_Longitude1;
        private double m_Latitude2;
        private double m_Longitude2;

        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }
        public string RoadwayId
        {
            get { return m_RoadwayId; }
            set { m_RoadwayId = value; }
        }
        public double Latitude1
        {
            get { return m_Latitude1; }
            set { m_Latitude1 = value; }
        }
        public double Longitude1
        {
            get { return m_Longitude1; }
            set { m_Longitude1 = value; }
        }
        public double Latitude2
        {
            get { return m_Latitude2; }
            set { m_Latitude2 = value; }
        }
        public double Longitude2
        {
            get { return m_Longitude2; }
            set { m_Longitude2 = value; }
        }
        public double MileMarker
        {
            get { return m_MileMarker; }
            set { m_MileMarker = value; }
        }

        public clsMileMarker()
        {
            RoadwayId = string.Empty;
            m_Latitude1 = 0;
            m_Longitude1 = 0;
            m_Latitude2 = 0;
            m_Longitude2 = 0;
            m_MileMarker = 0;
            m_Direction = clsEnums.enDirection.NA;
        }

        ~clsMileMarker()
        {
        }
    }
}
