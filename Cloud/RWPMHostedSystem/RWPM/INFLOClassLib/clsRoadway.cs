using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsRoadway
    {
        private string m_Identifier;
        private string m_Name;
        private string m_StrDir;    //direction as a text field
        private clsEnums.enDirection m_Direction;
        private double m_BeginMM;       //Roadway monitored section beginning mile marker
        private double m_EndMM;       //Roadway monitored section beginning mile marker
        private double m_Grade;
        private double m_LowerHeading;
        private double m_UpperHeading;
        private double m_RecurringCongestionMMLocation; //Location of recurring congestion on roadway by direction
        private clsEnums.enDirection m_MMIncreasingDirection;
        private List<clsRoadwayLink> m_RoadwayLinksList;
        
        public List<clsRoadwayLink> RoadwayLinksList
        {
            get { return m_RoadwayLinksList; }
            set { m_RoadwayLinksList = value; }
        }

        public string Identifier
        {
            get { return m_Identifier; }
            set { m_Identifier = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public clsEnums.enDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }
        public clsEnums.enDirection MMIncreasingDirection
        {
            get { return m_MMIncreasingDirection; }
            set { m_MMIncreasingDirection = value; }
        }
        public string StrDir
        {
            get { return m_StrDir; }
            set { m_StrDir = value; }
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
        public double Grade
        {
            get { return m_Grade; }
            set { m_Grade = value; }
        }
        public double RecurringCongestionMMLocation
        {
            get { return m_RecurringCongestionMMLocation; }
            set { m_RecurringCongestionMMLocation = value; }
        }

        public double LowerHeading
        {
            get { return m_LowerHeading; }
            set { m_LowerHeading = value; }
        }

        public double UpperHeading  
        {
            get { return m_UpperHeading; }
            set { m_UpperHeading = value; }
        }

        public clsRoadway()
        {
        }
        
        ~clsRoadway()
        {
        }
    }
}
