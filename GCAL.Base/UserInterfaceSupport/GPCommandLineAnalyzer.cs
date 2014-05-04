using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCAL.Base
{
    public class GPCommandLineAnalyzer
    {
        public int LastArgError = 0;

        public int SetArgLastError(int a)
        {
            LastArgError = a;
            return a;
        }

        public int GetArg_Year(string str, ref int nYear)
        {
            int n;

            if (int.TryParse(str, out n) == false || n < 1600)
            {
                return SetArgLastError(10);
            }

            if (n > 3999)
                return SetArgLastError(11);

            nYear = n;

            return 0;
        }

        //////////////////////////////////////////////////////////////////////
        //

        public int GetArg_Int(string str, ref int nInteger)
        {
            return (int.TryParse(str, out nInteger) ? 0 : 1);
        }

        //////////////////////////////////////////////////////////////////////
        //

        public int GetArg_Masa(string str)
        {
            int nMasa = 0;

            string[] masaname = new string[]
	        {
		        "visnu",
		        "madhusudana",
		        "trivikrama",
		        "vamana",
		        "sridhara",
		        "hrsikesa",
		        "padmanadbha",
		        "damodara",
		        "kesava",
		        "narayana",
		        "madhava",
		        "govinda",
		        "purusottama"
	        };


            for (int i = 0; i < 13; i++)
            {
                if (masaname[i] == str)
                {
                    return MasaIndexToInternal(i);
                }
            }

            if (int.TryParse(str, out nMasa))
            {
                return MasaIndexToInternal(nMasa - 1);
            }

            return 11;
        }

        public int MasaIndexToInternal(int nMasa)
        {
            int[] nMasaIndex = new int[] { 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12 };

            if (nMasa < 0)
                return 0;
            else if (nMasa > 12)
                return 0;
            else
                return nMasaIndex[nMasa];
        }

        public int GetArg_EarthPos(string str, ref double Latitude, ref double Longitude)
        {
            double l = 0.0;
            double sig = 1.0;
            double coma = 10.0;
            bool after_coma = false;
            bool is_deg = false;
            bool bNorth = false;

            string s = str;

            foreach (char ps in s)
            {
                switch (ps)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (after_coma)
                        {
                            if (is_deg)
                            {
                                l += (ps - '0') * 5.0 / (coma * 3.0);
                            }
                            else
                            {
                                l += (ps - '0') / coma;
                            }
                            coma *= 10.0;
                        }
                        else
                        {
                            l = l * 10.0 + (ps - '0');
                        }
                        break;
                    case 'n':
                    case 'N':
                        after_coma = true;
                        is_deg = true;
                        sig = 1.0;
                        bNorth = true;
                        break;
                    case 's':
                    case 'S':
                        bNorth = true;
                        after_coma = true;
                        is_deg = true;
                        sig = -1.0;
                        break;
                    case 'e':
                    case 'E':
                        bNorth = false;
                        after_coma = true;
                        is_deg = true;
                        sig = 1.0;
                        break;
                    case 'w':
                    case 'W':
                        bNorth = false;
                        after_coma = true;
                        is_deg = true;
                        sig = -1.0;
                        break;
                    case '.':
                        after_coma = true;
                        is_deg = false;
                        break;
                    case '-':
                        sig = -1.0;
                        break;
                    case '+':
                        sig = 1.0;
                        break;
                    case ';':
                        if (bNorth == true)
                        {
                            //bLat = true;
                            Latitude = l * sig;
                            bNorth = false;
                        }
                        else
                        {
                            //bLon = true;
                            Longitude = l * sig;
                            bNorth = true;
                        }
                        l = 0.0;
                        sig = 1.0;
                        after_coma = false;
                        is_deg = false;
                        coma = 10.0;
                        break;
                    default:
                        break;
                }
            }

            if (bNorth == true)
            {
                //bLat = true;
                Latitude = l * sig;
            }
            else
            {
                //bLon = true;
                Longitude = l * sig;
            }

            return 0;
        }

        public int GetArg_TimeZone(string str, ref double TimeZone)
        {
            double l = 0.0;
            double sig = 1.0;
            double coma = 10.0;
            bool after_coma = false;
            bool is_deg = false;

            string s = str;

            foreach (char ps in s)
            {
                switch (ps)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (after_coma)
                        {
                            if (is_deg)
                            {
                                l += (ps - '0') * 5.0 / (coma * 3.0);
                            }
                            else
                            {
                                l += (ps - '0') / coma;
                            }
                            coma *= 10.0;
                        }
                        else
                        {
                            l = l * 10.0 + (ps - '0');
                        }
                        break;
                    case 'W':
                        after_coma = true;
                        sig = -1.0;
                        is_deg = true;
                        break;
                    case 'E':
                        after_coma = true;
                        sig = 1.0;
                        is_deg = true;
                        break;
                    case '-':
                        //after_coma = true;
                        sig = -1.0;
                        break;
                    case '+':
                        //after_coma = true;
                        sig = 1.0;
                        break;
                    case '.':
                        after_coma = true;
                        is_deg = false;
                        break;
                    case ':':
                        after_coma = true;
                        is_deg = true;
                        break;
                    default:
                        return SetArgLastError(14);
                }
            }

            TimeZone = l * sig;

            return 0;
        }

        public int GetArg_Tithi(string str)
        {
            string[] tithi = new string[] {
		        "Pratipat",
		        "Dvitiya",
		        "Tritiya",
		        "Caturthi",
		        "Pancami",
		        "Sasti",
		        "Saptami",
		        "Astami",
		        "Navami",
		        "Dasami",
		        "Ekadasi",
		        "Dvadasi",
		        "Trayodasi",
		        "Caturdasi",
		        "Amavasya",
		        "Pratipat",
		        "Dvitiya",
		        "Tritiya",
		        "Caturthi",
		        "Pancami",
		        "Sasti",
		        "Saptami",
		        "Astami",
		        "Navami",
		        "Dasami",
		        "Ekadasi",
		        "Dvadasi",
		        "Trayodasi",
		        "Caturdasi",
		        "Purnima"
	        };

            int i;

            for (i = 0; i < 30; i++)
            {
                if (tithi[i] == str)
                {
                    return i % 15;
                }
            }

            if (int.TryParse(str, out i))
            {
                return i - 1;
            }

            return 0;
        }

        public int GetArg_Paksa(string str)
        {
            if ((str[0] == 'g') || (str[0] == 'G') || (str[0] == '1'))
            {
                return 1;
            }

            return 0;
        }

        public int GetArg_Date(string str, out GPGregorianTime vc)
        {
            vc = new GPGregorianTime(GPLocation.getEmptyLocation());
            string[] parts = str.Replace('/', '-').Split('-');
            if (parts.Length < 3)
                return 1;
            int day, month, year;
            int.TryParse(parts[0], out day);
            if (day == 0)
                day = 1;
            int.TryParse(parts[1], out month);
            if (month == 0)
                month = 1;
            int.TryParse(parts[2], out year);

            vc.setDate(year, month, day);
            return 0;
        }

        public int GetArg_VaisnDate(string str, out GPVedicTime va)
        {
            va = new GPVedicTime();

            string[] parts = str.Replace('/', '-').Split('-');
            if (parts.Length < 4)
                return 1;


            va.tithi = GetArg_Tithi(parts[0]) + GetArg_Paksa(parts[1]) * 15;
            va.masa = GetArg_Masa(parts[2]);
            int.TryParse(parts[3], out va.gyear);

            return 0;
        }

        public int GetArg_Interval(string pszText, ref int A, ref int B)
        {
            A = 0;
            B = 0;

            string[] parts = pszText.Replace(':', '-').Split('-');
            if (parts.Length < 2)
                return 1;

            int.TryParse(parts[0], out A);
            int.TryParse(parts[1], out B);

            if (A == 0)
                A = B;
            else if (B == 0)
                B = A;

            return 0;
        }

        public int GetArg_Time(string str, out GPGregorianTime vc)
        {
            vc = new GPGregorianTime(GPLocation.getEmptyLocation());
            double l = 0.0;
            double coma = 10.0;
            bool after_coma = false;
            bool is_deg = false;

            string s = str;

            foreach (char ch in str)
            {
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (after_coma)
                        {
                            if (is_deg)
                            {
                                l += (ch - '0') * 5.0 / (coma * 3.0);
                            }
                            else
                            {
                                l += (ch - '0') / coma;
                            }
                            coma *= 10.0;
                        }
                        else
                        {
                            l = l * 10.0 + (ch - '0');
                        }
                        break;
                    case ':':
                        after_coma = true;
                        is_deg = true;
                        break;
                    default:
                        return SetArgLastError(14);
                }
            }

            vc.setDayHours(l / 24.0);

            return 0;
        }

        /// <summary>
        /// Main function of this class
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool ParseCommandArguments(string[] args)
        {
            GPLocationProvider loc = new GPLocationProvider();
            GPLocation loc1 = new GPLocation();
            GPGregorianTime vcStart = new GPGregorianTime(loc), vcEnd = new GPGregorianTime(loc);
            GPVedicTime vaStart = new GPVedicTime(), vaEnd = new GPVedicTime();
            int nCount;
            int nReq = 0;
            string strFileOut = "";

            try
            {
                loc1.setLatitudeNorthPositive(0.0);
                loc1.setLongitudeEastPositive(0.0);
                loc1.setTimeZoneName("");
                loc.setDefaultLocation(loc1);
                vcStart.Clear();
                vcEnd = vcStart;
                vaStart.tithi = vaStart.masa = vaStart.gyear = 0;
                vaEnd = vaStart;
                nCount = -1;

                int argc = args.Length;
                for (int i = 0; i < argc; i++)
                {
                    //TRACE2("arg %d = %s\n", i, args[i]);
                    if (args[i] == "-L")
                    {
                        if (argc >= i + 2)
                        {
                            loc1.setLongitudeString(args[i + 1]);
                            double lat = 0.0;
                            double longi = 0.0;
                            GetArg_EarthPos(args[i + 1], ref lat, ref longi);
                            loc1.setLongitudeEastPositive(longi);
                            loc1.setLatitudeNorthPositive(lat);
                            //TRACE2("-L latitude=%f longitude=%f\n", loc.m_fLatitude, loc.m_fLongitude);
                        }
                        i++;
                    }
                    else if (args[i] == "-N")
                    {
                        if (argc >= i + 2)
                        {
                            loc1.setCity(args[i + 1]);
                            //TRACE1("-N name=%s\n", loc.m_strName);
                        }
                        i++;
                    }
                    else if (args[i] == "-SV")
                    {
                        if (argc >= i + 2)
                        {
                            GetArg_VaisnDate(args[i + 1], out vaStart);
                        }
                        i++;
                    }
                    else if (args[i] == "-SG")
                    {
                        if (argc >= i + 2)
                        {
                            GetArg_Date(args[i + 1], out vcStart);
                        }
                        i++;
                    }
                    else if (args[i] == "-ST")
                    {
                        if (argc >= i + 2)
                        {
                            GetArg_Time(args[i + 1], out vcStart);
                        }
                        i++;
                    }
                    else if (args[i] == "-EG")
                    {
                        if (argc >= i + 2)
                        {
                            GetArg_Date(args[i + 1], out vcEnd);
                            //AfxTrace("-EG day=%d month=%d year=%d\n", vcEnd.day, vcEnd.month, vcEnd.year);
                        }
                        i++;
                    }
                    else if (args[i] == "-EV")
                    {
                        if (argc >= i + 2)
                        {
                            GetArg_VaisnDate(args[i + 1], out vaEnd);
                            //AfxTrace("-EV tithi=%d masa=%d gyear=%d\n", vaEnd.tithi, vaEnd.masa, vaEnd.gyear);
                        }
                        i++;
                    }
                    else if (args[i] == "-EC")
                    {
                        if (argc >= i + 2)
                        {
                            int.TryParse(args[i + 1], out nCount);
                        }
                        i++;
                    }
                    else if (args[i] == "-TZ")
                    {
                        if (argc >= i + 2)
                        {
                            loc1.setTimeZoneName(args[i + 1]);
                        }
                        i++;
                    }
                    else if (args[i] == "-O")
                    {
                        if (argc >= i + 2)
                        {
                            strFileOut = args[i + 1];
                        }
                        i++;
                    }
                    else if (args[i] == "-R")
                    {
                        if (argc >= i + 2)
                        {
                            if (args[i + 1] == "calendar")
                            {
                                nReq = 10;
                            }
                            else if (args[i + 1] == "appday")
                            {
                                nReq = 11;
                            }
                            else if (args[i + 1] == "tithi")
                            {
                                nReq = 12;
                            }
                            else if (args[i + 1] == "sankranti")
                            {
                                nReq = 13;
                            }
                            else if (args[i + 1] == "naksatra")
                            {
                                nReq = 14;
                            }
                            else if (args[i + 1] == "firstday")
                            {
                                nReq = 15;
                            }
                            else if (args[i + 1] == "gcalendar")
                            {
                                nReq = 16;
                            }
                            else if (args[i + 1] == "gtithi")
                            {
                                nReq = 17;
                            }
                            else if (args[i + 1] == "next")
                            {
                                nReq = 18;
                            }
                            else if (args[i + 1] == "help")
                            {
                                nReq = 60;
                            }
                            /*else if (args[i+1] == "")
                            {
                            } else if (args[i+1] == "")
                            {
                            } else if (args[i+1] == "")
                            {
                            } else if (args[i+1] == "")
                            {
                            }*/
                        }
                        i++;
                    }
                }

                vcStart.setLocationProvider(loc);
                vcEnd.setLocationProvider(loc);

                switch (nReq)
                {
                    case 10:
                    case 13:
                    case 14:
                        if (vcStart.getYear() == 0 && vaStart.gyear != 0)
                            GPEngine.VATIMEtoVCTIME(vaStart, out vcStart, loc);
                        if (vcEnd.getYear() == 0 && vaEnd.gyear != 0)
                            GPEngine.VATIMEtoVCTIME(vaEnd, out vcEnd, loc);
                        break;
                    default:
                        break;
                }

                if (vcStart.getYear() != 0 && vcEnd.getYear() != 0 && nCount < 0)
                    nCount = Convert.ToInt32(vcEnd.getJulianLocalNoon() - vcStart.getJulianLocalNoon());

                if (nCount < 0)
                    nCount = 30;

                GPAppDayResults appday = new GPAppDayResults();
                GPCalendarResults calendar = new GPCalendarResults();
                //AfxTrace("Count === %d\n", nCount);

                StringBuilder fout = new StringBuilder();
                switch (nReq)
                {
                    case 10:
                        // -R -O -LAT -LON -SG -C [-DST -NAME]
                        vcStart.NextDay();
                        vcStart.PreviousDay();
                        calendar.CalculateCalendar(vcStart, nCount);
                        FormaterXml.WriteCalendarXml(calendar, fout);
                        break;
                    case 11:
                        // -R -O -LAT -LON -SG -ST [-NAME]
                        appday.calculateAppearanceDayData(loc, vcStart);
                        FormaterXml.FormatAppDayXML(appday, fout);
                        break;
                    case 12:
                        FormaterXml.WriteXML_Tithi(fout, loc, vcStart);
                        break;
                    case 13:
                        if (vcEnd.getYear() == 0)
                        {
                            vcEnd = vcStart;
                            vcEnd.AddDays(nCount);
                        }
                        FormaterXml.WriteXML_Sankrantis(fout, loc, vcStart, vcEnd);
                        break;
                    case 14:
                        FormaterXml.WriteXML_Naksatra(fout, loc, vcStart, nCount);
                        break;
                    case 15:
                        FormaterXml.WriteXML_FirstDay_Year(fout, vcStart);
                        break;
                    case 16:
                        vcStart = GPGaurabdaYear.getFirstDayOfYear(loc, vcStart.getYear());
                        vcEnd = GPGaurabdaYear.getFirstDayOfYear(loc, vcStart.getYear() + 1);
                        nCount = Convert.ToInt32(vcEnd.getJulianLocalNoon() - vcStart.getJulianLocalNoon());
                        calendar.CalculateCalendar(vcStart, nCount);
                        FormaterXml.WriteCalendarXml(calendar, fout);
                        break;
                    case 17:
                        FormaterXml.WriteXML_GaurabdaTithi(fout, loc, vaStart, vaEnd);
                        break;
                    case 18:
                        FormaterXml.WriteXML_GaurabdaNextTithi(fout, loc, vcStart, vaStart);
                        break;
                }
                // application should be windowless
                // since some parameters are present

                File.WriteAllText(strFileOut, fout.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during execution: " + ex.Message);
            }

            return true;
        }

    }
}
