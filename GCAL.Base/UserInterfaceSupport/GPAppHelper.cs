using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCAL.Base
{
    public class GPAppHelper
    {
        public GPGregorianTime TodayDate;
        public GPGregorianTime YesterdayDate;
        public GPGregorianTime TomorrowDate;

        private static string[] childsylable = new[] {
		        "chu", "che", "cho", "la", //asvini
		        "li", "lu", "le", "lo", // bharani
		        "a", "i", "u", "e", //krtika
		        "o", "va", "vi", "vo", // rohini
		        "ve", "vo", "ka", "ke", // mrgasira
		        "ku","gha","ng","chha", // ardra
		        "ke","ko","ha","hi", // punarvasu
		        "hu","he","ho","da", // pushya
		        "di","du","de","do", //aslesa
		        "ma","mi","mu","me", //magha
		        "mo","ta","ti","tu", //purvaphalguni
		        "te","to","pa","pi", //uttaraphalguni
		        "pu","sha","na","tha",//hasta
		        "pe","po","ra","ri",//chitra
		        "ru","re","ra","ta",//svati
		        "ti","tu","te","to",//visakha
		        "na","ni","nu","ne",// anuradha
		        "no","ya","yi","yu",//jyestha
		        "ye","yo","ba","bi",// mula
		        "bu","dha","bha","dha",//purvasada
		        "be","bo","ja","ji",// uttarasada
		        "ju","je","jo","gha",//sravana
		        "ga","gi","gu","ge",// dhanistha
		        "go","sa","si","su",//satabhisda
		        "se","so","da","di",//purvabhadra
		        "du","tha","jna","da",//uttarabhadra
		        "de","do","cha","chi"// revati

	        };

        public static string[] childsylable2 = new[] 
        {
		    "a.. e.. la..",
		    "u.. ba.. va..",
		    "ka.. gha..",
		    "da.. ha..",
		    "ma..",
		    "pa..",
		    "ra.. ta..",
		    "na.. ya..",
		    "dha.. bha... pha..",
		    "kha.. ja..",
		    "ga.. sa.. sha..",
		    "da.. ca.. tha.. jha.."
	    };


        public GPAppHelper()
        {
            GPLocationProvider loc = getMyLocation();
            TodayDate = new GPGregorianTime(loc);
            TodayDate.Today();
            YesterdayDate = new GPGregorianTime(loc);
            YesterdayDate.Today();
            YesterdayDate.PreviousDay();
            TomorrowDate = new GPGregorianTime(loc);
            TomorrowDate.Today();
            TomorrowDate.NextDay();
        }


        private static GPLocationProvider myLocation = null;

        public static GPLocationProvider getMyLocation()
        {
            if (myLocation == null)
            {
                myLocation = new GPLocationProvider();
                bool bSucc = false;
                string possibleFile = GPFileHelper.getAppDataFile("mylocation.xml");
                if (File.Exists(possibleFile))
                {
                    try
                    {
                        myLocation.loadXml(possibleFile);
                        bSucc = true;
                    }
                    catch
                    {
                    }
                }

                if (bSucc == false)
                {
                    GPLocation loc = new GPLocation();
                    loc.setCity(GPUserDefaults.StringForKey("myloc.city", "Mayapur"));
                    loc.setCountryCode(GPUserDefaults.StringForKey("myloc.country", "IN"));
                    loc.setLatitudeNorthPositive(double.Parse(GPUserDefaults.StringForKey("myloc.lat", "23.423413")));
                    loc.setLongitudeEastPositive(double.Parse(GPUserDefaults.StringForKey("myloc.lon", "88.388079")));
                    loc.setTimeZoneName(GPUserDefaults.StringForKey("myloc.tzname", "Asia/Calcutta"));
                    myLocation.setDefaultLocation(loc);
                }
            }

            return myLocation;
        }

        public static void saveMyLocation()
        {
            string possibleFile = GPFileHelper.getAppDataFile("mylocation.xml");
            try
            {
                if (myLocation != null)
                    myLocation.saveXml(possibleFile);
            }
            catch
            {
            }

        }

        public static void setMyLocation(GPLocationProvider value)
        {
            if (value != null)
            {
                myLocation = value;

                GPUserDefaults.SetStringForKey("myloc.city", value.getCity());
                GPUserDefaults.SetStringForKey("myloc.country", value.getLocation(0).getCountryCode());
                GPUserDefaults.SetStringForKey("myloc.lat", value.GetLatitudeNorthPositive().ToString());
                GPUserDefaults.SetStringForKey("myloc.lon", value.GetLongitudeEastPositive().ToString());
                GPUserDefaults.SetStringForKey("myloc.tzname", value.getLocation(0).getTimeZoneName());
            }
        }

        private static GPAppHelper sharedObject = null;

        public static GPAppHelper getShared()
        {
            if (sharedObject == null)
                sharedObject = new GPAppHelper();
            return sharedObject;
        }

        public static string getMonthAbr(int i)
        {
            return GPStrings.getString(64 + i);
        }

        public static void hoursToParts(double aHours, out int h1, out int m1)
        {
            double hours = Math.Abs(aHours);
            h1 = Convert.ToInt32(Math.Floor(hours));
            m1 = Convert.ToInt32((hours - Math.Floor(hours)) * 60);
        }

        /// <summary>
        /// get date text for today screen
        /// </summary>
        /// <param name="vc"></param>
        public static string getDateText(GPGregorianTime vc)
        {
            string str = string.Empty;

            getShared().TodayDate = new GPGregorianTime(vc);
            getShared().TodayDate.Today();

            int input = Convert.ToInt32(vc.getJulianLocalNoon());
            int today = Convert.ToInt32(getShared().TodayDate.getJulianLocalNoon());

            if ((vc.getDay() > 0) && (vc.getDay() < 32) && (vc.getMonth() > 0) && (vc.getMonth() < 13) && (vc.getYear() >= 1500) && (vc.getYear() < 4000))
            {
                if (input - today == 0)
                {
                    str = string.Format("{0} ({1}) - {2}", vc.ToString(), GPStrings.getString(43), GPStrings.getString(vc.getDayOfWeek()));
                }
                else if (input - today == 1)
                {
                    str = string.Format("{0} ({1}) - {2}", vc.ToString(), GPStrings.getString(854), GPStrings.getString(vc.getDayOfWeek()));
                }
                else if (input - today == -1)
                {
                    str = string.Format("{0} ({1}) - {2}", vc.ToString(), GPStrings.getString(853), GPStrings.getString(vc.getDayOfWeek()));
                }
                else
                {
                    str = string.Format("{0} - {1}", vc.ToString(), GPStrings.getString(vc.getDayOfWeek()]);
                }
            }

            return str;
        }

        public static int ComboMasaToMasa(int nComboMasa)
        {
            return (nComboMasa == 12) ? 12 : ((nComboMasa + 11) % 12);
        }

        public static int MasaToComboMasa(int nMasa)
        {
            return (nMasa == 12) ? 12 : ((nMasa + 1) % 12);
        }

        public static string GetNaksatraChildSylable(int n, int pada)
        {
            int i = (n * 4 + pada) % 108;
            return childsylable[i];
        }

        public static string GetRasiChildSylable(int n)
        {
            return childsylable2[n % 12];
        }

        public static string GetMahadvadasiName(int i)
        {
            switch (i)
            {
                case GPConstants.EV_NULL:
                case GPConstants.EV_SUDDHA:
                    return null;
                case GPConstants.EV_UNMILANI:
                    return GPStrings.getString(733);
                case GPConstants.EV_TRISPRSA:
                case GPConstants.EV_UNMILANI_TRISPRSA:
                    return GPStrings.getString(734);
                case GPConstants.EV_PAKSAVARDHINI:
                    return GPStrings.getString(735);
                case GPConstants.EV_JAYA:
                    return GPStrings.getString(736);
                case GPConstants.EV_VIJAYA:
                    return GPStrings.getString(737);
                case GPConstants.EV_PAPA_NASINI:
                    return GPStrings.getString(738);
                case GPConstants.EV_JAYANTI:
                    return GPStrings.getString(739);
                case GPConstants.EV_VYANJULI:
                    return GPStrings.getString(740);
                default:
                    return null;
            }
        }

        public static string GetFastingName(int i)
        {
            switch (i)
            {
                case GPConstants.FAST_NOON:
                    return GPStrings.getString(931);
                case GPConstants.FAST_SUNSET:
                    return GPStrings.getString(932);
                case GPConstants.FAST_MOONRISE:
                    return GPStrings.getString(933);
                case GPConstants.FAST_DUSK:
                    return GPStrings.getString(934);
                case GPConstants.FAST_MIDNIGHT:
                    return GPStrings.getString(935);
                case GPConstants.FAST_DAY:
                    return GPStrings.getString(936);
                default:
                    return null;
            }
        }


        public static string GetEkadasiName(int nMasa, int nPaksa)
        {
            return GPStrings.getString(560 + nMasa * 2 + nPaksa);
        }

        public static string GetDSTSignature(int nDST)
        {
            return (nDST != 0) ? GPStrings.getString(968) : GPStrings.getString(969);
        }

        public static string GetParanaReasonText(int eparana_type)
        {
            switch (eparana_type)
            {
                case GPConstants.EP_TYPE_3DAY:
                    return GPStrings.getString(165);
                case GPConstants.EP_TYPE_4TITHI:
                    return GPStrings.getString(166);
                case GPConstants.EP_TYPE_SUNRISE:
                    return GPStrings.getString(169);
                case GPConstants.EP_TYPE_TEND:
                    return GPStrings.getString(167);
                case GPConstants.EP_TYPE_NAKEND:
                    return GPStrings.getString(168);
                default:
                    break;
            }

            return string.Empty;
        }

        public static string GetTextLatitude(double d)
        {
            int a0, a1;
            char c0;

            c0 = d < 0.0 ? 'S' : 'N';
            d = d < 0.0 ? -d : d;
            a0 = Convert.ToInt32(d);
            a1 = Convert.ToInt32((d - a0) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a0, c0, a1);
        }

        public static string GetTextLongitude(double d)
        {
            int a0, a1;
            char c0;

            c0 = d < 0.0 ? 'W' : 'E';
            a0 = Convert.ToInt32(Math.Abs(d));
            a1 = Convert.ToInt32((Math.Abs(d) - a0) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a0, c0, a1);
        }

        public static string GetTextTimeZone(long aad)
        {
            int a4, a5;
            int sig = 1;
            int d = Convert.ToInt32(aad);
            if (d < 0)
            {
                sig = -1;
                d = Math.Abs(d);
            }
            a4 = Convert.ToInt32(d/3600);
            a5 = Convert.ToInt32((d - a4*3600)/60);

            return string.Format("{0}{1}:{2:00}", (sig > 0 ? '+' : '-'), a4, a5);
        }

        public static string GetTextTimeZoneArg(double d)
        {
            int a4, a5;
            int sig;

            if (d < 0.0)
            {
                sig = -1;
                d = -d;
            }
            else
            {
                sig = 1;
            }
            a4 = Convert.ToInt32(d);
            a5 = Convert.ToInt32((d - a4) * 60 + 0.5);

            return string.Format("{0}{1}{2:00}", a4, (sig > 0 ? 'E' : 'W'), a5);
        }


        public static string getShortVersionText()
        {
            return string.Format("{0} {1}", GPStrings.getString(130), GPFileHelper.FileVersion);
        }

        public static string getLongVersionText()
        {
            return string.Format("{0} {1}", GPStrings.getString(131), GPFileHelper.FileVersion);
        }

        public static string CenterString(string s, int width)
        {
            if (s.Length > width)
                return s;
            return s.PadLeft((s.Length + width) / 2).PadRight(width);
        }

        public static string CenterString(string s, int width, char paddingChar)
        {
            if (s.Length > width)
                return s;
            return s.PadLeft((s.Length + width) / 2, paddingChar).PadRight(width, paddingChar);
        }


        public static string MakeFilterString(params FileFormatType[] formats)
        {
            string s = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (FileFormatType ft in formats)
            {
                s = string.Empty;
                if (ft == FileFormatType.PlainText)
                {
                    sb.AppendFormat("{0} (*.txt)|*.txt|", GPStrings.getString(400));
                }
                else if (ft == FileFormatType.RichText)
                {
                    sb.AppendFormat("{0} (*.rtf)|*.rtf|", GPStrings.getString(401));
                }
                else if (ft == FileFormatType.Xml)
                {
                    sb.AppendFormat("{0} (*.xml)|*.xml|", GPStrings.getString(404));
                }
                else if (ft == FileFormatType.Vcal)
                {
                    sb.AppendFormat("{0} (*.vcal)|*.vcal|", GPStrings.getString(406));
                }
                else if (ft == FileFormatType.Ical)
                {
                    sb.AppendFormat("{0} (*.ical)|*.ical|", GPStrings.getString(407));
                }
                else if (ft == FileFormatType.Csv)
                {
                    sb.AppendFormat("{0} (*.csv)|*.csv|", GPStrings.getString(405));
                }
                else if (ft == FileFormatType.HtmlTable)
                {
                    sb.AppendFormat("{0} (*.htm)|*.htm|", GPStrings.getString(403));
                }
                else if (ft == FileFormatType.HtmlText)
                {
                    sb.AppendFormat("{0} (*.htm)|*.htm|", GPStrings.getString(402));
                }
            }
            sb.Append("|");
            return sb.ToString();
        }

        public static string AnnouncementTitleClass = "AnnTitle";
        public static bool AnnouncementFullWidth = true;

        public static string GenerateAnnouncementHtmlString(string title, string contentHtml, string contentId, bool display)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div style='display:{0};padding:5px;border-color:black;border-width=1px;border-style:solid;' id='{1}Main'>", (display ? "block" : "none"), contentId);
            if (title != null)
                sb.AppendFormat("  <span style='font-size:120%;font-family:Tahoma;font-weight:bold;'>{1}</span><br>&nbsp;", AnnouncementTitleClass,title);
            sb.AppendFormat("<div id='{0}'>", contentId);
            sb.AppendLine(contentHtml);
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<p></p>");

            return sb.ToString();
        }


        public static string GenerateStartupPage()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Startup Screen</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("<!--");
            sb.AppendLine(FormaterHtml.CurrentFormattingStyles);
            sb.AppendLine("-->");
            sb.AppendLine("</style>");
            sb.AppendLine("<script>");
            sb.AppendLine("function ShowElement(elemId,disp) { var elem = document.getElementById(elemId); elem.style.display=disp; } ");
            sb.AppendLine("</script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<table align=center><tr><td valign=top width=33%>");

            
            sb.AppendLine(GenerateAnnouncementHtmlString(GPStrings.getString(451), GPStrings.getString(453), "nvid", false));


            string tip = NextStartupTip();
            if (tip != null)
            {
                sb.AppendLine(GenerateAnnouncementHtmlString(GPStrings.getString(467), tip, "startuptip", true));
            }
            else
            {
                sb.AppendLine(GenerateAnnouncementHtmlString(GPStrings.getString(467), "Chant Hare Krishna and be happy.", "startuptip", false));
            }

            sb.AppendLine("</td><td valign=top>");

            sb.AppendLine(GenerateAnnouncementHtmlString(GPStrings.getString(452), GPStrings.getString(454), "nefid", false));
            sb.AppendLine(GenerateAnnouncementHtmlString(null, GPStrings.getString(454), "todaypart", false));


            sb.AppendLine("</td></tr></table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");


            return sb.ToString();
        }

        public static string NextStartupTip()
        {
            bool startupTips = GPUserDefaults.BoolForKey("app.startup.tips", true);
            if (startupTips)
            {
                //GPStrings gstr = GPStrings.getSharedStrings();
                int startupTipsCounter = GPUserDefaults.IntForKey("app.startup.tips.counter", 0);
                int count;
                for (count = 1200; count <= 1228; count++)
                {
                    if (GPStrings.getString(count).Length == 0)
                        break;
                }
                count -= 1200;

                startupTipsCounter = startupTipsCounter % count;
                string ret = GPStrings.getString(1200 + startupTipsCounter);
                startupTipsCounter = (startupTipsCounter + 1) % count;
                GPUserDefaults.SetIntForKey("app.startup.tips.counter", startupTipsCounter);
                return ret;
            }
            return null;
        }

        /// <summary>
        /// Maybe better to use function Formatter.getHtmlStringFromUnicode()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToHtmlString(string s)
        {
            return s.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
        }

    }

    public enum FileFormatType
    {
        PlainText,
        RichText,
        HtmlText,
        HtmlTable,
        Xml,
        Csv,
        Vcal,
        Ical
    }


}
