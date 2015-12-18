using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public class clsRoadwaySubLink
    {
        private string m_RoadwayID;
        private int m_Identifier;
        private double m_BeginMM;
        private double m_EndMM;
        private int m_NumberLanes;
        private double m_SpeedLimit;
        private int m_SmoothedSpeedIndex;
        private double[] m_SmoothedSpeed;
        private clsEnums.enDirection m_Direction;

        private bool m_Queued;
        private bool m_Congested;
        private int m_TotalNumberCVs;
        private int m_PrevTotalNumberCVs;
        private int m_VolumeDiff;
        private double m_Density;
        private double m_PrevDensity;
        private double m_FlowRate;
        private double m_DensityDiff;
        private double m_ShockWaveRate;
        private int m_NumberQueuedCVs;
        private double m_PercentQueuedCVs;

        private DateTime m_DateProcessed;
        private DateTime m_CVDateProcessed;
        private DateTime m_TSSDateProcessed;
        private DateTime m_WRTMDateProcessed;
        private DateTime m_TroupeProcessed;
        private DateTime m_RecommendationDate;

        private double m_CVAvgSpeed;
        private double m_TSSAvgSpeed;
        private double m_WRTMSpeed;
        private double m_TroupeSpeed;
        private double m_HarmonizedSpeed;
        private bool m_TroupeInclusionOverride;
        private bool m_BeginTroupe;
        private bool m_BeginSpdHarm;
        private bool m_SpdHarmInclusionOverride;
        private double m_RecommendedSpeed;
        private clsEnums.enRecommendedSpeedSource m_RecommendedSpeedSource;

        private List<clsCVData> m_CVList;

        private int m_VSLSignID;
        private string m_VSLSignaName;
        private int m_DMSID;
        private string m_DMSName;
        private int m_RSEID;
        private string m_RSEName;

        public clsRoadwaySubLink(int SmoothedSpeedArraySize)
        {
            m_SmoothedSpeed = new double[SmoothedSpeedArraySize];
        }
        public clsRoadwaySubLink()
        {

        }

        public List<clsCVData> CVList
        {
            get { return m_CVList; }
            set { m_CVList = value; }
        }
        public double PercentQueuedCVs
        {
            get { return m_PercentQueuedCVs; }
            set { m_PercentQueuedCVs = value; }
        }
        public int SmoothedSpeedIndex
        {
            get { return m_SmoothedSpeedIndex; }
            set { m_SmoothedSpeedIndex = value; }
        }
        public double[] SmoothedSpeed
        {
            get { return m_SmoothedSpeed; }
            set { m_SmoothedSpeed = value; }
        }
        public DateTime DateProcessed
        {
            get { return m_DateProcessed; }
            set { m_DateProcessed = value; }
        }
        public DateTime CVDateProcessed
        {
            get { return m_CVDateProcessed; }
            set { m_CVDateProcessed = value; }
        }
        public DateTime TSSDateProcessed
        {
            get { return m_TSSDateProcessed; }
            set { m_TSSDateProcessed = value; }
        }
        public DateTime WRTMDateProcessed
        {
            get { return m_WRTMDateProcessed; }
            set { m_WRTMDateProcessed = value; }
        }
        public DateTime TroupeProcessed
        {
            get { return m_TroupeProcessed; }
            set { m_TroupeProcessed = value; }
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
        public DateTime RecommendationDate
        {
            get { return m_RecommendationDate; }
            set { m_RecommendationDate = value; }
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
        public double CVAvgSpeed
        {
            get { return m_CVAvgSpeed; }
            set { m_CVAvgSpeed = value; }
        }
        public double TSSAvgSpeed
        {
            get { return m_TSSAvgSpeed; }
            set { m_TSSAvgSpeed = value; }
        }
        public double WRTMSpeed
        {
            get { return m_WRTMSpeed; }
            set { m_WRTMSpeed = value; }
        }
        public double TroupeSpeed
        {
            get { return m_TroupeSpeed; }
            set { m_TroupeSpeed = value; }
        }
        public double HarmonizedSpeed
        {
            get { return m_HarmonizedSpeed; }
            set { m_HarmonizedSpeed = value; }
        }
        public bool TroupeInclusionOverride
        {
            get { return m_TroupeInclusionOverride; }
            set { m_TroupeInclusionOverride = value; }
        }
        public bool SpdHarmInclusionOverride
        {
            get { return m_SpdHarmInclusionOverride; }
            set { m_SpdHarmInclusionOverride = value; }
        }
        public bool BeginTroupe
        {
            get { return m_BeginTroupe; }
            set { m_BeginTroupe = value; }
        }
        public bool BeginSpdHarm
        {
            get { return m_BeginSpdHarm; }
            set { m_BeginSpdHarm = value; }
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
        public int TotalNumberCVs
        {
            get { return m_TotalNumberCVs; }
            set { m_TotalNumberCVs = value; }
        }
        public int PrevTotalNumberCVs
        {
            get { return m_PrevTotalNumberCVs; }
            set { m_PrevTotalNumberCVs = value; }
        }

        public int VolumeDiff
        {
            get { return m_VolumeDiff; }
            set { m_VolumeDiff = value; }
        }
        public double Density
        {
            get { return m_Density; }
            set { m_Density = value; }
        }
        public double PrevDensity
        {
            get { return m_PrevDensity; }
            set { m_PrevDensity = value; }
        }
        public double FlowRate
        {
            get { return m_FlowRate; }
            set { m_FlowRate = value; }
        }
        public double DensityDiff
        {
            get { return m_DensityDiff; }
            set { m_DensityDiff = value; }
        }
        public double ShockWaveRate
        {
            get { return m_ShockWaveRate; }
            set { m_ShockWaveRate = value; }
        }

        public int NumberQueuedCVs
        {
            get { return m_NumberQueuedCVs; }
            set { m_NumberQueuedCVs = value; }
        }
        /*public int NumberCongestedCVs
        {
            get { return m_NumberCongestedCVs; }
            set { m_NumberCongestedCVs = value; }
        }*/

        ~clsRoadwaySubLink()
        {
        }
}
}
