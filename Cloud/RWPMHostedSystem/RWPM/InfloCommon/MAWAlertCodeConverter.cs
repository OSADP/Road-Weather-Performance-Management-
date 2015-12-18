using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon
{
    public class MAWAlertCodeConverter
    {
        public static string GetPrecipitationAlertTextFromCode(int code)
        {
            string text = "";
            switch(code)
            {
                case 0:
                    text = "clear";
                    break;
                case 1:
                    text = "light rain";
                    break;
                case 2:
                    text = "moderate rain";
                    break;
                case 3:
                    text = "heavy rain";
                    break;
                case 4:
                    text = "light rain/snow mix";
                    break;
                case 5:
                    text = "moderate rain/snow mix";
                    break;
                case 6:
                    text = "heavy rain/snow mix";
                    break;
                case 7:
                    text = "light snow";
                    break;
                case 8:
                    text = "moderate snow";
                    break;
                case 9:
                    text = "heavy snow";
                    break;
            }

            return text;
        }

        public static string GetPavementAlertTextFromCode(int code)
        {
            string text = "";
            switch (code)
            {
                case 0:
                    text = "clear";
                    break;
                case 1:
                    text = "wet roads";
                    break;
                case 2:
                    text = "snowy roads";
                    break;
                case 3:
                    text = "snowy, slick roads";
                    break;
                case 4:
                    text = "icy roads";
                    break;
                case 5:
                    text = "icy, slick roads";
                    break;
                case 6:
                    text = "hydroplaning possible";
                    break;
                case 7:
                    text = "black ice";
                    break;
                case 8:
                    text = "icy roads possible";
                    break;
                case 9:
                    text = "icy, slick roads possible";
                    break;
            }

            return text;
        }

        public static string GetVisibilityAlertTextFromCode(int code)
        {
            string text = "";
            switch (code)
            {
                case 0:
                    text = "clear";
                    break;
                case 1:
                    text = "low visibility";
                    break;
                case 2:
                    text = "blowing snow";
                    break;
                case 3:
                    text = "fog";
                    break;
                case 4:
                    text = "haze";
                    break;
                case 5:
                    text = "dust";
                    break;
                case 6:
                    text = "smoke";
                    break;
            }

            return text;
        }

        public static string GetActionTextFromCode(int code)
        {
            string text = "";
            switch (code)
            {
                case 0:
                    text = "";
                    break;
                case 1:
                    text = "use caution";
                    break;
                case 2:
                    text = "drive slowly and use caution";
                    break;
                case 3:
                    text = "delay travel, seek alternate route, or drive slowly and use extreme caution";
                    break;
            }

            return text;
        }
    }
}
