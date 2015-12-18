using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INFLOClassLib;
using InfloCommon.Repositories;
using Ninject;
using SharpRepository.Repository;
using InfloCommon;
using System.Data;
using InfloCommon.Models; ///for dataSet

namespace DataProcessorWorkerRole
{
    /// <summary>
    /// This class largely ported over from desktop "InfloApps" GUI application.
    /// </summary>
    public class DataProcessing
    {
        static int a = 0;

        public DataProcessing(string infloConfigXml)
        {//string infloConfigXml = "";
            a++;
            Init();
            chkFilterQueues.Checked = true;  //What was this for on the GUI?
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "Initializing DataProcessing");
            frmINFLOApps_Load(infloConfigXml);
        }

        private Display.MimicCheckBox chkFilterQueues = new Display.MimicCheckBox();

        //private double FeetPerMile = 5280;

        //clsDatabase related variables
        //private CExcelDoc m_ExcelDoc = new CExcelDoc();


        /// <summary>
        /// 3 different kinds of data are dumped to excel.
        /// </summary>
        enum LogToExcelType
        {
            TSS,
            CVSPD,
            CV,
            CVNoData
        }

        private string LogDirectoryPath = string.Empty;

        private long CVDataHarmonization = 0;
        private long TSSDataHarmonization = 0;
        private clsDatabase DB;

        //private long Interval = 0;

        //Configuration files variables
        private string INFLOConfigFile = string.Empty;

        //Roadway entity lists
        private clsRoadway Roadway = new clsRoadway();
        private List<clsRoadway> RList = new List<clsRoadway>();
        private List<clsRoadwayLink> RLList = new List<clsRoadwayLink>();
        public List<clsRoadwaySubLink> RSLList = new List<clsRoadwaySubLink>();
        private List<clsDetectorStation> DSList = new List<clsDetectorStation>();
        private List<clsDetectionZoneWSDOT> DZList = new List<clsDetectionZoneWSDOT>();
        //private List<clsRoadway> tmpRList = new List<clsRoadway>();
        private List<clsRoadwayLink> tmpRLList = new List<clsRoadwayLink>();
        private List<clsRoadwaySubLink> tmpRSLList = new List<clsRoadwaySubLink>();
        private List<clsDetectorStation> tmpDSList = new List<clsDetectorStation>();
        private List<clsDetectionZoneWSDOT> tmpDZList = new List<clsDetectionZoneWSDOT>();






        private long CVDataFileCounter = 0;


        private int NumberNoCVDataIntervals = 0;
        private int NumberNoTSSDataIntervals = 0;

        private DateTime CVPrevWakeupTime = DateTime.Now;
        private DateTime CVCurrWakeupTime = DateTime.Now;
        private double CVTimeDiff = 0;

        private DateTime TSSPrevWakeupTime = DateTime.Now;
        private DateTime TSSCurrWakeupTime = DateTime.Now;
        private double TSSTimeDiff = 0;

        private clsRoadwaySubLink QueuedSubLink;
        private clsRoadwayLink QueuedLink;

        //CV Data Aggregator related variables
        private List<clsCVData> CurrIntervalCVList = new List<clsCVData>();
        private DateTime DateGenerated = DateTime.Now;

        //TSS data aggregator interval related variables
        private List<clsDetectionZone> CurrIntervalTSSDataList = new List<clsDetectionZone>();

        private double TroupingEndMM = 0;
        private double TroupingEndSpeed = 0;

        private Display DisplayForm = new Display();

        private bool Stopped = false;


        private void LogToExcel(LogToExcelType type, DateTime currDateTime)
        { }
        private string InitializeExcelWorksheets(string retValue)
        { return ""; }
        /*
             #region ExcelLogging
                     //private List<Microsoft.Office.Interop.Excel.Worksheet> m_worksheets;
             private Microsoft.Office.Interop.Excel.Range m_workSheet_range = null;

             private int m_row;
             private int m_col;
             private int m_startupRow;
             private int m_startupColomn;

             //Excel
             Microsoft.Office.Interop.Excel.Application CVQWARNExcelApp = new Microsoft.Office.Interop.Excel.Application();
             Microsoft.Office.Interop.Excel.Workbook CVWorkbook = null;
             Microsoft.Office.Interop.Excel.Worksheet[] CVWorkSheets = new Microsoft.Office.Interop.Excel.Worksheet[3];

             Microsoft.Office.Interop.Excel.Application CVSPDHarmExcelApp = new Microsoft.Office.Interop.Excel.Application();
             Microsoft.Office.Interop.Excel.Workbook CVSPDHarmWorkbook = null;
             Microsoft.Office.Interop.Excel.Worksheet[] CVSPDHarmWorkSheets = new Microsoft.Office.Interop.Excel.Worksheet[3];

             Microsoft.Office.Interop.Excel.Application TSSQWARNExcelApp = new Microsoft.Office.Interop.Excel.Application();
             Microsoft.Office.Interop.Excel.Workbook TSSWorkbook = null;
             Microsoft.Office.Interop.Excel.Worksheet[] TSSWorkSheets = new Microsoft.Office.Interop.Excel.Worksheet[3];

             private int TSSWSCurrRow = 0;
             private int CVWSCurrRow = 0;
             private int CVSPDHarmWSCurrRow = 0;

             private void LogToExcel(LogToExcelType type, DateTime currDateTime)
             {
                 //Abort if logging has been disabled.
                 if (chkExcelDataLogging.Checked == false) return;

                 //See what type of data we are logging to which file.
                 if (type == LogToExcelType.CV)
                 {
                     //Excel - Log Sublink CV speed, CV % queued vehicles, CV # Queued vehicles, and CV Total # vehicles into Excel worksheet
                     Color myPurpleColor = Color.FromArgb(255, 192, 255);
                     #region "Log Sublink CV speed, CV % queued vehicles, CV # Queued vehicles, and CV Total # vehicles into Excel worksheet"
                     int s = 0;
                     CVWorkSheets[1].Cells[CVWSCurrRow, 1] = currDateTime;
                     foreach (clsRoadwaySubLink rsl in RSLList)
                     {
                         //Excel
                         CVWorkSheets[1].Cells[CVWSCurrRow, s + 2] = rsl.CVAvgSpeed.ToString("0") + "::" + rsl.PercentQueuedCVs.ToString("0") + "::" + rsl.NumberQueuedCVs.ToString("0") + "::" + rsl.TotalNumberCVs;
                         if (rsl.Queued == true)
                         {
                             CVWorkSheets[1].Cells[CVWSCurrRow, s + 2].Interior.Color = System.Drawing.Color.Red;
                         }
                         else
                         {
                             CVWorkSheets[1].Cells[CVWSCurrRow, s + 2].Interior.Color = Color.LightGreen; // myPurpleColor;
                         }
                         s = s + 1;
                     }
                     CVWSCurrRow = CVWSCurrRow + 1;
                     #endregion
                 }
                 else if (type == LogToExcelType.CVNoData)
                 {
                     #region "Update Sublink CV data Excel worksheet to indicate no CV Data was found "
                     CVWorkSheets[1].Cells[CVWSCurrRow, 1] = currDateTime;
                     CVWorkSheets[1].Cells[CVWSCurrRow, 2] = "No CV Data was found. Number of No CV Data Intervals = " + NumberNoCVDataIntervals;

                     CVWSCurrRow = CVWSCurrRow + 1;
                     #endregion
                 }
                 #region "Excel Data Logging"
                 else if (type == LogToExcelType.TSS)
                 {
                     //Excel
                     TSSWorkSheets[1].Cells[TSSWSCurrRow, 1] = currDateTime;
                     for (int s = 0; s < RLList.Count; s++)
                     {
                         //Excel.Range tmpRange = (Excel.Range)TSSWorkSheets[1].Range[TSSWorkSheets[1].Cells[TSSWSCurrRow, (s * 5) + 4], TSSWorkSheets[1].Cells[TSSWSCurrRow, (s + 1) * 5 + 3]];
                         //tmpRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                         //tmpRange.MergeCells = true;
                         TSSWorkSheets[1].Cells[TSSWSCurrRow, s + 4] = RLList[s].TSSAvgSpeed.ToString("0"); // +"-" + RLList[s].Queued;
                         if (RLList[s].Queued == true)
                         {
                             TSSWorkSheets[1].Cells[TSSWSCurrRow, s + 4].Interior.Color = System.Drawing.Color.Red;
                         }
                         else if (RLList[s].Congested == true)
                         {
                             TSSWorkSheets[1].Cells[TSSWSCurrRow, s + 4].Interior.Color = System.Drawing.Color.Orange;
                         }
                         else
                         {
                             TSSWorkSheets[1].Cells[TSSWSCurrRow, s + 4].Interior.Color = System.Drawing.Color.LightGreen;
                         }
                         TSSWorkSheets[1].Cells[TSSWSCurrRow, 2] = RLList[s].StartInterval;
                         TSSWorkSheets[1].Cells[TSSWSCurrRow, 3] = RLList[s].EndInterval;
                     }
                     TSSWorkSheets[1].Range[TSSWorkSheets[1].Cells[TSSWSCurrRow, 1], TSSWorkSheets[1].Cells[TSSWSCurrRow, RLList.Count + 4]].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                     TSSWorkSheets[1].Range[TSSWorkSheets[1].Cells[TSSWSCurrRow, 1], TSSWorkSheets[1].Cells[TSSWSCurrRow, RLList.Count + 4]].Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                     TSSWSCurrRow = TSSWSCurrRow + 1;
                 }
                 #endregion
                 if (type == LogToExcelType.CVSPD)
                 {
                     #region "Log Trouping and Harmonized sublink speed into Excel worksheet"
                     //Excel
                     int t = 0;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, 1] = currDateTime;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, 2] = "CVsSpd" + ":" + clsGlobalVars.BOQMMLocation + ":" + TroupingEndMM;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, 1] = currDateTime;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, 2] = "TSSSpd" + ":" + clsGlobalVars.BOQMMLocation + ":" + TroupingEndMM; ;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, 1] = currDateTime;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, 2] = "RecSpd" + ":" + clsGlobalVars.BOQMMLocation + ":" + TroupingEndMM; ;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, 1] = currDateTime;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, 2] = "TroSpd" + ":" + clsGlobalVars.BOQMMLocation + ":" + TroupingEndMM; ;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, 1] = currDateTime;
                     CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, 2] = "HarSpd" + ":" + clsGlobalVars.BOQMMLocation + ":" + TroupingEndMM; ;
                     foreach (clsRoadwaySubLink rsl in RSLList)
                     {
                         //Excel
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3] = rsl.CVAvgSpeed.ToString("0");
                         #region "CVSpeed Color"
                         if (rsl.CVAvgSpeed > 65.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.LightSeaGreen;
                         }
                         else if (rsl.CVAvgSpeed > 60.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.PaleTurquoise;
                         }
                         else if (rsl.CVAvgSpeed > 55.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.LimeGreen;
                         }
                         else if (rsl.CVAvgSpeed > 50.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.YellowGreen;
                         }
                         else if (rsl.CVAvgSpeed >= 45.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.Yellow;
                         }
                         else if (rsl.CVAvgSpeed > 40.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.Gold;
                         }
                         else if (rsl.CVAvgSpeed > 35.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.DarkOrange;
                         }
                         else if (rsl.CVAvgSpeed > 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.Tomato;
                         }
                         else if (rsl.CVAvgSpeed <= 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow, t + 3].Interior.Color = System.Drawing.Color.Red;
                         }
                         #endregion
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3] = rsl.TSSAvgSpeed.ToString("0");
                         #region "TSSSpeed Color"
                         if (rsl.TSSAvgSpeed > 65.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.LightSeaGreen;
                         }
                         else if (rsl.TSSAvgSpeed > 60.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.PaleTurquoise;
                         }
                         else if (rsl.TSSAvgSpeed > 55.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.LimeGreen;
                         }
                         else if (rsl.TSSAvgSpeed > 50.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.YellowGreen;
                         }
                         else if (rsl.TSSAvgSpeed > 45.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.Yellow;
                         }
                         else if (rsl.TSSAvgSpeed > 40.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.Gold;
                         }
                         else if (rsl.TSSAvgSpeed > 35.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.DarkOrange;
                         }
                         else if (rsl.TSSAvgSpeed > 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.Tomato;
                         }
                         else if (rsl.TSSAvgSpeed <= 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 1, t + 3].Interior.Color = System.Drawing.Color.Red;
                         }
                         #endregion
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3] = rsl.RecommendedSpeed.ToString("0");
                         #region "RecommendedSpeed Color"
                         if (rsl.RecommendedSpeed > 65.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.LightSeaGreen;
                         }
                         else if (rsl.RecommendedSpeed > 60.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.PaleTurquoise;
                         }
                         else if (rsl.RecommendedSpeed > 55.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.LimeGreen;
                         }
                         else if (rsl.RecommendedSpeed > 50.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.YellowGreen;
                         }
                         else if (rsl.RecommendedSpeed > 45.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.Yellow;
                         }
                         else if (rsl.RecommendedSpeed > 40.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.Gold;
                         }
                         else if (rsl.RecommendedSpeed > 35.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.DarkOrange;
                         }
                         else if (rsl.RecommendedSpeed > 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.Tomato;
                         }
                         else if (rsl.RecommendedSpeed <= 30.0)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 2, t + 3].Interior.Color = System.Drawing.Color.Red;
                         }
                         #endregion
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, t + 3] = rsl.TroupeSpeed.ToString("0");
                         #region "Troupe Color"
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, t + 3].Interior.Color = System.Drawing.Color.White;
                         if (rsl.TroupeInclusionOverride == true)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, t + 3].Interior.Color = System.Drawing.Color.Silver;
                         }
                         if (rsl.BeginTroupe == true)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 3, t + 3].Interior.Color = System.Drawing.Color.Pink;
                         }
                         #endregion
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, t + 3] = rsl.HarmonizedSpeed.ToString("0");
                         #region "Troupe Color"
                         CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, t + 3].Interior.Color = System.Drawing.Color.White;
                         if (rsl.SpdHarmInclusionOverride == true)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, t + 3].Interior.Color = System.Drawing.Color.Silver;
                         }
                         if (rsl.BeginSpdHarm == true)
                         {
                             CVSPDHarmWorkSheets[1].Cells[CVSPDHarmWSCurrRow + 4, t + 3].Interior.Color = System.Drawing.Color.Pink;
                         }
                         #endregion

                         t = t + 1;
                     }
                     //Excel
                     CVSPDHarmWSCurrRow = CVSPDHarmWSCurrRow + 6;
                     #endregion
                 }

             }
        
             private string InitializeExcelWorksheets(string retValue)
             {
                 //Excel
                 CVQWARNExcelApp.Visible = true;
                 CVSPDHarmExcelApp.Visible = true;
                 TSSQWARNExcelApp.Visible = true;
                 CVQWARNExcelApp.StandardFont = "Arial Narrow";
                 CVSPDHarmExcelApp.StandardFont = "Arial Narrow";
                 TSSQWARNExcelApp.StandardFont = "Arial Narrow";
                 CVQWARNExcelApp.StandardFontSize = 12;
                 CVSPDHarmExcelApp.StandardFontSize = 12;
                 TSSQWARNExcelApp.StandardFontSize = 12;

                 //Microsoft.Office.Interop.Excel.Workbook CVWorkbook = null;
                 //Microsoft.Office.Interop.Excel.Workbook TSSWorkbook = null;
                 CVWorkbook = CVQWARNExcelApp.Workbooks.Add(1);
                 CVSPDHarmWorkbook = CVSPDHarmExcelApp.Workbooks.Add(1);
                 TSSWorkbook = TSSQWARNExcelApp.Workbooks.Add(1);
                 //Microsoft.Office.Interop.Excel.Worksheet[] CVWorkSheets = new Microsoft.Office.Interop.Excel.Worksheet[3];
                 //Microsoft.Office.Interop.Excel.Worksheet[] TSSWorkSheets = new Microsoft.Office.Interop.Excel.Worksheet[3];



                 //Initialize Microsoft CV and TSS worksheets used to display link and sublink queue data
                 TSSWorkSheets[1] = TSSWorkbook.Worksheets.Add();
                 TSSWorkSheets[1].Name = "LinkQueuedStatus";//name of worksheet tab.
                 retValue = InitializeLinkQueuedStateWorksheet(ref TSSWorkSheets[1], RLList);
                 TSSWSCurrRow = 6;

                 CVSPDHarmWorkSheets[1] = CVSPDHarmWorkbook.Worksheets.Add();
                 CVSPDHarmWorkSheets[1].Name = "SublinkTroupeStatus";//name of worksheet tab.
                 retValue = InitializeSublinkTroupeWorksheet(ref CVSPDHarmWorkSheets[1], RSLList);
                 CVSPDHarmWSCurrRow = 6;

                 CVWorkSheets[1] = CVWorkbook.Worksheets.Add();
                 CVWorkSheets[1].Name = "SubLinkQueuedState";//name of worksheet tab.
                 retValue = InitializeSublinkQueuedStateWorksheet(ref CVWorkSheets[1], RSLList);
                 CVWSCurrRow = 6;
                 return retValue;
             }
             //Excel
             private string InitializeLinkQueuedStateWorksheet(ref Microsoft.Office.Interop.Excel.Worksheet TSSWS, List<clsRoadwayLink> RLList)
             {
                 string retValue = string.Empty;

                 try
                 {
                     //Header
                     TSSWS.Cells[1, 1] = "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name;
                     TSSWS.Cells[2, 1] = "Direction: " + Roadway.Direction; ;
                     TSSWS.Cells[3, 1] = "From MM: " + Roadway.BeginMM.ToString("0.00") + " To MM: " + Roadway.EndMM.ToString("0.00");
                     TSSWS.Cells[4, 1] = "Recurring Congestion MM: " + Roadway.RecurringCongestionMMLocation.ToString("0.00");

                     //Start of column headers.
                     TSSWS.Cells[5, 1] = "Date/Time";
                     TSSWS.Cells[5, 2] = "StartInterval";
                     TSSWS.Cells[5, 3] = "EndInterval";
                     #region "Original Code"
                     /*for (int i = 0; i < RLList.Count; i++)
                     {
                         Excel.Range tmpRange = (Excel.Range)TSSWS.Range[TSSWS.Cells[5, (i * 5) + 4], TSSWS.Cells[5, (i + 1) * 5 + 3]];
                         //tmpRange = tmpRange.Select();
                         //tmpRange.ReadingOrder
                         tmpRange.Orientation = 90;
                         tmpRange.RowHeight = 95;
                         //tmpRange.AddIndent = false;
                         //tmpRange.IndentLevel = 0;
                         tmpRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                         tmpRange.WrapText = false;
                         tmpRange.ShrinkToFit = false;
                         tmpRange.MergeCells = true;
                         TSSWS.Cells[5, (i * 5) + 4] = (i + 1) + " - " + RLList[i].BeginMM + "-TO-" + RLList[i].EndMM;
                         TSSWS.Cells[5, (i*5) + 4].Interior.Color = System.Drawing.Color.Orange;
                     }
                     //TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count]].Merge();
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Interior.Color = System.Drawing.Color.Orange;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Borders.Colorindex = 1;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Style.Font.Name = "Arial Narrow";
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Style.Font.Bold = true;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Style.Font.Size = 12;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count]].EntireRow.Autofit();
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].Rows.Autofit();
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].ColumnWidth = 3;
                     TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count * 5]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                     Excel.Range RowRange = (Excel.Range)TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, 3]];
                     RowRange.EntireColumn.ColumnWidth = 10;
                     RowRange.EntireRow.RowHeight = 95;*/
        /*
#endregion

for (int i = 0; i < RLList.Count; i++)
{
TSSWS.Cells[5, i + 4] = (i + 1).ToString() + " - " + RLList[i].BeginMM + "-TO-" + RLList[i].EndMM;
TSSWS.Cells[5, i + 4].Interior.Color = System.Drawing.Color.Orange;
}
//TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count]].Merge();
Excel.Range tmpRange = (Excel.Range)TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count + 3]];
tmpRange.Interior.Color = System.Drawing.Color.Orange;
tmpRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
tmpRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
tmpRange.Style.Font.Name = "Arial Narrow";
tmpRange.Style.Font.Bold = true;
tmpRange.Style.Font.Size = 12;
tmpRange.ColumnWidth = 3;
tmpRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
tmpRange.Orientation = 90;
tmpRange.RowHeight = 125;
Excel.Range RowRange = (Excel.Range)TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, 3]];
RowRange.EntireColumn.ColumnWidth = 10;
RowRange.EntireRow.RowHeight = 125;
TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count + 3]].Borders.Colorindex = 1;
//TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count + 3]].EntireRow.Autofit();
TSSWS.Range[TSSWS.Cells[5, 1], TSSWS.Cells[5, RLList.Count + 3]].Rows.Autofit();

}
catch (Exception ex)
{
retValue = "Error in intializing the TSS Link queued state Excel worksheet" + "\r\n\t" + ex.Message;
}

return retValue;
}
private string InitializeSublinkQueuedStateWorksheet(ref Microsoft.Office.Interop.Excel.Worksheet CVWS, List<clsRoadwaySubLink> RSLList)
{
string retValue = string.Empty;

CVWS.Cells[1, 1] = "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name;
CVWS.Cells[2, 1] = "Direction: " + Roadway.Direction; ;
CVWS.Cells[3, 1] = "From MM: " + Roadway.BeginMM.ToString("0.00") + " To MM: " + Roadway.EndMM.ToString("0.00");
CVWS.Cells[4, 1] = "Recurring Congestion MM: " + Roadway.RecurringCongestionMMLocation.ToString("0.00");

CVWS.Cells[5, 1] = "Date/Time";
//TSSWS.Cells[5, 1] = "Start Interval";
//TSSWS.Cells[5, 1] = "EndInterval";
try
{
for (int i = 0; i < RSLList.Count; i++)
{
CVWS.Cells[5, i+2] = (i + 1).ToString() + " - " + RSLList[i].BeginMM.ToString() +  "-TO-" + RSLList[i].EndMM.ToString();
                    
CVWS.Cells[5, i + 2].Interior.Color = System.Drawing.Color.Orange;
}
Excel.Range tmpRange = (Excel.Range)CVWS.Range[CVWS.Cells[5, 1], CVWS.Cells[5, RSLList.Count +1]];
tmpRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
tmpRange.WrapText = false;
tmpRange.Orientation = 90;
tmpRange.AddIndent = false;
tmpRange.IndentLevel = 0;
tmpRange.ShrinkToFit = false;
//tmpRange.ReadingOrder
//tmpRange.MergeCells = true;
tmpRange.Interior.Color = System.Drawing.Color.Orange;
tmpRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
tmpRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
tmpRange.Borders.ColorIndex = 1;
tmpRange.Style.Font.Name = "Arial Narrow";
tmpRange.Style.Font.Bold = true;
tmpRange.Style.Font.Size = 12;
tmpRange.EntireRow.AutoFit();
tmpRange.Rows.AutoFit();
tmpRange.ColumnWidth = 3;
tmpRange.RowHeight = 125;
Excel.Range RowRange = (Excel.Range)CVWS.Range[CVWS.Cells[5, 1], CVWS.Cells[1000, 1]];
RowRange.EntireColumn.ColumnWidth = 15;

}
catch (Exception ex)
{
retValue = "Error in intializing the CV SubLink queued state Excel worksheet" + "\r\n\t" + ex.Message; 
}


return retValue;
}
private string InitializeSublinkTroupeWorksheet(ref Microsoft.Office.Interop.Excel.Worksheet CVWS, List<clsRoadwaySubLink> RSLList)
{
string retValue = string.Empty;
CVWS.Cells[1, 1] = "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name;
CVWS.Cells[2, 1] = "Direction: " + Roadway.Direction; ;
CVWS.Cells[3, 1] = "From MM: " + Roadway.BeginMM.ToString("0.00") + " To MM: " + Roadway.EndMM.ToString("0.00");
CVWS.Cells[4, 1] = "Recurring Congestion MM: " + Roadway.RecurringCongestionMMLocation.ToString("0.00");

CVWS.Cells[5, 1] = "Date/Time";
CVWS.Cells[5, 2] = "BOQ/Trouping";
//TSSWS.Cells[5, 1] = "Start Interval";
//TSSWS.Cells[5, 1] = "EndInterval";
try
{
for (int i = 0; i < RSLList.Count; i++)
{
CVWS.Cells[5, i + 3] = (i + 1).ToString() + " - " + RSLList[i].BeginMM.ToString() + "-TO-" + RSLList[i].EndMM.ToString();

CVWS.Cells[5, i + 3].Interior.Color = System.Drawing.Color.Orange;
}
Excel.Range tmpRange = (Excel.Range)CVWS.Range[CVWS.Cells[5, 1], CVWS.Cells[5, RSLList.Count + 2]];
tmpRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
tmpRange.WrapText = false;
tmpRange.Orientation = 90;
tmpRange.AddIndent = false;
tmpRange.IndentLevel = 0;
tmpRange.ShrinkToFit = false;
//tmpRange.ReadingOrder
//tmpRange.MergeCells = true;
tmpRange.Interior.Color = System.Drawing.Color.Orange;
tmpRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
tmpRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
tmpRange.Borders.ColorIndex = 1;
tmpRange.Style.Font.Name = "Arial Narrow";
tmpRange.Style.Font.Bold = true;
tmpRange.Style.Font.Size = 12;
tmpRange.EntireRow.AutoFit();
tmpRange.Rows.AutoFit();
tmpRange.ColumnWidth = 3;
tmpRange.RowHeight = 100;
Excel.Range RowRange = (Excel.Range)CVWS.Range[CVWS.Cells[5, 1], CVWS.Cells[5, 2]];
RowRange.EntireColumn.ColumnWidth = 15;

}
catch (Exception ex)
{
retValue = "Error in intializing the CV SubLink queued state Excel worksheet" + "\r\n\t" + ex.Message;
}


return retValue;
}
#endregion

*/

        private void LogTxtMsg(LoggerType typeOfMsg, LoggerLevel level, string Text)
        {
            LoggingTableEntity logThis = new LoggingTableEntity(DateTime.Now, typeOfMsg, level, Text);

            //Log to cloud
            TableLogger.SubmitLogEntry(logThis);
        }


        private void frmINFLOApps_Load(string infloConfigFile)
        {
            string retValue = string.Empty;
            ////Declare Microsoft Excel workbooks used for displaying link and sublink data.
            ////Microsoft.Office.Interop.Excel.Application CVQWARNExcelApp = new Microsoft.Office.Interop.Excel.Application();
            ////Microsoft.Office.Interop.Excel.Application TSSQWARNExcelApp = new Microsoft.Office.Interop.Excel.Application();
            //DisplayForm.Show();

            ////DisplayForm.Refresh();

            //this.Show();
            //this.Refresh();



            //splitContainer1.SplitterDistance = 150;
            //splitContainer4.SplitterDistance = 50;
            //splitContainer5.SplitterDistance = 50;

            //string TodayDirectoryName = DateTime.Now.ToString("MM-dd-yyyy");
            //LogDirectoryPath = System.Windows.Forms.Application.StartupPath + "\\DataLog\\" + TodayDirectoryName;
            //Directory.CreateDirectory(LogDirectoryPath);


            #region "Read the INFLO configuration file and initialize the INFLO thresholds and parameters"
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\nRead the INFLO configuration file and intialize the INFLO algorithms thresholds and parameters.");

            INFLOConfigFile = infloConfigFile;// System.Windows.Forms.Application.StartupPath + "\\Config\\INFLOConfig.xml";
            //INFLOConfigFile = clsGlobalVars.INFLOConfigFile;

            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\nReading the contents of the INFLO configuration file: " + INFLOConfigFile);

            if (INFLOConfigFile.Length > 0)
            {
                retValue = clsMiscFunctions.ReadINFLOConfigFile(INFLOConfigFile, ref Roadway);
                if (retValue.Length > 0)
                {

                    LogTxtMsg(LoggerType.Init, LoggerLevel.Error,
                        "\r\nThe INFLO application is terminating. Ret Value from ReadINFLOConfigFile is "
                        + retValue.ToString() + ".  Config file: " + INFLOConfigFile);
                    return;
                }

                UpdateInfloConfigurationThresholds();

                //Add the combineddetectionzoneconfigfile
            }
            else
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tThe INFLO configuration file name was not specified in the Global variables class.\r\nThe INFLO Application is terminating.");
                return;
            }
            #endregion

            if (clsGlobalVars.CVDataSmoothedSpeedArraySize == 0)
            {
                clsGlobalVars.CVDataSmoothedSpeedArraySize = (int)(Math.Ceiling((double)clsGlobalVars.TSSDataPollingFrequency / (double)clsGlobalVars.CVDataPollingFrequency));
            }


            DisplayForm.lblRoadway.Text = "Roadway:" + Roadway.Identifier + " :: " + Roadway.Name;
            DisplayForm.lblRoadwayDirection.Text = "Direction: " + Roadway.Direction;
            DisplayForm.lblRoadwayExtent.Text = "From MM: " + Roadway.BeginMM + " To MM: " + Roadway.EndMM;
            DisplayForm.txtFOQ.Text = Roadway.RecurringCongestionMMLocation.ToString("0.00");

            #region "Establish connection to INFLO database"
            LogTxtMsg(LoggerType.Init, LoggerLevel.Info, "\r\n\tEstablish connection to the INFLO database: " +
                              "\r\n\t\tDBInterfaceType: " + clsGlobalVars.DBInterfaceType +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.AccessDBFileName +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.DSNName +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.SqlServer +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.SqlServerDatabase +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.SqlServerUserId +
                              "\r\n\t\tAccessDBFileName: " + clsGlobalVars.SqlStrConnection);
            if (clsGlobalVars.DBInterfaceType.Length > 0)
            {
                DB = new clsDatabase(clsGlobalVars.DBInterfaceType);
                if (DB.ConnectionStr.Length > 0)
                {
                    LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\n\tDatabase Connection string: " + DB.ConnectionStr);
                    retValue = DB.OpenDBConnection();
                    if (retValue.Length > 0)
                    {

                        LogTxtMsg(LoggerType.Init, LoggerLevel.Error,
                            "\r\nThe INFLO application is terminating. Ret Value from OpenDBConnection is "
                        + retValue.ToString() + ".");
                        return;
                    }
                }
                else
                {
                    LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\r\n\tError in generating connection string to INFLO database.");
                    return;
                }
            }
            else
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tThe INFLO Application can not connect to the INFLO database. " +
                                             "\r\n\tThe DBInterfaceType= " + clsGlobalVars.DBInterfaceType + "  is not specified.");
                return;
            }
            #endregion

            #region "Get available Roadway info from INFLO database"

            //LogTxtMsg("Get list of available Roadways from the INFLO database: ");
            //retValue = GetRoadwayInfo(DB, ref RList);
            //if (retValue .Length > 0)
            //{
            //    LogTxtMsg("\tError in getting the available roadways from the INFLO database: \r\n" + retValue);
            //    return;
            //}
            #endregion

            LogTxtMsg(LoggerType.FillDataSetLog, LoggerLevel.Info, "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name
            + "\r\n\tDirection: " + Roadway.Direction
            + "\r\n\tFrom MM: " + Roadway.BeginMM + " To MM: " + Roadway.EndMM
            + "\r\n\tDate/Time, DataType, NumberRecords,");

            #region "Get available Roadway Links from INFLO database"
            //Get list of available infrastructure roadway links from the INFLO database
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "Get list of available Roadway Links from the INFLO database: ");
            tmpRLList.Clear();
            retValue = GetRoadwayLinks(DB, ref tmpRLList);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tError in getting the available roadway links from the INFLO database: \r\n" + retValue);
                return;
            }
            if (tmpRLList.Count > 0)
            {
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {

                    //RLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                    //RLList.OrderBy(l => l.BeginMM);
                    var NewList = tmpRLList.OrderBy(x => x.BeginMM);
                    foreach (clsRoadwayLink link in NewList)
                    {
                        RLList.Add(link);
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    //RLList.OrderByDescending(l => l.BeginMM);
                    var NewList = tmpRLList.OrderByDescending(x => x.BeginMM);
                    foreach (clsRoadwayLink link in NewList)
                    {
                        RLList.Add(link);
                    }
                }
            }
            #endregion

            #region "Get available Roadway Sub-Links from INFLO database"
            //Get list of available infrastructure roadway sub-links from the INFLO database
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "Get list of available Roadway Sub-Links from the INFLO database: ");
            tmpRSLList.Clear();
            retValue = GetRoadwaySubLinks(DB, ref tmpRSLList);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tError in getting the available roadway sub-links from the INFLO database: \r\n" + retValue);
                return;
            }

            foreach (clsRoadwaySubLink rsl in tmpRSLList)
            {
                rsl.SmoothedSpeed = new double[clsGlobalVars.CVDataSmoothedSpeedArraySize];
                rsl.SmoothedSpeedIndex = 0;
            }
            if (tmpRSLList.Count > 0)
            {
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {

                    var NewList = tmpRSLList.OrderBy(x => x.BeginMM);
                    foreach (clsRoadwaySubLink sublink in NewList)
                    {
                        RSLList.Add(sublink);
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    var NewList = tmpRSLList.OrderByDescending(x => x.BeginMM);
                    foreach (clsRoadwaySubLink sublink in NewList)
                    {
                        RSLList.Add(sublink);
                    }
                    //RLList.OrderByDescending(l => l.BeginMM);
                }
                #region "Commented section"
                /*if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {

                    //RLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                    RSLList.OrderBy(l => l.BeginMM);
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    RSLList.OrderByDescending(l => l.BeginMM);
                    var result = from element in RSLList orderby element descending select element;
                    List<clsRoadwaySubLink> newsublinklist = new List<clsRoadwaySubLink>();
                    foreach (clsRoadwaySubLink sublink in RSLList)
                    {
                        newsublinklist.Add(sublink);
                    }
                    var NewList = newsublinklist.OrderByDescending(x => x.BeginMM);
                    foreach (clsRoadwaySubLink rsl in NewList)
                    {
                        Console.WriteLine(rsl.BeginMM);
                    }
                    RSLList.Clear();
                    foreach (clsRoadwaySubLink sublink in NewList)
                    {
                        RSLList.Add(sublink);
                    }

                    //RSLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                    //foreach (clsRoadwaySubLink rsl in RSLList)
                    //{
                    //    Console.WriteLine( rsl.BeginMM);
                    //}
                }*/
                #endregion
            }
            #endregion

            #region "Get available Detector Stations from INFLO database"
            //Get list of available infrastructure detector stations from the INFLO database
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "Get list of available Detector stations from the INFLO database: ");
            tmpDSList.Clear();
            retValue = GetDetectorStations(DB, ref tmpDSList);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tError in getting the available detector stations from the INFLO database: \r\n" + retValue);
                return;
            }
            if (tmpDSList.Count > 0)
            {
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {

                    //RLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                    var NewList = tmpDSList.OrderBy(x => x.MMLocation);
                    foreach (clsDetectorStation ds in NewList)
                    {
                        DSList.Add(ds);
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    //RLList.OrderByDescending(l => l.BeginMM);
                    var NewList = tmpDSList.OrderByDescending(x => x.MMLocation);
                    foreach (clsDetectorStation ds in NewList)
                    {
                        DSList.Add(ds);
                    }
                }

            }
            #endregion

            #region "Get available Detection Zones from INFLO database"
            //Get list of available infrastructure detection zones from the INFLO database
            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "Get list of available Detection zones from the INFLO database: ");
            tmpDZList.Clear();
            retValue = GetDetectionZones(DB, ref tmpDZList);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\tError in getting the available detection zones from the INFLO database: \r\n" + retValue);
                return;
            }
            if (tmpDZList.Count > 0)
            {
                var NewList = tmpDZList.OrderBy(x => x.DSIdentifier);
                foreach (clsDetectionZoneWSDOT dz in NewList)
                {
                    DZList.Add(dz);
                }
                #region "Commented section. Not needed since we are ordering by the detector station identifier"
                /*  if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {

                   // DZList.OrderBy(l => l.DSIdentifier);
                    var NewList = tmpDZList.OrderBy(x => x.DSIdentifier);
                    foreach (clsDetectionZoneWSDOT dz in NewList)
                    {
                        DZList.Add(dz);
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    //DZList.OrderByDescending(l => l.BeginMM);
                    var NewList = tmpDZList.OrderByDescending(x => x.DSIdentifier);
                    foreach (clsDetectionZoneWSDOT dz in NewList)
                    {
                        DZList.Add(dz);
                    }
                }*/
                #endregion
            }
            #endregion


            //Open CV and TSS data processing log files


            LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name
             + "\r\n\tDirection: " + Roadway.Direction
            + "\r\n\tFrom MM: " + Roadway.BeginMM + " To MM: " + Roadway.EndMM);
            LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\n\tTSS BOQ: " + clsGlobalVars.InfrastructureBOQMMLocation
            + "\r\n\tTSS BOQTime: " + clsGlobalVars.InfrastructureBOQTime
            + "\r\n\tCV BOQ: " + clsGlobalVars.CVBOQMMLocation
            + "\r\n\tCV BOQTime: " + clsGlobalVars.CVBOQTime
            + "\r\n\tBOQ: " + clsGlobalVars.BOQMMLocation
            + "\r\n\tBOQTime: " + clsGlobalVars.BOQTime
            + "\r\n\tQRate: " + clsGlobalVars.QueueRate
            + "\r\n\tQChange: " + clsGlobalVars.QueueChange
            + "\r\n\tQSource: " + clsGlobalVars.QueueSource);


            LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "Roadway: " + Roadway.Identifier + " :: " + Roadway.Name
            + "\r\n\tDirection: " + Roadway.Direction
            + "\r\n\tFrom MM: " + Roadway.BeginMM + " To MM: " + Roadway.EndMM);




            retValue = InitializeExcelWorksheets(retValue);

            //Set the infrastructure data availability and initialize the infrastructure BOQ variables
            if (RLList.Count > 0)
            {
                clsGlobalVars.InfrastructureDataAvailable = true;
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, " Roadway Links Count: " + RLList.Count + " ::   Infrastructure data available");
            }
            else
            {
                clsGlobalVars.InfrastructureDataAvailable = false;
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, " Roadway Links Count: " + RLList.Count + " ::   Infrastructure data not available");
            }

            //Set the CV data availability and initialize the infrastructure BOQ variables
            if (RSLList.Count > 0)
            {
                clsGlobalVars.CVDataAvailable = true;
                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, " Roadway Sublink Count: " + RSLList.Count + " :: CV data available");
            }
            else
            {
                clsGlobalVars.CVDataAvailable = false;
                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, " Roadway Sublink Count: " + RSLList.Count + " ::  CV data not available");
            }

            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\nFinished processing the INFLO Application configuration files");
        }



        private void Init()
        {
            string retValue = string.Empty;

            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\nStart the connected vehicle, infrastructure, and weather data processors if data sources are available");

            retValue = InitClsGlobalVars();
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.Init, LoggerLevel.Error, "\r\n\tError in starting the INFLO Q-WARN and SPD-HARM algorithms.\r\n\t" + retValue);

            }
            else
            {
                DisplayForm.txtBOQ.Text = string.Empty;
                DisplayForm.txtCVBOQ.Text = string.Empty;
                DisplayForm.txtCVDate.Text = string.Empty;
                DisplayForm.txtTSSBOQ.Text = string.Empty;
                DisplayForm.txtTSSDate.Text = string.Empty;
                DisplayForm.ClearCVSubLinkQueuedStatus();
                DisplayForm.ClearCVSubLinkTroupeStatus();
                DisplayForm.ClearCVSubLinkSPDHarmStatus();
                DisplayForm.ClearTSSQueuedLinkStatus();
                DisplayForm.ClearTSSSPDHarmLinkStatus();


                Stopped = false;
            }

            LogTxtMsg(LoggerType.Init, LoggerLevel.Debug, "\r\nFinished Starting the CV, infrastructure and weather data processors");
        }

        private void btnStopINFLO_Click_1(object sender, EventArgs e)
        {
            Stopped = true;

        }

        private string InitClsGlobalVars()
        {
            string retValue = string.Empty; ;



            LogTxtMsg(LoggerType.QueueLog, LoggerLevel.Info, "PrevBOQ,PrevBOQTime,BOQ,BOQTime,MMChange, TimeDiff, QueueRate,QueueChange,Source, CVBOQ, TSSBOQ, TotalCVs, PrevTotalCVs, VolumeDiff, FlowRate, Density, PrevDensity, DensityDiff, ShockwaveRate");

            clsGlobalVars.CVBOQMMLocation = -1;
            clsGlobalVars.CVBOQTime = DateTime.Now;
            clsGlobalVars.PrevCVBOQMMLocation = -1;
            clsGlobalVars.PrevCVBOQTime = DateTime.Now;

            clsGlobalVars.InfrastructureBOQMMLocation = -1;
            clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
            clsGlobalVars.PrevInfrastructureBOQMMLocation = -1;
            clsGlobalVars.PrevInfrastructureBOQTime = DateTime.Now;

            //Reset BOQ values
            clsGlobalVars.BOQMMLocation = -1;
            clsGlobalVars.BOQTime = DateTime.Now;

            clsGlobalVars.PrevBOQMMLocation = -1;
            clsGlobalVars.PrevBOQTime = DateTime.Now;

            clsGlobalVars.QueueRate = 0;
            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.NA;
            clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;

            #region "Sync with WSDOT files"
            /*clsGlobalVars.TSSDataCurrDateTime = DateTime.Now;
            clsGlobalVars.TSSDataCurrInterval = 42;
            clsGlobalVars.TSSDataCurrDateTime = clsGlobalVars.TSSDataCurrDateTime.AddHours(-2);
            clsGlobalVars.TSSDataCurrDateTime = clsGlobalVars.TSSDataCurrDateTime.AddMinutes(5);
            clsGlobalVars.TSSDataNumDetectionZones = 0;

            clsGlobalVars.TSSDataCurrFileName = clsGlobalVars.WSDOT20SecDataFilesURL + "NG_20SecData_" + clsGlobalVars.TSSDataCurrDateTime.Year.ToString("0000") + clsGlobalVars.TSSDataCurrDateTime.Month.ToString("00") +
                                                clsGlobalVars.TSSDataCurrDateTime.Day.ToString("00") + "_" + clsGlobalVars.TSSDataCurrDateTime.Hour.ToString("00") + clsGlobalVars.TSSDataCurrDateTime.Minute.ToString("00");

            clsGlobalVars.TSSDataCurrWebFileName = clsGlobalVars.TSSDataCurrFileName + clsGlobalVars.TSSDataCurrInterval.ToString("00") + ".dat";
            //var result = GetFileViaHttp(@"http://data.wsdot.wa.gov/traffic/NW/FlowData/20second/NG_20SecData_20141125_100002.dat");

            #region "Find the latest WSDOT 20 second real-time traffic data file"
            var result = new byte[1];
            TimeSpan StartSynching = new TimeSpan(DateTime.Now.Ticks);
            do
            {
                try
                {
                    result = clsMiscFunctions.GetFileViaHttp(clsGlobalVars.TSSDataCurrWebFileName);
                    if (result.Length == 1)
                    {
                        clsGlobalVars.TSSDataCurrInterval = clsGlobalVars.TSSDataCurrInterval - 20;
                        if (clsGlobalVars.TSSDataCurrInterval < 0)
                        {
                            clsGlobalVars.TSSDataCurrInterval = 42;
                            clsGlobalVars.TSSDataCurrDateTime = clsGlobalVars.TSSDataCurrDateTime.AddMinutes(-1);
                        }
                        clsGlobalVars.TSSDataCurrFileName = clsGlobalVars.WSDOT20SecDataFilesURL + "NG_20SecData_" + clsGlobalVars.TSSDataCurrDateTime.Year.ToString("0000") + clsGlobalVars.TSSDataCurrDateTime.Month.ToString("00") +
                                                            clsGlobalVars.TSSDataCurrDateTime.Day.ToString("00") + "_" + clsGlobalVars.TSSDataCurrDateTime.Hour.ToString("00") + clsGlobalVars.TSSDataCurrDateTime.Minute.ToString("00");
                        clsGlobalVars.TSSDataCurrWebFileName = clsGlobalVars.TSSDataCurrFileName + clsGlobalVars.TSSDataCurrInterval.ToString("00") + ".dat";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            } while (result.Length == 1);
            #endregion
            TimeSpan EndSynching = new TimeSpan(DateTime.Now.Ticks);
            */
            #endregion


            return retValue;
        }


        private string GetRoadwayInfo(clsDatabase DB, ref List<clsRoadway> RList)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;


            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

            List<Configuration_Roadway> data = uow.Configuration_Roadways.ToList();


            //display roadway information if it already exists
            LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable Roadway Info: ");


            foreach (Configuration_Roadway r in data)
            {
                clsRoadway tmpRd = new clsRoadway();

                clsEnums.enDirection tmpDirection = clsEnums.enDirection.NA;

                clsEnums.enDirection tmpIncreasingDirection = clsEnums.enDirection.NA;


                tmpRd.Identifier = r.RoadwayId;//this is allowed to be null?

                tmpRd.Name = r.Name;

                tmpRd.Grade = GetNullable(r.Grade);

                tmpRd.BeginMM = r.BeginMM;

                tmpRd.EndMM = r.EndMM;
                tmpDirection = (clsEnums.enDirection)(int.Parse(r.Direction));
                tmpRd.Direction = tmpDirection;

                tmpIncreasingDirection = (clsEnums.enDirection)(int.Parse(r.MMIncreasingDirection));
                tmpRd.MMIncreasingDirection = tmpIncreasingDirection;

                tmpRd.RecurringCongestionMMLocation = GetNullable(r.RecurringCongestionMMLocation);


                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\t\t" + tmpRd.Identifier + ", " + tmpRd.Name + ", " + tmpRd.Direction + ", " +
                                          tmpRd.Grade + ", " + tmpRd.BeginMM + ", " + tmpRd.EndMM + ", " +
                                          tmpRd.RecurringCongestionMMLocation + ", " + tmpRd.MMIncreasingDirection);
                RList.Add(tmpRd);

            }
            return retValue;
        }

        private void UpdateInfloConfigurationThresholds()
        {
            bool add = false;
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            Configuration_INFLOThresholds config = uow.Configuration_INFLOThresholds.FirstOrDefault();
            if (config == null)
            {
                config = new Configuration_INFLOThresholds();
                add = true;
            }

            config.CVDataPollingFrequency = (short)clsGlobalVars.CVDataPollingFrequency;
            config.TSSDataPollingFrequency = (short)clsGlobalVars.TSSDataPollingFrequency;
            config.CVDataLoadingFrequency = (short)clsGlobalVars.CVDataLoadingFrequency;
            config.TSSDataLoadingFrequency = (short)clsGlobalVars.TSSDataLoadingFrequency;
            config.ESSDataPollingFrequency = (short)clsGlobalVars.ESSDataPollingFrequency;
            config.MobileESSDataPollingFrequency = (short)clsGlobalVars.MobileESSDataPollingFrequency;
            config.QueuedSpeedThreshold = (short)clsGlobalVars.LinkQueuedSpeedThreshold;
            config.CongestedSpeedThreshold = (short)clsGlobalVars.LinkCongestedSpeedThreshold;
            config.MinimumDisplaySpeed = (short)clsGlobalVars.MinimumDisplaySpeed;
            config.TroupeRange = (short)clsGlobalVars.TroupeRange;
            config.ThreeGantrySpeed = (short)clsGlobalVars.ThreeGantrySpeed;
            config.RecurringCongestionMMLocation = null;
            config.SublinkLength = clsGlobalVars.SubLinkLength;
            config.SublinkPercentQueuedCV = (short)clsGlobalVars.SubLinkPercentQueuedCV;
            config.MaximumDisplaySpeed = (short)clsGlobalVars.MaximumDisplaySpeed;//Questionable cast - but db is a short.
            config.COFLowerThreshold = clsGlobalVars.COFLowerThreshold;
            config.COFUpperThreshold = clsGlobalVars.COFUpperThreshold;
            config.RoadSurfaceStatusDryCOF = clsGlobalVars.RoadSurfaceStatusDryCOF;
            config.RoadSurfaceStatusSnowCOF = clsGlobalVars.RoadSurfaceStatusSnowCOF;
            config.RoadSurfaceStatusWetCOF = clsGlobalVars.RoadSurfaceStatusWetCOF;
            config.RoadSurfaceStatusIceCOF = clsGlobalVars.RoadSurfaceStatusIceCOF;
            config.WRTMMaxRecommendedSpeed = clsGlobalVars.WRTMMaxRecommendedSpeed;
            config.WRTMMaxRecommendedSpeedLevel1 = clsGlobalVars.WRTMRecommendedSpeedLevel1;
            config.WRTMMaxRecommendedSpeedLevel2 = clsGlobalVars.WRTMRecommendedSpeedLevel2;
            config.WRTMMaxRecommendedSpeedLevel3 = clsGlobalVars.WRTMRecommendedSpeedLevel3;
            config.WRTMMaxRecommendedSpeedLevel4 = clsGlobalVars.WRTMRecommendedSpeedLevel4;
            config.WRTMMinRecommendedSpeed = clsGlobalVars.WRTMMinRecommendedSpeed;
            config.VisibilityThreshold = clsGlobalVars.VisibilityThreshold;
            config.VisibilityStatusClear = clsGlobalVars.VisibilityStatusClear;
            config.VisibilityStatusFogNotPatchy = clsGlobalVars.VisibilityStatusFogNotPatchy;
            config.VisibilityStatusPatchyFog = clsGlobalVars.VisibilityStatusPatchyFog;
            config.VisibilityStatusBlowingSnow = clsGlobalVars.VisibilityStatusBlowingSnow;
            config.VisibilityStatusSmoke = clsGlobalVars.VisibilityStatusSmoke;
            config.VisibilityStatusSeaSpray = clsGlobalVars.VisibilityStatusSeaSpray;
            config.VisibilityStatusVehicleSpray = clsGlobalVars.VisibilityStatusVehicleSpray;
            config.VisibilityStatusBlowingDust = clsGlobalVars.VisibilityStatusBlowingDust;
            config.VisibilityStatusBlowingSand = clsGlobalVars.VisibilityStatusBlowingSand;
            config.VisibilityStatusSunGlare = clsGlobalVars.VisibilityStatusSunGlare;
            config.VisibilityStatusSwarmsOfInsects = clsGlobalVars.VisibilityStatusSwarmsOfInsects;
            config.OccupancyThreshold = (short)clsGlobalVars.OccupancyThreshold;//Questionable cast - but db is a short.
            config.VolumeThreshold = (short)clsGlobalVars.VolumeThreshold;//Questionable cast- but db is a short.
            if (add) uow.Configuration_INFLOThresholds.Add(config);
            uow.SaveChanges();
        }

        /// <summary>
        /// Gets Roadwaylinks belonging to the (selected via config file) Roadway's
        /// id,  and Begin and End locations located within the Begin and End bounds of the roadway.
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="RLList"></param>
        /// <returns></returns>
        private string GetRoadwayLinks(clsDatabase DB, ref List<clsRoadwayLink> RLList)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;

            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

            List<Configuration_RoadwayLinks> data;
            var roadwayLinks = uow.Configuration_RoadwayLinks
               .Where(d => d.RoadwayId == Roadway.Identifier.ToString());
            if (Roadway.Direction == Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.BeginMM >= Roadway.BeginMM
                    && d.EndMM <= Roadway.EndMM)).ToList();
            }
            else// if (Roadway.Direction != Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.BeginMM <= Roadway.BeginMM
                   && d.EndMM >= Roadway.EndMM)).ToList();

            }

            LogTxtMsg(LoggerType.FillDataSetLog, LoggerLevel.Info, "GetRoadwayLinks," + data.Count);


            //display roadway link information if it already exists
            LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable Roadway links: ");


            foreach (Configuration_RoadwayLinks r in data)
            {
                clsRoadwayLink tmpRLL = new clsRoadwayLink();



                tmpRLL.RoadwayID = r.RoadwayId;//this is allowed to be null?

                tmpRLL.Identifier = int.Parse(r.LinkId);

                tmpRLL.BeginMM = r.BeginMM;

                tmpRLL.EndMM = r.EndMM;

                tmpRLL.NumberLanes = GetNullable(r.NumberLanes);

                tmpRLL.NumberDetectionStations = GetNullable(r.NumberDetectorStations);

                tmpRLL.DetectionStations = GetNullable(r.DetectorStations);


                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\t\t" + tmpRLL.RoadwayID + ", " + tmpRLL.Identifier
                    + ", " + tmpRLL.BeginMM + ", " + tmpRLL.EndMM + ", " + tmpRLL.NumberLanes + ", "
                    + tmpRLL.NumberDetectionStations + ", " + tmpRLL.DetectionStations);
                tmpRLL.Direction = Roadway.Direction;
                RLList.Add(tmpRLL);

            }
            return retValue;
        }
        /// <summary>
        /// Gets RoadwaySublinks belonging to the (selected via config file) Roadway's
        /// id,  and Begin and End locations located within the Begin and End bounds of the roadway.
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="RSLList"></param>
        /// <returns></returns>
        private string GetRoadwaySubLinks(clsDatabase DB, ref List<clsRoadwaySubLink> RSLList)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;

            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            List<Configuration_RoadwaySubLinks> data;
            var roadwayLinks = uow.Configuration_RoadwaySubLinks
               .Where(d => d.RoadwayId == Roadway.Identifier.ToString());
            if (Roadway.Direction == Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.BeginMM >= Roadway.BeginMM
                    && d.EndMM <= Roadway.EndMM)).ToList();
            }
            else// if (Roadway.Direction != Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.BeginMM <= Roadway.BeginMM
                   && d.EndMM >= Roadway.EndMM)).ToList();

            }

            LogTxtMsg(LoggerType.FillDataSetLog, LoggerLevel.Info, "GetRoadwaySubLinks," + data.Count);


            //display roadway sublink information if it already exists
            LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable Roadway sublinks: ");


            foreach (Configuration_RoadwaySubLinks r in data)
            {
                clsRoadwaySubLink tmpRSL = new clsRoadwaySubLink();



                tmpRSL.RoadwayID = r.RoadwayId;

                tmpRSL.Identifier = int.Parse(r.SubLinkId);

                tmpRSL.BeginMM = r.BeginMM;

                tmpRSL.EndMM = r.EndMM;

                tmpRSL.NumberLanes = GetNullable(r.NumberLanes);

                tmpRSL.Direction = clsEnums.GetDirIndexFromString(r.Direction);//this is allowed to be null?

                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\t\t" + tmpRSL.RoadwayID + ", " + tmpRSL.Identifier + ", "
                    + tmpRSL.BeginMM + ", " + tmpRSL.EndMM + ", " + tmpRSL.NumberLanes + ", " + tmpRSL.Direction);
                RSLList.Add(tmpRSL);

            }
            return retValue;
        }

        short GetNullable(short? n)
        {
            if (n == null) return 0;
            else return (short)n;
        }
        string GetNullable(string n)
        {
            if (n == null) return "";
            else return n;
        }
        double GetNullable(double? n)
        {
            if (n == null) return 0;
            else return (double)n;
        }
        DateTime GetNullable(DateTime? n)
        {
            if (n == null) return DateTime.UtcNow;
            else return (DateTime)n;
        }
        bool GetNullable(bool? n)
        {
            if (n == null) return false;
            else return (bool)n;
        }

        /// <summary>
        /// Gets Detector Stations belonging to the (selected via config file) Roadway's
        /// id, direction, and mileMarker location located within the Begin and End bounds of the roadway.
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DSList"></param>
        /// <returns></returns>
        private string GetDetectorStations(clsDatabase DB, ref List<clsDetectorStation> DSList)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;


            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            List<Configuration_TSSDetectorStation> data;
            var roadwayLinks = uow.Configuration_TSSDetectorStations
               .Where(d => d.RoadwayId == Roadway.Identifier.ToString()
               && d.Direction == Roadway.Direction.ToString());
            if (Roadway.Direction == Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.MMLocation >= Roadway.BeginMM
                    && d.MMLocation <= Roadway.EndMM)).ToList();
            }
            else// if (Roadway.Direction != Roadway.MMIncreasingDirection)
            {
                data = roadwayLinks.Where(d => (d.MMLocation <= Roadway.BeginMM
                   && d.MMLocation >= Roadway.EndMM)).ToList();

            }

            LogTxtMsg(LoggerType.FillDataSetLog, LoggerLevel.Info, "GetDetectorStations," + data.Count);

            //display Detector station information 
            LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable detector stations: ");


            foreach (Configuration_TSSDetectorStation s in data)
            {
                clsDetectorStation tmpDS = new clsDetectorStation();
                string RoadLinkID = string.Empty;
                string DSID = string.Empty;
                string RoadwayId = string.Empty;
                double MMLocation = 0;
                int NumDetectionZones = 0;
                string DetectionZones = string.Empty;


                tmpDS.LinkIdentifier = int.Parse(s.LinkId);//this field allowed to be null?

                tmpDS.Identifier = int.Parse(s.DSId);

                tmpDS.RoadwayIdentifier = s.RoadwayId;//this field allowed to be null?

                tmpDS.MMLocation = s.MMLocation;

                tmpDS.NumberDetectionZones = s.NumberDetectionZones;

                tmpDS.DetectionZones = GetNullable(s.DetectionZones);

                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\t\t" + tmpDS.LinkIdentifier + ", " + tmpDS.Identifier + ", " + tmpDS.RoadwayIdentifier + ","
                    + tmpDS.MMLocation + ", " + tmpDS.NumberDetectionZones + ", " + tmpDS.DetectionZones);
                DSList.Add(tmpDS);

            }
            return retValue;
        }
        /// <summary>
        /// Gets Detection Zones belonging to the (selected via config file) Roadway's
        /// id, and direction.        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DZList"></param>
        /// <returns></returns>
        private string GetDetectionZones(clsDatabase DB, ref List<clsDetectionZoneWSDOT> DZList)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;
            string CurrentSection = string.Empty;

            try
            {
                CurrentSection = "Get DZ Info from database";

                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

                List<Configuration_TSSDetectionZone> data = uow.Configuration_TSSDetectionZones
                    .Where(d => (d.Direction == Roadway.Direction.ToString()
                     && d.RoadwayId == Roadway.Identifier.ToString())).ToList();



                //display Detector station information 
                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable detection zones: ");

                foreach (Configuration_TSSDetectionZone z in data)
                {
                    clsDetectionZoneWSDOT tmpDZ = new clsDetectionZoneWSDOT();

                    clsEnums.enDirection Direction = clsEnums.enDirection.NA;
                    CurrentSection = z.DSId + ", " + z.DZId + ", " + z.LaneNumber;//in case of error



                    tmpDZ.DSIdentifier = int.Parse(z.DSId);

                    tmpDZ.Identifier = z.DZId;

                    //KG: mmlocation does not exist in database.
                    //case "mmlocation":
                    //    CurrentSection = "Row: " + i + "\t Col: " + col.ColumnName + "\tValue: " + row[col].ToString();
                    //    if (row[col].ToString().Length > 0)
                    //    {
                    //        MMLocation = double.Parse(row[col].ToString());
                    //        tmpDZ.MMLocation = MMLocation;
                    //    }
                    //    break;


                    tmpDZ.RTLanes = z.LaneNumber;

                    tmpDZ.DataType = GetNullable(z.DZType);

                    tmpDZ.LaneType = GetNullable(z.LaneType);

                    tmpDZ.Description = GetNullable(z.LaneDescription);

                    tmpDZ.DataType = GetNullable(z.DataType);
                    Direction = clsEnums.GetDirIndexFromString(z.Direction);//this field allowed to be null?
                    tmpDZ.Direction = Direction;


                    LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\t\t" + z.DSId + ", " + z.DZId + ", " + z.DZType + ", " + z.DataType + ", "
                        //+ MMLocation + ", " 
                                              + z.LaneNumber + ", " + z.LaneType + ", " + z.LaneDescription + ", " + z.Direction);
                    DZList.Add(tmpDZ);
                }
                //}
            }
            catch (Exception ex)
            {
                retValue = "\tError in getting the Detection zones info from the INFLO database. dsId,dzid,laneNo: " + CurrentSection + "\r\n\t\t" + ex.Message + "\r\n" + ex.StackTrace;
                return retValue;
            }
            return retValue;
        }

        private double GetMinimumSpeed(double TSSAvgSpeed, double WRTMSpeed, double CVAvgSpeed)
        {
            double[] spdArray = new double[3];
            int Indx = 0;
            double MinSpeed = 0;

            if (TSSAvgSpeed > 0)
            {
                spdArray[Indx++] = TSSAvgSpeed;
            }
            else
            {
                spdArray[Indx++] = clsGlobalVars.MaximumDisplaySpeed;
            }


            if (WRTMSpeed > 0)
            {
                spdArray[Indx++] = WRTMSpeed;
            }
            else
            {
                spdArray[Indx++] = clsGlobalVars.MaximumDisplaySpeed;
            }
            if (CVAvgSpeed > 0)
            {
                spdArray[Indx++] = CVAvgSpeed;
            }
            else
            {
                spdArray[Indx++] = clsGlobalVars.MaximumDisplaySpeed;
            }
            if (Indx > 0)
            {
                MinSpeed = spdArray.Min();
            }
            return MinSpeed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="QuedSubLink">The RSL that was determined previously in the code to be the furthest upstream
        /// RSL that reported being queued. Found from CV data processing.</param>
        /// <param name="QuedLink">Found from TSS data processing</param>
        /// <param name="Roadway"></param>
        private void DetermineBOQ(clsRoadwaySubLink QuedSubLink, clsRoadwayLink QuedLink, clsRoadway Roadway)
        {
            LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\r\n\r\n\tReconcile CV BOQ MM and TSS BOQ MM locations:");
            LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\r\n\t\tCurr  CV BOQ MM: " + clsGlobalVars.CVBOQMMLocation.ToString("0.0") + "\t CV BOQ Speed: " + clsGlobalVars.CVBOQSublinkSpeed.ToString("0"));
            LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tCurr TSS BOQ MM: " + clsGlobalVars.InfrastructureBOQMMLocation.ToString("0.0") + "\t TSS BOQ Speed: " + clsGlobalVars.InfrastructureBOQMMLocation.ToString("0"));

            #region "Reconcile CV BOQ MM Location and TSS data BOQ MM Location"

            clsEnums.enQueueSource tmpQueueSource = clsEnums.enQueueSource.NA;
            double tmpBOQMMLocationChange = 0;

            double tmpQueDiff = 0;
            double tmpTSSQueDiff = 0;
            double tmpCVQueDiff = 0;
            clsEnums.enQueueCahnge tmpQueChangeDirection = clsEnums.enQueueCahnge.NA;

            //Mile marker increasing direction = roadway direction
            if (Roadway.Direction == Roadway.MMIncreasingDirection)
            {
                #region "Queue Exists"
                if (clsGlobalVars.BOQMMLocation != -1)
                {
                    tmpTSSQueDiff = clsGlobalVars.InfrastructureBOQMMLocation - clsGlobalVars.BOQMMLocation;
                    tmpCVQueDiff = clsGlobalVars.CVBOQMMLocation - clsGlobalVars.BOQMMLocation;
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength) && (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength))
                            {
                                if (tmpCVQueDiff <= tmpTSSQueDiff)
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.CV;
                                    tmpBOQMMLocationChange = tmpCVQueDiff;
                                }
                                else
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.TSS;
                                    tmpBOQMMLocationChange = tmpTSSQueDiff;
                                }
                            }
                            else if (Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                            else if (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            if (tmpCVQueDiff <= tmpTSSQueDiff)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                            else
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.CV;
                            tmpBOQMMLocationChange = tmpCVQueDiff;
                        }
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.TSS;
                            tmpBOQMMLocationChange = tmpTSSQueDiff;
                        }
                    }
                    else
                    {
                        tmpQueDiff = Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation;
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.NA;
                                tmpBOQMMLocationChange = tmpQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.NA;
                            tmpBOQMMLocationChange = tmpQueDiff;
                        }
                    }
                    if (tmpBOQMMLocationChange < 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Growing;
                    }
                    else if (tmpBOQMMLocationChange > 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Dissipating;
                    }
                    else if (tmpBOQMMLocationChange == 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Same;
                    }
                }
                #endregion
                #region "No Queue"
                else if (clsGlobalVars.BOQMMLocation == -1)
                {
                    tmpTSSQueDiff = clsGlobalVars.InfrastructureBOQMMLocation - Roadway.RecurringCongestionMMLocation;
                    tmpCVQueDiff = clsGlobalVars.CVBOQMMLocation - Roadway.RecurringCongestionMMLocation;
                    //If BOTH cv and tss monitors have come up with a BOQ, reconcile them together.
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength) && (Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength))
                            {
                                if ((tmpCVQueDiff <= tmpTSSQueDiff) && (clsGlobalVars.CVBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.CV;
                                    tmpBOQMMLocationChange = tmpCVQueDiff;
                                }
                                else if (clsGlobalVars.InfrastructureBOQMMLocation <= Roadway.RecurringCongestionMMLocation)
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.TSS;
                                    tmpBOQMMLocationChange = tmpTSSQueDiff;
                                }
                            }
                            else if ((Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength) && (clsGlobalVars.InfrastructureBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                            else if ((Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength) && (clsGlobalVars.CVBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            if ((tmpCVQueDiff <= tmpTSSQueDiff) && (clsGlobalVars.CVBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                            else if (clsGlobalVars.InfrastructureBOQMMLocation <= Roadway.RecurringCongestionMMLocation)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)//else if only CV has found a BOQ, use that.
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength) && (clsGlobalVars.CVBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else if (clsGlobalVars.CVBOQMMLocation <= Roadway.RecurringCongestionMMLocation)
                        {
                            tmpQueueSource = clsEnums.enQueueSource.CV;
                            tmpBOQMMLocationChange = tmpCVQueDiff;
                        }
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)//else if only TSS has found a BOQ, use that.
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength) && (clsGlobalVars.InfrastructureBOQMMLocation <= Roadway.RecurringCongestionMMLocation))
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                        else if (clsGlobalVars.InfrastructureBOQMMLocation <= Roadway.RecurringCongestionMMLocation)
                        {
                            tmpQueueSource = clsEnums.enQueueSource.TSS;
                            tmpBOQMMLocationChange = tmpTSSQueDiff;
                        }
                    }
                    else
                    {
                        tmpQueueSource = clsEnums.enQueueSource.NA;
                        tmpBOQMMLocationChange = 0;
                    }
                    if (tmpBOQMMLocationChange <= 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Growing;
                    }
                    else if (tmpBOQMMLocationChange > 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Dissipating;
                    }
                }
                #endregion
            }
            //Mile marker increasing direction = opposite to roadway direction
            else if (Roadway.Direction != Roadway.MMIncreasingDirection)
            {
                #region "Queue Exists"
                if (clsGlobalVars.BOQMMLocation != -1)
                {
                    tmpTSSQueDiff = clsGlobalVars.InfrastructureBOQMMLocation - clsGlobalVars.BOQMMLocation;
                    tmpCVQueDiff = clsGlobalVars.CVBOQMMLocation - clsGlobalVars.BOQMMLocation;
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength) && (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength))
                            {
                                if (tmpCVQueDiff >= tmpTSSQueDiff)
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.CV;
                                    tmpBOQMMLocationChange = tmpCVQueDiff;
                                }
                                else
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.TSS;
                                    tmpBOQMMLocationChange = tmpTSSQueDiff;
                                }
                            }
                            else if (Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                            else if (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            if (tmpCVQueDiff >= tmpTSSQueDiff)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                            else
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpCVQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.CV;
                            tmpBOQMMLocationChange = tmpCVQueDiff;
                        }
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpTSSQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.TSS;
                            tmpBOQMMLocationChange = tmpTSSQueDiff;
                        }
                    }
                    else
                    {
                        tmpQueDiff = Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation;
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpQueDiff) <= clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.NA;
                                tmpBOQMMLocationChange = tmpQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.NA;
                            tmpBOQMMLocationChange = tmpQueDiff;
                        }
                    }
                    if (tmpBOQMMLocationChange > 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Growing;
                    }
                    else if (tmpBOQMMLocationChange < 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Dissipating;
                    }
                    else if (tmpBOQMMLocationChange == 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Same;
                    }
                }
                #endregion
                #region "No Queue"
                else if (clsGlobalVars.BOQMMLocation == -1)
                {
                    tmpTSSQueDiff = clsGlobalVars.InfrastructureBOQMMLocation - Roadway.RecurringCongestionMMLocation;
                    tmpCVQueDiff = clsGlobalVars.CVBOQMMLocation - Roadway.RecurringCongestionMMLocation;
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if ((Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength) && (Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength))
                            {
                                if (tmpCVQueDiff >= tmpTSSQueDiff)
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.CV;
                                    tmpBOQMMLocationChange = tmpCVQueDiff;
                                }
                                else
                                {
                                    tmpQueueSource = clsEnums.enQueueSource.TSS;
                                    tmpBOQMMLocationChange = tmpTSSQueDiff;
                                }
                            }
                            else if (Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                            else if (Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            if (tmpCVQueDiff >= tmpTSSQueDiff)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                            else
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpCVQueDiff) <= 2 * clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.CV;
                                tmpBOQMMLocationChange = tmpCVQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.CV;
                            tmpBOQMMLocationChange = tmpCVQueDiff;
                        }
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        if (chkFilterQueues.Checked == true)
                        {
                            if (Math.Abs(tmpTSSQueDiff) <= 2 * clsGlobalVars.LinkLength)
                            {
                                tmpQueueSource = clsEnums.enQueueSource.TSS;
                                tmpBOQMMLocationChange = tmpTSSQueDiff;
                            }
                        }
                        else
                        {
                            tmpQueueSource = clsEnums.enQueueSource.TSS;
                            tmpBOQMMLocationChange = tmpTSSQueDiff;
                        }
                    }
                    else
                    {
                        tmpQueueSource = clsEnums.enQueueSource.NA;
                        tmpBOQMMLocationChange = 0;
                    }
                    if (tmpBOQMMLocationChange > 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Growing;
                    }
                    else if (tmpBOQMMLocationChange <= 0)
                    {
                        tmpQueChangeDirection = clsEnums.enQueueCahnge.Dissipating;
                    }
                }
                #endregion
            }

            if (tmpQueueSource == clsEnums.enQueueSource.CV)
            {
                clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                clsGlobalVars.BOQTime = DateTime.Now;
                clsGlobalVars.BOQSpeed = clsGlobalVars.CVBOQSublinkSpeed;
            }
            else if (tmpQueueSource == clsEnums.enQueueSource.TSS)
            {
                clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                clsGlobalVars.BOQTime = DateTime.Now;
                clsGlobalVars.BOQSpeed = clsGlobalVars.InfrastructureBOQLinkSpeed;
            }
            //else if ((tmpQueueSource == clsEnums.enQueueSource.NA) && (tmpBOQMMLocationChange != 0))
            else if ((tmpQueueSource == clsEnums.enQueueSource.NA))
            {
                clsGlobalVars.BOQMMLocation = -1;
                clsGlobalVars.BOQTime = DateTime.Now;
                clsGlobalVars.BOQSpeed = 0;
            }
            #endregion

            TimeSpan span = clsGlobalVars.BOQTime.Subtract(clsGlobalVars.PrevBOQTime);
            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
            clsGlobalVars.QueueChange = tmpQueChangeDirection;

            #region "Determine queue rate of change"
            string tmpSublinkData = string.Empty;
            if (QueuedSubLink != null)
            {
                tmpSublinkData = QueuedSubLink.TotalNumberCVs + "," + QueuedSubLink.PrevTotalNumberCVs + "," + QueuedSubLink.VolumeDiff + "," + QueuedSubLink.FlowRate.ToString("0.00") + "," +
                                 QueuedSubLink.Density.ToString("0.00") + "," + QueuedSubLink.PrevDensity.ToString("0.00") + "," + QueuedSubLink.DensityDiff + "," + QueuedSubLink.ShockWaveRate.ToString("0.00");
            }

            if (Stopped == false)
            {
                LogTxtMsg(LoggerType.QueueLog, LoggerLevel.Info, clsGlobalVars.PrevBOQMMLocation.ToString("0.00") + "," +
                                   clsGlobalVars.PrevBOQTime.Hour + ":" + clsGlobalVars.PrevBOQTime.Minute + ":" + clsGlobalVars.PrevBOQTime.Second + ":" + clsGlobalVars.PrevBOQTime.Millisecond + "," +
                                   clsGlobalVars.BOQMMLocation.ToString("0.00") + "," +
                                   clsGlobalVars.BOQTime.Hour + ":" + clsGlobalVars.BOQTime.Minute + ":" + clsGlobalVars.BOQTime.Second + ":" + clsGlobalVars.BOQTime.Millisecond + "," +
                                   tmpBOQMMLocationChange + "," + span.TotalSeconds.ToString("0") + "," + clsGlobalVars.QueueRate.ToString("0") + "," + clsGlobalVars.QueueChange.ToString() + "," +
                                   clsGlobalVars.QueueSource.ToString() + "," + clsGlobalVars.CVBOQMMLocation.ToString("0.00") + "," + clsGlobalVars.InfrastructureBOQMMLocation.ToString("0.00") + "," + tmpSublinkData);
            }
            LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tPrev BOQ MM: " + clsGlobalVars.PrevBOQMMLocation.ToString("0.0") + "\t\tTime: " + clsGlobalVars.PrevBOQTime);
            if (clsGlobalVars.PrevBOQMMLocation != clsGlobalVars.BOQMMLocation)
            {
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tCurr BOQ MM:  " + clsGlobalVars.BOQMMLocation + "\t\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());
                clsGlobalVars.PrevBOQMMLocation = clsGlobalVars.BOQMMLocation;
                clsGlobalVars.PrevBOQTime = clsGlobalVars.BOQTime;
            }
            if (clsGlobalVars.BOQMMLocation != -1)
            {
                InsertQueueInfoIntoINFLODatabase(clsGlobalVars.BOQMMLocation, clsGlobalVars.BOQTime, clsGlobalVars.QueueRate, clsGlobalVars.QueueChange, clsGlobalVars.QueueSource, clsGlobalVars.QueueSpeed, Roadway);
            }
            //else
            //{
            //    InsertQueueInfoIntoINFLODatabase(0, clsGlobalVars.BOQTime, clsGlobalVars.QueueRate, clsGlobalVars.QueueChange, clsGlobalVars.QueueSource, clsGlobalVars.QueueSpeed, Roadway);
            //}
            #endregion

            //LogTxtMsg(txtTMEQWARNLog, "\r\n\t\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);
            //CVDataProcessor.WriteLine("\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);

            //LogTxtMsg(txtINFLOLog, "\t\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());
            //CVDataProcessor.WriteLine("\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());

            //Current BOQ
            if (clsGlobalVars.BOQMMLocation == -1)
            {
                clsGlobalVars.QueueLength = 0;
                clsGlobalVars.QueueSpeed = 0;
                DisplayForm.txtBOQ.Text = "No Queue";
                DisplayForm.txtQueueLength.Text = "";
                DisplayForm.txtQueueGrowthRate.Text = "";
                DisplayForm.txtQueueSpeed.Text = "";

            }
            else
            {
                double TotalQuedSublinksCVSpeed = 0;
                double TotalQuedSublinksVolume = 0;
                foreach (clsRoadwaySubLink RSL in RSLList)
                {
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        if ((RSL.BeginMM >= clsGlobalVars.BOQMMLocation) && (RSL.BeginMM < Roadway.RecurringCongestionMMLocation))
                        {
                            TotalQuedSublinksCVSpeed = TotalQuedSublinksCVSpeed + RSL.TotalNumberCVs * RSL.CVAvgSpeed;
                            TotalQuedSublinksVolume = TotalQuedSublinksVolume + RSL.TotalNumberCVs;
                        }
                    }
                    else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                    {
                        if ((RSL.BeginMM <= clsGlobalVars.BOQMMLocation) && (RSL.BeginMM > Roadway.RecurringCongestionMMLocation))
                        {
                            TotalQuedSublinksCVSpeed = TotalQuedSublinksCVSpeed + RSL.TotalNumberCVs * RSL.CVAvgSpeed;
                            TotalQuedSublinksVolume = TotalQuedSublinksVolume + RSL.TotalNumberCVs;
                        }
                    }
                }
                clsGlobalVars.QueueSpeed = TotalQuedSublinksCVSpeed / TotalQuedSublinksVolume;
                clsGlobalVars.QueueLength = Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation;
                DisplayForm.txtBOQ.Text = clsGlobalVars.BOQMMLocation.ToString();
                DisplayForm.txtQueueLength.Text = Math.Abs((Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation)).ToString("0.00");
                DisplayForm.txtQueueGrowthRate.Text = clsGlobalVars.QueueRate.ToString("0");
                DisplayForm.txtQueueSpeed.Text = clsGlobalVars.QueueSpeed.ToString("0");
            }
            LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
            //CVDataProcessor.WriteLine("\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
        }
        private void ApplyWRTMSpeed(ref List<clsRoadwayLink> RLList, double WRTMRecommendedSpeed, double WRTMBeginMM, double WRTMEndMM)
        {
            foreach (clsRoadwayLink RL in RLList)
            {
                if ((RL.BeginMM >= WRTMBeginMM) && (RL.BeginMM < WRTMEndMM))
                {
                    RL.WRTMSpeed = WRTMRecommendedSpeed;
                }
            }
        }
        private void DetermineLinkHarmonizedSpeed(List<clsRoadwaySubLink> RSLList, ref List<clsRoadwayLink> RLList, double TroupingEndMM)
        {
            foreach (clsRoadwayLink RL in RLList)
            {
                RL.RecommendedSpeed = 0;
                foreach (clsRoadwaySubLink RSL in RSLList)
                {
                    if (RL.BeginMM < TroupingEndMM)
                    {
                        if (RSL.BeginMM == RL.BeginMM)
                        {
                            RL.RecommendedSpeed = RSL.HarmonizedSpeed;//kg?? we set RLs Recommended speed, but it is not used? RSL's stuff is used in next function to generate spdHarm messages.
                            break;
                        }
                    }
                    else
                    {
                        if (RSL.BeginMM == RL.BeginMM)
                        {
                            RL.RecommendedSpeed = RSL.RecommendedSpeed;
                            break;
                        }
                    }
                }
            }
        }

        public void tmrCVData_Tick()
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            string tmpFileName = string.Empty;
            string CVWakeupTimeStr = string.Empty;

            //Calculate time difference between previous wakeuptime of the CV Timer for processing CV data and the current wakeupt time
            CVCurrWakeupTime = DateTime.Now;
            TimeSpan spandiff = CVCurrWakeupTime.Subtract(CVPrevWakeupTime);
            CVTimeDiff = spandiff.TotalMilliseconds;
            CVWakeupTimeStr = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "-" +
                              DateTime.Now.Minute.ToString("00") + "-" + DateTime.Now.Second.ToString("00") + "-" + DateTime.Now.Millisecond.ToString("00") + ", " + CVTimeDiff;

            CVPrevWakeupTime = CVCurrWakeupTime;

            DateTime currDateTime = DateTime.Now;
            currDateTime = TimeZoneInfo.ConvertTimeToUtc(currDateTime, TimeZoneInfo.Local);



            LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tTime difference between CV Timer concecutive wakeup times: " + CVTimeDiff.ToString("0"));
            LogTxtMsg(LoggerType.CV, LoggerLevel.Info,
                                    "\tGet CV traffic data for the last " + clsGlobalVars.CVDataPollingFrequency + " seconds interval From: " +
                                    currDateTime.AddSeconds(-clsGlobalVars.CVDataPollingFrequency) + "\tTo: " + currDateTime);

            //Get Last interval CV data
            #region "Get last interval CV data
            TimeSpan GetCVDataTime = new TimeSpan(DateTime.Now.Ticks);
            int NumberRecordsRetrieved = 0;
            retValue = GetLastIntervalCVData(DB, currDateTime, Roadway.Identifier.ToString(), ref CurrIntervalCVList,
                Roadway.Direction, Roadway.LowerHeading, Roadway.UpperHeading, ref NumberRecordsRetrieved);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\r\n\t" + retValue);

            }
            TimeSpan EndGetCVDataTime = new TimeSpan(DateTime.Now.Ticks);
            LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\t\tTime for retrieving " + CurrIntervalCVList.Count
                + " CV traffic data records from INFLO database: \t"
                + (EndGetCVDataTime.TotalMilliseconds - GetCVDataTime.TotalMilliseconds).ToString("0") + " msecs");

            #endregion


            if (NumberRecordsRetrieved > 0)//We got new CV data from the database.
            {
                CVWakeupTimeStr = CVWakeupTimeStr + ",NoCVRecords, " + NumberRecordsRetrieved;
                CVDataHarmonization++;
                DisplayForm.txtDataTypeCount.Text = "CV-Count: " + CVDataHarmonization;
                DisplayForm.txtCVRecords.Text = @"# CVs: " + NumberRecordsRetrieved.ToString();

                NumberNoCVDataIntervals = 0;
                DisplayForm.DisableNoCVDataMessage();
                //Reset Troupe speed, and inclusion flag for every sublink
                foreach (clsRoadwaySubLink rsl in RSLList)
                {
                    rsl.TroupeSpeed = 0;
                    rsl.TroupeInclusionOverride = false;
                    rsl.BeginTroupe = false;
                    rsl.TroupeProcessed = DateTime.Now;
                    rsl.HarmonizedSpeed = 0;
                    rsl.SpdHarmInclusionOverride = false;
                    rsl.BeginSpdHarm = false;
                    rsl.RecommendedSpeed = clsGlobalVars.MaximumDisplaySpeed;
                }

                #region "If CV data was found for the last 5 seconds"

                #region "Log received CV Data"
                CVDataFileCounter++;

                //tmpFileName = LogDirectoryPath + "\\CVDataFile-" + CVDataFileCounter.ToString();
                //CVDataLog = new StreamWriter(tmpFileName);
                //CVDataLog.WriteLine(DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Year.ToString("0000") + " " + DateTime.Now.Hour.ToString("00") + ":" +
                //                    DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + ":" + DateTime.Now.Millisecond.ToString("00"));
                ////CVDataProcessor.WriteLine("\r\n\tNomadicDeviceID, DateGenerated, Heading, latitude, Longitude, Speed, MMlocation, Queued, Temperature, CoefficientFriction, RoadwayId, SubLinkId, Direction");
                //foreach (clsCVData CV in CurrIntervalCVList)
                //{
                //    CVDataLog.WriteLine(CV.NomadicDeviceID + ", " + CV.DateGenerated + ", " + CV.Heading + ", " + CV.Latitude + ", " + CV.Longitude + ", " + CV.Speed + ", " + CV.MMLocation + ", " +
                //                              CV.Queued + ", " + CV.Temperature + ", " + CV.CoefficientFriction + ", " + CV.RoadwayID + ", " + CV.SublinkID + ", " + CV.Direction);
                //}
                //CVDataLog.Close();
                //CVDataLog = null;
                foreach (clsCVData CV in CurrIntervalCVList)
                {
                    LogTxtMsg(LoggerType.CVDataLog, LoggerLevel.Info, CVDataFileCounter.ToString() + ", " + CV.NomadicDeviceID + ", " + CV.DateGenerated + ", " + CV.Heading + ", " + CV.Latitude + ", " + CV.Longitude + ", " + CV.Speed + ", " + CV.MMLocation + ", " +
                                              CV.Queued + ", " + CV.Temperature + ", " + CV.CoefficientFriction + ", " + CV.RoadwayID + ", " + CV.SublinkID + ", " + CV.Direction);
                }
                #endregion

                #region "Calculate sublink speed and queued state using CV data
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug,
                    "\r\n\t\tUpdate roadway sublink traffic parameters using CV data from the last  "
                    + clsGlobalVars.CVDataPollingFrequency + " seconds interval.");
                retValue = ProcessRoadwaySublinkQueuedStatus(Roadway, ref CurrIntervalCVList, ref RSLList);
                if (retValue.Length > 0)
                {
                    LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\r\n\t" + retValue);
                    return;
                }
                #endregion

                //#region "Log Updated sublink data"
                //CVDataProcessor.WriteLine("\r\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond + 
                //                                   "\tUpdated Roadway SubLink status using CV data.");
                //CVDataProcessor.WriteLine("\r\n\tRoadwayId, SubLinkId, DateProcessed, BeginMM, EndMM, NumberQueuedCVs, TotalNumberCVS, PercentQueuedCVs, Queued, AvgSpeed, Direction");
                //foreach (clsRoadwaySubLink rsl in RSLList)
                //{
                //    CVDataProcessor.WriteLine("\t" + rsl.RoadwayID + ", " + rsl.Identifier + ", " + rsl.DateProcessed + ", " + rsl.BeginMM + ", " + rsl.EndMM + ", " + rsl.NumberQueuedCVs + ", " +
                //                              rsl.TotalNumberCVs + ", " + rsl.PercentQueuedCVs + ", " + rsl.Queued + ", " + rsl.CVAvgSpeed.ToString("0") + ", " + rsl.Direction);
                //}
                //#endregion

                LogToExcel(LogToExcelType.CV, currDateTime);
                TimeSpan Processingtime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tTime for processing " + CurrIntervalCVList.Count
                    + " CV traffic data records from the last interval: \t"
                    + (Processingtime.TotalMilliseconds - EndGetCVDataTime.TotalMilliseconds).ToString("0") + " msecs");

                #region "Insert Sublink Status Info into INFLO Database"
                ///retValue = InsertSubLinkStatusIntoINFLODatabase();
                //if (retValue.Length > 0)
                //{
                //    LogTxtMsg(retValue);
                //}
                //else
                //{
                //    LogTxtMsg(DateTime.Now + "\t\tFinish inserting processed TSS roadway links status into INFLO database");
                //}
                #endregion


                //Determine CV sublink BOQ location
                TimeSpan BeginCVBOQ = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\r\n\t\tDetermine CV SubLink BOQ location:");


                clsGlobalVars.PrevCVBOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                clsGlobalVars.PrevCVBOQTime = clsGlobalVars.CVBOQTime;
                clsGlobalVars.CVBOQMMLocation = -1;

                #region "Determine CV Sublink BOQ MM Location"
                //TotalCVs = 0;
                //QueuedCVs = 0;
                QueuedSubLink = new clsRoadwaySubLink();
                //RSLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                foreach (clsRoadwaySubLink RSL in RSLList)
                {
                    //the most downstream sublink is always considered the front of the queue (FOQ).
                    //Move opposite of that to find the Back. BOQ
                    if (RSL.Queued == true)
                    {
                        if (Roadway.MMIncreasingDirection == Roadway.Direction)
                        {
                            /*The RecurringCongestionMMLocation is the location of the front of the queue.
                             * The Roadway mile markers are usually increasing in one direction and decreasing 
                             * in the opposite direction. Usually they number highway mile markers to be increasing
                             * in the northbound direction. For example, IH-35 which is a north/south interstate
                             * highway starts with mile marker 1 and the mile marker numbers increase as you go up
                             * north on it. The IH-35 southbound will use the same mile markers, but they will be
                             * decreasing as you are traveling south on it.
                             * The back of the queue in the northbound direction should be a mile marker less than
                             * the RecurringMMLocation (Front of the queue). While in the southbound direction, 
                             * the back of queue should be a mile marker greater than the Recurring MMLocation. 
                             * The back of the queue is always upstream of the front of the queue in the direction 
                             * of travel.
                                */
                            if (RSL.BeginMM < Roadway.RecurringCongestionMMLocation)
                            {
                                if (clsGlobalVars.CVBOQMMLocation != -1)
                                {//if this RSL is more upstream than the current BackOfQueue, then save this one instead.
                                    if (RSL.BeginMM < clsGlobalVars.CVBOQMMLocation) //kg: couldn't we have just initialized the CVBOQMMLocation to = Roadway.RecurringCongestionMMLocation, then the first one would always be accepted and we would not need the if/else?
                                    {
                                        clsGlobalVars.CVBOQMMLocation = RSL.BeginMM;
                                        clsGlobalVars.CVBOQTime = DateTime.Now;
                                        clsGlobalVars.CVBOQSublinkSpeed = RSL.CVAvgSpeed;
                                        //QueuedSubLink = new clsRoadwaySubLink();
                                        QueuedSubLink = RSL;
                                    }
                                }
                                else
                                {//grab the first one we see as initial condition
                                    clsGlobalVars.CVBOQMMLocation = RSL.BeginMM;
                                    clsGlobalVars.CVBOQTime = DateTime.Now;
                                    clsGlobalVars.CVBOQSublinkSpeed = RSL.CVAvgSpeed;
                                    //QueuedSubLink = new clsRoadwaySubLink();
                                    QueuedSubLink = RSL;
                                }
                            }
                        }
                        else if (Roadway.MMIncreasingDirection != Roadway.Direction)
                        {
                            if (RSL.BeginMM > Roadway.RecurringCongestionMMLocation)
                            {
                                if (clsGlobalVars.CVBOQMMLocation != -1)
                                {
                                    if (RSL.BeginMM > clsGlobalVars.CVBOQMMLocation)
                                    {
                                        clsGlobalVars.CVBOQMMLocation = RSL.BeginMM;
                                        clsGlobalVars.CVBOQTime = DateTime.Now;
                                        clsGlobalVars.CVBOQSublinkSpeed = RSL.CVAvgSpeed;
                                        //QueuedSubLink = new clsRoadwaySubLink();
                                        QueuedSubLink = RSL;
                                    }
                                }
                                else
                                {
                                    clsGlobalVars.CVBOQMMLocation = RSL.BeginMM;
                                    clsGlobalVars.CVBOQTime = DateTime.Now;
                                    clsGlobalVars.CVBOQSublinkSpeed = RSL.CVAvgSpeed;
                                    //QueuedSubLink = new clsRoadwaySubLink();
                                    QueuedSubLink = RSL;
                                }
                            }
                        }
                    }
                }
                #endregion


                //DisplayForm.ClearCVSubLinkQueuedStatus();
                //DisplayForm.DisplayCVSubLinkQueuedStatus(RSLList);

                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\t\t\tPrev CV  BOQ MM location: "
                    + clsGlobalVars.PrevCVBOQMMLocation + "\tTime: " + clsGlobalVars.PrevCVBOQTime);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\t\t\tCurr CV  BOQ MM location: "
                    + clsGlobalVars.CVBOQMMLocation + "\tTime: " + clsGlobalVars.CVBOQTime);

                //kg: all display stuff. avl in clsGlobalVars
                ////CV BOQ
                //DisplayForm.txtCVDate.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                //if (clsGlobalVars.CVBOQMMLocation == -1)
                //{
                //    DisplayForm.txtCVBOQ.Text = "No Queue";
                //    CVWakeupTimeStr = CVWakeupTimeStr + ",CVBOQ, " + clsGlobalVars.CVBOQMMLocation;
                //}
                //else
                //{
                //    txtCVBOQMMLocation.Text = clsGlobalVars.CVBOQMMLocation.ToString();
                //    txtCVBOQDate.Text = clsGlobalVars.CVBOQTime.ToString();
                //    DisplayForm.txtCVBOQ.Text = clsGlobalVars.CVBOQMMLocation.ToString();
                //    CVWakeupTimeStr = CVWakeupTimeStr + ",CVBOQ, " + clsGlobalVars.CVBOQMMLocation;
                //}

                ////CV Previou BOQ
                //if (clsGlobalVars.PrevCVBOQMMLocation == -1)
                //{
                //    txtCVPrevBOQMMLocation.Text = "No Queue";
                //    txtCVPrevBOQDate.Text = clsGlobalVars.PrevCVBOQTime.ToString();
                //}
                //else
                //{
                //    txtCVPrevBOQMMLocation.Text = clsGlobalVars.PrevCVBOQMMLocation.ToString();
                //    txtCVPrevBOQDate.Text = clsGlobalVars.PrevCVBOQTime.ToString();
                //}

                TimeSpan EndCVBOQ = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tTime for processing CVBOQ MM Location: \t" + (EndCVBOQ.TotalMilliseconds - BeginCVBOQ.TotalMilliseconds).ToString("0") + " msecs");

                #region "Reconcile the CV BOQ and TSS BOQ - Old Algorithm"
                /*
                LogTxtMsg("\tDetermine BOQ MM location from CV and TSS BOQ MM locations:");
                CVDataProcessor.WriteLine("\r\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond +
                                          "\tDetermine BOQ MM location from CV and TSS BOQ MM locations:");

                #region "Reconcile CV BOQ MM Location and TSS data BOQ MM Location"
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (clsGlobalVars.CVBOQMMLocation < clsGlobalVars.InfrastructureBOQMMLocation)
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                        }
                        else
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                    }
                    else
                    {
                        clsGlobalVars.BOQMMLocation = -1;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (clsGlobalVars.CVBOQMMLocation > clsGlobalVars.InfrastructureBOQMMLocation)
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                        }
                        else
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                    }
                    else
                    {
                        clsGlobalVars.BOQMMLocation = -1;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;
                    }
                }
                #endregion

                double tmpBOQMMLocationChange = 0;

                #region "Determine queue rate of change"
                    TimeSpan span = clsGlobalVars.BOQTime.Subtract(clsGlobalVars.PrevBOQTime);
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation != -1))
                        {
                            tmpBOQMMLocationChange = clsGlobalVars.BOQMMLocation - clsGlobalVars.PrevBOQMMLocation;
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            if (tmpBOQMMLocationChange < 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                            }
                            else if (tmpBOQMMLocationChange > 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                            }
                            else if (tmpBOQMMLocationChange == 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                            }
                        }
                        else if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation == -1))
                        {
                            tmpBOQMMLocationChange = (Roadway.RecurringCongestionMMLocation - clsGlobalVars.PrevBOQMMLocation);
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                        }
                        else if ((clsGlobalVars.PrevBOQMMLocation == -1) && (clsGlobalVars.BOQMMLocation != -1))
                        {
                            tmpBOQMMLocationChange = (Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation);
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                        }
                        else
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                            clsGlobalVars.QueueRate = 0;
                        }
                        
                        QueueLog.WriteLine(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond + ",CV," + 
                                           clsGlobalVars.PrevBOQMMLocation.ToString("0.0") + "," + clsGlobalVars.PrevBOQTime.ToString() + "," + 
                                           clsGlobalVars.BOQMMLocation.ToString("0.0") + "," + clsGlobalVars.BOQTime.ToString() + "," + clsGlobalVars.QueueRate.ToString("0.00") + "," + 
                                           clsGlobalVars.QueueChange.ToString() + "," + clsGlobalVars.QueueSource.ToString());
                        if (clsGlobalVars.PrevBOQMMLocation != clsGlobalVars.BOQMMLocation)
                        {
                            clsGlobalVars.PrevBOQMMLocation = clsGlobalVars.BOQMMLocation;
                            clsGlobalVars.PrevBOQTime = clsGlobalVars.BOQTime;
                        }
                    }
                    else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                    {
                        if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation != -1))
                        {
                            tmpBOQMMLocationChange = clsGlobalVars.BOQMMLocation - clsGlobalVars.PrevBOQMMLocation;
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            if (tmpBOQMMLocationChange < 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                            }
                            else if (tmpBOQMMLocationChange > 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                            }
                            else if (tmpBOQMMLocationChange == 0)
                            {
                                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                            }
                        }
                        else if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation == -1))
                        {
                            tmpBOQMMLocationChange = (clsGlobalVars.PrevBOQMMLocation - Roadway.RecurringCongestionMMLocation);
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                        }
                        else if ((clsGlobalVars.PrevBOQMMLocation == -1) && (clsGlobalVars.BOQMMLocation != -1))
                        {
                            tmpBOQMMLocationChange = (clsGlobalVars.BOQMMLocation - Roadway.RecurringCongestionMMLocation);
                            clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                        }
                        else
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                            clsGlobalVars.QueueRate = 0;
                        }
                        QueueLog.WriteLine(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond + ",CV," +
                                            clsGlobalVars.PrevBOQMMLocation.ToString("0.0") + "," + clsGlobalVars.PrevBOQTime.ToString() + "," +
                                            clsGlobalVars.BOQMMLocation.ToString("0.0") + "," + clsGlobalVars.BOQTime.ToString() + "," + clsGlobalVars.QueueRate.ToString("0.00") + "," +
                                            clsGlobalVars.QueueChange.ToString() + "," + clsGlobalVars.QueueSource.ToString());
                        if (clsGlobalVars.PrevBOQMMLocation != clsGlobalVars.BOQMMLocation)
                        {
                            clsGlobalVars.PrevBOQMMLocation = clsGlobalVars.BOQMMLocation;
                            clsGlobalVars.PrevBOQTime = clsGlobalVars.BOQTime;
                        }
                    }
                #endregion

                LogTxtMsg("\t\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);
                CVDataProcessor.WriteLine("\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);

                LogTxtMsg("\t\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());
                CVDataProcessor.WriteLine("\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());


                //Previous BOQ
                if (clsGlobalVars.PrevBOQMMLocation == -1)
                {
                    txtPrevBOQMMLocation.Text = "No Queue";
                    txtPrevBOQDate.Text = string.Empty;
                }
                else
                {
                    txtPrevBOQMMLocation.Text = clsGlobalVars.PrevBOQMMLocation.ToString();
                    txtPrevBOQDate.Text = clsGlobalVars.PrevBOQTime.ToString();
                }

                //Current BOQ
                if (clsGlobalVars.BOQMMLocation == -1)
                {
                    txtBOQMMLocation.Text = "No Queue";
                    txtBOQDate.Text = string.Empty;
                    DisplayForm.txtBOQ.Text = "No Queue";
                }
                else
                {
                    txtBOQMMLocation.Text = clsGlobalVars.BOQMMLocation.ToString();
                    txtBOQDate.Text = clsGlobalVars.BOQTime.ToString();
                    DisplayForm.txtBOQ.Text = clsGlobalVars.BOQMMLocation.ToString();
                }
                txtBOQGrowthRate.Text = clsGlobalVars.QueueRate.ToString(); ;
                txtBOQGrowthType.Text = clsGlobalVars.QueueChange.ToString();

                LogTxtMsg("\t\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
                CVDataProcessor.WriteLine("\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
                */
                #endregion



                #region "Reconcile the CV BOQ and TSS BOQ"
                TimeSpan BeginBOQ = new TimeSpan(DateTime.Now.Ticks);
                DetermineBOQ(QueuedSubLink, QueuedLink, Roadway);
                TimeSpan EndBOQ = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tTime for reconciling BOQ MM Location: \t" + (EndBOQ.TotalMilliseconds - BeginBOQ.TotalMilliseconds).ToString("0") + " msecs");
                #endregion
                CVWakeupTimeStr = CVWakeupTimeStr + ",BOQ, " + clsGlobalVars.BOQMMLocation;

                //Start the sublink trouping process
                //The Troupe is a group of links/sublinks  whose speeds are within a certain range
                //(which is user defined in the configuration file)
                TimeSpan StartTrouping = new TimeSpan(DateTime.Now.Ticks);

                //Determine the Recommended speed for each sublink
                #region "Determine sublink recommended speed"
                foreach (clsRoadwayLink rl in RLList)
                {
                    foreach (clsRoadwaySubLink rsl in RSLList)
                    {
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            if ((rsl.BeginMM >= rl.BeginMM) && (rsl.BeginMM < rl.EndMM))
                            {
                                rsl.TSSAvgSpeed = rl.TSSAvgSpeed;
                                rsl.WRTMSpeed = rl.WRTMSpeed;
                                //Average all cars within the sublink to get the speed of the sublink, and compare
                                //to the infrastructure speed, and weather speed, and take the lower of the three speeds.
                                rsl.RecommendedSpeed = GetMinimumSpeed(rsl.TSSAvgSpeed, rsl.WRTMSpeed, rsl.CVAvgSpeed);
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            if ((rsl.BeginMM <= rl.BeginMM) && (rsl.BeginMM > rl.EndMM))
                            {
                                rsl.TSSAvgSpeed = rl.TSSAvgSpeed;
                                rsl.WRTMSpeed = rl.WRTMSpeed;
                                rsl.RecommendedSpeed = GetMinimumSpeed(rsl.TSSAvgSpeed, rsl.WRTMSpeed, rsl.CVAvgSpeed);
                            }
                        }
                    }
                }
                #endregion

                //Determine the ending MM for the trouping process
                #region "Determine the Trouping Ending MM Location"
                /*if (clsGlobalVars.BOQMMLocation == -1)
                {
                    //Locate the first sublink upstream of the Recurring congestion MMLocation that is congested 
                    for (int r = RSLList.Count - 1; r >= 0; r--)
                    {
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[r].BeginMM < Roadway.RecurringCongestionMMLocation)
                            {
                                if ((RSLList[r].RecommendedSpeed > clsGlobalVars.LinkQueuedSpeedThreshold) && (RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                {
                                    TroupingEndMM = RSLList[r].BeginMM;
                                    TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                    break;
                                }
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[r].BeginMM > Roadway.RecurringCongestionMMLocation)
                            {
                                if ((RSLList[r].RecommendedSpeed > clsGlobalVars.LinkQueuedSpeedThreshold) && (RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                {
                                    TroupingEndMM = RSLList[r].BeginMM;
                                    TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (clsGlobalVars.BOQMMLocation > 0)
                {
                    TroupingEndMM = clsGlobalVars.BOQMMLocation;
                    TroupingEndSpeed = clsGlobalVars.BOQSpeed;
                }*/
                //Locate the first sublink upstream of the Recurring congestion MMLocation that is congested 
                TroupingEndMM = 0;
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {
                    //TroupingEndMM = Roadway.RecurringCongestionMMLocation + 1000;
                    for (int r = RSLList.Count - 1; r >= 0; r--)
                    {
                        if (RSLList[r].BeginMM < Roadway.RecurringCongestionMMLocation)
                        {
                            if ((RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                            {
                                if (RSLList[r].BeginMM > TroupingEndMM)
                                {
                                    TroupingEndMM = RSLList[r].BeginMM;
                                    TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                    //break;
                                }
                            }
                        }
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    //TroupingEndMM = Roadway.RecurringCongestionMMLocation - 1000;
                    for (int r = RSLList.Count - 1; r >= 0; r--)
                    {
                        if (RSLList[r].BeginMM > Roadway.RecurringCongestionMMLocation)
                        {
                            if ((RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                            {
                                if (TroupingEndMM == 0)
                                {
                                    TroupingEndMM = RSLList[r].BeginMM;
                                    TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                }
                                else if (RSLList[r].BeginMM < TroupingEndMM)
                                {
                                    TroupingEndMM = RSLList[r].BeginMM;
                                    TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                    //break;
                                }
                            }
                        }
                    }
                }
                #endregion

                //DisplayForm.ClearCVSubLinkTroupeStatus();
                //DisplayForm.ClearCVSubLinkSPDHarmStatus();
                //DisplayForm.ClearTSSSPDHarmLinkStatus();

                //perform the Trouping and Harmonization processes
                if (TroupingEndMM > 0)
                {
                    #region "Perform trouping and harmonization"
                    DisplayForm.DisableNoTroupingMessage();
                    DisplayForm.DisableNoSpdHarmMessage();
                    retValue = CalculateSublinkTroupeSpeed(ref RSLList, TroupingEndMM, TroupingEndSpeed);
                    if (retValue.Length > 0)
                    {
                        LogTxtMsg(LoggerType.CV, LoggerLevel.Error, "CalculateSublinkTroupeSpeed returned: " + retValue);
                    }
                    retValue = CalculateSublinkHarmonizedSpeed(ref RSLList, TroupingEndMM, TroupingEndSpeed);
                    if (retValue.Length > 0)
                    {
                        LogTxtMsg(LoggerType.CV, LoggerLevel.Error, "CalculateSublinkHarmonizedSpeed returned: " + retValue);
                    }

                    #region "Log Trouping and Harmonized sublink speeds"
                    LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\r\n\tUpdated Roadway SubLink Recommended, Troupe, and Harmonized Speeds using CV data.");
                    LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\r\n\tRoadwayId, SubLinkId, DateProcessed, BeginMM, EndMM, NumberQueuedCVs, TotalNumberCVS, PercentQueuedCVs, Queued, CVAvgSpd, TSSAvgSpd, RecommendedSpd, TroupeSpd, TroupePverride, HarmonizedSpeed, HarmonizationOverride, Direction");
                    foreach (clsRoadwaySubLink rsl in RSLList)
                    {
                        LogTxtMsg(LoggerType.CV, LoggerLevel.Info, "\t" + rsl.RoadwayID + ", " + rsl.Identifier + ", " + rsl.DateProcessed + ", " + rsl.BeginMM + ", " + rsl.EndMM + ", " + rsl.NumberQueuedCVs + ", " +
                                                  rsl.TotalNumberCVs + ", " + rsl.PercentQueuedCVs + ", " + rsl.Queued + ", " + rsl.CVAvgSpeed.ToString("0") + ", " + rsl.TSSAvgSpeed.ToString("0") + ", " +
                                                  rsl.RecommendedSpeed.ToString("0") + ", " + rsl.TroupeSpeed.ToString("0") + ", " + rsl.TroupeInclusionOverride.ToString() + ", " +
                                                   rsl.HarmonizedSpeed.ToString() + ", " + rsl.SpdHarmInclusionOverride.ToString() + ", " + rsl.Direction);
                    }
                    #endregion

                    //DisplayForm.DisplaySublinkTroupeSpeed(RSLList, TroupingEndMM, Roadway);
                    //DisplayForm.DisplaySublinkHarmonizedSpeed(RSLList, TroupingEndMM, Roadway);
                    DetermineLinkHarmonizedSpeed(RSLList, ref RLList, TroupingEndMM);
                    //DisplayForm.DisplayTSSSPDHarmLinkStatus(RLList);

                    GenerateSPDHarmMessages(RSLList, TroupingEndMM, clsGlobalVars.BOQMMLocation, Roadway);

                    #endregion
                    CVWakeupTimeStr = CVWakeupTimeStr + ",Trouping, " + TroupingEndMM;
                }
                else
                {
                    #region "If no trouping is required"
                    LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tNo Trouping is required for sublinks because no queued or congested sublinks were detected");
                    DisplayForm.EnableNoTroupingMessage(Roadway.RecurringCongestionMMLocation);
                    DisplayForm.EnableNoSpdHarmMessage(Roadway.RecurringCongestionMMLocation);
                    #endregion
                    CVWakeupTimeStr = CVWakeupTimeStr + ",Trouping, " + TroupingEndMM;
                }

                LogToExcel(LogToExcelType.CVSPD, currDateTime);

                TimeSpan EndTrouping = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tTime for sublink speed harmonization: \t" + (EndTrouping.TotalMilliseconds - StartTrouping.TotalMilliseconds).ToString("0") + " msecs");

                #endregion
            }
            else//No new data retrieved.
            {
                DisplayForm.txtDataTypeCount.Text = "";
                CVDataHarmonization = 0;
                DisplayForm.txtCVRecords.Text = @"# CVs: " + NumberRecordsRetrieved.ToString();

                #region "If NO CV data was retrieved for the last five seconds"
                NumberNoCVDataIntervals = NumberNoCVDataIntervals + 1;
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tNo CV traffic data was retrieved for the last " + clsGlobalVars.CVDataPollingFrequency + " second interval. " +
                                        "\r\n\t\tNumber of " + clsGlobalVars.CVDataPollingFrequency + " second intervals with no CV data retrieved: \t" + NumberNoCVDataIntervals);
                DisplayForm.txtCVDate.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                CVWakeupTimeStr = CVWakeupTimeStr + ",NoCVData, " + NumberNoCVDataIntervals;

                LogToExcel(LogToExcelType.CVNoData, currDateTime);

                if (NumberNoCVDataIntervals > clsGlobalVars.CVDataSmoothedSpeedArraySize)
                {
                    if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        clsGlobalVars.CVBOQMMLocation = -1;
                        clsGlobalVars.CVBOQTime = DateTime.Now;
                    }
                    foreach (clsRoadwaySubLink RSL in RSLList)
                    {
                        RSL.CVAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                        RSL.Queued = false;
                        RSL.Congested = false;
                        //RSL.HarmonizedSpeed = clsGlobalVars.MaximumDisplaySpeed;
                        //RSL.TroupeSpeed = clsGlobalVars.MaximumDisplaySpeed;
                    }
                    DisplayForm.EnableNoCVDataMessage(NumberNoCVDataIntervals);
                    DisplayForm.txtCVBOQ.Text = "No Queue";
                    DisplayForm.ClearCVSubLinkQueuedStatus();
                    CVWakeupTimeStr = CVWakeupTimeStr + ",CVBOQ-1, " + clsGlobalVars.CVBOQMMLocation;


                    if (NumberNoTSSDataIntervals > 0)
                    {
                        CVWakeupTimeStr = CVWakeupTimeStr + ",NOTSSDataIntervals, " + NumberNoTSSDataIntervals;
                        DisplayForm.txtDataTypeCount.Text = @"No CVs#: " + NumberNoCVDataIntervals + @"No TSS#: " + NumberNoTSSDataIntervals;
                        DisplayForm.txtBOQ.Text = "No Queue";
                        DisplayForm.txtCVBOQ.Text = "No Queue";
                        DisplayForm.txtTSSBOQ.Text = "No Queue";
                        DisplayForm.txtQueueGrowthRate.Text = "";
                        DisplayForm.txtQueueSpeed.Text = "";
                        DisplayForm.txtQueueLength.Text = "";

                        DisplayForm.ClearCVSubLinkSPDHarmStatus();
                        DisplayForm.ClearCVSubLinkTroupeStatus();
                        DisplayForm.ClearTSSSPDHarmLinkStatus();
                        //Reset Troupe speed, and inclusion flag for every sublink
                        foreach (clsRoadwaySubLink rsl in RSLList)
                        {
                            rsl.TroupeSpeed = 0;
                            rsl.TroupeInclusionOverride = false;
                            rsl.BeginTroupe = false;
                            rsl.TroupeProcessed = DateTime.Now;
                            rsl.HarmonizedSpeed = 0;
                            rsl.SpdHarmInclusionOverride = false;
                            rsl.BeginSpdHarm = false;
                        }
                        DisplayForm.EnableNoSpdHarmMessage(Roadway.RecurringCongestionMMLocation);
                        DisplayForm.EnableNoTroupingMessage(Roadway.RecurringCongestionMMLocation);


                        LogToExcel(LogToExcelType.CVSPD, currDateTime);
                    }
                    //else
                    //{
                    //    DisplayForm.DisableNoSpdHarmMessage();
                    //    DisplayForm.DisableNoTroupingMessage();
                    //}
                }
                #endregion
            }



        }


        public void tmrTSSData_Tick()
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            string TSSWakeupTimeStr = string.Empty;

            //Interval = Interval + 1;

            //Calculate time difference between previous wakeuptime of the CV Timer for processing CV data and the current wakeupt time
            TSSCurrWakeupTime = DateTime.Now;
            TimeSpan spandiff = TSSCurrWakeupTime.Subtract(TSSPrevWakeupTime);
            TSSTimeDiff = spandiff.TotalMilliseconds;
            TSSWakeupTimeStr = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "-" +
                                      DateTime.Now.Minute.ToString("00") + "-" + DateTime.Now.Second.ToString("00") + "-" + DateTime.Now.Millisecond.ToString("00") + ", " + TSSTimeDiff;
            TSSPrevWakeupTime = TSSCurrWakeupTime;


            DateTime currDateTime = DateTime.Now;
            currDateTime = TimeZoneInfo.ConvertTimeToUtc(currDateTime, TimeZoneInfo.Local);



            LogTxtMsg(LoggerType.TSS, LoggerLevel.Info,
                                     "\tGet TSS detection zone traffic data for the last " + clsGlobalVars.TSSDataLoadingFrequency + " seconds interval from: " +
                                     currDateTime.AddSeconds(-clsGlobalVars.TSSDataLoadingFrequency) + "\tTo: " + currDateTime);

            //Get Last interval TSS data
            #region "Get last interval TSS data"
            TimeSpan GetTSSDataTime = new TimeSpan(DateTime.Now.Ticks);
            int NumberRecordsRetrieved = 0;
            retValue = GetLastIntervalDetectionZoneStatus(DB, currDateTime, ref DZList, ref NumberRecordsRetrieved);
            if (retValue.Length > 0)
            {
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Error, "GetLastIntervalDetectionZoneStatus returned:" + retValue);
            }
            TimeSpan EndGetTSSDataTime = new TimeSpan(DateTime.Now.Ticks);
            #endregion

            if (NumberRecordsRetrieved > 0)
            {
                TSSWakeupTimeStr = TSSWakeupTimeStr + ",NumRecords, " + NumberRecordsRetrieved;
                DisplayForm.txtTSSRecords.Text = @"# Detectorss: " + NumberRecordsRetrieved.ToString();
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\t\tTime for retrieving "
                    + DZList.Count + " detection zone traffic data records from INFLO database: "
                    + (EndGetTSSDataTime.TotalMilliseconds - GetTSSDataTime.TotalMilliseconds).ToString("0") + " msecs");

                #region "Process detection station traffic data"
                TimeSpan StartProcessingDSStatus = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info,
                    "\r\n\t\tCalculate detector station traffic parameters (average speed, occupancy and volume) using detection zone traffic data.");
                retValue = ProcessDetectorStationStatus();
                TimeSpan EndProcessingDSStatus = new TimeSpan(DateTime.Now.Ticks);
                if (retValue.Length > 0)
                {
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Error, "ProcessDetectorStationStatus returned: " + retValue);
                }
                else
                {
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\t\tTime for calculating TSS detector station traffic parameters: "
                        + EndProcessingDSStatus.Subtract(StartProcessingDSStatus).TotalMilliseconds + " milliseconds");
                }
                #endregion

                #region "Process Roadway Link traffic data"
                TimeSpan StartProcessingRLStatus = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\n\t\tDetermine roadway link average speed, occupancy, volume and queued state.");
                retValue = ProcessLinkInfrastructureStatus(Roadway);
                if (retValue.Length > 0)
                {
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "ProcessLinkInfrastructureStatus returned: " + retValue);

                }
                else
                {
                    TimeSpan EndProcessingRLStatus = new TimeSpan(DateTime.Now.Ticks);
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Info,
                        "\r\n\tTime for setting TSS Link traffic parameters: "
                        + EndProcessingRLStatus.Subtract(StartProcessingRLStatus).TotalMilliseconds + " milliseconds");
                }
                #endregion

                LogToExcel(LogToExcelType.TSS, currDateTime);

                TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tTime for processing: TSS records for the last " + clsGlobalVars.TSSDataLoadingFrequency + " seconds interval: " + (endtime.TotalMilliseconds - EndGetTSSDataTime.TotalMilliseconds).ToString("0") + " msecs");

                /*LogTxtMsg("\r\n\tLoad roadway link traffic data into INFLO database.");
                retValue = InsertLinkStatusIntoINFLODatabase();
                if (retValue.Length > 0)
                {
                    LogTxtMsg(retValue);
                }
                else
                {
                    LogTxtMsg("\r\n\tFinish loading TSS roadway links traffic data into INFLO database");
                }*/

                //Determine TSS link BOQ location
                TimeSpan BeginTSSBOQ = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\n\t\tDetermine TSS BOQ location:");

                clsGlobalVars.PrevInfrastructureBOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                clsGlobalVars.PrevInfrastructureBOQTime = clsGlobalVars.InfrastructureBOQTime;
                clsGlobalVars.InfrastructureBOQMMLocation = -1;

                #region "Determine TSS BOQ MM Location"
                QueuedLink = new clsRoadwayLink();
                //RLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                foreach (clsRoadwayLink RL in RLList)
                {
                    if (RL.Queued == true)
                    {
                        if (Roadway.MMIncreasingDirection == Roadway.Direction)
                        {
                            if (RL.BeginMM < Roadway.RecurringCongestionMMLocation)
                            {
                                if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                                {
                                    if (RL.BeginMM < clsGlobalVars.InfrastructureBOQMMLocation)
                                    {
                                        clsGlobalVars.InfrastructureBOQMMLocation = RL.BeginMM;
                                        clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
                                        clsGlobalVars.InfrastructureBOQLinkSpeed = RL.TSSAvgSpeed;
                                        //QueuedLink = new clsRoadwayLink();
                                        QueuedLink = RL;
                                        //LogTxtMsg("\t\t\tTSS BOQ MM Location: " + clsGlobalVars.InfrastructureBOQMMLocation);
                                    }
                                }
                                else
                                {
                                    clsGlobalVars.InfrastructureBOQMMLocation = RL.BeginMM;
                                    clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
                                    clsGlobalVars.InfrastructureBOQLinkSpeed = RL.TSSAvgSpeed;
                                    //QueuedLink = new clsRoadwayLink();
                                    QueuedLink = RL;
                                    //LogTxtMsg("\t\t\tTSS BOQ MM Location: " + clsGlobalVars.InfrastructureBOQMMLocation);
                                }
                            }
                        }
                        else if (Roadway.MMIncreasingDirection != Roadway.Direction)
                        {
                            if (RL.BeginMM > Roadway.RecurringCongestionMMLocation)
                            {
                                if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                                {
                                    if (RL.BeginMM > clsGlobalVars.InfrastructureBOQMMLocation)
                                    {
                                        clsGlobalVars.InfrastructureBOQMMLocation = RL.BeginMM;
                                        clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
                                        clsGlobalVars.InfrastructureBOQLinkSpeed = RL.TSSAvgSpeed;
                                        //QueuedLink = new clsRoadwayLink();
                                        QueuedLink = RL;
                                        //LogTxtMsg("\t\t\tTSS BOQ MM Location: " + clsGlobalVars.InfrastructureBOQMMLocation);
                                    }
                                }
                                else
                                {
                                    clsGlobalVars.InfrastructureBOQMMLocation = RL.BeginMM;
                                    clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
                                    clsGlobalVars.InfrastructureBOQLinkSpeed = RL.TSSAvgSpeed;
                                    //QueuedLink = new clsRoadwayLink();
                                    QueuedLink = RL;
                                    //LogTxtMsg("\t\t\tTSS BOQ MM Location: " + clsGlobalVars.InfrastructureBOQMMLocation);
                                }
                            }
                        }
                    }
                }
                #endregion
                /*
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {
                    if ((Interval == 2) || (Interval == 4) || (Interval == 6) || (Interval == 7) || (Interval == 8))
                    {
                        clsGlobalVars.InfrastructureBOQMMLocation = Roadway.RecurringCongestionMMLocation - Interval;
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    if ((Interval == 2) || (Interval == 4) || (Interval == 6) || (Interval == 7) || (Interval == 8))
                    {
                        clsGlobalVars.InfrastructureBOQMMLocation = Roadway.RecurringCongestionMMLocation + Interval;
                    }
                }
                */
                DisplayForm.txtTSSDate.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                //DisplayForm.ClearTSSQueuedLinkStatus();
                //DisplayForm.DisplayTSSQueuedLinkStatus(RLList);

                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\t\tPrev TSS BOQ MM location: "
                    + clsGlobalVars.PrevInfrastructureBOQMMLocation + "\tTime: " + clsGlobalVars.PrevInfrastructureBOQTime);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\t\tCurr TSS BOQ MM location: "
                    + clsGlobalVars.InfrastructureBOQMMLocation + "\tTime: " + clsGlobalVars.InfrastructureBOQTime);

                //kg: all display stuff. available in clsGlobalVars
                ////Infrastructure BOQ
                //if (clsGlobalVars.InfrastructureBOQMMLocation == -1)
                //{
                //    txtTSSBOQMMLocation1.Text = "No Queue";
                //    txtTSSBOQDate1.Text = string.Empty;
                //    DisplayForm.txtTSSBOQ.Text = "No Queue";
                //    TSSWakeupTimeStr = TSSWakeupTimeStr + ", BOQ, " + clsGlobalVars.InfrastructureBOQMMLocation;
                //}
                //else
                //{
                //    txtTSSBOQMMLocation1.Text = clsGlobalVars.InfrastructureBOQMMLocation.ToString();
                //    txtTSSBOQDate1.Text = clsGlobalVars.InfrastructureBOQTime.ToString();
                //    DisplayForm.txtTSSBOQ.Text = clsGlobalVars.InfrastructureBOQMMLocation.ToString();
                //    TSSWakeupTimeStr = TSSWakeupTimeStr + ", BOQ, " + clsGlobalVars.InfrastructureBOQMMLocation;
                //}
                ////Previous Infrastructure BOQ
                //if (clsGlobalVars.PrevInfrastructureBOQMMLocation == -1)
                //{
                //    txtTSSPrevBOQMMLocation1.Text = "No Queue";
                //    txtTSSPrevBOQDate1.Text = string.Empty;
                //}
                //else
                //{
                //    txtTSSPrevBOQMMLocation1.Text = clsGlobalVars.PrevInfrastructureBOQMMLocation.ToString();
                //    txtTSSPrevBOQDate1.Text = clsGlobalVars.PrevInfrastructureBOQTime.ToString();
                //}

                TimeSpan EndTSSBOQ = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tTime for processing  TSS BOQ MM Location: \t"
                    + (EndTSSBOQ.TotalMilliseconds - BeginTSSBOQ.TotalMilliseconds).ToString("0") + " msecs");

                #region "Reconcile TSS BOQ and CV BOQ - Old Algorithm"
                /*
                LogTxtMsg("\r\nDetermine BOQ location from TSS and CV BOQ MM locations:");

                clsGlobalVars.BOQMMLocation = -1;
                //clsGlobalVars.BOQTime = DateTime.Now;
                clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;

                #region "Reconcile TSS BOQ MM Location and CV data BOQ MM Location"
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (clsGlobalVars.CVBOQMMLocation < clsGlobalVars.InfrastructureBOQMMLocation)
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                        }
                        else
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                    }
                    else
                    {
                        clsGlobalVars.BOQMMLocation = -1;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    if ((clsGlobalVars.InfrastructureBOQMMLocation != -1) && (clsGlobalVars.CVBOQMMLocation != -1))
                    {
                        if (clsGlobalVars.CVBOQMMLocation > clsGlobalVars.InfrastructureBOQMMLocation)
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                        }
                        else
                        {
                            clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                            clsGlobalVars.BOQTime = DateTime.Now;
                            clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                        }
                    }
                    else if (clsGlobalVars.CVBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.CVBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.CV;
                    }
                    else if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        clsGlobalVars.BOQMMLocation = clsGlobalVars.InfrastructureBOQMMLocation;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.TSS;
                    }
                    else
                    {
                        clsGlobalVars.BOQMMLocation = -1;
                        clsGlobalVars.BOQTime = DateTime.Now;
                        clsGlobalVars.QueueSource = clsEnums.enQueueSource.NA;
                    }
                }
                #endregion

                LogTxtMsg("\tCurr BOQ location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());

                clsGlobalVars.QueueRate = 0;
                clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.NA;

                double tmpBOQMMLocationChange = 0;
                #region "Determine Queue growth rate"
                if (Roadway.Direction == Roadway.MMIncreasingDirection)
                {
                    TimeSpan span = clsGlobalVars.BOQTime.Subtract(clsGlobalVars.PrevBOQTime);
                    if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation != -1))
                    {
                        tmpBOQMMLocationChange = clsGlobalVars.BOQMMLocation - clsGlobalVars.PrevBOQMMLocation;
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        if (tmpBOQMMLocationChange < 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                        }
                        else if (tmpBOQMMLocationChange > 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                        }
                        else if (tmpBOQMMLocationChange == 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                        }
                    }
                    else if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation == -1))
                    {
                        tmpBOQMMLocationChange = (Roadway.RecurringCongestionMMLocation - clsGlobalVars.PrevBOQMMLocation);
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                    }
                    else if ((clsGlobalVars.PrevBOQMMLocation == -1) && (clsGlobalVars.BOQMMLocation != -1))
                    {
                        tmpBOQMMLocationChange = (Roadway.RecurringCongestionMMLocation - clsGlobalVars.BOQMMLocation);
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                    }
                    else
                    {
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                        clsGlobalVars.QueueRate = 0;
                    }
                    QueueLog.WriteLine(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond + ",CV," +
                                       clsGlobalVars.PrevBOQMMLocation.ToString("0.0") + "," + clsGlobalVars.PrevBOQTime.ToString() + "," +
                                       clsGlobalVars.BOQMMLocation.ToString("0.0") + "," + clsGlobalVars.BOQTime.ToString() + "," + clsGlobalVars.QueueRate.ToString("0.00") + "," +
                                       clsGlobalVars.QueueChange.ToString() + "," + clsGlobalVars.QueueSource.ToString());
                    if (clsGlobalVars.PrevBOQMMLocation != clsGlobalVars.BOQMMLocation)
                    {
                        clsGlobalVars.PrevBOQMMLocation = clsGlobalVars.BOQMMLocation;
                        clsGlobalVars.PrevBOQTime = clsGlobalVars.BOQTime;
                    }
                }
                else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                {
                    TimeSpan span = clsGlobalVars.BOQTime.Subtract(clsGlobalVars.PrevBOQTime);
                    if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation != -1))
                    {
                        tmpBOQMMLocationChange = clsGlobalVars.BOQMMLocation - clsGlobalVars.PrevBOQMMLocation;
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        if (tmpBOQMMLocationChange < 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                        }
                        else if (tmpBOQMMLocationChange > 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                        }
                        else if (tmpBOQMMLocationChange == 0)
                        {
                            clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                        }
                    }
                    else if ((clsGlobalVars.PrevBOQMMLocation != -1) && (clsGlobalVars.BOQMMLocation == -1))
                    {
                        tmpBOQMMLocationChange = (clsGlobalVars.PrevBOQMMLocation - Roadway.RecurringCongestionMMLocation);
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Dissipating;
                    }
                    else if ((clsGlobalVars.PrevBOQMMLocation == -1) && (clsGlobalVars.BOQMMLocation != -1))
                    {
                        tmpBOQMMLocationChange = (clsGlobalVars.BOQMMLocation - Roadway.RecurringCongestionMMLocation);
                        clsGlobalVars.QueueRate = ((Math.Abs(tmpBOQMMLocationChange) * 3600) / span.TotalSeconds);
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Growing;
                    }
                    else
                    {
                        clsGlobalVars.QueueChange = clsEnums.enQueueCahnge.Same;
                        clsGlobalVars.QueueRate = 0;
                    }
                    QueueLog.WriteLine(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "::" + DateTime.Now.Millisecond + ",TSS," +
                                        clsGlobalVars.PrevBOQMMLocation.ToString("0.0") + "," + clsGlobalVars.PrevBOQTime.ToString() + "," +
                                        clsGlobalVars.BOQMMLocation.ToString("0.0") + "," + clsGlobalVars.BOQTime.ToString() + "," + clsGlobalVars.QueueRate.ToString("0.00") + "," +
                                        clsGlobalVars.QueueChange.ToString() + "," + clsGlobalVars.QueueSource.ToString());
                    if (clsGlobalVars.PrevBOQMMLocation != clsGlobalVars.BOQMMLocation)
                    {
                        clsGlobalVars.PrevBOQMMLocation = clsGlobalVars.BOQMMLocation;
                        clsGlobalVars.PrevBOQTime = clsGlobalVars.BOQTime;
                    }
                }
                #endregion

                LogTxtMsg("\t\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);
                TSSDataProcessor.WriteLine ("\tPrev BOQ MM location: " + clsGlobalVars.PrevBOQMMLocation + "\tTime: " + clsGlobalVars.PrevBOQTime);

                LogTxtMsg("\t\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());
                TSSDataProcessor.WriteLine("\tCurr BOQ MM location: " + clsGlobalVars.BOQMMLocation + "\tTime: " + clsGlobalVars.BOQTime + "\tSource: " + clsGlobalVars.QueueSource.ToString());

                //Previous BOQ
                if (clsGlobalVars.PrevBOQMMLocation == -1)
                {
                    txtPrevBOQMMLocation1.Text = "No Queue";
                    txtPrevBOQDate1.Text = string.Empty;
                }
                else
                {
                    txtPrevBOQMMLocation1.Text = clsGlobalVars.PrevBOQMMLocation.ToString();
                    txtPrevBOQDate1.Text = clsGlobalVars.PrevBOQTime.ToString();
                }

                //Current BOQ
                if (clsGlobalVars.BOQMMLocation == -1)
                {
                    txtBOQMMLocation1.Text = "No Queue";
                    txtBOQDate1.Text = string.Empty;
                    DisplayForm.txtBOQ.Text = "NO Queue";
                }
                else
                {
                    txtBOQMMLocation1.Text = clsGlobalVars.BOQMMLocation.ToString();
                    txtBOQDate1.Text = clsGlobalVars.BOQTime.ToString();
                    DisplayForm.txtBOQ.Text = clsGlobalVars.BOQMMLocation.ToString();
                }
                txtBOQGrowthRate1.Text = clsGlobalVars.QueueRate.ToString(); ;
                txtBOQGrowthType1.Text = clsGlobalVars.QueueChange.ToString();

                LogTxtMsg("\t\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
                TSSDataProcessor.WriteLine("\tCurr BOQ rate of growth: " + clsGlobalVars.QueueRate.ToString("0.00") + "\tQueue direction: " + clsGlobalVars.QueueChange.ToString() + "\tQueue source: " + clsGlobalVars.QueueSource.ToString());
                */
                #endregion

                if (NumberNoCVDataIntervals > 0)
                {
                    TSSWakeupTimeStr = TSSWakeupTimeStr + ", NOCVData, " + NumberNoCVDataIntervals;
                    TSSDataHarmonization++;
                    DisplayForm.txtDataTypeCount.Text = "TSS-Count: " + TSSDataHarmonization;
                    //DisplayForm.ClearCVSubLinkTroupeStatus();
                    //DisplayForm.ClearCVSubLinkSPDHarmStatus();
                    //DisplayForm.ClearTSSSPDHarmLinkStatus();
                    //Reset Troupe speed, and inclusion flag for every sublink
                    foreach (clsRoadwaySubLink rsl in RSLList)
                    {
                        rsl.TroupeSpeed = 0;
                        rsl.TroupeInclusionOverride = false;
                        rsl.BeginTroupe = false;
                        rsl.TroupeProcessed = DateTime.Now;
                        rsl.HarmonizedSpeed = 0;
                        rsl.SpdHarmInclusionOverride = false;
                        rsl.BeginSpdHarm = false;
                        rsl.RecommendedSpeed = clsGlobalVars.MaximumDisplaySpeed;
                    }
                    #region "Reconcile the CV BOQ and TSS BOQ"
                    TimeSpan BeginBOQ = new TimeSpan(DateTime.Now.Ticks);
                    DetermineBOQ(QueuedSubLink, QueuedLink, Roadway);
                    TimeSpan EndBOQ = new TimeSpan(DateTime.Now.Ticks);
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tTime for reconciling BOQ MM Location: \t"
                        + (EndBOQ.TotalMilliseconds - BeginBOQ.TotalMilliseconds).ToString("0") + " msecs");
                    #endregion

                    //Start the sublink trouping process
                    TimeSpan StartTrouping = new TimeSpan(DateTime.Now.Ticks);

                    //Determine the Recommended speed for each sublink
                    #region "Determine sublink recommended speed"
                    IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
                    //Query all sites from the database, need only site id and corresponding milemarker.
                    //Not all roadway links will align with a site
                    List<SitesByMileMarker> sites = uow.Sites.Select(m => new SitesByMileMarker { Id = m.Id, MileMarker = m.MileMarker }).OrderBy(m => m.MileMarker).ToList();

                    foreach (clsRoadwayLink rl in RLList)
                    {
                        //See if we have any new weather data we should load into RL.WRTMSpeed
                        rl.WRTMSpeed = GetWRTMForLink(uow, sites, rl);

                        foreach (clsRoadwaySubLink rsl in RSLList)
                        {
                            if (Roadway.Direction == Roadway.MMIncreasingDirection)
                            {
                                if ((rsl.BeginMM >= rl.BeginMM) && (rsl.BeginMM < rl.EndMM))
                                {
                                    rsl.TSSAvgSpeed = rl.TSSAvgSpeed;
                                    rsl.WRTMSpeed = rl.WRTMSpeed;
                                    rsl.RecommendedSpeed = GetMinimumSpeed(rsl.TSSAvgSpeed, rsl.WRTMSpeed, rsl.CVAvgSpeed);
                                }
                            }
                            else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                            {
                                if ((rsl.BeginMM <= rl.BeginMM) && (rsl.BeginMM > rl.EndMM))
                                {
                                    rsl.TSSAvgSpeed = rl.TSSAvgSpeed;
                                    rsl.WRTMSpeed = rl.WRTMSpeed;
                                    rsl.RecommendedSpeed = GetMinimumSpeed(rsl.TSSAvgSpeed, rsl.WRTMSpeed, rsl.CVAvgSpeed);
                                }
                            }
                        }
                    }
                    #endregion

                    //Determine the ending MM for the trouping process
                    #region "Determine the Trouping Ending MM Location"
                    //Locate the first sublink upstream of the Recurring congestion MMLocation that is congested 
                    /*if (clsGlobalVars.BOQMMLocation == -1)
                    {
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            TroupingEndMM = Roadway.RecurringCongestionMMLocation + 1000;
                            for (int r = RSLList.Count - 1; r >= 0; r--)
                            {
                                if (RSLList[r].BeginMM < Roadway.RecurringCongestionMMLocation)
                                {
                                    if ((RSLList[r].RecommendedSpeed > clsGlobalVars.LinkQueuedSpeedThreshold) && (RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                    {
                                        if (RSLList[r].BeginMM < TroupingEndMM)
                                        {
                                            TroupingEndMM = RSLList[r].BeginMM;
                                            TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                            //break;
                                        }
                                    }
                                }
                            }
                            if (TroupingEndMM > Roadway.RecurringCongestionMMLocation)
                            {
                                TroupingEndMM = 0;
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            TroupingEndMM = Roadway.RecurringCongestionMMLocation - 1000;
                            for (int r = RSLList.Count - 1; r >= 0; r--)
                            {
                                if (RSLList[r].BeginMM > Roadway.RecurringCongestionMMLocation)
                                {
                                    if ((RSLList[r].RecommendedSpeed > clsGlobalVars.LinkQueuedSpeedThreshold) && (RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                    {
                                        if (RSLList[r].BeginMM > TroupingEndMM)
                                        {
                                            TroupingEndMM = RSLList[r].BeginMM;
                                            TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                            //break;
                                        }
                                    }
                                }
                            }
                            if (TroupingEndMM < Roadway.RecurringCongestionMMLocation)
                            {
                                TroupingEndMM = 0;
                            }

                        }
                    }
                    else if (clsGlobalVars.BOQMMLocation > 0)
                    {
                        TroupingEndMM = clsGlobalVars.BOQMMLocation;
                        TroupingEndSpeed = clsGlobalVars.BOQSpeed;
                    }*/

                    TroupingEndMM = 0;
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        //TroupingEndMM = Roadway.RecurringCongestionMMLocation + 1000;
                        for (int r = RSLList.Count - 1; r >= 0; r--)
                        {
                            if (RSLList[r].BeginMM < Roadway.RecurringCongestionMMLocation)
                            {
                                if ((RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                {
                                    if (RSLList[r].BeginMM > TroupingEndMM)
                                    {
                                        TroupingEndMM = RSLList[r].BeginMM;
                                        TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                        //break;
                                    }
                                }
                            }
                        }
                        //if (TroupingEndMM > Roadway.RecurringCongestionMMLocation)
                        //{
                        //    TroupingEndMM = 0;
                        //}
                    }
                    else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                    {
                        //TroupingEndMM = Roadway.RecurringCongestionMMLocation - 1000;
                        for (int r = RSLList.Count - 1; r >= 0; r--)
                        {
                            if (RSLList[r].BeginMM > Roadway.RecurringCongestionMMLocation)
                            {
                                if ((RSLList[r].RecommendedSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold))
                                {
                                    if (TroupingEndMM == 0)
                                    {
                                        TroupingEndMM = RSLList[r].BeginMM;
                                        TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                    }
                                    else if (RSLList[r].BeginMM < TroupingEndMM)
                                    {
                                        TroupingEndMM = RSLList[r].BeginMM;
                                        TroupingEndSpeed = RSLList[r].RecommendedSpeed;
                                        //break;
                                    }
                                }
                            }
                        }
                        //if (TroupingEndMM < Roadway.RecurringCongestionMMLocation)
                        //{
                        //    TroupingEndMM = 0;
                        //}
                    }
                    #endregion

                    //perform the Trouping and Harmonization processes

                    if (TroupingEndMM > 0)
                    {
                        TSSWakeupTimeStr = TSSWakeupTimeStr + ", Trouping, " + TroupingEndMM;
                        #region "Perform trouping and harmonization"
                        DisplayForm.DisableNoTroupingMessage();
                        DisplayForm.DisableNoSpdHarmMessage();
                        retValue = CalculateSublinkTroupeSpeed(ref RSLList, TroupingEndMM, TroupingEndSpeed);
                        if (retValue.Length > 0)
                        {
                            LogTxtMsg(LoggerType.TSS, LoggerLevel.Error, "CalculateSublinkTroupeSpeed returned: " + retValue);
                        }
                        retValue = CalculateSublinkHarmonizedSpeed(ref RSLList, TroupingEndMM, TroupingEndSpeed);
                        if (retValue.Length > 0)
                        {
                            LogTxtMsg(LoggerType.TSS, LoggerLevel.Error, "CalculateSublinkHarmonizedSpeed returned: " + retValue);
                        }
                        LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\nUpdated Roadway SubLink Recommended, Troupe, and Harmonized Speeds using CV data.");
                        LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\n\tRoadwayId, SubLinkId, DateProcessed, BeginMM, EndMM, NumberQueuedCVs, TotalNumberCVS, PercentQueuedCVs, Queued, CVAvgSpd, TSSAvgSpd, RecommendedSpd, TroupeSpd, TroupePverride, HarmonizedSpeed, HarmonizationOverride, Direction");
                        foreach (clsRoadwaySubLink rsl in RSLList)
                        {
                            LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\t" + rsl.RoadwayID + ", " + rsl.Identifier + ", " + rsl.DateProcessed + ", " + rsl.BeginMM + ", " + rsl.EndMM + ", " + rsl.NumberQueuedCVs + ", " +
                                                      rsl.TotalNumberCVs + ", " + rsl.PercentQueuedCVs + ", " + rsl.Queued + ", " + rsl.CVAvgSpeed.ToString("0") + ", " + rsl.TSSAvgSpeed.ToString("0") + ", " +
                                                      rsl.RecommendedSpeed.ToString("0") + ", " + rsl.TroupeSpeed.ToString("0") + ", " + rsl.TroupeInclusionOverride.ToString() + ", " +
                                                       rsl.HarmonizedSpeed.ToString() + ", " + rsl.SpdHarmInclusionOverride.ToString() + ", " + rsl.Direction);
                        }
                        //DisplayForm.DisplaySublinkTroupeSpeed(RSLList, TroupingEndMM, Roadway);
                        //DisplayForm.DisplaySublinkHarmonizedSpeed(RSLList, TroupingEndMM, Roadway);
                        DetermineLinkHarmonizedSpeed(RSLList, ref RLList, TroupingEndMM);
                        //DisplayForm.DisplayTSSSPDHarmLinkStatus(RLList);

                        GenerateSPDHarmMessages(RSLList, TroupingEndMM, clsGlobalVars.BOQMMLocation, Roadway);
                        #endregion
                    }
                    else
                    {
                        TSSWakeupTimeStr = TSSWakeupTimeStr + ", NoTrouping, " + TroupingEndMM;
                        #region "If no trouping is required"
                        LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tNo Trouping is required for sublinks because no queued or congested sublinks were detected");

                        DisplayForm.EnableNoTroupingMessage(Roadway.RecurringCongestionMMLocation);
                        DisplayForm.EnableNoSpdHarmMessage(Roadway.RecurringCongestionMMLocation);

                        #endregion
                    }

                    LogToExcel(LogToExcelType.CVSPD, currDateTime);

                    TimeSpan EndTrouping = new TimeSpan(DateTime.Now.Ticks);
                    LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tTime for sublink speed harmonization: \t" + (EndTrouping.TotalMilliseconds - StartTrouping.TotalMilliseconds).ToString("0") + " msecs");
                }
                else
                {
                    TSSDataHarmonization = 0;
                }

                LogTxtMsg(LoggerType.TSS, LoggerLevel.Info, "\r\n\tFinished processing TSS data for the last "
                    + clsGlobalVars.TSSDataLoadingFrequency + " seconds interval");
                NumberNoTSSDataIntervals = 0;
                DisplayForm.DisableNoTSSDataMessage();
            }
            else//No TSS rec retrieved from db.
            {
                TSSDataHarmonization = 0;
                NumberNoTSSDataIntervals = NumberNoTSSDataIntervals + 1;
                LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\r\n\t\tNo TSS traffic data was retrieved for the last " + clsGlobalVars.TSSDataPollingFrequency + " second interval." +
                                        "\r\n\t\tNumber of " + clsGlobalVars.TSSDataLoadingFrequency + " second intervals with no TSS data retrieved: " + NumberNoTSSDataIntervals);
                DisplayForm.txtTSSDate.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                TSSWakeupTimeStr = TSSWakeupTimeStr + ", NOTSSData, " + NumberNoTSSDataIntervals;
                if (NumberNoTSSDataIntervals >= (60 / clsGlobalVars.TSSDataLoadingFrequency))
                {
                    TSSWakeupTimeStr = TSSWakeupTimeStr + ", NOTSSData-SetQueue=-1, " + NumberNoTSSDataIntervals;
                    #region "If NO TSS data was retrieved for the last five seconds"
                    if (clsGlobalVars.InfrastructureBOQMMLocation != -1)
                    {
                        clsGlobalVars.InfrastructureBOQMMLocation = -1;
                        clsGlobalVars.InfrastructureBOQTime = DateTime.Now;
                    }
                    DisplayForm.EnableNoTSSDataMessage(NumberNoTSSDataIntervals);
                    DisplayForm.txtTSSBOQ.Text = "No Queue";
                    DisplayForm.ClearTSSQueuedLinkStatus();
                    //if (clsGlobalVars.CVBOQMMLocation == -1)
                    //{
                    //    if (clsGlobalVars.BOQMMLocation != -1)
                    //    {
                    //        clsGlobalVars.BOQMMLocation = -1;
                    //        clsGlobalVars.BOQTime = DateTime.Now;
                    //    }
                    //    DisplayForm.txtBOQ.Text = "No Queue";
                    //}
                    //if (NumberNoCVDataIntervals > 0)
                    //{
                    foreach (clsRoadwayLink RL in RLList)
                    {
                        RL.TSSAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                    }
                    //DisplayForm.ClearCVSubLinkSPDHarmStatus();
                    //DisplayForm.ClearCVSubLinkTroupeStatus();
                    DisplayForm.EnableNoSpdHarmMessage(Roadway.RecurringCongestionMMLocation);
                    DisplayForm.EnableNoTroupingMessage(Roadway.RecurringCongestionMMLocation);
                    //}
                    #endregion
                }
            }
        }

        /// <summary>
        /// Finds the closes pik site for the roadway link. Looks up the latest weather observation for that
        /// site.  Estimates a WRTM speed for the current weather conditions.
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="sites"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public double GetWRTMForLink(IUnitOfWork uow, List<SitesByMileMarker> sites, clsRoadwayLink link)
        {
            double wrtm = 0;//Always return zero if we don't find any information

            int? siteid = GetNearestSiteId(sites, link);
            if (siteid != null)
            {
                //Grab the most recent observation, no older than 15 minutes
                DateTime dtMinutesAgo = DateTime.UtcNow.AddMinutes(-15);
                SiteObservation so = uow.SiteObservations.Where(s => s.SiteId == (int)siteid &&
                    s.DateTime > dtMinutesAgo).OrderByDescending(o => o.DateTime).FirstOrDefault();
                if (so != null)
                {
                    wrtm = GetWRTMRecommendationForObservation(so);
                }

            }
            return wrtm;
        }

        /// <summary>
        /// Uses the text-based weather updates from the site observation to try to estimate
        /// a recommended WRTM speed.
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        public double GetWRTMRecommendationForObservation(SiteObservation observation)
        {
            if (observation == null) return 0;

            if (observation.Pavement.Contains("wet") || observation.Pavement.Contains("slick")
                || observation.RoadTemp > 35)
            {
                //raining
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Added WRTM recommendation (50) for site " + observation.SiteId
                  + "Vis: " + observation.Visibility + "Pavement: " + observation.Pavement + "RoadTemp: " + observation.RoadTemp);
                return 50;
            }
            if (observation.Plow.Contains("snow") || observation.Plow.Contains("yes")
                || observation.Pavement.Contains("slick") || observation.Pavement.Contains("ice") || observation.Pavement.Contains("icy")
                )
            {
                //snow / ice
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Added WRTM recommendation (35) for site " + observation.SiteId
                + "Vis: " + observation.Visibility + "Pavement: " + observation.Pavement + "Plow: " + observation.Plow);
                return 35;
            }
            if (observation.Visibility.Contains("low")
               )
            {
                //low vis
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Added WRTM recommendation (45) for site " + observation.SiteId
                 + "Vis: " + observation.Visibility + "Pavement: " + observation.Pavement + "Plow: " + observation.Plow);
                return 45;
            }
            return 0;
        }

        public class SitesByMileMarker
        {
            public int Id { get; set; }
            public double? MileMarker { get; set; }
        }
        /// <summary>
        /// Sites, when added, are mapped to a Roadway Link if the site exists within the link.
        /// There are more sites than links, thus some links will not be in the sites table.
        /// In those cases, we will try to return the nearest link within a range, or null if no
        /// near links have a site.
        /// 
        ///         //Currently, the sites loaded from Pik alert are like:
        ///Id	MileMarker	Description
        ///39	9.5	MN ROAD SEGMENT Interstate 35w 2
        ///40	12	MN ROAD SEGMENT Interstate 35w 1
        ///27	18.4	MN ROAD SEGMENT Interstate 35w 20
        ///
        ///And our links are
        ///RoadwayId	LinkId	BeginMM	EndMM
        ///I35W_N	27	10.9	11.6
        ///I35W_N	28	11.6	12
        ///I35W_N	29	12	12.6
        ///I35W_N	30	12.6	13
        ///I35W_N	31	13	13.5
        ///I35W_N	32	13.5	13.8
        ///I35W_N	33	13.8	14.2
        ///I35W_N	34	14.2	14.8
        ///I35W_N	35	14.8	15.1
        ///I35W_N	36	15.1	15.4
        ///I35W_N	37	15.4	15.8
        ///
        ///So obviously our 29th link should be assigned to site 40, because it starts at MM12 and the site is at MM12.
        ///But the links surrounding site 40 should also be assigned to 40, since that is their closest site:
        ///
        ///The next lower site is at MM9.5.  The midpoint between MM9.5 and MM12 is 10.75, thus links under 10.75 should be
        ///assigned to MM9.5, and links over 10.75 should be assigned to MM12.
        ///
        /// *Note: Because the GetNearestSiteId reverses the begin/end milemarkers to do the comparison if the roadway
        ///is in the decreasing direction, the comparision thresholds are actually based on the end MM for South.
        ///This is okay since we are finding these for weather, which really should not be very variable from one site to the next
        ///so it is not a big deal on the boundary which gets it.
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        public int? GetNearestSiteId(List<SitesByMileMarker> sites, clsRoadwayLink link)
        {
            int? siteId = null;


            double startMM, endMM; //Start and End need to be reversed if the increasing MM direction is backwards for the roadway.

            if (Roadway.Direction == Roadway.MMIncreasingDirection)
            {
                startMM = link.BeginMM;
                endMM = link.EndMM;
            }
            else
            {
                startMM = link.EndMM;
                endMM = link.BeginMM;
            }

            for (int i = 0; i < 5; i++) // Span about 10 milemarkers in range looking for a matching site.
            {
                var s = sites.Where(l => l.MileMarker >= startMM && l.MileMarker <= endMM).FirstOrDefault();
                if (s != null)
                {
                    return s.Id;
                }
                //Link not found. Expand search and see if we find one
                startMM -= 1;
                endMM += 1;
            }
            return siteId;
        }

        /// <summary>
        /// Queries the database for newly reported data from CVs (connected vehicle).
        /// compares the speed of the vehicle to the threshold config QueueSpeedThreshold and sets
        /// Queued to true if the speed is less than that.
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="IntervalUTCTime"></param>
        /// <param name="RoadwayId"></param>
        /// <param name="CVList"></param>
        /// <param name="Direction"></param>
        /// <param name="LHeading"></param>
        /// <param name="UHeading"></param>
        /// <param name="NumberRecordsRetrieved"></param>
        /// <returns></returns>
        private string GetLastIntervalCVData(clsDatabase DB, DateTime IntervalUTCTime, string RoadwayId, ref List<clsCVData> CVList, clsEnums.enDirection Direction, double LHeading, double UHeading, ref int NumberRecordsRetrieved)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;
            try
            {
                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

                DateTime sinceIntervalUTCTime = IntervalUTCTime.AddSeconds(-(10));

                List<TME_CVData_Input> tmeCvDataInputs = uow.TME_CVData_Inputs
                    .Where(d => (d.DateGenerated >= sinceIntervalUTCTime)
                     && (d.DateGenerated <= IntervalUTCTime)
                     && d.RoadwayId == RoadwayId).ToList();
                NumberRecordsRetrieved = tmeCvDataInputs.Count;
                foreach (TME_CVData_Input tme in tmeCvDataInputs)
                {
                    clsCVData tmpCV = new clsCVData();

                    tmpCV.NomadicDeviceID = GetNullable(tme.NomadicDeviceID);
                    tmpCV.RoadwayID = GetNullable(tme.RoadwayId);
                    tmpCV.Speed = GetNullable(tme.Speed);
                    tmpCV.Latitude = GetNullable(tme.Latitude);
                    tmpCV.Longitude = GetNullable(tme.Longitude);
                    tmpCV.MMLocation = tme.MMLocation;
                    tmpCV.CoefficientFriction = GetNullable(tme.CoefficientOfFriction);
                    tmpCV.Temperature = GetNullable(tme.Temperature);
                    tmpCV.LateralAcceleration = GetNullable(tme.LateralAcceleration);
                    tmpCV.LongitudinalAcceleration = GetNullable(tme.LongitudinalAcceleration);


                    #region "Commented Heading section"
                    /*if (tmpCV.Heading > 0)
                            {
                                if ((tmpCV.Heading >= 337.5) || (tmpCV.Heading < 22.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("North");
                                }
                                else if ((tmpCV.Heading >= 22.5) || (tmpCV.Heading < 67.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("NE");
                                }
                                else if ((tmpCV.Heading >= 67.5) || (tmpCV.Heading < 112.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("East");
                                }
                                else if ((tmpCV.Heading >= 112.5) || (tmpCV.Heading < 157.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("SE");
                                }
                                else if ((tmpCV.Heading >= 157.5) || (tmpCV.Heading < 202.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("South");
                                }
                                else if ((tmpCV.Heading >= 202.5) || (tmpCV.Heading < 247.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("SW");
                                }
                                else if ((tmpCV.Heading >= 247.5) || (tmpCV.Heading < 292.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("West");
                                }
                                else if ((tmpCV.Heading >= 292.5) || (tmpCV.Heading < 337.5))
                                {
                                    tmpCV.Direction = clsEnums.GetDirIndexFromString("NW");
                                }
                            }
                            else
                            {
                                tmpCV.Direction = Direction;
                            }*/
                    #endregion

                    //LogTxtMsg("\t\t" + NomadicDeviceId + ", " + RoadwayId + ", " + Speed + ", " + Latitude + ", " + Longitude + ", " + Heading + ", " + DateGenerated + ", " +
                    //                  MMLocation + ", " + CVQueuedState + ", " + CoefficientOfFriction + ", " + Temperature + ", " + Direction);
                    tmpCV.Direction = Direction;
                    tmpCV.Queued = false;
                    if (tmpCV.Speed <= clsGlobalVars.LinkQueuedSpeedThreshold)
                    {
                        tmpCV.Queued = true;
                    }
                    CVList.Add(tmpCV);

                }

            }
            catch (Exception exc)
            {
                retValue = "Error in retrieving CV data for the last interval from: " + IntervalUTCTime.AddSeconds(-clsGlobalVars.CVDataPollingFrequency) + " to: " + IntervalUTCTime + "\r\n\t" + exc.Message;
                return retValue;
            }

            return retValue;
        }

        /// <summary>
        /// Iterates over all the sublinks. Calculates various metrics to characterize the sublinks making up the 
        /// road section (both queued and unqueued parts).
        /// </summary>
        /// <param name="Roadway"></param>
        /// <param name="CVList"></param>
        /// <param name="RSLList"></param>
        /// <returns></returns>
        private string ProcessRoadwaySublinkQueuedStatus(clsRoadway Roadway, ref List<clsCVData> CVList, ref List<clsRoadwaySubLink> RSLList)
        {
            string retValue = string.Empty;
            string CVID = string.Empty;
            string CurrRecord = string.Empty;
            double TotalSpeed = 0;
            string tmpSublinkData = string.Empty;

            try
            {
                CVList.Sort((l, r) => l.MMLocation.CompareTo(r.MMLocation));
                //RSLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));

                foreach (clsRoadwaySubLink RSL in RSLList)
                {
                    RSL.CVAvgSpeed = 0;
                    RSL.TotalNumberCVs = 0;
                    RSL.Congested = false;
                    RSL.Queued = false;
                    RSL.TotalNumberCVs = 0;
                    RSL.NumberQueuedCVs = 0;
                    RSL.PercentQueuedCVs = 0;
                    RSL.SmoothedSpeed[clsGlobalVars.CVDataSmoothedSpeedIndex] = 0;
                    TotalSpeed = 0;

                    RSL.CVList = new List<clsCVData>();

                    //Of the new data retrieved from TME_CVDataInput, match each to its sublink by using MMLocation. Keep track
                    //of whether the data reported if it was queued, and what the speed was.
                    foreach (clsCVData CV in CVList)
                    {
                        //LogTxtMsg("\tProcessing roadway sublink: " + RSL.Identifier + "\tFromMM: " + RSL.BeginMM + "\tToMM: " + RSL.EndMM + "\tDirection: " + RSL.Direction + "\tCV: " + CV.NomadicDeviceID + " - " + CV.Direction + " - " + CV.MMLocation);
                        CurrRecord = "RSL: " + RSL.Identifier + "   CV: " + CV.NomadicDeviceID;
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            if ((CV.MMLocation >= RSL.BeginMM) && (CV.MMLocation < RSL.EndMM) && (RSL.RoadwayID.ToString() == CV.RoadwayID))
                            {
                                //Multiple vehicles may have reported in data from this sublink's piece of road.
                                //Keep track of each vehicle, and the number of vehicles, and sum the speed, so the speed of all
                                //reporting vehicles can be averaged.
                                CVID = CV.NomadicDeviceID;//unused?
                                CV.SublinkID = RSL.Identifier.ToString();
                                RSL.CVList.Add(CV);
                                RSL.TotalNumberCVs = RSL.TotalNumberCVs + 1;
                                if (CV.Queued == true)
                                {
                                    RSL.NumberQueuedCVs = RSL.NumberQueuedCVs + 1;
                                }
                                TotalSpeed = TotalSpeed + CV.Speed;
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            if ((CV.MMLocation <= RSL.BeginMM) && (CV.MMLocation > RSL.EndMM) && (RSL.RoadwayID.ToString() == CV.RoadwayID))
                            {
                                CVID = CV.NomadicDeviceID;
                                CV.SublinkID = RSL.Identifier.ToString();
                                RSL.CVList.Add(CV);
                                RSL.TotalNumberCVs = RSL.TotalNumberCVs + 1;
                                if (CV.Queued == true)
                                {
                                    RSL.NumberQueuedCVs = RSL.NumberQueuedCVs + 1;
                                }
                                TotalSpeed = TotalSpeed + CV.Speed;
                            }
                        }
                        //else if (CV.MMLocation >= RSL.EndMM)
                        //{
                        //    break;
                        //}
                    }
                    //We found reported information for this sublink.  Calculate summary data to characterize sublink.
                    if (RSL.TotalNumberCVs > 0)
                    {
                        RSL.SmoothedSpeed[clsGlobalVars.CVDataSmoothedSpeedIndex] = TotalSpeed / RSL.TotalNumberCVs;
                        RSL.PercentQueuedCVs = (RSL.NumberQueuedCVs * 100) / RSL.TotalNumberCVs;
                        RSL.VolumeDiff = RSL.TotalNumberCVs - RSL.PrevTotalNumberCVs;

                        RSL.FlowRate = (RSL.VolumeDiff / CVTimeDiff) * 3600;
                        RSL.Density = (RSL.TotalNumberCVs / clsGlobalVars.SubLinkLength);
                        RSL.DensityDiff = RSL.Density - RSL.PrevDensity;
                        if (RSL.DensityDiff > 0)
                        {
                            RSL.ShockWaveRate = (RSL.FlowRate / RSL.DensityDiff);
                        }
                        tmpSublinkData = tmpSublinkData + RSL.Identifier + "," + RSL.PrevTotalNumberCVs + ";" + RSL.TotalNumberCVs + ";" + RSL.VolumeDiff + ";" + RSL.PercentQueuedCVs + ";" +
                                         CVTimeDiff + ";" + RSL.FlowRate.ToString("0") + ";" + RSL.Density.ToString("0") + ";" + RSL.PrevDensity.ToString("0") + ";" +
                                         RSL.DensityDiff + ";" + RSL.ShockWaveRate.ToString("0") + ",";
                        RSL.PrevDensity = RSL.Density;
                        RSL.PrevTotalNumberCVs = RSL.TotalNumberCVs;
                    }

                    TotalSpeed = 0;
                    int NoIntervals = 0;
                    for (int i = 0; i < clsGlobalVars.CVDataSmoothedSpeedArraySize; i++)
                    {
                        TotalSpeed = TotalSpeed + RSL.SmoothedSpeed[i];
                        if (RSL.SmoothedSpeed[i] > 0)
                        {
                            NoIntervals++;
                        }
                    }

                    if (NoIntervals > 0)
                    {
                        RSL.CVAvgSpeed = TotalSpeed / NoIntervals;
                    }

                    if (RSL.CVAvgSpeed == 0)
                    {
                        RSL.CVAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                    }
                    else if (RSL.CVAvgSpeed > clsGlobalVars.MaximumDisplaySpeed)
                    {
                        RSL.CVAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                    }
                    if (RSL.CVAvgSpeed > clsGlobalVars.LinkCongestedSpeedThreshold)
                    {
                        RSL.Congested = false;
                    }
                    else if (RSL.CVAvgSpeed > clsGlobalVars.LinkQueuedSpeedThreshold)//if>LinkQueuedSpeedThreshold && <LinkCongestedSpeedThreshold
                    {
                        RSL.Congested = true;
                    }
                    RSL.Queued = false;
                    if (RSL.PercentQueuedCVs >= clsGlobalVars.SubLinkPercentQueuedCV)
                    {
                        RSL.Queued = true;
                    }
                    RSL.CVDateProcessed = DateTime.UtcNow;
                    //LogTxtMsg("\t\t\tRSL CVAvg speed: " + RSL.CVAvgSpeed.ToString("0") + "\tNumCVs: " + RSL.TotalNumberCVs + "\tNumQueuedCVs: " + RSL.NumberQueuedCVs + "\t\t%QueuedCVs: " + RSL.PercentQueuedCVs.ToString("00") +
                    //                             "\tQueued " + RSL.Queued + "\tDateProcessed: " + RSL.CVDateProcessed + "\tBeginMM: " + RSL.BeginMM);
                }

                LogTxtMsg(LoggerType.SublinkDataLog, LoggerLevel.Info, tmpSublinkData);
            }
            catch (Exception ex)
            {
                retValue = "Error in processing roadway sublink status from CV data. Error Hint - Current record: " + CurrRecord + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }
        private string CalculateSublinkTroupeSpeed(ref List<clsRoadwaySubLink> RSLList, double TroupingEndMM, double TroupingEndSpeed)
        {
            string retValue = string.Empty;
            string CVID = string.Empty;
            string CurrRecord = string.Empty;
            int TroupeStartSublink = -1;
            double LastTroupeAvgSpeed = 0;
            clsTroupe tmpTroupe = new clsTroupe();

            try
            {
                //Sort sublinks based on sublink identifier assuming that sublinks are numberered starting from the sublink which is furthest upstream from the reuccuring congestion location
                //The furthest upstream sublink from the recurring congestion location is numbered as 1.
                RSLList.Sort((l, r) => l.Identifier.CompareTo(r.Identifier));


                //Find the furthest upstream link from the recurring congestion location with a recommended speed > 0 to start the first troupe
                for (int i = 0; i < RSLList.Count; i++)
                {
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        if ((RSLList[i].RecommendedSpeed > 0) && (RSLList[i].BeginMM < TroupingEndMM))
                        {
                            TroupeStartSublink = i;
                            tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                            tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                            tmpTroupe.AvgSpeed = 0;
                            tmpTroupe.NumberSubLinks = 1;
                            tmpTroupe.SubLinks.Add(RSLList[i]);
                            RSLList[i].BeginTroupe = true;
                            break;
                        }
                    }
                    else
                    {
                        if ((RSLList[i].RecommendedSpeed > 0) && (RSLList[i].BeginMM > TroupingEndMM))
                        {
                            TroupeStartSublink = i;
                            tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                            tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                            tmpTroupe.AvgSpeed = 0;
                            tmpTroupe.NumberSubLinks = 1;
                            tmpTroupe.SubLinks.Add(RSLList[i]);
                            RSLList[i].BeginTroupe = true;
                            break;
                        }
                    }
                }

                //Process the rest of the sublinks
                if (TroupeStartSublink != -1)
                {
                    for (int i = TroupeStartSublink + 1; i < RSLList.Count; i++)
                    {
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[i].BeginMM < TroupingEndMM)
                            {
                                if ((RSLList[i].RecommendedSpeed >= (tmpTroupe.MaxSpeed - clsGlobalVars.TroupeRange)) &&
                                   (RSLList[i].RecommendedSpeed <= (tmpTroupe.MinSpeed + clsGlobalVars.TroupeRange)))
                                {
                                    tmpTroupe.SubLinks.Add(RSLList[i]);
                                    RSLList[i].TroupeInclusionOverride = false;
                                    if (RSLList[i].RecommendedSpeed > tmpTroupe.MaxSpeed)
                                    {
                                        tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                    }
                                    if (RSLList[i].RecommendedSpeed < tmpTroupe.MinSpeed)
                                    {
                                        tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                    }
                                }
                                else
                                {
                                    tmpTroupe.CalculateTroupeAvgSpeed();
                                    tmpTroupe.CalculateTroupeLength();
                                    tmpTroupe.CalculateTroupeTravelTime();

                                    if (tmpTroupe.TravelTime < clsGlobalVars.DSD)
                                    {
                                        tmpTroupe.SubLinks.Add(RSLList[i]);
                                        RSLList[i].TroupeInclusionOverride = true;
                                        if (RSLList[i].RecommendedSpeed > tmpTroupe.MaxSpeed)
                                        {
                                            tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                        }
                                        if (RSLList[i].RecommendedSpeed < tmpTroupe.MinSpeed)
                                        {
                                            tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                        }
                                    }
                                    else
                                    {
                                        //Assign the troupe avgspeed to all sublinks in the troupe
                                        LastTroupeAvgSpeed = tmpTroupe.AvgSpeed;
                                        foreach (clsRoadwaySubLink TSL in tmpTroupe.SubLinks)
                                        {
                                            foreach (clsRoadwaySubLink RSL in RSLList)
                                            {
                                                if (TSL.Identifier == RSL.Identifier)
                                                {
                                                    RSL.TroupeSpeed = tmpTroupe.AvgSpeed;
                                                    break;
                                                }
                                            }
                                        }

                                        tmpTroupe.SubLinks.Clear();
                                        tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                        tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                        tmpTroupe.AvgSpeed = 0;
                                        tmpTroupe.NumberSubLinks = 1;
                                        tmpTroupe.SubLinks.Add(RSLList[i]);
                                        RSLList[i].BeginTroupe = true;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[i].BeginMM > TroupingEndMM)
                            {
                                if ((RSLList[i].RecommendedSpeed >= (tmpTroupe.MaxSpeed - clsGlobalVars.TroupeRange)) &&
                                (RSLList[i].RecommendedSpeed <= (tmpTroupe.MinSpeed + clsGlobalVars.TroupeRange)))
                                {
                                    tmpTroupe.SubLinks.Add(RSLList[i]);
                                    RSLList[i].TroupeInclusionOverride = false;
                                    if (RSLList[i].RecommendedSpeed > tmpTroupe.MaxSpeed)
                                    {
                                        tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                    }
                                    if (RSLList[i].RecommendedSpeed < tmpTroupe.MinSpeed)
                                    {
                                        tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                    }
                                }
                                else
                                {
                                    tmpTroupe.CalculateTroupeAvgSpeed();
                                    tmpTroupe.CalculateTroupeLength();
                                    tmpTroupe.CalculateTroupeTravelTime();

                                    if (tmpTroupe.TravelTime < clsGlobalVars.DSD)
                                    {
                                        tmpTroupe.SubLinks.Add(RSLList[i]);
                                        RSLList[i].TroupeInclusionOverride = true;
                                        if (RSLList[i].RecommendedSpeed > tmpTroupe.MaxSpeed)
                                        {
                                            tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                        }
                                        if (RSLList[i].RecommendedSpeed < tmpTroupe.MinSpeed)
                                        {
                                            tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                        }
                                    }
                                    else
                                    {
                                        //Assign the troupe avgspeed to all sublinks in the troupe
                                        LastTroupeAvgSpeed = tmpTroupe.AvgSpeed;
                                        foreach (clsRoadwaySubLink TSL in tmpTroupe.SubLinks)
                                        {
                                            foreach (clsRoadwaySubLink RSL in RSLList)
                                            {
                                                if (TSL.Identifier == RSL.Identifier)
                                                {
                                                    RSL.TroupeSpeed = tmpTroupe.AvgSpeed;
                                                    break;
                                                }
                                            }
                                        }

                                        tmpTroupe.SubLinks.Clear();
                                        tmpTroupe.MaxSpeed = RSLList[i].RecommendedSpeed;
                                        tmpTroupe.MinSpeed = RSLList[i].RecommendedSpeed;
                                        tmpTroupe.AvgSpeed = 0;
                                        tmpTroupe.NumberSubLinks = 1;
                                        tmpTroupe.SubLinks.Add(RSLList[i]);
                                        RSLList[i].BeginTroupe = true;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        //LogTxtMsg("\t\t\tRSL CVAvg speed: " + RSL.CVAvgSpeed.ToString("0") + "\tNumCVs: " + RSL.TotalNumberCVs + "\tNumQueuedCVs: " + RSL.NumberQueuedCVs + "\t\t%QueuedCVs: " + RSL.PercentQueuedCVs.ToString("00") +
                        //                             "\tQueued " + RSL.Queued + "\tDateProcessed: " + RSL.CVDateProcessed + "\tBeginMM: " + RSL.BeginMM);
                    }

                    if (tmpTroupe.SubLinks.Count > 0)
                    {
                        tmpTroupe.CalculateTroupeAvgSpeed();
                        tmpTroupe.CalculateTroupeLength();
                        tmpTroupe.CalculateTroupeTravelTime();

                        //travel time of the troupe based on the average speed of troupe and length of troupe
                        if (tmpTroupe.TravelTime < clsGlobalVars.DSD)
                        {
                            //Join the sublinks to the previous troupe and assign the last troupe avgspeed to the sublinks in the current troupe since they cannot be a troupe on their own
                            foreach (clsRoadwaySubLink TSL in tmpTroupe.SubLinks)
                            {
                                foreach (clsRoadwaySubLink RSL in RSLList)
                                {
                                    if (TSL.Identifier == RSL.Identifier)
                                    {
                                        RSL.TroupeSpeed = LastTroupeAvgSpeed;
                                        RSL.TroupeInclusionOverride = true;
                                        RSL.BeginTroupe = false;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Assign the troupe avgspeed to all sublinks in the troupe
                            foreach (clsRoadwaySubLink TSL in tmpTroupe.SubLinks)
                            {
                                foreach (clsRoadwaySubLink RSL in RSLList)
                                {
                                    if (TSL.Identifier == RSL.Identifier)
                                    {
                                        RSL.TroupeSpeed = tmpTroupe.AvgSpeed;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    LogTxtMsg(LoggerType.SpdHarm, LoggerLevel.Debug, "\r\n\tNo sublink was found with speed > 0 upstream of the congestion or queue location");
                }
            }
            catch (Exception ex)
            {
                retValue = "Error in processing roadway sublinks to determine troupes of sublinks." + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }

        private string CalculateSublinkHarmonizedSpeed(ref List<clsRoadwaySubLink> RSLList, double TroupingEndMM, double TroupingEndSpeed)
        {
            string retValue = string.Empty;
            string CVID = string.Empty;
            string CurrRecord = string.Empty;
            int SPDHarmStartSublink = 0;
            double SPDHarmGroupHarmonizedSpeed = 0;
            int SPDHarmGroupNumberOfSubLinks = 0;
            int tmpNumberofSublinks = 0;
            double LastSPDHarmGroupHarmonizedSpeed = 0;
            double SpeedDiff = 0;

            try
            {
                //Sort sublinks based on sublink identifier assuming that sublinks are numberered starting from the sublink which is furthest upstream from the reuccuring congestion location
                //The furthest upstream sublink from the recurring congestion location is numbered as 1.
                RSLList.Sort((l, r) => l.Identifier.CompareTo(r.Identifier));

                List<clsRoadwaySubLink> tmpSPDHarmSublinkGroup = new List<clsRoadwaySubLink>();

                //Find the furthest link upstream from the recurring congestion location where Trouping ended to start the speed harmonization process
                for (int i = RSLList.Count - 1; i >= 0; i--)
                {
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        if ((RSLList[i].RecommendedSpeed > 0) && (RSLList[i].BeginMM < TroupingEndMM))
                        {
                            SPDHarmStartSublink = i;
                            break;
                        }
                    }
                    else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                    {
                        if ((RSLList[i].RecommendedSpeed > 0) && (RSLList[i].BeginMM > TroupingEndMM))
                        {
                            SPDHarmStartSublink = i;
                            break;
                        }
                    }
                }
                //Reset all the sublink troupe speed that are less than the minimum display speed to the minimum display speed
                for (int j = SPDHarmStartSublink; j >= 0; j--)
                {
                    if (RSLList[j].TroupeSpeed < clsGlobalVars.LinkQueuedSpeedThreshold)
                    {
                        RSLList[j].TroupeSpeed = clsGlobalVars.LinkQueuedSpeedThreshold;
                    }
                }

                SPDHarmGroupHarmonizedSpeed = TroupingEndSpeed;
                if (TroupingEndSpeed < clsGlobalVars.LinkQueuedSpeedThreshold)
                {
                    SPDHarmGroupHarmonizedSpeed = clsGlobalVars.LinkQueuedSpeedThreshold;
                }

                if ((SPDHarmGroupHarmonizedSpeed % clsGlobalVars.TroupeRange) != 0)
                {
                    SPDHarmGroupHarmonizedSpeed = (int)((SPDHarmGroupHarmonizedSpeed + 5) / 5) * 5;
                }

                SpeedDiff = RSLList[SPDHarmStartSublink].TroupeSpeed - SPDHarmGroupHarmonizedSpeed;
                if (SpeedDiff > 5) // greater than five miles per hour 
                {
                    SPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed + 5;
                }
                else
                {
                    SPDHarmGroupHarmonizedSpeed = RSLList[SPDHarmStartSublink].TroupeSpeed;
                }

                RSLList[SPDHarmStartSublink].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                RSLList[SPDHarmStartSublink].BeginSpdHarm = true;
                SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                tmpNumberofSublinks = 1;
                tmpSPDHarmSublinkGroup.Add(RSLList[SPDHarmStartSublink]);

                /*for (int j = SPDHarmStartSublink-1; j >= 0; j--)
                {
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        if ((RSLList[j].RecommendedSpeed > 0) && (RSLList[j].BeginMM < TroupingEndMM))
                        {
                            SPDHarmStartSublink = i;

                            SpeedDiff = RSLList[i].TroupeSpeed - SPDHarmGroupHarmonizedSpeed;

                            LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                            if ((SpeedDiff > 5) || (SpeedDiff < 0))// greater than five miles per hour or < 0 in case the speed was less than the previous 
                            {
                                SPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed + 5;
                            }
                            else
                            {
                                SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                            }

                            
                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                            RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                            LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                            RSLList[i].BeginSpdHarm = true;
                            SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD)/(clsGlobalVars.SubLinkLength * 3600)));
                            tmpNumberofSublinks = 1;
                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                            break;
                        }
                    }
                    else
                    {
                        if ((RSLList[i].RecommendedSpeed > 0) && (RSLList[i].BeginMM > TroupingEndMM))
                        {
                            SPDHarmStartSublink = i;
                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                            RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                            LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                            RSLList[i].BeginSpdHarm = true;
                            SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                            tmpNumberofSublinks = 1;
                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                            break;
                        }
                    }
                }*/

                //Start processing the rest of the sublinks
                if (SPDHarmStartSublink != 0)
                {
                    for (int i = SPDHarmStartSublink - 1; i >= 0; i--)
                    {
                        if (Roadway.Direction == Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[i].BeginMM < TroupingEndMM)
                            {
                                if (RSLList[i].TroupeSpeed == SPDHarmGroupHarmonizedSpeed)
                                {
                                    //Current sublink troupe speed = current sublink harmonization group  speed
                                    //Add the sublink to the sublink harmonization group, Change the sublink harmonizedspeed and continue
                                    RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                    tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                    tmpNumberofSublinks++;
                                }
                                //If the current sublink troupe speed > current sublink harmonization group speed
                                else if (RSLList[i].TroupeSpeed > SPDHarmGroupHarmonizedSpeed)
                                {   //if the number of sublink in the harmonization group > number sublinks required for DSD
                                    if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //End the current sublink harmonization group and start a new sublink harmonization group and add the current sublink to it.
                                        SpeedDiff = RSLList[i].TroupeSpeed - SPDHarmGroupHarmonizedSpeed;

                                        LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        if (SpeedDiff > 5) // greater than fivemiles per hour
                                        {
                                            SPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed + 5;
                                        }
                                        else
                                        {
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                        }
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        RSLList[i].BeginSpdHarm = true;
                                        tmpNumberofSublinks = 1;
                                        tmpSPDHarmSublinkGroup.Clear();
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                    }
                                    // else if the number of sublinks in the harmonization group < number of sublinks required for DSD
                                    else if (tmpNumberofSublinks < SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //Lower the speed of the current sublink to the current harmonization group speed, add the sublink to the group and continue
                                        tmpNumberofSublinks++;
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        RSLList[i].SpdHarmInclusionOverride = true;
                                    }
                                }
                                //else if current sublink troupe speed is < current harmonization group speed 
                                else if (RSLList[i].TroupeSpeed < (SPDHarmGroupHarmonizedSpeed))
                                {
                                    //if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    //{
                                    //}
                                    //if the number of sublinks in the current harmonization group is < number of sublinks required for DSD
                                    if (tmpNumberofSublinks < SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //Assign previous group harmonized speed to the sublink if the speed of the last speed harmonization group is < than the sublink troupe speed
                                        if (LastSPDHarmGroupHarmonizedSpeed <= SPDHarmGroupHarmonizedSpeed)
                                        {
                                            foreach (clsRoadwaySubLink TSL in tmpSPDHarmSublinkGroup)
                                            {
                                                foreach (clsRoadwaySubLink RSL in RSLList)
                                                {
                                                    if (TSL.Identifier == RSL.Identifier)
                                                    {
                                                        RSL.HarmonizedSpeed = LastSPDHarmGroupHarmonizedSpeed;
                                                        RSL.SpdHarmInclusionOverride = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                            RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                            RSLList[i].BeginSpdHarm = true;
                                            tmpNumberofSublinks = 1;
                                            tmpSPDHarmSublinkGroup.Clear();
                                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                            SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                        }
                                        else
                                        {
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                            foreach (clsRoadwaySubLink TSL in tmpSPDHarmSublinkGroup)
                                            {
                                                foreach (clsRoadwaySubLink RSL in RSLList)
                                                {
                                                    if (TSL.Identifier == RSL.Identifier)
                                                    {
                                                        RSL.HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                                        RSL.SpdHarmInclusionOverride = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                            tmpNumberofSublinks++;
                                        }
                                    }
                                    else if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //start a new spd harm group
                                        LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        RSLList[i].BeginSpdHarm = true;
                                        tmpNumberofSublinks = 1;
                                        tmpSPDHarmSublinkGroup.Clear();
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                    }
                                }
                            }
                        }
                        else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                        {
                            if (RSLList[i].BeginMM > TroupingEndMM)
                            {
                                if (RSLList[i].TroupeSpeed == SPDHarmGroupHarmonizedSpeed)
                                {
                                    //Current sublink troupe speed = current sublink harmonization group  speed
                                    //Add the sublink to the sublink harmonization group, Change the sublink harmonizedspeed and continue
                                    RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                    tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                    tmpNumberofSublinks++;
                                }
                                //If the current sublink troupe speed > current sublink harmonization group speed
                                else if (RSLList[i].TroupeSpeed > SPDHarmGroupHarmonizedSpeed)
                                {   //if the number of sublink in the harmonization group > number sublinks required for DSD
                                    if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //End the current sublink harmonization group and start a new sublink harmonization group and add the current sublink to it.
                                        SpeedDiff = RSLList[i].TroupeSpeed - SPDHarmGroupHarmonizedSpeed;

                                        LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        if (SpeedDiff > 5) // greater than fivemiles per hour
                                        {
                                            SPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed + 5;
                                        }
                                        else
                                        {
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                        }
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        RSLList[i].BeginSpdHarm = true;
                                        tmpNumberofSublinks = 1;
                                        tmpSPDHarmSublinkGroup.Clear();
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                    }
                                    // else if the number of sublinks in the harmonization group < number of sublinks required for DSD
                                    else if (tmpNumberofSublinks < SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //Lower the speed of the current sublink to the current harmonization group speed, add the sublink to the group and continue
                                        tmpNumberofSublinks++;
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        RSLList[i].SpdHarmInclusionOverride = true;
                                    }
                                }
                                //else if current sublink troupe speed is < current harmonization group speed 
                                else if (RSLList[i].TroupeSpeed < (SPDHarmGroupHarmonizedSpeed))
                                {
                                    //if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    //{
                                    //}
                                    //if the number of sublinks in the current harmonization group is < number of sublinks required for DSD
                                    if (tmpNumberofSublinks < SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //Assign previous group harmonized speed to the sublink if the speed of the last speed harmonization group is < than the sublink troupe speed
                                        if (LastSPDHarmGroupHarmonizedSpeed <= SPDHarmGroupHarmonizedSpeed)
                                        {
                                            foreach (clsRoadwaySubLink TSL in tmpSPDHarmSublinkGroup)
                                            {
                                                foreach (clsRoadwaySubLink RSL in RSLList)
                                                {
                                                    if (TSL.Identifier == RSL.Identifier)
                                                    {
                                                        RSL.HarmonizedSpeed = LastSPDHarmGroupHarmonizedSpeed;
                                                        RSL.SpdHarmInclusionOverride = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                            RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                            RSLList[i].BeginSpdHarm = true;
                                            tmpNumberofSublinks = 1;
                                            tmpSPDHarmSublinkGroup.Clear();
                                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                            SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                        }
                                        else
                                        {
                                            SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                            foreach (clsRoadwaySubLink TSL in tmpSPDHarmSublinkGroup)
                                            {
                                                foreach (clsRoadwaySubLink RSL in RSLList)
                                                {
                                                    if (TSL.Identifier == RSL.Identifier)
                                                    {
                                                        RSL.HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                                        RSL.SpdHarmInclusionOverride = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                            tmpNumberofSublinks++;
                                        }
                                    }
                                    else if (tmpNumberofSublinks >= SPDHarmGroupNumberOfSubLinks)
                                    {
                                        //start a new spd harm group
                                        LastSPDHarmGroupHarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        SPDHarmGroupHarmonizedSpeed = RSLList[i].TroupeSpeed;
                                        RSLList[i].HarmonizedSpeed = SPDHarmGroupHarmonizedSpeed;
                                        RSLList[i].BeginSpdHarm = true;
                                        tmpNumberofSublinks = 1;
                                        tmpSPDHarmSublinkGroup.Clear();
                                        tmpSPDHarmSublinkGroup.Add(RSLList[i]);
                                        SPDHarmGroupNumberOfSubLinks = (int)(Math.Ceiling((SPDHarmGroupHarmonizedSpeed * clsGlobalVars.DSD) / (clsGlobalVars.SubLinkLength * 3600)));
                                    }
                                }
                            }
                        }
                        //LogTxtMsg("\t\t\tRSL CVAvg speed: " + RSL.CVAvgSpeed.ToString("0") + "\tNumCVs: " + RSL.TotalNumberCVs + "\tNumQueuedCVs: " + RSL.NumberQueuedCVs + "\t\t%QueuedCVs: " + RSL.PercentQueuedCVs.ToString("00") +
                        //                             "\tQueued " + RSL.Queued + "\tDateProcessed: " + RSL.CVDateProcessed + "\tBeginMM: " + RSL.BeginMM);
                    }

                    if ((tmpSPDHarmSublinkGroup.Count > 0) && (SPDHarmGroupHarmonizedSpeed > 0))
                    {
                        //Assign previous group harmonized speed to the sublink if the speed of the last speed harmonization group is < than the sublink troupe speed
                        if ((LastSPDHarmGroupHarmonizedSpeed > 0) && (LastSPDHarmGroupHarmonizedSpeed < SPDHarmGroupHarmonizedSpeed))
                        {
                            if (tmpNumberofSublinks < SPDHarmGroupNumberOfSubLinks)
                            {
                                foreach (clsRoadwaySubLink TSL in tmpSPDHarmSublinkGroup)
                                {
                                    foreach (clsRoadwaySubLink RSL in RSLList)
                                    {
                                        if (TSL.Identifier == RSL.Identifier)
                                        {
                                            RSL.HarmonizedSpeed = LastSPDHarmGroupHarmonizedSpeed;
                                            RSL.SpdHarmInclusionOverride = true;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    LogTxtMsg(LoggerType.SpdHarm, LoggerLevel.Debug,
                        "\r\n\tNo sublink was found with speed > 0 upstream of the congestion or queue location");
                }
            }
            catch (Exception ex)
            {
                retValue = "Error in processing roadway sublinks to determine harmonized speed." + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }
        private string InsertSubLinkStatusIntoINFLODatabase()
        {
            string retValue = string.Empty;
            string DSID = string.Empty;
            string CurrRecord = string.Empty;
            try
            {
                string sqlStr = string.Empty;
                TimeSpan starttime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "Adding roadway sublink dynamic information to INFLO database: ");
                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
                foreach (clsRoadwaySubLink tmpRSL in RSLList)
                {

                    TME_CVData_SubLink input = new TME_CVData_SubLink();
                    input.RoadwayId = tmpRSL.RoadwayID.ToString();
                    input.SubLinkId = tmpRSL.Identifier.ToString();
                    input.DateProcessed = tmpRSL.DateProcessed;
                    input.IntervalLength = (short)clsGlobalVars.CVDataPollingFrequency;
                    input.TSSAvgSpeed = tmpRSL.TSSAvgSpeed;
                    input.CVAvgSpeed = tmpRSL.CVAvgSpeed;
                    input.WRTMSpeed = tmpRSL.WRTMSpeed;
                    input.RecommendedTargetSpeed = (short)tmpRSL.RecommendedSpeed;
                    input.RecommendedTargetSpeedSource = tmpRSL.RecommendedSpeedSource.ToString();
                    input.CVQueuedState = tmpRSL.Queued;
                    input.CVCongestedState = tmpRSL.Congested;
                    input.NumberCVs = (short)tmpRSL.TotalNumberCVs;
                    input.NumberQueuedCVs = (short)tmpRSL.NumberQueuedCVs;
                    input.PercentQueuedCVs = (short)tmpRSL.PercentQueuedCVs;

                    uow.TME_CVData_SubLinks.Add(input);

                    //kg: bug below: looks like queued and congested are flipped.
                    //sqlStr = "INSERT INTO TME_CVData_SubLink(RoadwayId, SubLinkId, DateProcessed, IntervalLength, TSSAvgSpeed, CVAvgSpeed, WRTMSpeed, RecommendedSpeed, RecommendedSpeedSource, CVQueued, CVCongested, NumberCVs,NumberQueuedCVs, PercentQueuedCVs) " +
                    //            "Values('" + tmpRSL.RoadwayID + "', '" + tmpRSL.Identifier + "', #" + tmpRSL.DateProcessed + "#, " + clsGlobalVars.CVDataPollingFrequency + ", " + tmpRSL.TSSAvgSpeed.ToString("0") + ", '" +
                    //                         tmpRSL.CVAvgSpeed + ", " + tmpRSL.WRTMSpeed.ToString("0.0") + ", " + tmpRSL.RecommendedSpeed.ToString("0") + ", '" + tmpRSL.RecommendedSpeedSource.ToString() + "', " +
                    //                         tmpRSL.Congested + ", " + tmpRSL.Queued + ", " + tmpRSL.TotalNumberCVs + ", " + tmpRSL.NumberQueuedCVs + ", " + tmpRSL.PercentQueuedCVs.ToString("00") + ")";
                    //retValue = DB.InsertRow(sqlStr);

                    LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "\t\tRoadway Sublink data added to INFLO database: " + tmpRSL.RoadwayID + ", " + tmpRSL.Identifier + ", " + tmpRSL.DateProcessed + ", " + clsGlobalVars.CVDataPollingFrequency + ", " + tmpRSL.TSSAvgSpeed + ", " +
                                                tmpRSL.CVAvgSpeed + ", " + tmpRSL.WRTMSpeed + ", " + tmpRSL.RecommendedSpeed + ", " + tmpRSL.RecommendedSpeedSource + ", " +
                                                tmpRSL.Congested + ", " + tmpRSL.Queued + ", " + tmpRSL.TotalNumberCVs + ", " + tmpRSL.NumberQueuedCVs + ", " + tmpRSL.PercentQueuedCVs.ToString("00"));
                }
                TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.CV, LoggerLevel.Debug, "Time for adding: " + RSLList.Count + " Sublink records into database" + (endtime.TotalMilliseconds - starttime.TotalMilliseconds).ToString("0") + " msecs");
            }
            catch (Exception ex)
            {
                retValue = "Error in adding Sublink data into INFLO database. Error Hint - Current record: " + CurrRecord + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }

        static int TSSDataQuickIdFilter = 0;
        /// <summary>
        /// Called from tmrTSSData_Tick to get data from the database.
        /// </summary>
        /// <param name="DB">Database instatiated based on settings from config file.</param>
        /// <param name="IntervalUTCTime">utc time Now</param>
        /// <param name="DZList">list to load new data into (by identifier)</param>
        /// <param name="NumberRecordsRetrieved"></param>
        /// <returns></returns>
        private string GetLastIntervalDetectionZoneStatus(clsDatabase DB, DateTime IntervalUTCTime, ref List<clsDetectionZoneWSDOT> DZList, ref int NumberRecordsRetrieved)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;
            List<clsDetectionZone> LastIntervalDZList = new List<clsDetectionZone>();
            NumberRecordsRetrieved = 0;
            retValue = string.Empty;
            try
            {
                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
                DateTime sinceIntervalUTCTime = IntervalUTCTime.AddSeconds(-clsGlobalVars.TSSDataPollingFrequency);

                if (TSSDataQuickIdFilter == 0)
                {
                    //This table accrues 2.5 million rows in ~10 days.  Now that we added an integer unique id column,
                    //let's use this to quickly filter out a few million rows before the date filter has to work.  The id
                    //doesn't have to be exact, since the date filter is still employed.
                    int? qf = uow.TME_TSSData_Inputs.OrderByDescending(q => q.Id).Select(q => q.Id).FirstOrDefault();
                    if (qf != null)
                    {
                        //This only needs to be in the ballpark. We get 500-900 samples empirically in 10 minutes, so
                        //for our 5 minute window, lets overlap a thousand just for good measure.
                        TSSDataQuickIdFilter = (int)qf - 1000;
                    }
                }

                List<TME_TSSData_Input> data = uow.TME_TSSData_Inputs
                    .Where(d => (d.Id > TSSDataQuickIdFilter
                        && d.DateReceived >= sinceIntervalUTCTime && d.RoadwayId == Roadway.Identifier))
                     .ToList(); //kg: added roadway id to this query. we get a lot of data that isn't for this instance.




                LogTxtMsg(LoggerType.FillDataSetLog, LoggerLevel.Info, "GetLastIntervalDetectionZoneStatus," + data.Count
                    + "," + IntervalUTCTime);


                //display Detector station information 
                LogTxtMsg(LoggerType.Config, LoggerLevel.Debug, "\tAvailable detection zone records: ");



                foreach (TME_TSSData_Input d in data)
                {
                    //Reset variables.
                    clsDetectionZone tmpDZ = new clsDetectionZone();

                    //Just make sure the quick filter is set to any one of the returned set, so the number keeps increasing.
                    TSSDataQuickIdFilter = d.Id;

                    //Store each item to the  clsDetectionZone tmpDZ .
                    if (d.DSId != null)
                    {
                        tmpDZ.DSIdentifier = int.Parse(d.DSId);//this field allowed to be null?   
                    }
                    tmpDZ.Identifier = d.DZId;
                    //KG: mmlocation does not exist in table.
                    //case "mmlocation":
                    //    if (row[col].ToString().Length > 0)
                    //    {
                    //        MMLocation = double.Parse(row[col].ToString());
                    //        tmpDZ.MMLocation = MMLocation;
                    //    }
                    //    break;
                    tmpDZ.DateReceived = GetNullable(d.DateReceived);
                    tmpDZ.StartInterval = GetNullable(d.StartInterval);
                    tmpDZ.EndInterval = GetNullable(d.EndInterval);
                    tmpDZ.IntervalLength = GetNullable(d.IntervalLength);
                    tmpDZ.BeginTime = GetNullable(d.BeginTime);
                    tmpDZ.EndTime = GetNullable(d.EndTime);
                    tmpDZ.Volume = d.Volume;
                    tmpDZ.AvgSpeed = d.AvgSpeed;
                    tmpDZ.Occupancy = d.Occupancy;
                    tmpDZ.Queued = GetNullable(d.Queued);
                    tmpDZ.Congested = GetNullable(d.Congested);
                    tmpDZ.DZStatus = GetNullable(d.DZStatus);
                    tmpDZ.DataType = GetNullable(d.DataType);

                    //LogTxtMsg("\t\t" + DSID + ", " + DZID + ", " + DateReceived + ", " + StartInterval + ", " + EndInterval + ", " + BeginTime + ", " + EndTime + ", " +
                    //                                      IntervalLength + ", " + Volume + ", " + AvgSpeed + ", " + Occ + ", " + Queued + ", " + Congested + ", " + DZStatus + ", " + DataType + ", " + MMLocation);

                    //Save the  clsDetectionZone tmpDZ  to LastIntervalDZList for each row retrieved.
                    LastIntervalDZList.Add(tmpDZ);
                }
                foreach (clsDetectionZoneWSDOT DZ in DZList)//Iterate over input list passed in.
                {
                    //Clear out previous value from this input.
                    DZ.AvgSpeed = 0;
                    DZ.Occupancy = 0;
                    DZ.Volume = 0;
                    DZ.Queued = false;
                    DZ.Congested = false;
                    DZ.DateReceived = DateTime.Now;
                    DZ.BeginTime = DateTime.Now;
                    DZ.EndTime = DateTime.Now;
                    DZ.StartInterval = 0;
                    DZ.EndInterval = 0;
                    DZ.IntervalLength = 0;
                    DZ.DZStatus = "NoNewData";

                    foreach (clsDetectionZone NewDZ in LastIntervalDZList)
                    {
                        //If the retrieved row's identifier matches the one in the input list, then save the data.
                        if (DZ.Identifier == NewDZ.Identifier)
                        {
                            DZ.AvgSpeed = NewDZ.AvgSpeed;
                            DZ.Occupancy = NewDZ.Occupancy;
                            DZ.Volume = NewDZ.Volume;
                            DZ.Queued = NewDZ.Queued;
                            DZ.Congested = NewDZ.Congested;
                            DZ.DateReceived = NewDZ.DateReceived;
                            DZ.BeginTime = NewDZ.BeginTime;
                            DZ.EndTime = NewDZ.EndTime;
                            DZ.StartInterval = NewDZ.StartInterval;
                            DZ.EndInterval = NewDZ.EndInterval;
                            DZ.IntervalLength = NewDZ.IntervalLength;
                            DZ.DZStatus = "NewData";
                            NumberRecordsRetrieved = NumberRecordsRetrieved + 1;
                            break;
                        }
                    }
                    if (DZ.DZStatus.ToUpper() == "NewData".ToUpper())//If the identifier was found, we have new data.
                    {
                        DZ.NoNewData = false;
                        DZ.NumberNoNewDataIntervals = 0;
                    }
                    else if (DZ.DZStatus.ToUpper() == "NoNewData".ToUpper())
                    {
                        DZ.NoNewData = true;
                        DZ.NumberNoNewDataIntervals = DZ.NumberNoNewDataIntervals + 1;
                    }



                }

            }
            catch (Exception ex)
            {
                retValue = "Error in retrieving Detection Zone data for the last interval from: "
                    + IntervalUTCTime.AddSeconds(-clsGlobalVars.TSSDataPollingFrequency)
                    + " for road " + Roadway.Identifier + " since Id " + TSSDataQuickIdFilter + "\r\n\t EXCEPTION:"
                    + ex.Message + "  " + ex.StackTrace;
                return retValue;
            }

            return retValue;
        }
        private string ProcessDetectorStationStatus()
        {
            string retValue = string.Empty;
            double TotalSpeed = 0;
            int TotalVol = 0;
            double TotalOcc = 0;
            double MaxOcc = 0;
            int NumDZs = 0;
            string DZID = string.Empty;
            string CurrRecord = string.Empty;
            DateTime DateReceived = DateTime.Now;
            DateTime BeginTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            int StartInterval = 0;
            int EndInterval = 0;
            int IntervalLength = 0;
            try
            {
                //KG useful in debugging.  
                List<string> zones = DSList.Select(m => m.DetectionZones).ToList();
                foreach (clsDetectorStation DS in DSList)
                {
                    TotalSpeed = 0;
                    TotalVol = 0;
                    TotalOcc = 0;
                    MaxOcc = 0;
                    NumDZs = 0;
                    DS.AvgSpeed = 0;
                    DateReceived = DateTime.Now.Date;
                    BeginTime = DateTime.Now.Date;
                    EndTime = DateTime.Now.Date;
                    StartInterval = 0;
                    EndInterval = 0;
                    IntervalLength = 0;
                    DS.Volume = 0;
                    DS.Congested = false;
                    DS.Queued = false;
                    DS.Occupancy = 0;
                    DS.DateReceived = DateTime.Now.Date;
                    DS.BeginTime = DateTime.Now.Date;
                    DS.EndTime = DateTime.Now.Date;
                    DS.StartInterval = 0;
                    DS.EndInterval = 0;
                    DS.IntervalLength = 0;
                    //LogTxtMsg("\t\t\tProcessing detector station: " + DS.Identifier);

                    string[] sField;
                    sField = DS.DetectionZones.Split(',');

                    foreach (clsDetectionZoneWSDOT DZ in DZList)
                    {
                        //LogTxtMsg("\t\t\t\tDetection zone: " + DZ.Identifier);
                        CurrRecord = "DS: " + DS.Identifier + "   DZ: " + DZ.Identifier;
                        DZID = DZ.Identifier.ToString();
                        if (sField.Length > 0)
                        {
                            for (int j = 0; j < sField.Length; j++)
                            {
                                if (sField[j].ToUpper() == DZID.ToUpper())
                                {
                                    if (Roadway.Direction == DZ.Direction)
                                    {
                                        //if (DZ.AvgSpeed > 0)
                                        //{
                                        //TotalSpeed = TotalSpeed + (DZ.AvgSpeed * DZ.Volume);
                                        TotalSpeed = TotalSpeed + (DZ.AvgSpeed * DZ.Volume);
                                        TotalVol = TotalVol + DZ.Volume;
                                        if (DZ.Occupancy > MaxOcc)
                                        {
                                            MaxOcc = DZ.Occupancy;
                                        }
                                        TotalOcc = TotalOcc + DZ.Occupancy;
                                        NumDZs = NumDZs + 1;
                                        DateReceived = DZ.DateReceived;
                                        BeginTime = DZ.BeginTime;
                                        EndTime = DZ.EndTime;
                                        StartInterval = DZ.StartInterval;
                                        EndInterval = DZ.EndInterval;
                                        IntervalLength = DZ.IntervalLength;
                                        //}
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (NumDZs > 0)
                    {
                        //DS.AvgSpeed = TotalSpeed / NumDZs;
                        if (TotalVol > 0)
                        {
                            DS.AvgSpeed = TotalSpeed / TotalVol;
                        }
                        else
                        {
                            DS.AvgSpeed = 0;
                        }
                        DS.Volume = TotalVol;
                        DS.Occupancy = MaxOcc;
                        //DS.Occupancy = TotalOcc / NumDZs;
                        DS.EndTime = EndTime;
                        DS.BeginTime = BeginTime;
                        DS.StartInterval = StartInterval;
                        DS.EndInterval = EndInterval;
                        DS.DateReceived = DateReceived;
                        DS.IntervalLength = IntervalLength;
                        //LogTxtMsg("\t\t\tDS Avg speed: " + DS.AvgSpeed.ToString("0") + "\tVolume: " + DS.Volume + "\tOccupancy: " + DS.Occupancy.ToString("0.0") + " " + DS.BeginTime + " " + DS.EndTime + " " +
                        //                                                   DS.StartInterval + " " + DS.EndInterval + " " + DS.IntervalLength + " " + DS.DateReceived);
                        //Update Congested State and Queued state
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "Error in processing detector station status from detection zones. Current record: " + CurrRecord + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }
        private string ProcessLinkInfrastructureStatus(clsRoadway Roadway)
        {
            string retValue = string.Empty;
            string DSID = string.Empty;
            string CurrRecord = string.Empty;
            string RLDS = string.Empty;
            try
            {
                //KG useful in debugging.  
                List<double> speeds = DSList.Select(m => m.AvgSpeed).ToList();
                List<int> vol = DSList.Select(m => m.Volume).ToList();
                List<double> occ = DSList.Select(m => m.Occupancy).ToList();
                foreach (clsRoadwayLink RL in RLList)
                {
                    RL.TSSAvgSpeed = 0;
                    RL.Volume = 0;
                    RL.Occupancy = 0;
                    RL.Congested = false;
                    RL.Queued = false;
                    RL.Congested = false;
                    RLDS = RL.DetectionStations;
                    foreach (clsDetectorStation DS in DSList)
                    {
                        //LogTxtMsg("\t\t\t\tDetector station: " + DS.Identifier);
                        CurrRecord = "RL: " + RL.Identifier + "   DS: " + DS.Identifier;
                        DSID = DS.Identifier.ToString();
                        if (RLDS == DSID)
                        {
                            //LogTxtMsg("\t\t\tProcessing roadway link: " + RL.Identifier + "\tFromMM: " + RL.BeginMM + "\tToMM: " + RL.EndMM + "\tRLDS: " + RL.DetectionStations + "\tDS: " + DS.Identifier);
                            if ((DS.Occupancy < clsGlobalVars.OccupancyThreshold) && (DS.Volume < clsGlobalVars.VolumeThreshold))
                            {
                                RL.TSSAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                                RL.Volume = DS.Volume;
                                RL.Occupancy = DS.Occupancy;
                            }
                            else if ((DS.Occupancy < clsGlobalVars.OccupancyThreshold) || (DS.Volume < clsGlobalVars.VolumeThreshold))
                            {
                                RL.TSSAvgSpeed = DS.AvgSpeed;
                                RL.Volume = DS.Volume;
                                RL.Occupancy = DS.Occupancy;
                            }
                            else if ((DS.Occupancy >= clsGlobalVars.OccupancyThreshold) && (DS.Volume >= clsGlobalVars.VolumeThreshold))
                            {
                                RL.TSSAvgSpeed = DS.AvgSpeed;
                                RL.Volume = DS.Volume;
                                RL.Occupancy = DS.Occupancy;
                            }
                            if (RL.TSSAvgSpeed == 0)
                            {
                                RL.TSSAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                            }
                            else if (RL.TSSAvgSpeed > clsGlobalVars.MaximumDisplaySpeed)
                            {
                                RL.TSSAvgSpeed = clsGlobalVars.MaximumDisplaySpeed;
                            }

                            if (RL.TSSAvgSpeed <= clsGlobalVars.LinkQueuedSpeedThreshold)
                            {
                                RL.Queued = true;
                            }
                            else if (RL.TSSAvgSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold)
                            {
                                RL.Congested = true;
                            }
                            RL.DateProcessed = DateTime.UtcNow;
                            RL.StartInterval = DS.StartInterval;
                            RL.EndInterval = DS.EndInterval;
                            //LogTxtMsg("\t\t\tRL Avg speed: " + RL.TSSAvgSpeed + "\tVolume: " + RL.Volume + "\tOccupancy: " + RL.Occupancy + "\tQueued: " + RL.Queued +
                            //                             "\tCongested: " + RL.Congested + "\tDateProcessed: " + RL.DateProcessed);
                            break;
                        }
                    }
                }
                //KG commented out 9/2015. Hassan doesn't recall what this was hardcoded to address.
                //#region "Check for the two NB links"
                //if (Roadway.Direction == clsEnums.enDirection.NB)
                //{
                //    foreach (clsRoadwayLink RL in RLList)
                //    {
                //        if ((RL.Identifier == 5) && (RL.BeginMM == 158.45) && (RL.EndMM == 158.92))
                //        {
                //            RL.TSSAvgSpeed = (RLList[2].TSSAvgSpeed + RLList[4].TSSAvgSpeed) / 2;
                //            if (RL.TSSAvgSpeed <= clsGlobalVars.LinkQueuedSpeedThreshold)
                //            {
                //                RL.Queued = true;
                //            }
                //            else if (RL.TSSAvgSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold)
                //            {
                //                RL.Congested = true;
                //            }
                //            break;
                //        }
                //    }
                //    foreach (clsRoadwayLink RL in RLList)
                //    {
                //        if ((RL.Identifier == 22) && (RL.BeginMM == 164.26) && (RL.EndMM == 164.66))
                //        {
                //            RL.TSSAvgSpeed = (RLList[19].TSSAvgSpeed + RLList[21].TSSAvgSpeed) / 2;
                //            if (RL.TSSAvgSpeed <= clsGlobalVars.LinkQueuedSpeedThreshold)
                //            {
                //                RL.Queued = true;
                //            }
                //            else if (RL.TSSAvgSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold)
                //            {
                //                RL.Congested = true;
                //            }
                //            break;
                //        }
                //    }
                //}
                //#endregion
            }
            catch (Exception ex)
            {
                retValue = "Error in processing roadway link status from detector station. Error Hint - Current record: " + CurrRecord + "\r\n" + ex.Message;
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Commented out because it is apparently deprecated. It was no longer being called, but when i went
        /// to convert it to use the UnitOfWork, the table is now called TME_TSSESS_Link and is missing WRTMSpeed and now has an ESSSpeeInsertSubLinkStatusIntoINFLODatabased.
        /// </summary>
        /// <returns></returns>
        /* private string InsertLinkStatusIntoINFLODatabase()
         {
             string retValue = string.Empty;
             string DSID = string.Empty;
             string CurrRecord = string.Empty;
             try
             {
                 string sqlStr = string.Empty;
                 TimeSpan starttime = new TimeSpan(DateTime.Now.Ticks);

                 //OleDbParameter parm = new OleDbParameter("?", OleDbType.Date);
                 //parm.Value = DateTime.Now;
                 //cmd.Parameters.Add(parm); 
                  foreach (clsRoadwayLink tmpRL in RLList)
                 {
                     //DateTime tmpDate = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);

                     sqlStr = "INSERT INTO TME_TSS_Link(RoadwayId, LinkId, DateProcessed, IntervalLength, TSSSpeed, TSSVolume, TSSOccupancy, WRTMSpeed, RecommendedSpeed, RecommendedSpeedSource, Congested, Queued) " +
                                 "Values('" + tmpRL.RoadwayID + "', '" + tmpRL.Identifier + "', #" + tmpRL.DateProcessed + "#, " + clsGlobalVars.TSSDataLoadingFrequency + ", " + tmpRL.TSSAvgSpeed.ToString("0") + ", " +
                                              tmpRL.Volume + ", " + tmpRL.Occupancy.ToString("0.0") + ", " + tmpRL.WRTMSpeed.ToString("0") + ", " + tmpRL.RecommendedSpeed.ToString("0") + ", '" +
                                              tmpRL.RecommendedSpeedSource.ToString() + "', " + tmpRL.Congested + ", " + tmpRL.Queued + ")";
                     retValue = DB.InsertRow(sqlStr);
                     if (retValue.Length > 0)
                     {
                         return retValue;
                     }
                     LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\t\tRoadway link TSS data added to database: " + tmpRL.RoadwayID + ", " + tmpRL.Identifier + ", " + tmpRL.TSSAvgSpeed + ", " + tmpRL.Volume + ", " +
                                                                                     tmpRL.Occupancy + ", " + tmpRL.WRTMSpeed + ", " + tmpRL.RecommendedSpeedSource + ", " + tmpRL.RecommendedSpeedSource + ", " +
                                                                                     tmpRL.Congested + ", " + tmpRL.Queued + ", " + tmpRL.DateProcessed + ", " + clsGlobalVars.TSSDataLoadingFrequency);
                 }
                 TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
                 LogTxtMsg(LoggerType.TSS, LoggerLevel.Debug, "\r\n\tTime for adding: " + RLList.Count + " records into database" + (endtime.TotalMilliseconds - starttime.TotalMilliseconds).ToString("0") + " msecs");
             }
             catch (Exception ex)
             {
                 retValue = "Error in adding TSS link data into INFLO database. Error Hint - Current record: " + CurrRecord + "\r\n" + ex.Message;
                 return retValue;
             }
             return retValue;
         }
         */

        private string InsertQueueInfoIntoINFLODatabase(double BOQMMLocation, DateTime BOQTime, double QueueRate, clsEnums.enQueueCahnge QueueChange, clsEnums.enQueueSource QueueSource, double QueueSpeed, clsRoadway Roadway)
        {
            string retValue = string.Empty;
            try
            {
                string sqlStr = string.Empty;
                TimeSpan starttime = new TimeSpan(DateTime.Now.Ticks);

                DateTime DateGenerated = TimeZoneInfo.ConvertTimeToUtc(BOQTime, TimeZoneInfo.Local);
                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

                //Before we add a new spd harm for this location, we need to invalidate all the previous ones for this same location.
                var invalidate = uow.TMEOutput_QWARNMessage_CVs
 .Where(d => d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE && d.RoadwayID == Roadway.Identifier && d.FOQMMLocation == Roadway.EndMM);
                foreach (var s in invalidate)
                {
                    s.ValidityDuration = Common.INFLO_VALIDITY_DURATION_AUTO_INACTIVE;//Make invalid
                }


                TMEOutput_QWARNMessage_CV input = new TMEOutput_QWARNMessage_CV();
                input.RoadwayID = Roadway.Identifier.ToString();
                input.DateGenerated = DateGenerated;
                input.BOQMMLocation = BOQMMLocation;
                input.FOQMMLocation = Roadway.RecurringCongestionMMLocation;
                input.RateOfQueueGrowth = QueueRate;
                input.SpeedInQueue = (short)QueueSpeed;
                input.ValidityDuration = Common.INFLO_VALIDITY_DURATION_ACTIVE;

                uow.TMEOutput_QWARNMessage_CVs.Add(input);
                uow.SaveChanges();

                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tQueue Info added to database: " + Roadway.Identifier.ToString() + ", " + BOQTime.ToString() + ", " + BOQMMLocation + ", " + Roadway.RecurringCongestionMMLocation + ", " +
                                                                                QueueRate.ToString("0") + ", " + QueueChange + ", " + QueueSpeed.ToString("0") + ", " + "60");

                TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tTime for adding Queue info to database: " + (endtime.TotalMilliseconds - starttime.TotalMilliseconds).ToString("0") + " msecs");
            }
            catch (Exception ex)
            {
                retValue = "\tError in adding Queue info into INFLO database." + "\r\n\t" + ex.Message;
                return retValue;
            }
            return retValue;
        }

        private string GenerateSPDHarmMessages(List<clsRoadwaySubLink> RSLList, double TroupingEndMM, double BOQMMLocation, clsRoadway Roadway)
        {
            string retValue = string.Empty;

            double RecommendedSpeed = 0;
            double BeginMM = 0;
            string Justification = string.Empty;
            double EndMM = 0;

            try
            {
                IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

                //RSLList.Sort((l, r) => l.BeginMM.CompareTo(r.BeginMM));
                if (BOQMMLocation > 0)
                {
                    Justification = "Queue";
                }
                else
                {
                    Justification = "Congestion";
                }

                RecommendedSpeed = RSLList[0].HarmonizedSpeed;
                BeginMM = RSLList[0].BeginMM;
                EndMM = RSLList[0].EndMM;

                for (int i = 1; i < RSLList.Count; i++)
                {
                    if (Roadway.Direction == Roadway.MMIncreasingDirection)
                    {
                        //if (RSLList[i].BeginMM < TroupingEndMM)
                        if (RSLList[i].BeginMM < Roadway.RecurringCongestionMMLocation)
                        {
                            if (RSLList[i].HarmonizedSpeed == RecommendedSpeed)
                            {
                                EndMM = RSLList[i].EndMM;
                            }
                            else if (RSLList[i].HarmonizedSpeed != RecommendedSpeed)
                            {
                                if ((RecommendedSpeed > 0) && (RecommendedSpeed <= (clsGlobalVars.LinkCongestedSpeedThreshold + 5)))
                                {
                                    retValue = InsertSPDHarmInfoIntoINFLODatabase(uow, Roadway.Identifier.ToString(), DateTime.Now.AddMilliseconds(100), RecommendedSpeed, BeginMM, EndMM, Justification);
                                    if (retValue.Length > 0)
                                    {
                                        LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Insert SPDHarmMsg: " + Roadway.Identifier + ", " + DateTime.Now.ToString() + ", " + RecommendedSpeed + ", " + BeginMM + ", " +
                                                                EndMM + ", " + Justification + ", 60" + "\r\n\t" + retValue);
                                    }
                                }
                                RecommendedSpeed = RSLList[i].HarmonizedSpeed;
                                BeginMM = RSLList[i].BeginMM;
                                EndMM = RSLList[i].EndMM;
                            }
                        }
                        //else if (RSLList[i].BeginMM >= TroupingEndMM)
                        else if (RSLList[i].BeginMM >= Roadway.RecurringCongestionMMLocation)
                        {
                            break;
                        }
                    }
                    else if (Roadway.Direction != Roadway.MMIncreasingDirection)
                    {
                        //if (RSLList[i].BeginMM > TroupingEndMM)
                        if (RSLList[i].BeginMM > Roadway.RecurringCongestionMMLocation)
                        {
                            if (RSLList[i].HarmonizedSpeed == RecommendedSpeed)
                            {
                                EndMM = RSLList[i].EndMM;
                            }
                            else if (RSLList[i].HarmonizedSpeed != RecommendedSpeed)
                            {
                                if ((RecommendedSpeed > 0) && (RecommendedSpeed <= (clsGlobalVars.LinkCongestedSpeedThreshold + 5)))
                                {
                                    retValue = InsertSPDHarmInfoIntoINFLODatabase(uow, Roadway.Identifier.ToString(), DateTime.Now.AddMilliseconds(100), RecommendedSpeed, BeginMM, EndMM, Justification);
                                    if (retValue.Length > 0)
                                    {
                                        LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Insert SPDHarmMsg: " + Roadway.Identifier + ", " + DateTime.Now.ToString() + ", " + RecommendedSpeed + ", " + BeginMM + ", " +
                                                                EndMM + ", " + Justification + ", 60" + "\r\n\t" + retValue);
                                    }
                                }
                                RecommendedSpeed = RSLList[i].HarmonizedSpeed;
                                BeginMM = RSLList[i].BeginMM;
                                EndMM = RSLList[i].EndMM;
                            }
                        }
                        // else if (RSLList[i].BeginMM <= TroupingEndMM)
                        else if (RSLList[i].BeginMM <= Roadway.RecurringCongestionMMLocation)
                        {
                            break;
                        }
                    }
                }
                if ((RecommendedSpeed > 0) && (RecommendedSpeed <= (clsGlobalVars.LinkCongestedSpeedThreshold + 5)))
                {
                    retValue = InsertSPDHarmInfoIntoINFLODatabase(uow, Roadway.Identifier.ToString(), DateTime.Now.AddMilliseconds(100), RecommendedSpeed, BeginMM, EndMM, Justification);
                    if (retValue.Length > 0)
                    {
                        LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "Insert SPDHarmMsg: " + Roadway.Identifier + ", " + DateTime.Now.ToString() + ", " + RecommendedSpeed + ", " + BeginMM + ", " +
                                                EndMM + ", " + Justification + ", 60" + "\r\n\t" + retValue);
                    }
                }
                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                retValue = "\tError in g SPDenerating SPD Harm message info." + "\r\n\t" + ex.Message;
                return retValue;
            }
            return retValue;
        }
        private string InsertSPDHarmInfoIntoINFLODatabase(IUnitOfWork uow, string RoadwayId, DateTime CurrentTime, double RecommendedSpeed, double BeginMM, double EndMM, string Justification)
        {
            string retValue = string.Empty;
            string sqlStr = string.Empty;
            try
            {
                TimeSpan starttime = new TimeSpan(DateTime.Now.Ticks);
                DateTime DateGenerated = TimeZoneInfo.ConvertTimeToUtc(CurrentTime, TimeZoneInfo.Local);

                //Before we add a new spd harm for this location, we need to invalidate all the previous ones for this same location.
                var invalidate = uow.TMEOutput_SPDHARMMessage_CVs
 .Where(d => d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE && d.RoadwayId == RoadwayId && d.BeginMM == BeginMM && d.EndMM == EndMM);
                foreach (var s in invalidate)
                {
                    s.ValidityDuration = Common.INFLO_VALIDITY_DURATION_AUTO_INACTIVE;//Make invalid
                }

                TMEOutput_SPDHARMMessage_CV input = new TMEOutput_SPDHARMMessage_CV();
                input.RoadwayId = Roadway.Identifier.ToString();
                input.DateGenerated = DateGenerated;
                input.RecommendedSpeed = (short)RecommendedSpeed;
                input.BeginMM = BeginMM;
                input.EndMM = EndMM;
                input.Justification = Justification;
                input.ValidityDuration = Common.INFLO_VALIDITY_DURATION_ACTIVE;

                uow.TMEOutput_SPDHARMMessage_CVs.Add(input);


                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tSPDHarm Message added to database: " + RoadwayId + ", " + DateGenerated.ToString() + ", " + RecommendedSpeed.ToString("0") + ", " + BeginMM.ToString("0.00") + ", " +
                                                                                EndMM.ToString("0.00") + ", " + Justification + ", " + "60");

                TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
                LogTxtMsg(LoggerType.Other, LoggerLevel.Debug, "\t\tTime for adding SPDHarm Messages into database: " + (endtime.TotalMilliseconds - starttime.TotalMilliseconds).ToString("0") + " msecs");
            }
            catch (Exception ex)
            {
                retValue = "\tError in adding SPDHarm message info into INFLO database." + "\r\n\t" + ex.Message;
                return retValue;
            }
            return retValue;
        }


        /// <summary>
        /// Used to be called from GUI using GUI field txtSubLinkPercentQueuedCVs.
        /// Used to show message boxes for invalid inputs, now returns them.
        /// Returns "" if good.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string btnUpdatePercentQueuedCVs_Click_1(string txtSubLinkPercentQueuedCVs)
        {
            string rtn = "";
            string tmpStrQueuedPercent = string.Empty;
            int tmpIntQueuedPercent = 0;
            tmpStrQueuedPercent = txtSubLinkPercentQueuedCVs;
            if (tmpStrQueuedPercent.Length > 0)
            {
                try
                {
                    tmpIntQueuedPercent = int.Parse(tmpStrQueuedPercent);
                    if ((tmpIntQueuedPercent > 0) && (tmpIntQueuedPercent <= 100))
                    {
                        clsGlobalVars.SubLinkPercentQueuedCV = tmpIntQueuedPercent;
                    }
                    else
                    {
                        rtn = "Please check the value entered for sublink % queued CVs and reneter a value between 1 - 100";
                    }
                }
                catch (Exception exc)
                {
                    rtn = "Please check the value entered for sublink % queued CVs and reneter a value between 1 - 100 \r\n\t" + exc.Message;
                }
            }
            return rtn;
        }
        /// <summary>
        /// Used to be called from GUI using GUI field  txtVisibility,txtGripFactor, txtWRTMBeginMM,txtWRTMEndMM.
        /// Used to show message boxes for invalid inputs, now returns them.
        /// Returns "" if good.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string txtUpdateWeather_Click(string tmpStrVisibility, string tmpStrGripFactor,
            string tmpStrBeginMM, string tmpStrEndMM)
        {
            string rtn = "";
            double tmpDoubleBeginMM = 0;
            double tmpDoubleEndMM = 0;
            int tmpIntVisibility = 0;
            double tmpDoubleGripFactor = 0;


            // txtRecommendedWRTMSpeed.Text = "";

            if (tmpStrBeginMM.Length > 0)
            {
                try
                {
                    tmpDoubleBeginMM = double.Parse(tmpStrBeginMM);
                    if (tmpDoubleBeginMM < 0)
                    {
                        return "Please check the value entered for WRTM Begin MM and reneter a value > 0";
                    }
                    else
                    {
                        clsGlobalVars.WRTMBeginMM = tmpDoubleBeginMM;
                    }
                }
                catch (Exception exc)
                {
                    return "Please check the value entered for WRTM Begin MM and reneter a value > 0 \r\n\t" + exc.Message;
                }
            }
            if (tmpStrEndMM.Length > 0)
            {
                try
                {
                    tmpDoubleEndMM = double.Parse(tmpStrEndMM);
                    if (tmpDoubleEndMM < 0)
                    {
                        return "Please check the value entered for WRTM End MM and reneter a value > 0";
                    }
                    else
                    {
                        clsGlobalVars.WRTMEndMM = tmpDoubleEndMM;
                    }
                }
                catch (Exception exc)
                {
                    return "Please check the value entered for WRTM End MM and reneter a value > 0 \r\n\t" + exc.Message;
                }
            }
            if (tmpStrVisibility.Length > 0)
            {
                try
                {
                    tmpIntVisibility = int.Parse(tmpStrVisibility);
                    if (tmpIntVisibility < 0)
                    {
                        return "Please check the value entered for Visibility and reneter a value > 0";
                    }
                }
                catch (Exception exc)
                {
                    return "Please check the value entered for Visibility and reneter a value > 0 \r\n\t" + exc.Message;
                }
            }
            if (tmpStrGripFactor.Length > 0)
            {
                try
                {
                    tmpDoubleGripFactor = double.Parse(tmpStrGripFactor);
                    if (tmpDoubleGripFactor < 0)
                    {
                        return "Please check the value entered for Coefficient of Friction and reneter a value > 0";
                    }
                }
                catch (Exception exc)
                {
                    return "Please check the value entered for Coefficient of Friction and reneter a value > 0 \r\n\t" + exc.Message;
                }
            }
            if (tmpIntVisibility >= 500)
            {
                if (tmpDoubleGripFactor >= 0.7)
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMMaxRecommendedSpeed, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    // txtRecommendedWRTMSpeed.Text = "WRTMMaxSpeed: " + clsGlobalVars.WRTMMaxRecommendedSpeed.ToString();
                }
                else if ((tmpDoubleGripFactor > 0.3) && (tmpDoubleGripFactor < 0.7))
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMRecommendedSpeedLevel1, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    // txtRecommendedWRTMSpeed.Text = "WRTMSpeedLevel1: " + clsGlobalVars.WRTMRecommendedSpeedLevel1.ToString();
                }
                else if (tmpDoubleGripFactor <= 0.3)
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMRecommendedSpeedLevel2, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    // txtRecommendedWRTMSpeed.Text = "WRTMSpeedLevel2: " + clsGlobalVars.WRTMRecommendedSpeedLevel2.ToString();
                }
            }
            else if (tmpIntVisibility < 500)
            {
                if (tmpDoubleGripFactor >= 0.7)
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMRecommendedSpeedLevel3, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    // txtRecommendedWRTMSpeed.Text = "WRTMSpeedLevel3: " + clsGlobalVars.WRTMRecommendedSpeedLevel3.ToString();
                }
                else if ((tmpDoubleGripFactor > 0.3) && (tmpDoubleGripFactor < 0.7))
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMRecommendedSpeedLevel4, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    //txtRecommendedWRTMSpeed.Text = "WRTMSpeedLevel4: " + clsGlobalVars.WRTMRecommendedSpeedLevel4.ToString();
                }
                else if (tmpDoubleGripFactor <= 0.3)
                {
                    ApplyWRTMSpeed(ref RLList, clsGlobalVars.WRTMMinRecommendedSpeed, clsGlobalVars.WRTMBeginMM, clsGlobalVars.WRTMEndMM);
                    //txtRecommendedWRTMSpeed.Text = "WRTMMinSpeed: " + clsGlobalVars.WRTMMinRecommendedSpeed.ToString();
                }
            }
            return rtn;
        }
        /// <summary>
        /// Used to be called from GUI using GUI field  txtDSD.
        /// Used to show message boxes for invalid inputs, now returns them.
        /// Returns "" if good.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string btnUpdateDecisionSightDistance_Click(string tmpStrDSD)
        {
            string rtn = "";
            int tmpIntDSD = 0;

            if (tmpStrDSD.Length > 0)
            {
                try
                {
                    tmpIntDSD = int.Parse(tmpStrDSD);
                    if ((tmpIntDSD > 0) && (tmpIntDSD <= 300))
                    {
                        clsGlobalVars.DSD = tmpIntDSD;
                    }
                    else
                    {
                        return "Please check the value entered for DSD and reneter a value between 1 - 300";
                    }
                }
                catch (Exception exc)
                {
                    return "Please check the value entered for DSD and reneter a value between 1 - 300 \r\n\t" + exc.Message;
                }
            }
            return rtn;
        }


    }
}
