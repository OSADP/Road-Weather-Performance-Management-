using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsTroupe
    {
        private double m_MaxSpeed;
        private double m_MinSpeed;
        private double m_AvgSpeed;
        private double m_Length;
        private double m_TravelTime;
        private int m_NumberSubLinks;
        private int m_StartSubLinkID;
        private int m_EndSubLinkID;
        private double m_StartSubLinkMM;
        private double m_EndSubLinkMM;
        List<clsRoadwaySubLink> m_SubLinks;

        public double MaxSpeed
        {
            get { return m_MaxSpeed; }
            set { m_MaxSpeed = value; }
        }
        public double MinSpeed
        {
            get { return m_MinSpeed; }
            set { m_MinSpeed = value; }
        }
        public double AvgSpeed
        {
            get { return m_AvgSpeed; }
            set { m_AvgSpeed = value; }
        }
        public double Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }
        public double TravelTime
        {
            get { return m_TravelTime; }
            set { m_TravelTime = value; }
        }
        public int NumberSubLinks
        {
            get { return m_NumberSubLinks; }
            set { m_NumberSubLinks = value; }
        }
        public int StartSubLinkID
        {
            get { return m_StartSubLinkID; }
            set { m_StartSubLinkID = value; }
        }
        public int EndSubLinkID
        {
            get { return m_EndSubLinkID; }
            set { m_EndSubLinkID = value; }
        }
        public double StartSubLinkMM
        {
            get { return m_StartSubLinkMM; }
            set { m_StartSubLinkMM = value; }
        }
        public double EndSubLinkMM
        {
            get { return m_EndSubLinkMM; }
            set { m_EndSubLinkMM = value; }
        }
        public string CalculateTroupeAvgSpeed()
        {
            string retValue = string.Empty;

            double TotalSpeed = 0;
            try
            {

                for (int j = 0; j < m_SubLinks.Count; j++)
                {
                    TotalSpeed = TotalSpeed + m_SubLinks[j].RecommendedSpeed;
                }
                m_AvgSpeed = TotalSpeed / m_SubLinks.Count;
                if ((m_AvgSpeed % 5) > 0)
                {
                    m_AvgSpeed = (int)((m_AvgSpeed + 5) / 5) * 5;
                }
            }
            catch (Exception ex)
            {
                retValue = "Error in calculating Troupe AvgSpeed.\r\n\t" + ex.Message;
            }
            return retValue;
        }
        public string CalculateTroupeLength()
        {
            string retValue = string.Empty;

            try
            {
                m_Length = m_SubLinks.Count * clsGlobalVars.SubLinkLength;
            }
            catch (Exception ex)
            {
                retValue = "Error in calculating Troupe Length.\r\n\t" + ex.Message;
            }
            return retValue;
        }
        public string CalculateTroupeTravelTime()
        {
            string retValue = string.Empty;

            try
            {
                m_TravelTime = (m_Length * 3600.0) / (m_AvgSpeed); //travel time of the troupe based on the average speed of troupe and length of troupe
            }
            catch (Exception ex)
            {
                retValue = "Error in calculating Troupe Length.\r\n\t" + ex.Message;
            }
            return retValue;
        }
        public List<clsRoadwaySubLink> SubLinks
        {
            get { return m_SubLinks; }
            set { m_SubLinks = value; }
        }
        public clsTroupe()
        {
            m_SubLinks = new List<clsRoadwaySubLink>();
            m_AvgSpeed = 0;
            m_Length = 0;
            m_TravelTime = 0;
            m_MaxSpeed = 0;
            m_MinSpeed = 0;
            m_NumberSubLinks = 0;
            m_StartSubLinkID = 0;
            m_EndSubLinkID = 0;
            m_StartSubLinkMM = 0;
            m_EndSubLinkMM = 0;
        }
        ~clsTroupe()
        {
            m_SubLinks = null;
        }
    }
}
