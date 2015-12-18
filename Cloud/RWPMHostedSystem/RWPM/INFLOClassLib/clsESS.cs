using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsESS
    {
        private int m_Identifier;
        private double m_Latitude;
        private double m_Longitude;
        private double m_MileMarker;
        private string m_ESSOperationType;
        private string m_ESSMobilityType;
        private DateTime m_TimeReceived;
        private DateTime m_TimeGenerated; 

        public int Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
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
        public double MileMarker
        {
            get { return m_MileMarker; }
            set { m_MileMarker = value; }
        }
        public string ESSOperationType
        {
            get { return m_ESSOperationType; }
            set { m_ESSOperationType = value; }
        }

        public clsESS()
        {
        }

        ~clsESS()
        {
        }
    }
}
