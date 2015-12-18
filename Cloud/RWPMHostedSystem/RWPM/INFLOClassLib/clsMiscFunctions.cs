using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Net;

namespace INFLOClassLib
{
    public static class clsMiscFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConfigFile">XML of the config file</param>
        /// <param name="Roadway"></param>
        /// <returns></returns>
        public static string ReadINFLOConfigFile(string ConfigFile, ref clsRoadway Roadway)
        {
            string retValue = string.Empty;
            string CurrentTag = string.Empty;

            if (ConfigFile.Length > 0)
            {
               // if (File.Exists(ConfigFile))
                //{
                    try
                    {
                        //XDocument doc = XDocument.Load(ConfigFile);
                        XDocument doc = XDocument.Parse(ConfigFile);
                        XElement root = doc.Root;

                        string strTagName = string.Empty;
                        string strTagValue = string.Empty;
                        CurrentTag = strTagName + "::" + strTagValue;
                        ///LogTxtMsg(txtLog, "Reading the INFLO application configuration file: ");

                        foreach (XElement element in root.Elements())
                        {
                            strTagName = element.Name.ToString();
                            strTagValue = element.Value.ToString();
                            CurrentTag = strTagName + "::" + strTagValue;

                            #region "DB Interface"
                            if (strTagName.ToUpper() == "DBInterface".ToUpper())
                            {
                                clsGlobalVars.DBInterfaceType = strTagValue;      //Possible DBInterface types: sqlserver, oledb, odbc

                                ///LogTxtMsg(txtLog, "\tDatabase Interface Type: ");
                                ///LogTxtMsg(txtLog, "\t\t--DBInterfaceType: " + strTagValue);
                            }
                            #endregion

                            #region "DB Connection information"
                            else if (strTagName.ToUpper() == "DBConnection".ToUpper())
                            {
                                ///LogTxtMsg(txtLog, "\tDatabase Connection Information: ");
                                foreach (XElement s in element.Elements())
                                {
                                    strTagName = s.Name.ToString();
                                    strTagValue = s.Value.ToString();
                                    CurrentTag = strTagName + "::" + strTagValue;
                                    if (strTagName.ToUpper() == "DSNName".ToUpper())
                                    {
                                        clsGlobalVars.DSNName = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "AccessDBFileName".ToUpper())
                                    {
                                        clsGlobalVars.AccessDBFileName = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "SqlServer".ToUpper())
                                    {
                                        clsGlobalVars.SqlServer = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "SqlServerDatabase".ToUpper())
                                    {
                                        clsGlobalVars.SqlServerDatabase = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "SqlServerUserId".ToUpper())
                                    {
                                        clsGlobalVars.SqlServerUserId = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "SqlServerPassword".ToUpper())
                                    {
                                        clsGlobalVars.SqlServerPassword = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "SqlServerConnection".ToUpper())
                                    {
                                        clsGlobalVars.SqlStrConnection = strTagValue;
                                        ///LogTxtMsg(txtLog, "DB Connection: " + strTagValue);
                                    }
                                }
                            }
                            #endregion

                            #region "Configuration Files"
                            else if (strTagName.ToUpper() == "ConfigurationFiles".ToUpper())
                            {
                                ///LogTxtMsg(txtLog, "\tConfiguration Files: ");
                                foreach (XElement s in element.Elements())
                                {
                                    strTagName = s.Name.ToString();
                                    strTagValue = s.Value.ToString();
                                    CurrentTag = strTagName + "::" + strTagValue;

                                    if (strTagName.ToUpper() == "INFLOConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.INFLOConfigFile = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--INFLO config file" + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RoadwayLinkConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.RoadwayLinkConfigFile = strTagValue.Trim();
                                        ///LogTxtMsg(txtLog, "\t\t--RoadwayLink configuration file: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RoadwayDetectionStationConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.DetectorStationConfigFile = strTagValue.Trim();
                                        ///LogTxtMsg(txtLog, "\t\t--Detection Station config file: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RoadwayDetectionZoneConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.DetectionZoneConfigFile = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--Detection Zone config filer: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RoadwayDetectionZoneCombinedConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.DetectionZoneCombinedConfigFile = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--Detection Zone config filer: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "MileMarkerConfigFile".ToUpper())
                                    {
                                        clsGlobalVars.MileMarkerConfigFile = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--Detection Zone config filer: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "CVDataDirectory".ToUpper())
                                    {
                                        clsGlobalVars.CVDataDirectory = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t\t-- CV Data Directory: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "TSSDataDirectory".ToUpper())
                                    {
                                        clsGlobalVars.TSSDataDirectory = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t\t-- TSS Data Directory: " + strTagValue);
                                    }
                                }
                            }
                            #endregion

                            #region "Roadway Information"
                            else if (strTagName.ToUpper() == "RoadwayInfo".ToUpper())
                            {
                                ///LogTxtMsg(txtLog, "\tRoadway Information: ");
                                foreach (XElement s in element.Elements())
                                {
                                    strTagName = s.Name.ToString();
                                    strTagValue = s.Value.ToString();
                                    CurrentTag = strTagName + "::" + strTagValue;

                                    if (strTagName.ToUpper() == "RoadwayName".ToUpper())
                                    {
                                        Roadway.Name = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway name: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RoadwayID".ToUpper())
                                    {
                                        Roadway.Identifier = strTagValue;
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway Identifier: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "BeginMM".ToUpper())
                                    {
                                        Roadway.BeginMM = double.Parse(strTagValue);
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway BeginMileMarker: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "EndMM".ToUpper())
                                    {
                                        Roadway.EndMM = double.Parse(strTagValue);
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway EndMileMarker: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "MMIncreasingDir".ToUpper())
                                    {
                                        Roadway.MMIncreasingDirection = clsEnums.GetDirIndexFromString(strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "Direction".ToUpper())
                                    {
                                        Roadway.StrDir = strTagValue;
                                        Roadway.Direction = clsEnums.GetDirIndexFromString(strTagValue);
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway Direction: " + strTagValue);
                                    }
                                    else if (strTagName.ToUpper() == "RecurringCongestionMMLocation".ToUpper())
                                    {
                                        Roadway.RecurringCongestionMMLocation = double.Parse(strTagValue);
                                        ///LogTxtMsg(txtLog, "\t\t--Roadway RecurringCongestionMMLocation: " + strTagValue);
                                    }
                                }
                            }
                            #endregion

                            #region "INFLO Thresholds"
                            else if (strTagName.ToUpper() == "MinimumDisplaySpeed".ToUpper())
                            {
                                clsGlobalVars.MinimumDisplaySpeed = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tMinimum Display Speed: \t" + strTagValue);
                            }
                            else if (strTagName.ToUpper() == "TroupeRange".ToUpper())
                            {
                                clsGlobalVars.TroupeRange = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tTroupe Range: \t\t" + strTagValue + " miles");
                            }
                            else if (strTagName.ToUpper() == "DSD".ToUpper())
                            {
                                clsGlobalVars.DSD = int.Parse(strTagValue);
                            }
                            else if (strTagName.ToUpper() == "ThreeGantrySpeed".ToUpper())
                            {
                                clsGlobalVars.ThreeGantrySpeed = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tThreeGantrySpeed: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "CVDataLoadingFrequencySecs".ToUpper())
                            {
                                clsGlobalVars.CVDataLoadingFrequency = int.Parse(strTagValue);
                            }
                            else if (strTagName.ToUpper() == "CVDataStartingInterval".ToUpper())
                            {
                                clsGlobalVars.CVDataCurrInterval = int.Parse(strTagValue);
                            }
                            else if (strTagName.ToUpper() == "CVDataPollingFrequencySecs".ToUpper())
                            {
                                clsGlobalVars.CVDataPollingFrequency = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tCVDataPollingFrequency: \t" + strTagValue + " seconds");
                            }
                            else if (strTagName.ToUpper() == "TSSDataPollingFrequencySecs".ToUpper())
                            {
                                clsGlobalVars.TSSDataPollingFrequency = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tTSSDataPollingFrequency: \t" + strTagValue + " seconds");
                            }
                            else if (strTagName.ToUpper() == "TSSDataLoadingFrequencySecs".ToUpper())
                            {
                                clsGlobalVars.TSSDataLoadingFrequency = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\t\tTSS data loading frequency: " + strTagValue);
                            }
                            else if (strTagName.ToUpper() == "TSSDataStartingInterval".ToUpper())
                            {
                                clsGlobalVars.TSSDataCurrInterval = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\t\tTSS data starting simulation interval: " + strTagValue);
                            }
                            else if (strTagName.ToUpper() == "ESSDataPollingFrequencyMins".ToUpper())
                            {
                                clsGlobalVars.ESSDataPollingFrequency = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tESSDataPollingFrequency: \t" + strTagValue + " minutes");
                            }
                            else if (strTagName.ToUpper() == "MobileESSDataPollingFrequencyMins".ToUpper())
                            {
                                clsGlobalVars.MobileESSDataPollingFrequency = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tMobileESSDataPollingFrequency: \t" + strTagValue + " minutes");
                            }
                            else if (strTagName.ToUpper() == "QueueSpeedThreshold".ToUpper())
                            {
                                clsGlobalVars.LinkQueuedSpeedThreshold = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tQueueSpeedThreshold: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "CongestionSpeedThreshold".ToUpper())
                            {
                                clsGlobalVars.LinkCongestedSpeedThreshold = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tCongestionSpeedThreshold: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "SubLinkLength".ToUpper())
                            {
                                clsGlobalVars.SubLinkLength = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tSubLinkLength: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "SubLinkPercentQueuedCV".ToUpper())
                            {
                                clsGlobalVars.SubLinkPercentQueuedCV = int.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "MaximumDisplaySpeed".ToUpper())
                            {
                                clsGlobalVars.MaximumDisplaySpeed = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "COFLowerThreshold".ToUpper())
                            {
                                clsGlobalVars.COFLowerThreshold = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "COFUpperThreshold".ToUpper())
                            {
                                clsGlobalVars.COFUpperThreshold = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "RoadSurfaceStatusDryCOF".ToUpper())
                            {
                                clsGlobalVars.RoadSurfaceStatusDryCOF = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "RoadSurfaceStatusSnowCOF".ToUpper())
                            {
                                clsGlobalVars.RoadSurfaceStatusSnowCOF = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "RoadSurfaceStatusWetCOF".ToUpper())
                            {
                                clsGlobalVars.RoadSurfaceStatusWetCOF = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "RoadSurfaceStatusIceCOF".ToUpper())
                            {
                                clsGlobalVars.RoadSurfaceStatusIceCOF = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMaxRecommendedSpeed".ToUpper())
                            {
                                clsGlobalVars.WRTMMaxRecommendedSpeed = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMaxRecommendedSpeedLevel1".ToUpper())
                            {
                                clsGlobalVars.WRTMRecommendedSpeedLevel1 = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMaxRecommendedSpeedLevel2".ToUpper())
                            {
                                clsGlobalVars.WRTMRecommendedSpeedLevel2 = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMaxRecommendedSpeedLevel3".ToUpper())
                            {
                                clsGlobalVars.WRTMRecommendedSpeedLevel3 = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMaxRecommendedSpeedLevel4".ToUpper())
                            {
                                clsGlobalVars.WRTMRecommendedSpeedLevel4 = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "WRTMMinRecommendedSpeed".ToUpper())
                            {
                                clsGlobalVars.WRTMMinRecommendedSpeed = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityThreshold".ToUpper())
                            {
                                clsGlobalVars.VisibilityThreshold = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusClear".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusClear = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusFogNotPatchy".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusFogNotPatchy = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusPatchyFog".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusPatchyFog = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusBlowingSnow".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusBlowingSnow = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusSmoke".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusSmoke = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusSeaSpray".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusSeaSpray = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusVehicleSpray".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusVehicleSpray = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusBlowingDust".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusBlowingDust = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusBlowingSand".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusBlowingSand = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusSunGlare".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusSunGlare = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VisibilityStatusSwarmsOfInsects".ToUpper())
                            {
                                clsGlobalVars.VisibilityStatusSwarmsOfInsects = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "OccupancyThreshold".ToUpper())
                            {
                                clsGlobalVars.OccupancyThreshold = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            else if (strTagName.ToUpper() == "VolumeThreshold".ToUpper())
                            {
                                clsGlobalVars.VolumeThreshold = double.Parse(strTagValue);
                                ///LogTxtMsg(txtLog, "\tPercentQueuedCV: \t" + strTagValue + " mph");
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        retValue = "Error in ReadINFLOConfigFile. Current Tag: " + CurrentTag + "\r\n\t" + ex.Message;
                        return retValue;
                        ///LogTxtMsg(txtLog, retValue);
                    }
                //}
                //else
                //{
                //    retValue = "INFLO Configuration file does not exist: " + ConfigFile;
                //    return retValue;
                //    ///LogTxtMsg(txtLog, retValue);
                //}
            }
            return retValue;
        }
        public static string ReadRoadwayLinkConfigFile(string ConfigFile, ref List<clsRoadwayLink> RLList)
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            ///LogTxtMsg(txtLog, "\r\nReading the Roadway Link configuration file.");
            if (File.Exists(ConfigFile))
            {
                try
                {
                    StreamReader fReader = null;

                    CurrentSection = "OpeningConfigFile";
                    fReader = File.OpenText(ConfigFile);
                    string tmpRoadwayId = "";
                    int tmpLinkNo = 0;
                    double tmpBeginMM = 0;
                    double tmpEndMM = 0;
                    int tmpNoLanes = 0;
                    int tmpDetectionStationNo = 0;
                    int tmpNoDetectionStations = 0;
                    string tmpDirection = string.Empty;
                    string sLine = string.Empty;
                    string[] sField;

                    RLList.Clear();

                    while ((sLine = fReader.ReadLine()) != null)
                    {
                        sField = sLine.Split(',');
                        if (sField[0].ToLower() == "RoadwayID".ToLower())
                        {
                            ///LogTxtMsg(txtLog, "\t" + sLine);
                            continue;
                        }
                        if (sField.Length < 6)
                        {
                            continue;
                        }
                        clsRoadwayLink RL = new clsRoadwayLink();
                        RL.DSList = new List<int>();

                        CurrentSection = "\r\n\r\nRoadway Link record processing";

                        tmpRoadwayId = sField[0];
                        tmpLinkNo = int.Parse(sField[1]);
                        tmpBeginMM = double.Parse(sField[2]);
                        tmpEndMM = double.Parse(sField[3]);
                        tmpNoLanes = int.Parse(sField[4]);
                        tmpDirection = sField[5];
                        tmpNoDetectionStations = int.Parse(sField[6]);
                        RL.RoadwayID = tmpRoadwayId;
                        RL.Identifier = tmpLinkNo;
                        RL.BeginMM = tmpBeginMM;
                        RL.EndMM = tmpEndMM;
                        RL.NumberLanes = tmpNoLanes;
                        RL.NumberDetectionStations = tmpNoDetectionStations;
                        RL.Direction = clsEnums.GetDirIndexFromString(tmpDirection);

                        string tmpDetectionStations = string.Empty;

                        for (int i = 1; i <= tmpNoDetectionStations; i++)
                        {
                            tmpDetectionStationNo = int.Parse(sField[6 + i]);
                            tmpDetectionStations = tmpDetectionStations + tmpDetectionStationNo + ",";
                            RL.DSList.Add(tmpDetectionStationNo);
                        }
                        if (tmpDetectionStations.Length > 0)
                        {
                            tmpDetectionStations = tmpDetectionStations.Substring(0, tmpDetectionStations.Length - 1);
                        }
                        RL.DetectionStations = tmpDetectionStations;
                        RLList.Add(RL);
                        ///LogTxtMsg(txtLog, "\t" + tmpLinkNo + "," + tmpBeginMM + "," + tmpEndMM + "," + tmpNoLanes + "," + tmpNoDetectionStations + "," + tmpDetectionStations);
                    }
                }
                catch (Exception ex)
                {
                    retValue = "\tError in reading the Roadway Link configuration file. Current Section: " + CurrentSection + "\r\n\t" + ex.Message;
                    return retValue;
                }
            }
            else
            {
                retValue = "\tThe Roadway Link configuration file does not exist: " + ConfigFile;
                return retValue;
            }
            return retValue;
        }
        public static string ReadDetectorStationConfigFile(string ConfigFile, ref List<clsDetectorStation> DSList)
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            ///LogTxtMsg(txtLog, "\r\n\r\nReading the Detection Station configuration file.");
            if (File.Exists(ConfigFile))
            {
                try
                {
                    StreamReader fReader = null;

                    CurrentSection = "OpeningConfigFile";
                    fReader = File.OpenText(ConfigFile);
                    string tmpRoadwayId = "";
                    int tmpLinkNo = 0;
                    int tmpDetectionStationNo = 0;
                    string tmpDetectionStationName = string.Empty;
                    double tmpMMLocation = 0;
                    double tmpLatitude = 0;
                    double tmpLongitude = 0;
                    int tmpNoLanes = 0;
                    int tmpNoDetectionZones = 0;
                    clsEnums.enDirection tmpDirection = clsEnums.enDirection.NA;
                    //Modified by Hassan Charara on 11/14/2014 by converting the tmpDetectionZoneNo from int to string
                    //int tmpDetectionZoneNo = 0;
                    string tmpDetectionZoneNo = string.Empty;

                    string sLine = string.Empty;
                    string[] sField;

                    DSList.Clear();
                    while ((sLine = fReader.ReadLine()) != null)
                    {
                        sField = sLine.Split(',');
                        if (sField[0].ToLower() == "RoadwayId".ToLower())
                        {
                            ///LogTxtMsg(txtLog, "\t" + sLine);
                            continue;
                        }
                        if (sField.Length < 5)
                        {
                            continue;
                        }
                        clsDetectorStation DS = new clsDetectorStation();
                        //Modified by Hassan Charara on 11/14/2014 
                        //DS.DetectionZonesList = new List<int>();
                        DS.DetectionZonesList = new List<string>();

                        CurrentSection = "Detection Station record processing";

                        tmpRoadwayId = sField[0];
                        tmpLinkNo = int.Parse(sField[1]);
                        tmpDetectionStationNo = int.Parse(sField[2]);
                        tmpMMLocation = double.Parse(sField[3]);
                        tmpDetectionStationName = sField[4];
                        tmpDirection = clsEnums.GetDirIndexFromString(sField[5]);
                        tmpNoLanes = int.Parse(sField[6]);
                        tmpLatitude = double.Parse(sField[7]);
                        tmpLongitude = double.Parse(sField[8]);
                        tmpNoDetectionZones = int.Parse(sField[9]);

                        DS.RoadwayIdentifier = tmpRoadwayId;
                        DS.LinkIdentifier = tmpLinkNo;
                        DS.Identifier = tmpDetectionStationNo;
                        DS.MMLocation = tmpMMLocation;
                        DS.Name = tmpDetectionStationName.Trim();
                        DS.Direction = tmpDirection;
                        DS.NumberLanes = tmpNoLanes;
                        DS.Latitude = tmpLatitude;
                        DS.Longitude = tmpLongitude;
                        DS.NumberDetectionZones = tmpNoDetectionZones;

                        string tmpDetectionZones = string.Empty;

                        for (int i = 1; i <= tmpNoDetectionZones; i++)
                        {
                            //Modified by Hassan Charar on 11/14/2014
                            //tmpDetectionZoneNo = int.Parse(sField[3 + i]);
                            tmpDetectionZoneNo = sField[9 + i];
                            tmpDetectionZones = tmpDetectionZones + tmpDetectionZoneNo + ",";
                            DS.DetectionZonesList.Add(tmpDetectionZoneNo);
                        }
                        if (tmpDetectionZones.Length > 0)
                        {
                            tmpDetectionZones = tmpDetectionZones.Substring(0, tmpDetectionZones.Length - 1);
                        }
                        DS.DetectionZones = tmpDetectionZones;
                        DSList.Add(DS);

                        ///LogTxtMsg(txtLog, "\t" + tmpDetectionStationNo + "," + tmpMMLocation + "," + tmpNoDetectionZones + "," + tmpDetectionZones);
                    }

                }
                catch (Exception ex)
                {
                    retValue = "\tError in reading the Detection Station configuration file. Current Section: " + CurrentSection + "\r\n\t" + ex.Message;
                    return retValue;
                }
            }
            else
            {
                retValue = "\tThe Detection Station configuration file does not exist: " + ConfigFile;
                return retValue;
            }
            return retValue;
        }
        public static string ReadDetectionZonesConfigFile(string ConfigFile, ref List<clsDetectionZone> DZList)
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            ///LogTxtMsg(txtLog, "\r\n\r\nReading the Detection Zones configuration file.");

            if (File.Exists(ConfigFile))
            {
                try
                {
                    StreamReader fReader = null;

                    CurrentSection = "OpeningConfigFile";
                    fReader = File.OpenText(ConfigFile);
                    //int tmpLinkNo = 0;
                    //int tmpDSNo = 0;
                    int tmpDetectionStationNo = 0;
                    //Modified by Hassan Charara on 11/14/2014 to convert tmpDetectionZoneNo from int to string
                    //int tmpDetectionZoneNo = 0;
                    string tmpDetectionZoneNo = string.Empty;
                    int tmpLaneNo = 0;
                    string tmpLaneDesc = string.Empty;
                    string tmpLaneType = string.Empty;
                    string tmpDZType = string.Empty;
                    double tmpMMLocation = 0.0;
                    int tmpRoadwayId = 0;
                    clsEnums.enDirection tmpDirection = clsEnums.enDirection.NA;
                    string sLine = string.Empty;
                    string[] sField;

                    DZList.Clear();

                    while ((sLine = fReader.ReadLine()) != null)
                    {
                        sField = sLine.Split(',');
                        if (sField[0].ToLower() == "DetectorStationNo".ToLower())
                        {
                            ///LogTxtMsg(txtLog, "\t" + sLine);
                            continue;
                        }
                        if (sField.Length < 8)
                            continue;

                        clsDetectionZone DZ = new clsDetectionZone();

                        CurrentSection = "Detection Zone record processing";

                        tmpDetectionStationNo = int.Parse(sField[0]);
                        tmpDetectionZoneNo = sField[1];
                        tmpLaneNo = int.Parse(sField[2]);
                        tmpLaneDesc = sField[3];
                        tmpMMLocation = double.Parse(sField[4]);
                        tmpDirection = clsEnums.GetDirIndexFromString(sField[5]);
                        if (sField[6] != null)
                            tmpLaneType = sField[6].ToUpper();
                        if (sField[7] != null)
                            tmpDZType = sField[7].ToUpper();
                        if (sField[8] != null)
                            tmpRoadwayId = int.Parse(sField[8]);

                        DZ.DSIdentifier = tmpDetectionStationNo;
                        DZ.Identifier = tmpDetectionZoneNo;
                        DZ.LaneNo = tmpLaneNo;
                        DZ.LaneDesc = tmpLaneDesc;
                        DZ.MMLocation = tmpMMLocation;
                        DZ.Direction = tmpDirection;
                        DZ.LaneType = tmpLaneType;
                        DZ.Type = tmpDZType;
                        DZ.RoadwayIdentifier = tmpRoadwayId;
                        DZList.Add(DZ);

                        ///LogTxtMsg(txtLog, "\t" + tmpDetectionStationNo + "," + tmpDetectionZoneNo + "," + tmpLaneNo + "," + tmpLaneDesc + "," + tmpMMLocation);
                    }
                }
                catch (Exception ex)
                {
                    retValue = "\tError in reading the Detection Zone information. Current Section: " + CurrentSection + "\r\n\t" + ex.Message;
                    return retValue;
                }
            }
            else
            {
                retValue = "\tThe Detection Zone configuration file does not exist: " + ConfigFile;
                return retValue;
            }
            return retValue;
        }
        public static string ReadMileMarkerConfigFile(string ConfigFile, ref List<clsMileMarker> MMList)
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            ///LogTxtMsg(txtLog, "\r\n\r\nReading the Mile Marker configuration file.");

            if (File.Exists(ConfigFile))
            {
                try
                {
                    StreamReader fReader = null;

                    CurrentSection = "OpeningConfigFile";
                    fReader = File.OpenText(ConfigFile);
                    //int tmpLinkNo = 0;
                    //int tmpDSNo = 0;
                    string tmpRoadwayId = string.Empty;
                    double tmpMM = 0;
                    double tmpLat1 = 0;
                    double tmpLon1 = 0;
                    double tmpLat2 = 0;
                    double tmpLon2 = 0;
                    string Direction = string.Empty;
                    clsEnums.enDirection tmpDirection = clsEnums.enDirection.NA;
                    string sLine = string.Empty;
                    string[] sField;

                    MMList.Clear();

                    while ((sLine = fReader.ReadLine()) != null)
                    {
                        sField = sLine.Split(',');
                        if (sField[0].ToLower() == "RoadwayId".ToLower())
                        {
                            continue;
                        }
                        if (sField.Length < 6)
                            continue;

                        clsMileMarker MM = new clsMileMarker();

                        CurrentSection = "MileMarker record processing";
                        tmpRoadwayId = sField[0];
                        tmpMM = double.Parse(sField[1]);
                        tmpLat1 = double.Parse(sField[2]);
                        tmpLon1 = double.Parse(sField[3]);
                        tmpLat2 = double.Parse(sField[4]);
                        tmpLon2  = double.Parse(sField[5]);
                        //tmpDirection = clsEnums.GetDirIndexFromString(sField[5]);

                        MM.RoadwayId = tmpRoadwayId;
                        MM.MileMarker = tmpMM;
                        MM.Latitude1 = tmpLat1;
                        MM.Longitude1 = tmpLon1;
                        MM.Latitude2 = tmpLat2;
                        MM.Longitude2    = tmpLon2;
                        //MM.Direction = tmpDirection;
                        MMList.Add(MM);

                        ///LogTxtMsg(txtLog, "\t" + tmpDetectionStationNo + "," + tmpDetectionZoneNo + "," + tmpLaneNo + "," + tmpLaneDesc + "," + tmpMMLocation);
                    }
                }
                catch (Exception ex)
                {
                    retValue = "\tError in reading the Mile marker information. Current Section: " + CurrentSection + "\r\n\t" + ex.Message;
                    return retValue;
                }
            }
            else
            {
                retValue = "\tThe Mile Marker configuration file does not exist: " + ConfigFile;
                return retValue;
            }
            return retValue;
        }


        public static string LoadRoadwayInfoIntoINFLODatabase(clsRoadway Roadway, ref clsDatabase DB)
        {
            string retValue = string.Empty;
            string sqlStr = string.Empty;
            string sqlQuery = string.Empty;
            bool RoadwayAlreadyinDatabase = false;

            try
            {
                //Check if Roadway information already exists in database
                #region "Load Roadway Info"
                sqlQuery = "Select * from Configuration_Roadway where RoadwayId='" + Roadway.Identifier + "' and " +
                             "Name='" + Roadway.Name + "' and " +
                             "BeginMM=" + Roadway.BeginMM + " and " +
                             "EndMM=" + Roadway.EndMM + " and " +
                             "RecurringCongestionMMLocation=" + Roadway.RecurringCongestionMMLocation + " and " +
                             "Direction='" + Roadway.Direction + "' and " +
                             "MMIncreasingDirection='" + Roadway.MMIncreasingDirection + "'";

                //Check if Roadway information exists in INFLO database: ");
                DataSet RoadwayDataSet = new DataSet("RoadwayInfo");
                retValue = DB.FillDataSet(sqlQuery, ref RoadwayDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display roadway information if it already exists
                if (RoadwayDataSet.Tables[0].Rows.Count > 0)
                {
                    ////LogTxtMsg(txtLog, "\tExisting Roadway Information in INFLO database: ");
                    ////LogTxtMsg(txtLog, "\t\tRoadID, RoadName, BeginMM, EndMM, Dir, MMDiR");
                    foreach (DataRow row in RoadwayDataSet.Tables[0].Rows)
                    {
                        string RoadID = string.Empty;
                        string RoadName = string.Empty;
                        double BeginMM = 0;
                        double EndMM = 0;
                        string Dir = string.Empty;
                        string MMDiR = string.Empty;
                        double RecurringCongestionMMLocation = 0;

                        foreach (DataColumn col in RoadwayDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "roadwayid":
                                    RoadID = row[col].ToString();
                                    break;
                                case "name":
                                    RoadName = row[col].ToString();
                                    break;
                                case "beginmm":
                                    BeginMM = double.Parse(row[col].ToString());
                                    break;
                                case "endmm":
                                    EndMM = double.Parse(row[col].ToString());
                                    break;
                                case "direction":
                                    Dir = row[col].ToString();
                                    break;
                                case "mmincreasingdirection":
                                    MMDiR = row[col].ToString();
                                    break;
                                case "recurringcongestionmmlocation":
                                    RecurringCongestionMMLocation = double.Parse(row[col].ToString());
                                    break;
                            }
                        }
                        if (Roadway.Identifier.ToString().ToUpper() == RoadID.ToUpper())
                        {
                            RoadwayAlreadyinDatabase = true;
                            //retValue = "Roadway is already in the database";
                            //return retValue;
                        }
                        ////LogTxtMsg(txtLog, "\t\t" + RoadID + ", " + RoadName + ", " + BeginMM + ", " + EndMM + ", " + Dir + ", " + MMDiR);
                    }
                }

                ////LogTxtMsg(txtLog, "\tAdd new Roadway to INFLO database: ");
                if ((RoadwayDataSet.Tables[0].Rows.Count == 0) || (RoadwayAlreadyinDatabase == false))
                {
                    //Update the RoadwayTable with Roadway information
                    sqlQuery = "INSERT INTO Configuration_Roadway(RoadwayId, Name, BeginMM, EndMM, RecurringCongestionMMLocation, Direction, MMIncreasingDirection) " +
                             "Values('" + Roadway.Identifier + "', '" + Roadway.Name + "', " + Roadway.BeginMM + ", " + Roadway.EndMM + ", " + Roadway.RecurringCongestionMMLocation + ", '" + Roadway.Direction + "', '" + Roadway.MMIncreasingDirection + "')";
                    retValue = string.Empty;
                    retValue = DB.InsertRow(sqlQuery);
                    ////LogTxtMsg(txtLog, "\t\tNew Roadway: " + Roadway.Identifier + ", " + Roadway.Name + ", " + Roadway.BeginMM + ", " + Roadway.EndMM + ", " + Roadway.Direction + ", " +
                    ////                                                   Roadway.MMIncreasingDirection + " added to database");
                }
                #endregion
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Roadway information into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
            return retValue;
        }
        public static string LoadRoadwaySubLinkInfoIntoINFLODatabase(clsRoadway Roadway, ref clsDatabase DB)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;

            try
            {
                //Check if Roadway sub-link information already exists in database
                #region "Load Roadway sub-link Info"
                List<clsRoadwaySubLink> ExistingRSLList = new List<clsRoadwaySubLink>();

                sqlQuery = "Select * from Configuration_RoadwaySubLinks where RoadwayId='" + Roadway.Identifier + "'";

                //Check if Roadway information exists in INFLO database
                DataSet SubLinkDataSet = new DataSet("SubLinkInfo");
                retValue = DB.FillDataSet(sqlQuery, ref SubLinkDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                if (SubLinkDataSet.Tables[0].Rows.Count > 0)
                {
                    ////LogTxtMsg(txtLog, "\tExisting Roadway Sub-Links in INFLO database: ");
                    ////LogTxtMsg(txtLog, "\t\tRoadID, RoadLinkID, BeginMM, EndMM");
                    foreach (DataRow row in SubLinkDataSet.Tables[0].Rows)
                    {
                        clsRoadwaySubLink tmpRSL = new clsRoadwaySubLink();
                        string RoadID = string.Empty;
                        string RoadLinkID = string.Empty;
                        double BeginMM = 0;
                        double EndMM = 0;
                        string Dir = string.Empty;
                        foreach (DataColumn col in SubLinkDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "roadwayid":
                                    RoadID = row[col].ToString();
                                    tmpRSL.RoadwayID = RoadID;
                                    break;
                                case "sublinkid":
                                    RoadLinkID = row[col].ToString();
                                    tmpRSL.Identifier = int.Parse(RoadLinkID);
                                    break;
                                case "beginmm":
                                    BeginMM = double.Parse(row[col].ToString());
                                    tmpRSL.BeginMM = BeginMM;
                                    break;
                                case "endmm":
                                    EndMM = double.Parse(row[col].ToString());
                                    tmpRSL.EndMM = EndMM;
                                    break;
                            }
                        }
                        ////LogTxtMsg(txtLog, "\t\t" + RoadID + ", " + RoadLinkID + ", " + BeginMM + ", " + EndMM);
                        ExistingRSLList.Add(tmpRSL);
                    }
                }

                //Update the Configuration_Roadway SubLinkInformation table with Roadway Sub-link info
                retValue = string.Empty;
                bool FoundRSL = false;
                double SLStartMM = Roadway.BeginMM;
                int SLID = 1;

                ////LogTxtMsg(txtLog, "\tAdd new Roadway Sub Links to INFLO database: ");
                if (Roadway.MMIncreasingDirection == Roadway.Direction)
                {
                    double SLEndMM = SLStartMM + 0.1;
                    while (SLEndMM <= Roadway.EndMM)
                    {
                        FoundRSL = false;

                        foreach (clsRoadwaySubLink RSL in ExistingRSLList)
                        {
                            if ((RSL.RoadwayID == Roadway.Identifier) && (RSL.Identifier == SLID) && (RSL.BeginMM.ToString("0.0") == SLStartMM.ToString("0.0")) && (RSL.EndMM.ToString("0.0") == SLEndMM.ToString("0.0")))
                            {
                                FoundRSL = true;
                                ////LogTxtMsg(txtLog, "\t\tNew Sub-Link: " + RSL.RoadwayID + ", " + RSL.Identifier + ", " + RSL.BeginMM.ToString("0.0") + ", " + RSL.EndMM.ToString("0.0") + " already in database");
                                break;
                            }
                        }
                        if (FoundRSL == false)
                        {
                            sqlQuery = "INSERT INTO Configuration_RoadwaySubLinks(RoadwayID, SubLinkId, Direction, BeginMM, EndMM) " +
                                        "Values('" + Roadway.Identifier + "', '" + SLID + "', '" + Roadway.Direction + "', " + SLStartMM.ToString("0.0") + ", " + SLEndMM.ToString("0.0") + ")";
                            retValue = DB.InsertRow(sqlQuery);
                            if (retValue.Length > 0)
                            {
                                ////LogTxtMsg(txtLog, retValue);
                                return retValue;
                            }
                            ////LogTxtMsg(txtLog, "\t\tNew Sub-Link: " + Roadway.Identifier + ", " + SLID + ", " + SLStartMM.ToString("0.0") + ", " + SLEndMM.ToString("0.0") + " added to database");
                        }
                        SLStartMM = SLStartMM + 0.1;
                        SLEndMM = SLStartMM + 0.1;
                        SLID = SLID + 1;
                    }
                }
                else if (Roadway.MMIncreasingDirection != Roadway.Direction)
                {
                    double SLEndMM = SLStartMM - 0.1;
                    while (SLEndMM >= Roadway.EndMM)
                    {
                        FoundRSL = false;

                        foreach (clsRoadwaySubLink RSL in ExistingRSLList)
                        {
                            if ((RSL.RoadwayID == Roadway.Identifier) && (RSL.Identifier == SLID) && (RSL.BeginMM.ToString("0.0") == SLStartMM.ToString("0.0")) && (RSL.EndMM.ToString("0.0") == SLEndMM.ToString("0.0")))
                            {
                                FoundRSL = true;
                                ////LogTxtMsg(txtLog, "\t\tNew Sub-Link: " + RSL.RoadwayID + ", " + RSL.Identifier + ", " + RSL.BeginMM.ToString("0.0") + ", " + RSL.EndMM.ToString("0.0") + " already in database");
                                break;
                            }
                        }
                        if (FoundRSL == false)
                        {
                            sqlQuery = "INSERT INTO Configuration_RoadwaySubLinks(RoadwayID, SubLinkId, Direction, BeginMM, EndMM) " +
                                        "Values('" + Roadway.Identifier + "', '" + SLID + "', '" + Roadway.Direction + "', " + SLStartMM.ToString("0.0") + ", " + SLEndMM.ToString("0.0") + ")";
                            retValue = DB.InsertRow(sqlQuery);
                            if (retValue.Length > 0)
                            {
                                ////LogTxtMsg(txtLog, retValue);
                                return retValue;
                            }
                            ////LogTxtMsg(txtLog, "\t\tNew Sub-Link: " + Roadway.Identifier + ", " + SLID + ", " + SLStartMM.ToString("0.0") + ", " + SLEndMM.ToString("0.0") + " added to database");
                        }
                        SLStartMM = SLStartMM - 0.1;
                        SLEndMM = SLStartMM - 0.1;
                        SLID = SLID + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Roadway Sub-Link information into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
                #endregion
            return retValue;
        }
        public static string LoadRoadwayLinkInfoIntoINFLODatabase(List<clsRoadwayLink> RLList, ref clsDatabase DB)
        {
            string retValue = string.Empty;
            string sqlQuery = string.Empty;

            try
            {
                //Check Roadwaylink inforamtion from inflodatabase
                #region "Load Roadway Link information"
                List<clsRoadwayLink> ExistingRLList = new List<clsRoadwayLink>();

                sqlQuery = "Select * from Configuration_RoadwayLinks";

                //Check if Roadway Link information exists in database:
                DataSet RoadLinkInfoDataSet = new DataSet("RoadwayLinkInfo");
                retValue = DB.FillDataSet(sqlQuery, ref RoadLinkInfoDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display roadway link information if it already exists
                if (RoadLinkInfoDataSet.Tables[0].Rows.Count > 0)
                {
                    ////LogTxtMsg(txtLog, "\tExisting Roadway Links in INFLO database: ");
                    ////LogTxtMsg(txtLog, "\t\t RoadID,  RoadLinkID, BeginMM, EndMM, NumLanes, NumDetectorStations, DetectorStations");
                    foreach (DataRow row in RoadLinkInfoDataSet.Tables[0].Rows)
                    {
                        clsRoadwayLink tmpRLL = new clsRoadwayLink();
                        string RoadID = string.Empty;
                        string RoadLinkID = string.Empty;
                        double BeginMM = 0;
                        double EndMM = 0;
                        string Dir = string.Empty;
                        int NumLanes = 0;
                        int NumDetectorStations = 0;
                        string DetectorStations = string.Empty;
                        string Direction = string.Empty;
                        foreach (DataColumn col in RoadLinkInfoDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "roadwayid":
                                    RoadID = row[col].ToString();
                                    tmpRLL.RoadwayID = RoadID;
                                    break;
                                case "linkid":
                                    RoadLinkID = row[col].ToString();
                                    tmpRLL.Identifier = int.Parse(RoadLinkID);
                                    break;
                                case "beginmm":
                                    BeginMM = double.Parse(row[col].ToString());
                                    tmpRLL.BeginMM = BeginMM;
                                    break;
                                case "endmm":
                                    EndMM = double.Parse(row[col].ToString());
                                    tmpRLL.EndMM = EndMM;
                                    break;
                                case "numberlanes":
                                    NumLanes = int.Parse(row[col].ToString());
                                    tmpRLL.NumberLanes = NumLanes;
                                    break;
                                case "numberdetectorstations":
                                    NumDetectorStations = int.Parse(row[col].ToString());
                                    tmpRLL.NumberDetectionStations = NumDetectorStations;
                                    break;
                                case "detectorstations":
                                    DetectorStations = row[col].ToString();
                                    tmpRLL.DetectionStations = DetectorStations;
                                    break;
                                case "direction":
                                    Direction = row[col].ToString();
                                    tmpRLL.Direction = clsEnums.GetDirIndexFromString(Direction);   
                                    break;
                            }
                        }
                        ////LogTxtMsg(txtLog, "\t\t" + RoadID + ", " + RoadLinkID + ", " + BeginMM + ", " + EndMM + ", " + NumLanes + ", " + NumDetectorStations + ", " + DetectorStations);
                        ExistingRLList.Add(tmpRLL);
                    }
                }

                //Update the Configuration_RoadwayLinkInformation table with Roadway link info
                retValue = string.Empty;
                bool FoundRL = false;
                ////LogTxtMsg(txtLog, "\tAdd new Roadway Links to INFLO database: ");
                foreach (clsRoadwayLink tmpRL in RLList)
                {
                    FoundRL = false;
                    foreach (clsRoadwayLink RL in ExistingRLList)
                    {
                        if ((RL.RoadwayID == tmpRL.RoadwayID) && (RL.Identifier == tmpRL.Identifier) && (RL.BeginMM == tmpRL.BeginMM) &&
                            (RL.EndMM == tmpRL.EndMM) && (RL.NumberLanes == tmpRL.NumberLanes) && (RL.NumberDetectionStations == tmpRL.NumberDetectionStations))
                        {
                            FoundRL = true;
                            ////LogTxtMsg(txtLog, "\t\tNew Link: " + tmpRL.RoadwayID + ", " + tmpRL.Identifier + ", " + tmpRL.BeginMM + ", " + tmpRL.EndMM + ", " +
                            ////                                                                tmpRL.NumberLanes + ", " + tmpRL.NumberDetectionStations + ", " + tmpRL.DetectionStations + " already in database");
                            break;
                        }
                    }
                    if (FoundRL == false)
                    {
                        sqlQuery = "INSERT INTO Configuration_RoadwayLinks(RoadwayID, LinkId, Direction, BeginMM, EndMM, NumberLanes, NumberDetectorStations, DetectorStations) " +
                                    "Values('" + tmpRL.RoadwayID + "', '" + tmpRL.Identifier + "', '" + tmpRL.Direction + "', " + tmpRL.BeginMM + ", " + tmpRL.EndMM + ", " +
                                                    tmpRL.NumberLanes + ", " + tmpRL.NumberDetectionStations + ", '" + tmpRL.DetectionStations + "')";
                        retValue = DB.InsertRow(sqlQuery);
                        if (retValue.Length > 0)
                        {
                            ////LogTxtMsg(txtLog, retValue);
                            return retValue;
                        }
                        ////LogTxtMsg(txtLog, "\t\tNew Link: " + tmpRL.RoadwayID + ", " + tmpRL.Identifier + ", " + tmpRL.BeginMM + ", " + tmpRL.EndMM + ", " +
                        ////                                                                tmpRL.NumberLanes + ", " + tmpRL.NumberDetectionStations + ", " + tmpRL.DetectionStations + " added to database");
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Roadway Link information into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
                #endregion

            return retValue;
        }
        public static string LoadDetectorStationInfoIntoINFLODatabase(List<clsDetectorStation> DSList, ref clsDatabase DB)
        {
            string sqlQuery = string.Empty;
            string retValue = string.Empty;

            try
            {
                //Check Detector Station inforamtion from inflo database
                #region "Load Detector Station Information"
                List<clsDetectorStation> ExistingDSList = new List<clsDetectorStation>();

                sqlQuery = "Select * from Configuration_TSSDetectorStation";

                //Check if Detection Station information exists in INFLO database
                DataSet DSInfoDataSet = new DataSet("DSInfo");
                retValue = DB.FillDataSet(sqlQuery, ref DSInfoDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display existing detector station information already in database
                if (DSInfoDataSet.Tables[0].Rows.Count > 0)
                {
                    ////LogTxtMsg(txtLog, "\tExisting Detection Stations in INFLO database: ");
                    ////LogTxtMsg(txtLog, "\t\t RoadLinkID, DSID, MMLocation, NumDetectionZones, DetectionZones");
                    foreach (DataRow row in DSInfoDataSet.Tables[0].Rows)
                    {
                        clsDetectorStation tmpDS = new clsDetectorStation();
                        string RoadwayId = string.Empty;
                        string RoadLinkID = string.Empty;
                        string DSID = string.Empty;
                        double MMLocation = 0;
                        double Latitude = 0;
                        double Longitude = 0;
                        string DSName = string.Empty;
                        clsEnums.enDirection Direction = clsEnums.enDirection.NA;
                        int NumberLanes = 0;
                        int NumDetectionZones = 0;
                        string DetectionZones = string.Empty;
                        foreach (DataColumn col in DSInfoDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "roadwayid":
                                    RoadwayId = row[col].ToString();
                                    tmpDS.RoadwayIdentifier = RoadLinkID;
                                    break;
                                case "linkid":
                                    RoadLinkID = row[col].ToString();
                                    tmpDS.LinkIdentifier = int.Parse(RoadLinkID);
                                    break;
                                case "dsid":
                                    DSID = row[col].ToString();
                                    tmpDS.Identifier = int.Parse(DSID);
                                    break;
                                case "mmlocation":
                                    MMLocation = double.Parse(row[col].ToString());
                                    tmpDS.MMLocation = MMLocation;
                                    break;
                                case "latitude":
                                    Latitude = double.Parse(row[col].ToString());
                                    tmpDS.Latitude = MMLocation;
                                    break;
                                case "longitude":
                                    Longitude = double.Parse(row[col].ToString());
                                    tmpDS.Longitude = MMLocation;
                                    break;
                                case "dsname":
                                    DSName = row[col].ToString().Trim();
                                    tmpDS.Name = DSName;
                                    break;
                                case "numberlanes":
                                    NumberLanes = int.Parse(row[col].ToString());
                                    tmpDS.NumberLanes = NumberLanes;
                                    break;
                                case "direction":
                                    Direction = clsEnums.GetDirIndexFromString(row[col].ToString());
                                    tmpDS.Direction = Direction;
                                    break;
                                case "numberdetectionzones":
                                    NumDetectionZones = int.Parse(row[col].ToString());
                                    tmpDS.NumberDetectionZones = NumDetectionZones;
                                    break;
                                case "detectionzones":
                                    DetectionZones = row[col].ToString();
                                    tmpDS.DetectionZones = DetectionZones;
                                    break;
                            }
                        }
                        ////LogTxtMsg(txtLog, "\t\t" + RoadLinkID + ", " + DSID + ", " + MMLocation + ", " + NumDetectionZones + ", " + DetectionZones);
                        ExistingDSList.Add(tmpDS);
                    }
                }

                //Update the Configuration_DetectionStation table with Detection station info
                retValue = string.Empty;
                bool FoundDS = false;
                ////LogTxtMsg(txtLog, "\tAdd new Detector Stations to INFLO database: ");
                foreach (clsDetectorStation tmpDS in DSList)
                {
                    FoundDS = false;
                    foreach (clsDetectorStation DS in ExistingDSList)
                    {
                        if ((DS.RoadwayIdentifier == tmpDS.RoadwayIdentifier) && (DS.LinkIdentifier == tmpDS.LinkIdentifier) && (DS.Identifier == tmpDS.Identifier) && (DS.MMLocation == tmpDS.MMLocation) &&
                            (DS.Name == tmpDS.Name) && (DS.Direction == tmpDS.Direction) && (DS.NumberLanes == tmpDS.NumberLanes) && (DS.Latitude == tmpDS.Latitude) && (DS.Longitude == tmpDS.Longitude) && 
                            (DS.NumberDetectionZones == tmpDS.NumberDetectionZones) && (DS.DetectionZones == tmpDS.DetectionZones))
                        {
                            FoundDS = true;
                            ////LogTxtMsg(txtLog, "\t\tNew Detection Station: " + tmpDS.LinkIdentifier + ", " + tmpDS.Identifier + ", " + tmpDS.MMLocation + ", " +
                            ////                                                 tmpDS.NumberDetectionZones + ", " + tmpDS.DetectionZones + "  already in database");
                            break;
                        }
                    }
                    if (FoundDS == false)
                    {
                        sqlQuery = "INSERT INTO Configuration_TSSDetectorStation(LinkId, DSId, MMLocation, NumberDetectionZones, DetectionZones, RoadwayId, DSName, Direction, NumberLanes, Latitude, Longitude) " +
                                "Values('" + tmpDS.LinkIdentifier + "', '" + tmpDS.Identifier + "', " + tmpDS.MMLocation + ", " + tmpDS.NumberDetectionZones + ", '" + tmpDS.DetectionZones + "', " + 
                                             tmpDS.RoadwayIdentifier + ", '" + tmpDS.Name + "', '" + tmpDS.Direction + "', " + tmpDS.NumberLanes + 
                                             ", " + tmpDS.Latitude + ", " + tmpDS.Longitude + ")";
                        retValue = DB.InsertRow(sqlQuery);
                        if (retValue.Length > 0)
                        {
                            ////LogTxtMsg(txtLog, retValue);
                            return retValue;
                        }
                        ////LogTxtMsg(txtLog, "\t\tNew Detection Station: " + tmpDS.LinkIdentifier + ", " + tmpDS.Identifier + ", " + tmpDS.MMLocation + ", " +
                        ////                                                     tmpDS.NumberDetectionZones + ", " + tmpDS.DetectionZones + " added to database");
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Detector Station into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
                #endregion

            return retValue;
        }
        public static string LoadDetectionZoneInfoIntoINFLODatabase(List<clsDetectionZone> DZList, ref clsDatabase DB)
        {
            string sqlQuery = string.Empty;
            string retValue = string.Empty;

            try
            {
                //Update the Configuration_DetectionZone table with Detection station info
                #region "Load Detection Zone Information"
                List<clsDetectionZone> ExistingDZList = new List<clsDetectionZone>();

                sqlQuery = "Select * from Configuration_TSSDetectionZone";

                retValue = string.Empty;
                //Check if Detection Zone information exists in INFLO database
                DataSet DZInfoDataSet = new DataSet("DZInfo");
                retValue = DB.FillDataSet(sqlQuery, ref DZInfoDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display existing detector zone information 
                
                if (DZInfoDataSet.Tables[0].Rows.Count > 0)
                {
                    ////LogTxtMsg(txtLog, "\tExisting Detection Zone in INFLO database: ");
                    ////LogTxtMsg(txtLog, "\t\t DSID, DZID, LaneNo, LaneDescription");
                    foreach (DataRow row in DZInfoDataSet.Tables[0].Rows)
                    {
                        clsDetectionZone tmpDZ = new clsDetectionZone();
                        string DZID = string.Empty;
                        string DSID = string.Empty;
                        string DZType = string.Empty;
                        string DataType = string.Empty;
                        int LaneNo = 0;
                        string LaneType = string.Empty;
                        string LaneDesc = string.Empty;
                        string RoadwayId = string.Empty;
                        clsEnums.enDirection Direction = clsEnums.enDirection.NA;

                        foreach (DataColumn col in DZInfoDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "dsid":
                                    DSID = row[col].ToString();
                                    tmpDZ.DSIdentifier = int.Parse(DSID);
                                    break;
                                case "dzid":
                                    DZID = row[col].ToString();
                                    tmpDZ.Identifier = DZID;
                                    break;
                                case "roadwayid":
                                    RoadwayId = row[col].ToString();
                                    tmpDZ.RoadwayIdentifier = int.Parse(RoadwayId);
                                    break;
                                case "dztype":
                                    DZType = row[col].ToString();
                                    tmpDZ.Type = DZType;
                                    break;
                                case "datatype":
                                    DataType = row[col].ToString();
                                    tmpDZ.DataType = DataType;
                                    break;
                                case "lanenumber":
                                    LaneNo = int.Parse(row[col].ToString());
                                    tmpDZ.LaneNo = LaneNo;
                                    break;
                                case "lanetype":
                                    LaneType = row[col].ToString();
                                    tmpDZ.LaneType = LaneType;
                                    break;
                                case "lanedescription":
                                    LaneDesc = row[col].ToString();
                                    tmpDZ.LaneDesc = LaneDesc;
                                    break;
                                case "direction":
                                    Direction = clsEnums.GetDirIndexFromString(row[col].ToString());
                                    tmpDZ.Direction = Direction;
                                    break;
                            }
                        }
                        ////LogTxtMsg(txtLog, "\t\t" + DSID + ", " + DZID + ", " + LaneNo + ", " + LaneDesc);
                        ExistingDZList.Add(tmpDZ);
                    }
                }

                //Update the Configuration_DetectionZone table with Detection zone info
                retValue = string.Empty;
                bool FoundDZ = false;
                ////LogTxtMsg(txtLog, "\tAdd new Detection Zones to INFLO database: ");
                foreach (clsDetectionZone tmpDZ in DZList)
                {
                    FoundDZ = false;
                    foreach (clsDetectionZone DZ in ExistingDZList)
                    {
                        if ((DZ.DSIdentifier == tmpDZ.DSIdentifier) && (DZ.Identifier == tmpDZ.Identifier) && (DZ.RoadwayIdentifier == tmpDZ.RoadwayIdentifier) && (DZ.LaneNo == tmpDZ.LaneNo) &&
                            (DZ.LaneDesc == tmpDZ.LaneDesc) && (DZ.LaneType.ToUpper() == tmpDZ.LaneType.ToUpper()) && (DZ.Type.ToUpper() == tmpDZ.Type.ToUpper()))
                        {
                            FoundDZ = true;
                            ////LogTxtMsg(txtLog, "\t\tNew Detection Zone: " + tmpDZ.DSIdentifier + ", " + tmpDZ.Identifier + ", " + tmpDZ.LaneNo + ", " + tmpDZ.LaneDesc + " already in database");
                            break;
                        }
                    }
                    if (FoundDZ == false)
                    {
                        sqlQuery = "INSERT INTO Configuration_TSSDetectionZone(DSId, DZId, DZType, DataType, LaneNumber, LaneType, LaneDescription, Direction, RoadwayId) " +
                                     "Values('" + tmpDZ.DSIdentifier + "', '" + tmpDZ.Identifier + "', '" + tmpDZ.Type.ToUpper() + "', '" + tmpDZ.DataType + "', " + tmpDZ.LaneNo + ", '" + tmpDZ.LaneType.ToUpper() + "', '" +
                                                  tmpDZ.LaneDesc + "', '" + tmpDZ.Direction + "', '" + tmpDZ.RoadwayIdentifier + "')";
                        retValue = DB.InsertRow(sqlQuery);
                        if (retValue.Length > 0)
                        {
                            ////LogTxtMsg(txtLog, retValue);
                            return retValue;
                        }
                        ////LogTxtMsg(txtLog, "\t\tNew Detection Zone: " + tmpDZ.DSIdentifier + ", " + tmpDZ.Identifier + ", " + tmpDZ.LaneNo + ", " + tmpDZ.LaneDesc + " added to database");
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Detection Zone information into the INFLO database. \r\n\t" + ex.Message + ex.StackTrace;
                return retValue;
            }
                #endregion
            return retValue;
        }
        public static string LoadMileMarkerInfoIntoINFLODatabase(List<clsMileMarker> MMList, ref clsDatabase DB)
        {
            string sqlQuery = string.Empty;
            string retValue = string.Empty;

            try
            {
                //Update the Configuration_RoadwayMileMarkers table with Mile Marker info
                #region "Load MileMarker Information"
                List<clsMileMarker> ExistingMMList = new List<clsMileMarker>();

                sqlQuery = "Select * from Configuration_RoadwayMileMarkers";

                retValue = string.Empty;
                //Check if Mile Marker information exists in INFLO database
                DataSet MMInfoDataSet = new DataSet("MMInfo");
                retValue = DB.FillDataSet(sqlQuery, ref MMInfoDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display existing Mile Marker information 
                if (MMInfoDataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in MMInfoDataSet.Tables[0].Rows)
                    {
                        clsMileMarker tmpMM = new clsMileMarker();
                        string RoadwayId = string.Empty;
                        double MM = 0;
                        double Lat1 = 0;
                        double Lon1 = 0;
                        double Lat2 = 0;
                        double Lon2 = 0;
                        clsEnums.enDirection Direction = clsEnums.enDirection.NA;

                        foreach (DataColumn col in MMInfoDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName.ToString().ToLower())
                            {
                                case "roadwayid":
                                    RoadwayId = row[col].ToString();
                                    tmpMM.RoadwayId = RoadwayId;
                                    break;
                                case "mmnumber":
                                    MM = double.Parse(row[col].ToString());
                                    tmpMM.MileMarker = MM;
                                    break;
                                case "latitude1":
                                    Lat1 = double.Parse(row[col].ToString());
                                    tmpMM.Latitude1 = Lat1;
                                    break;
                                case "longitude1":
                                    Lon1 = double.Parse(row[col].ToString());
                                    tmpMM.Longitude1 = Lon1;
                                    break;
                                case "latitude2":
                                    Lat2 = double.Parse(row[col].ToString());
                                    tmpMM.Latitude2 = Lat2;
                                    break;
                                case "longitude2":
                                    Lon2 = double.Parse(row[col].ToString());
                                    tmpMM.Longitude2 = Lon2;
                                    break;
                                case "direction":
                                    Direction = clsEnums.GetDirIndexFromString(row[col].ToString());
                                    tmpMM.Direction = Direction;
                                    break;
                            }
                        }
                        ExistingMMList.Add(tmpMM);
                    }
                }

                //Update the Configuration_RoadwayMileMarkers table with Mile Marker info
                retValue = string.Empty;
                bool FoundMM = false;
                foreach (clsMileMarker tmpMM in MMList)
                {
                    FoundMM = false;
                    foreach (clsMileMarker MM in ExistingMMList)
                    {
                        if ((MM.RoadwayId == tmpMM.RoadwayId) && (MM.MileMarker == tmpMM.MileMarker) &&
                            (MM.Latitude1 == tmpMM.Latitude1) && (MM.Longitude1 == tmpMM.Longitude1) &&
                            (MM.Latitude2 == tmpMM.Latitude2) && (MM.Longitude2 == tmpMM.Longitude2))
                        {
                            FoundMM = true;
                            break;
                        }
                    }
                    if (FoundMM == false)
                    {
                        sqlQuery = "INSERT INTO Configuration_RoadwayMileMarkers(RoadwayId, MMNumber, Latitude1, Longitude1, Latitude2, Longitude2) " +
                                     "Values('" + tmpMM.RoadwayId + "', " + tmpMM.MileMarker + ", " + tmpMM.Latitude1 + ", " + tmpMM.Longitude1 + ", " + 
                                                  tmpMM.Latitude2 + ", " + tmpMM.Longitude2 + ")";
                        retValue = DB.InsertRow(sqlQuery);
                        if (retValue.Length > 0)
                        {
                            return retValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Mile Marker information into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
                #endregion
            return retValue;
        }
        public static string LoadThresholdsIntoINFLODatabase(ref clsDatabase DB)
        {
            string sqlQuery = string.Empty;
            string retValue = string.Empty;

            try
            {
                //Update the Configuration_DetectionZone table with Detection station info
                #region "Load Detection Zone Information"

                sqlQuery = "Delete  from Configuration_INFLOThresholds";

                retValue = string.Empty;
                //Delete the thresholds and parameters already in the INFLO database
                DataSet ThresholdsDataSet = new DataSet("Thresholds");
                retValue = DB.FillDataSet(sqlQuery, ref ThresholdsDataSet);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }

                //display existing detector zone information
                if (ThresholdsDataSet.Tables.Count > 0)
                {
                    if (ThresholdsDataSet.Tables[0].Rows.Count > 0)
                    {
                        ////LogTxtMsg(txtLog, "\tExisting Thresholds in INFLO database: ");
                        foreach (DataRow row in ThresholdsDataSet.Tables[0].Rows)
                        {
                            foreach (DataColumn col in ThresholdsDataSet.Tables[0].Columns)
                            {
                                switch (col.ColumnName.ToString().ToLower())
                                {
                                    case "CVDataPollingFrequency":
                                        ////LogTxtMsg(txtLog, "CVDataPollingFrequency: " + row[col].ToString());
                                        break;
                                    case "TSSDataPollingFrequency":
                                        ////LogTxtMsg(txtLog, "TSSDataPollingFrequency: " + row[col].ToString());
                                        break;
                                    case "ESSDataPollingFrequency":
                                        ////LogTxtMsg(txtLog, "ESSDataPollingFrequency: " + row[col].ToString());
                                        break;
                                    case "MobileESSDataPollingFrequency":
                                        ////LogTxtMsg(txtLog, "MobileESSDataPollingFrequency: " + row[col].ToString());
                                        break;
                                    case "LinkCongestionSpeedThreshold":
                                        ////LogTxtMsg(txtLog, "LinkCongestionSpeedThreshold: " + row[col].ToString());
                                        break;
                                    case "SubLinkPercentQueuedVehicles":
                                        ////LogTxtMsg(txtLog, "SubLinkPercentQueuedVehicles: " + row[col].ToString());
                                        break;
                                    case "MinimumDisplaySpeed":
                                        ////LogTxtMsg(txtLog, "MinimumDisplaySpeed: " + row[col].ToString());
                                        break;
                                    case "TroupeRange":
                                        ////LogTxtMsg(txtLog, "TroupeRange: " + row[col].ToString());
                                        break;
                                    case "ThreeGantrySpeed":
                                        ////LogTxtMsg(txtLog, "ThreeGantrySpeed: " + row[col].ToString());
                                        break;
                                    case "CVDataLoadingFrequency":
                                        ////LogTxtMsg(txtLog, "CVDataLoadingFrequency: " + row[col].ToString());
                                        break;
                                    case "TSSDataLoadingFrequency":
                                        ////LogTxtMsg(txtLog, "TSSDataLoadingFrequency: " + row[col].ToString());
                                        break;
                                    case "SublinkLength":
                                        ////LogTxtMsg(txtLog, "SublinkLength: " + row[col].ToString());
                                        break;
                                    case "SublinkPercentQueuedLength":
                                        ////LogTxtMsg(txtLog, "SublinkPercentQueuedLength: " + row[col].ToString());
                                        break;
                                    case "MaximumDisplaySpeed":
                                        ////LogTxtMsg(txtLog, "MaximumDisplaySpeed: " + row[col].ToString());
                                        break;
                                    case "COFLowerThreshold":
                                        ////LogTxtMsg(txtLog, "COFLowerThreshold: " + row[col].ToString());
                                        break;
                                    case "COFUpperThreshold":
                                        ////LogTxtMsg(txtLog, "COFUpperThreshold: " + row[col].ToString());
                                        break;
                                    case "RoadSurfaceStatusDryCOF":
                                        ////LogTxtMsg(txtLog, "RoadSurfaceStatusDryCOF: " + row[col].ToString());
                                        break;
                                    case "RoadSurfaceStatusSnowCOF":
                                        ////LogTxtMsg(txtLog, "RoadSurfaceStatusSnowCOF: " + row[col].ToString());
                                        break;
                                    case "RoadSurfaceStatusWetCOF":
                                        ////LogTxtMsg(txtLog, "RoadSurfaceStatusWetCOF: " + row[col].ToString());
                                        break;
                                    case "RoadSurfaceStatusIceCOF":
                                        ////LogTxtMsg(txtLog, "RoadSurfaceStatusIceCOF: " + row[col].ToString());
                                        break;
                                    case "WRTMMaxRecommendedSpeedLevel1":
                                        ////LogTxtMsg(txtLog, "WRTMMaxRecommendedSpeedLevel1: " + row[col].ToString());
                                        break;
                                    case "WRTMMaxRecommendedSpeedLevel2":
                                        ////LogTxtMsg(txtLog, "WRTMMaxRecommendedSpeedLevel2: " + row[col].ToString());
                                        break;
                                    case "WRTMMaxRecommendedSpeedLevel3":
                                        ////LogTxtMsg(txtLog, "WRTMMaxRecommendedSpeedLevel3: " + row[col].ToString());
                                        break;
                                    case "WRTMMaxRecommendedSpeedLevel4":
                                        ////LogTxtMsg(txtLog, "WRTMMaxRecommendedSpeedLevel4: " + row[col].ToString());
                                        break;
                                    case "WRTMMinRecommendedSpeed":
                                        ////LogTxtMsg(txtLog, "WRTMMinRecommendedSpeed: " + row[col].ToString());
                                        break;
                                    case "VisibilityThreshold":
                                        ////LogTxtMsg(txtLog, "VisibilityThreshold: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusClear":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusClear: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusFogNotPatchy":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusFogNotPatchy: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusPatchyFog":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusPatchyFog: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusBlowingSnow":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusBlowingSnow: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusSmoke":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusSmoke: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusSeaSpray":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusSeaSpray: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusVehicleSpray":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusVehicleSpray: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusBlowingDust":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusBlowingDust: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusBlowingSand":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusBlowingSand: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusSunGlare":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusSunGlare: " + row[col].ToString());
                                        break;
                                    case "VisibilityStatusSwarmsOfInsects":
                                        ////LogTxtMsg(txtLog, "VisibilityStatusSwarmsOfInsects: " + row[col].ToString());
                                        break;
                                    case "OccupancyThreshold":
                                        ////LogTxtMsg(txtLog, "OccupancyThreshold: " + row[col].ToString());
                                        break;
                                    case "VolumeThreshold":
                                        ////LogTxtMsg(txtLog, "VolumeThreshold: " + row[col].ToString());
                                        break;
                                }
                            }
                        }
                    }
                }
                //Update the Configuration_INFLOThresholds table with Detection zone info
                retValue = string.Empty;
                ////LogTxtMsg(txtLog, "\tAdd new Thresholds to INFLO database: ");

                sqlQuery = "INSERT INTO Configuration_INFLOThresholds(CVDataPollingFrequency, TSSDataPollingFrequency, ESSDataPollingFrequency, MobileESSDataPollingFrequency, " +
                            "QueuedSpeedThreshold, CongestedSpeedThreshold, MinimumDisplaySpeed, TroupeRange, " +
                            "ThreeGantrySpeed, CVDataLoadingFrequency, TSSDataLoadingFrequency,  SublinkLength, SublinkPercentQueuedCV, " +
                            "MaximumDisplaySpeed, COFLowerThreshold, COFUpperThreshold, " +
                            "RoadSurfaceStatusDryCOF, RoadSurfaceStatusSnowCOF, RoadSurfaceStatusWetCOF,  RoadSurfaceStatusIceCOF,  WRTMMaxRecommendedSpeed, WRTMMaxRecommendedSpeedLevel1, " +
                            "WRTMMaxRecommendedSpeedLevel2, WRTMMaxRecommendedSpeedLevel3, WRTMMaxRecommendedSpeedLevel4,  WRTMMinRecommendedSpeed,  VisibilityThreshold, VisibilityStatusClear, " +
                            "VisibilityStatusFogNotPatchy, VisibilityStatusPatchyFog, VisibilityStatusBlowingSnow,  VisibilityStatusSmoke,  VisibilityStatusSeaSpray, VisibilityStatusVehicleSpray, " +
                            "VisibilityStatusBlowingDust, VisibilityStatusBlowingSand, VisibilityStatusSunGlare,  VisibilityStatusSwarmsOfInsects,  OccupancyThreshold, VolumeThreshold ) " +
                            "Values(" + clsGlobalVars.CVDataPollingFrequency + ", " + clsGlobalVars.TSSDataPollingFrequency + ", " + clsGlobalVars.ESSDataPollingFrequency +
                            ", " + clsGlobalVars.MobileESSDataPollingFrequency + ", " + clsGlobalVars.LinkQueuedSpeedThreshold + ", " + clsGlobalVars.LinkCongestedSpeedThreshold +
                            ", " + clsGlobalVars.MinimumDisplaySpeed + ", " + clsGlobalVars.TroupeRange +
                            ", " + clsGlobalVars.ThreeGantrySpeed + ", " + clsGlobalVars.CVDataLoadingFrequency + ", " + clsGlobalVars.TSSDataLoadingFrequency +
                            ", " + clsGlobalVars.SubLinkLength + ", " + clsGlobalVars.SubLinkPercentQueuedCV +
                            ", " + clsGlobalVars.MaximumDisplaySpeed + ", " + clsGlobalVars.COFLowerThreshold + ", " + clsGlobalVars.COFUpperThreshold +
                            ", " + clsGlobalVars.RoadSurfaceStatusDryCOF + ", " + clsGlobalVars.RoadSurfaceStatusSnowCOF + ", " + clsGlobalVars.RoadSurfaceStatusWetCOF +
                            ", " + clsGlobalVars.RoadSurfaceStatusIceCOF + ", " + clsGlobalVars.WRTMMaxRecommendedSpeed + ", " + clsGlobalVars.WRTMRecommendedSpeedLevel1 +
                            ", " + clsGlobalVars.WRTMRecommendedSpeedLevel2 + ", " + clsGlobalVars.WRTMRecommendedSpeedLevel3 + ", " + clsGlobalVars.WRTMRecommendedSpeedLevel4 +
                            ", " + clsGlobalVars.WRTMMinRecommendedSpeed + ", " + clsGlobalVars.VisibilityThreshold + ", " + clsGlobalVars.VisibilityStatusClear +
                            ", " + clsGlobalVars.VisibilityStatusFogNotPatchy + ", " + clsGlobalVars.VisibilityStatusPatchyFog + ", " + clsGlobalVars.VisibilityStatusBlowingSnow +
                            ", " + clsGlobalVars.VisibilityStatusSmoke + ", " + clsGlobalVars.VisibilityStatusSeaSpray + ", " + clsGlobalVars.VisibilityStatusVehicleSpray +
                            ", " + clsGlobalVars.VisibilityStatusBlowingDust + ", " + clsGlobalVars.VisibilityStatusBlowingSand + ", " + clsGlobalVars.VisibilityStatusSunGlare +
                            ", " + clsGlobalVars.VisibilityStatusSwarmsOfInsects + ", " + clsGlobalVars.OccupancyThreshold + ", " + clsGlobalVars.VolumeThreshold + ")";

                retValue = DB.InsertRow(sqlQuery);
                if (retValue.Length > 0)
                {
                    ////LogTxtMsg(txtLog, retValue);
                    return retValue;
                }
                /*////LogTxtMsg(txtLog, "\t\tCVDataPollingFrequency: " + clsGlobalVars.CVDataPollingFrequency + "\r\n" +
                                  "\t\tTSSDataPollingFrequency: " + clsGlobalVars.TSSDataPollingFrequency + "\r\n" +
                                  "\t\tESSDataPollingFrequency: " + clsGlobalVars.ESSDataPollingFrequency + "\r\n" +
                                  "\t\tMobileESSDataPollingFrequency: " + clsGlobalVars.MobileESSDataPollingFrequency + "\r\n" +
                                  "\t\tLinkQueuedSpeedThreshold: " + clsGlobalVars.LinkQueuedSpeedThreshold + "\r\n" +
                                  "\t\tLinkCongestedSpeedThreshold: " + clsGlobalVars.LinkCongestedSpeedThreshold + "\r\n" +
                                  "\t\tMinimumDisplaySpeed: " + clsGlobalVars.MinimumDisplaySpeed + "\r\n" +
                                  "\t\tTroupeRange: " + clsGlobalVars.TroupeRange + "\r\n" +
                                  "\t\tThreeGantrySpeed: " + clsGlobalVars.ThreeGantrySpeed + "\r\n" +
                                  "\t\tCVDataLoadingFrequency: " + clsGlobalVars.CVDataLoadingFrequency + "\r\n" +
                                  "\t\tTSSDataLoadingFrequency: " + clsGlobalVars.TSSDataLoadingFrequency + "\r\n" +
                                  "\t\tRecurringCongestionMMLocation: " + clsGlobalVars.RecurringCongestionMMLocation + "\r\n" +
                                  "\t\tSubLinkLength: " + clsGlobalVars.SubLinkLength + "\r\n" +
                                  "\t\tSubLinkPercentQueuedCV: " + clsGlobalVars.SubLinkPercentQueuedCV + "\r\n" +

                                  "\t\tMaximumDisplaySpeed: " + clsGlobalVars.MaximumDisplaySpeed + "\r\n" +
                                  "\t\tCOFLowerThreshold: " + clsGlobalVars.COFLowerThreshold + "\r\n" +
                                  "\t\tCOFUpperThreshold: " + clsGlobalVars.COFUpperThreshold + "\r\n" +
                                  "\t\tRoadSurfaceStatusDryCOF: " + clsGlobalVars.RoadSurfaceStatusDryCOF + "\r\n" +
                                  "\t\tRoadSurfaceStatusSnowCOF: " + clsGlobalVars.RoadSurfaceStatusSnowCOF + "\r\n" +
                                  "\t\tRoadSurfaceStatusWetCOF: " + clsGlobalVars.RoadSurfaceStatusWetCOF + "\r\n" +
                                  "\t\tRoadSurfaceStatusIceCOF: " + clsGlobalVars.RoadSurfaceStatusIceCOF + "\r\n" +
                                  "\t\tWRTMMaxRecommendedSpeed: " + clsGlobalVars.WRTMMaxRecommendedSpeed + "\r\n" +
                                  "\t\tWRTMMaxRecommendedSpeedLevel1: " + clsGlobalVars.WRTMMaxRecommendedSpeedLevel1 + "\r\n" +
                                  "\t\tWRTMMaxRecommendedSpeedLevel2: " + clsGlobalVars.WRTMMaxRecommendedSpeedLevel2 + "\r\n" +
                                  "\t\tWRTMMaxRecommendedSpeedLevel3: " + clsGlobalVars.WRTMMaxRecommendedSpeedLevel3 + "\r\n" +
                                  "\t\tWRTMMaxRecommendedSpeedLevel4: " + clsGlobalVars.WRTMMaxRecommendedSpeedLevel4 + "\r\n" +
                                  "\t\tWRTMMinRecommendedSpeed: " + clsGlobalVars.WRTMMinRecommendedSpeed + "\r\n" +
                                  "\t\tVisibilityThreshold: " + clsGlobalVars.VisibilityThreshold + "\r\n" +
                                  "\t\tVisibilityStatusClear: " + clsGlobalVars.VisibilityStatusClear + "\r\n" +
                                  "\t\tVisibilityStatusFogNotPatchy: " + clsGlobalVars.VisibilityStatusFogNotPatchy + "\r\n" +
                                  "\t\tVisibilityStatusPatchyFog: " + clsGlobalVars.VisibilityStatusPatchyFog + "\r\n" +
                                  "\t\tVisibilityStatusBlowingSnow: " + clsGlobalVars.VisibilityStatusBlowingSnow + "\r\n" +
                                  "\t\tVisibilityStatusSmoke: " + clsGlobalVars.VisibilityStatusSmoke + "\r\n" +
                                  "\t\tVisibilityStatusSeaSpray: " + clsGlobalVars.VisibilityStatusSeaSpray + "\r\n" +
                                  "\t\tVisibilityStatusVehicleSpray: " + clsGlobalVars.VisibilityStatusVehicleSpray + "\r\n" +
                                  "\t\tVisibilityStatusBlowingDust: " + clsGlobalVars.VisibilityStatusBlowingDust + "\r\n" +
                                  "\t\tVisibilityStatusBlowingSand: " + clsGlobalVars.VisibilityStatusBlowingSand + "\r\n" +
                                  "\t\tVisibilityStatusSunGlare: " + clsGlobalVars.VisibilityStatusSunGlare + "\r\n" +
                                  "\t\tVisibilityStatusSwarmsOfInsects: " + clsGlobalVars.VisibilityStatusSwarmsOfInsects + "\r\n" +
                                  "\t\tOccupancyThreshold: " + clsGlobalVars.OccupancyThreshold + "\r\n" +
                                  "\t\tVolumeThreshold: " + clsGlobalVars.VolumeThreshold);*/
            }
            catch (Exception ex)
            {
                retValue = "\tError in loading Threshold parameters into the INFLO database. \r\n\t" + ex.Message;
                return retValue;
            }
                #endregion
            return retValue;
        }

        public static DataTable MakeCVDataTable(List<clsCVData> CVList)
        {
            //Create a new TME_CVData_Input table
            DataTable CVDataTable = new DataTable("CVDataTable");

            //Add LinkId column
            DataColumn CVMessageIdentifier = new DataColumn();
            CVMessageIdentifier.DataType = System.Type.GetType("System.Int64");
            CVMessageIdentifier.ColumnName = "CVMessageIdentifier";
            CVDataTable.Columns.Add(CVMessageIdentifier);

            //Add DSId column
            DataColumn NomadicDeviceID = new DataColumn();
            NomadicDeviceID.DataType = System.Type.GetType("System.String");
            NomadicDeviceID.ColumnName = "NomadicDeviceID";
            CVDataTable.Columns.Add(NomadicDeviceID);

            //Add DSName column
            DataColumn DateGenerated = new DataColumn();
            DateGenerated.DataType = System.Type.GetType("System.DateTime");
            DateGenerated.ColumnName = "DateGenerated";
            CVDataTable.Columns.Add(DateGenerated);

            //Add DSId column
            DataColumn Speed = new DataColumn();
            Speed.DataType = System.Type.GetType("System.Double");
            Speed.ColumnName = "Speed";
            CVDataTable.Columns.Add(Speed);

            //Add DSId column
            DataColumn Heading = new DataColumn();
            Heading.DataType = System.Type.GetType("System.Int32");
            Heading.ColumnName = "Heading";
            CVDataTable.Columns.Add(Heading);

            //Add DSId column
            DataColumn Latitude = new DataColumn();
            Latitude.DataType = System.Type.GetType("System.Double");
            Latitude.ColumnName = "Latitude";
            CVDataTable.Columns.Add(Latitude);

            //Add DSId column
            DataColumn Longitude = new DataColumn();
            Longitude.DataType = System.Type.GetType("System.Double");
            Longitude.ColumnName = "Longitude";
            CVDataTable.Columns.Add(Longitude);

            //Add MMLocation column
            DataColumn MMLocation = new DataColumn();
            MMLocation.DataType = System.Type.GetType("System.Double");
            MMLocation.ColumnName = "MMLocation";
            CVDataTable.Columns.Add(MMLocation);

            //Add CVQueuedState column
            DataColumn CVQueuedState = new DataColumn();
            CVQueuedState.DataType = System.Type.GetType("System.String");
            CVQueuedState.ColumnName = "CVQueuedState";
            CVDataTable.Columns.Add(CVQueuedState);

            //Add CoefficientOfFriction column
            DataColumn CoefficientOfFriction = new DataColumn();
            CoefficientOfFriction.DataType = System.Type.GetType("System.Double");
            CoefficientOfFriction.ColumnName = "CoefficientOfFriction";
            CVDataTable.Columns.Add(CoefficientOfFriction);

            //Add Temperature column
            DataColumn Temperature = new DataColumn();
            Temperature.DataType = System.Type.GetType("System.Int32");
            Temperature.ColumnName = "Temperature";
            CVDataTable.Columns.Add(Temperature);

            //Add RoadwayId column
            DataColumn RoadwayId = new DataColumn();
            RoadwayId.DataType = System.Type.GetType("System.String");
            RoadwayId.ColumnName = "RoadwayId";
            CVDataTable.Columns.Add(RoadwayId);

            // Create an array for DataColumn objects.
            //DataColumn[] keys = new DataColumn[1];
            //keys[0] = productID;
            //CVDataTable.PrimaryKey = keys;


            foreach (clsCVData CV in CVList)
            {
                DataRow row = CVDataTable.NewRow();
                row["NomadicDeviceID"] = CV.NomadicDeviceID;
                row["DateGenerated"] = CV.DateGenerated;
                row["Speed"] = CV.Speed;
                row["Heading"] = CV.Heading;
                row["Latitude"] = CV.Latitude;
                row["Longitude"] = CV.Longitude;
                row["MMLocation"] = CV.MMLocation;
                row["CVQueuedState"] = CV.Queued;
                row["CoefficientOfFriction"] = CV.CoefficientFriction;
                row["Temperature"] = CV.Temperature;
                row["RoadwayID"] = CV.RoadwayID;
                CVDataTable.Rows.Add(row);
            }
            CVDataTable.AcceptChanges();
            return CVDataTable;
        }

        public static DataTable MakeTSSDataTable(List<clsDetectionZoneWSDOT> DZList)
        {
            //Create a new TME_CVData_Input table
            DataTable DZDataTable = new DataTable("DZDataTable");

            //Add DZId column
            DataColumn DZId = new DataColumn();
            DZId.DataType = System.Type.GetType("System.String");
            DZId.ColumnName = "DZId";
            DZDataTable.Columns.Add(DZId);

            //Add DSId column
            DataColumn DSId = new DataColumn();
            DSId.DataType = System.Type.GetType("System.String");
            DSId.ColumnName = "DSId";
            DZDataTable.Columns.Add(DSId);

            //Add DateReceived column
            DataColumn DateReceived = new DataColumn();
            DateReceived.DataType = System.Type.GetType("System.DateTime");
            DateReceived.ColumnName = "DateReceived";
            DZDataTable.Columns.Add(DateReceived);

            //Add StartInterval column
            DataColumn StartInterval = new DataColumn();
            StartInterval.DataType = System.Type.GetType("System.Int32");
            StartInterval.ColumnName = "StartInterval";
            DZDataTable.Columns.Add(StartInterval);

            //Add EndInterval column
            DataColumn EndInterval = new DataColumn();
            EndInterval.DataType = System.Type.GetType("System.Int32");
            EndInterval.ColumnName = "EndInterval";
            DZDataTable.Columns.Add(EndInterval);

            //Add IntervalLength column
            DataColumn IntervalLength = new DataColumn();
            IntervalLength.DataType = System.Type.GetType("System.Int32");
            IntervalLength.ColumnName = "IntervalLength";
            DZDataTable.Columns.Add(IntervalLength);

            //Add BeginTime column
            DataColumn BeginTime = new DataColumn();
            BeginTime.DataType = System.Type.GetType("System.DateTime");
            BeginTime.ColumnName = "BeginTime";
            DZDataTable.Columns.Add(BeginTime);

            //Add EndTime column
            DataColumn EndTime = new DataColumn();
            EndTime.DataType = System.Type.GetType("System.DateTime");
            EndTime.ColumnName = "EndTime";
            DZDataTable.Columns.Add(EndTime);

            //Add Volume column
            DataColumn Volume = new DataColumn();
            Volume.DataType = System.Type.GetType("System.Int32");
            Volume.ColumnName = "Volume";
            DZDataTable.Columns.Add(Volume);

            //Add Occupancy column
            DataColumn Occupancy = new DataColumn();
            Occupancy.DataType = System.Type.GetType("System.Double");
            Occupancy.ColumnName = "Occupancy";
            DZDataTable.Columns.Add(Occupancy);

            //Add AvgSpeed column
            DataColumn AvgSpeed = new DataColumn();
            AvgSpeed.DataType = System.Type.GetType("System.Double");
            AvgSpeed.ColumnName = "AvgSpeed";
            DZDataTable.Columns.Add(AvgSpeed);

            //Add Queued column
            DataColumn Queued = new DataColumn();
            Queued.DataType = System.Type.GetType("System.String");
            Queued.ColumnName = "Queued";
            DZDataTable.Columns.Add(Queued);

            //Add Congested column
            DataColumn Congested = new DataColumn();
            Congested.DataType = System.Type.GetType("System.String");
            Congested.ColumnName = "Congested";
            DZDataTable.Columns.Add(Congested);

            //Add DZStatus column
            DataColumn DZStatus = new DataColumn();
            DZStatus.DataType = System.Type.GetType("System.String");
            DZStatus.ColumnName = "DZStatus";
            DZDataTable.Columns.Add(DZStatus);

            //Add DataType column
            DataColumn DataType = new DataColumn();
            DataType.DataType = System.Type.GetType("System.String");
            DataType.ColumnName = "DataType";
            DZDataTable.Columns.Add(DataType);

            //Add RoadwayId column
            DataColumn RoadwayId = new DataColumn();
            RoadwayId.DataType = System.Type.GetType("System.Int32");
            RoadwayId.ColumnName = "RoadwayId";
            DZDataTable.Columns.Add(RoadwayId);

            // Create an array for DataColumn objects.
            //DataColumn[] keys = new DataColumn[1];
            //keys[0] = productID;
            //DZDataTable.PrimaryKey = keys;

            foreach (clsDetectionZoneWSDOT DZ in DZList)
            {
                DataRow row = DZDataTable.NewRow();
                row["DZId"] = DZ.Identifier;
                row["DSId"] = DZ.DSIdentifier;
                row["DateReceived"] = DZ.DateReceived;
                row["StartInterval"] = DZ.StartInterval;
                row["EndInterval"] = DZ.EndInterval;
                row["IntervalLength"] = DZ.IntervalLength;
                row["BeginTime"] = DZ.BeginTime;
                row["EndTime"] = DZ.EndTime;
                row["Volume"] = DZ.Volume;
                row["Occupancy"] = DZ.Occupancy;
                row["AvgSpeed"] = DZ.AvgSpeed;
                row["Queued"] = DZ.Queued;
                row["Congested"] = DZ.Congested;
                row["DZStatus"] = DZ.DZStatus;
                row["DataType"] = DZ.DataType;
                row["RoadwayId"] = DZ.RoadwayIdentifier;
                DZDataTable.Rows.Add(row);
            }
            DZDataTable.AcceptChanges();
            return DZDataTable;
        }

        public static byte[] GetFileViaHttp(string url)
        {
            string ErrorMsg = string.Empty;
            byte[] returnFile; //= new byte[];

            using (WebClient client = new WebClient())
            {
                try
                {
                    returnFile = client.DownloadData(url);
                    return returnFile;
                }
                catch (Exception ex)
                {
                    ErrorMsg = ex.Message;
                    returnFile = new byte[1];
                    return returnFile;
                }
            }

        }

        public static string ReadWSDOTDailyDetectorZonesFile(ref clsDetectionZoneWSDOT[] DZList, string DailyWSDOTDZWebFileName, int NumStations)
        {
            string retValue = string.Empty;
            try
            {
                var result = GetFileViaHttp(DailyWSDOTDZWebFileName);
                string str = Encoding.UTF8.GetString(result);
                string[] strArr = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArr[3].IndexOf("=") > 0)
                {
                    try
                    {
                        for (int i = 1; i <= NumStations; i++)
                        {
                            string[] strFields = strArr[3 + i].Split(',');
                            if (strFields.Length >= 6)
                            {
                                DZList[i] = new clsDetectionZoneWSDOT();
                                DZList[i].Identifier = strFields[0];
                                DZList[i].Location = strFields[1];
                                if ((strFields[2].ToUpper().Trim() == "NB") || (strFields[2].ToUpper().Trim() == "SB") || (strFields[2].ToUpper().Trim() == "EB") || (strFields[2].ToUpper().Trim() == "WB"))
                                {
                                    DZList[i].Location = DZList[i].Location + " - " + strFields[2];
                                    DZList[i].Milepost = (strFields[3]);
                                    DZList[i].Lanes = (strFields[4]);
                                    DZList[i].Lat = (strFields[5]);
                                    DZList[i].Lon = (strFields[6]);
                                }
                                else
                                {
                                    DZList[i].Milepost = (strFields[2]);
                                    DZList[i].Lanes = (strFields[3]);
                                    DZList[i].Lat = (strFields[4]);
                                    DZList[i].Lon = (strFields[5]);
                                }
                            }
                            else
                                continue;
                        }
                    }
                    catch (Exception exc)
                    {
                        retValue = "Error in processing DZ records. \r\n" + exc.Message;
                        return retValue;
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = "Error in reading DZ records file: " + DailyWSDOTDZWebFileName + ". \r\n" + ex.Message;
                return retValue;
            }

            return retValue;
        }

        public static string ReadWSDOTSelectedDetectorZonesFile(ref List<clsDetectionZoneWSDOT> DZList, string DZFileName)
        {
            string retValue = string.Empty;
            string CurrentSection = string.Empty;
            ///LogTxtMsg(txtLog, "\r\n\r\nReading the Detection Zones configuration file.");

            if (File.Exists(DZFileName))
            {
                try
                {
                    StreamReader fReader = null;

                    CurrentSection = "Opening: " + DZFileName;
                    fReader = File.OpenText(DZFileName);

                    //int tmpLinkNo = 0;
                    //int tmpDSNo = 0;
                    int tmpDetectionStationNo = 0;
                    //Modified by Hassan Charara on 11/14/2014 to convert tmpDetectionZoneNo from int to string
                    //int tmpDetectionZoneNo = 0;
                    string tmpDetectionZoneNo = string.Empty;
                    int tmpLaneNo = 0;
                    string tmpLaneDesc = string.Empty;
                    string tmpLaneType = string.Empty;
                    string tmpDZType = string.Empty;
                    double tmpMMLocation = 0.0;
                    int tmpRoadwayId = 0;

                    clsEnums.enDirection tmpDirection = clsEnums.enDirection.NA;
                    string sLine = string.Empty;
                    string[] sField;

                    while ((sLine = fReader.ReadLine()) != null)
                    {
                        sField = sLine.Split(',');
                        if (sField[0].ToLower() == "DetectorStationNo".ToLower())
                        {
                            ///LogTxtMsg(txtLog, "\t" + sLine);
                            continue;
                        }
                        if (sField.Length < 8)
                            continue;

                        clsDetectionZoneWSDOT DZ = new clsDetectionZoneWSDOT();

                        CurrentSection = "Detection Zone record processing";

                        tmpDetectionStationNo = int.Parse(sField[0]);
                        tmpDetectionZoneNo = sField[1];
                        tmpLaneNo = int.Parse(sField[2]);
                        tmpLaneDesc = sField[3];
                        tmpMMLocation = double.Parse(sField[4]);
                        tmpDirection = clsEnums.GetDirIndexFromString(sField[5]);
                        if (sField[6] != null)
                            tmpLaneType = sField[6].ToUpper();
                        if (sField[7] != null)
                            tmpDZType = sField[7].ToUpper();
                        if (sField[9] != null)
                            tmpRoadwayId = int.Parse(sField[9]);

                        DZ.DSIdentifier = tmpDetectionStationNo;
                        DZ.Identifier = tmpDetectionZoneNo;
                        DZ.Lanes = tmpLaneNo.ToString();
                        DZ.Location = tmpMMLocation.ToString();
                        DZ.Direction = tmpDirection;
                        DZ.LaneType = tmpLaneType;
                        DZ.DZType = tmpDZType;
                        DZ.ArrayIndex = 0;
                        DZ.RoadwayIdentifier = tmpRoadwayId;
                        DZ.Found = false;
                       

                        DZList.Add(DZ);

                        ///LogTxtMsg(txtLog, "\t" + tmpDetectionStationNo + "," + tmpDetectionZoneNo + "," + tmpLaneNo + "," + tmpLaneDesc + "," + tmpMMLocation);
                    }
                }
                catch (Exception ex)
                {
                    retValue = "\tError in reading the Detection Zone information. Current Section: " + CurrentSection + "\r\n\t" + ex.Message;
                    return retValue;
                }
            }
            else
            {
                retValue = "\tThe Detection Zone configuration file does not exist: " + DZFileName;
                return retValue;
            }

            return retValue;
        }
    }
}
