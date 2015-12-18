using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFLOClassLib
{
    public static class clsEnums
    {
        public enum enRecommendedSpeedSource
        {
            NA = 0,
            CV = 1,
            TSS = 2,
            WRTM = 3,
        };
        public enum enQueueSource
        {
            NA = 0,
            CV = 1,
            TSS = 2,
        };
        public enum enQueueCahnge
        {
            NA = 0,
            Growing = 1,
            Dissipating = 2,
            Same = 3,
        };
        public enum enDirection
        {
            NA = 0,
            NB = 1,
            SB = 2,
            EB = 3,
            WB = 4,
            NE = 5,
            NW = 6,
            SE = 7,
            SW = 8
        };
        public static enDirection GetDirIndexFromString(string p_Direction)
        {
            enDirection iDirectionIndex;
            string Direction = p_Direction.ToLower();

            switch (Direction)
            {
                case "north":
                case "northbound":
                case "nb":
                    {
                        iDirectionIndex = enDirection.NB;
                        break;
                    }
                case "south":
                case "southbound":
                case "sb":
                    {
                        iDirectionIndex = enDirection.SB;
                        break;
                    }
                case "east":
                case "eastbound":
                case "eb":
                    {
                        iDirectionIndex = enDirection.EB;
                        break;
                    }
                case "west":
                case "westbound":
                case "wb":
                    {
                        iDirectionIndex = enDirection.WB;
                        break;
                    }
                case "ne":
                case "northeast":
                    {
                        iDirectionIndex = enDirection.NE;
                        break;
                    }
                case "nw":
                case "northwest":
                    {
                        iDirectionIndex = enDirection.NW;
                        break;
                    }
                case "se":
                case "southeast":
                    {
                        iDirectionIndex = enDirection.SE;
                        break;
                    }
                case "sw":
                case "southwest":
                    {
                        iDirectionIndex = enDirection.SW;
                        break;
                    }
                default:
                    {
                        iDirectionIndex = enDirection.NA;
                        break;
                    }
            }

            return iDirectionIndex;
        }
        public static string DirectionString(enDirection p_DirectionIndex)
        {
            string retValue = string.Empty;
            switch (p_DirectionIndex)
            {
                case enDirection.NB:
                    retValue = "NB";
                    break;
                case enDirection.SB:
                    retValue = "SB";
                    break;
                case enDirection.EB:
                    retValue = "EB";
                    break;
                case enDirection.WB:
                    retValue = "WB";
                    break;
                case enDirection.NE:
                    retValue = "NE";
                    break;
                case enDirection.NW:
                    retValue = "NW";
                    break;
                case enDirection.SE:
                    retValue = "SE";
                    break;
                case enDirection.SW:
                    retValue = "SW";
                    break;
                default:
                    retValue = "NA";
                    break;
            }
            return retValue;
        }
    }
}
