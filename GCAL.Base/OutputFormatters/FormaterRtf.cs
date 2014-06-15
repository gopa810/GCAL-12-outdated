using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterRtf : Formatter
    {
        public static int g_HeaderSize = 36;
        public static int g_Header2Size = 32;
        public static int g_TextSize = 24;
        public static int g_NoteSize = 16;


        public static void AddNoteRtf(StringBuilder str)
        {
            str.AppendLine("\\par\\par{\\fs16\\cf10");
            str.Append(gpszSeparator);
            str.Append("\\par {\\b ");
            str.Append(getSharedStringRtf(978));
            str.Append("}");
            str.AppendLine("\\par\\pard");
            str.AppendLine();
            str.Append("\\tab ");
            str.Append(getSharedStringRtf(979));
            str.AppendLine("\\par");
            str.Append("\\tab ");
            str.Append(getSharedStringRtf(980));
            str.AppendLine("\\par");

            if (GPDisplays.Calendar.JulianDayVisible()
                || GPDisplays.Calendar.AyanamsaValueVisible()
                || GPDisplays.Calendar.SunLongitudeVisible()
                || GPDisplays.Calendar.MoonLongitudeVisible())
            {
                str.Append("\\tab ");
                str.Append(getSharedStringRtf(981));
                str.AppendLine("\\par");
            }

            // last line
            str.AppendLine("\\par");
            str.Append("\\tab ");
            str.AppendFormat("{0} {1}", getSharedStringRtf(982), GPAppHelper.getShortVersionText());
            str.AppendLine("}");

        }

        public static void AddListRtf(StringBuilder text, string pText)
        {
            text.Append("\\par\\tab ");
            text.Append(pText);
        }

        public static void AddTextLineRtf(StringBuilder text, string str)
        {
            text.Append("\\par ");
            text.AppendLine(str);
        }

        public static void AddListRtf(StringBuilder text, string pText, string pText2)
        {
            text.AppendFormat("\\par {0}\\tab {1}", pText, pText2);
            text.AppendLine();
        }

        public static int FormatEventsRtf(GPCoreEventResults inEvents, StringBuilder res)
        {
            int i;
            AppendRtfHeader(res);

            res.AppendFormat("{\\fs{0}\\f2 ", g_Header2Size, g_TextSize);
            res.AppendFormat(getSharedStringRtf(983), inEvents.m_vcStart, inEvents.m_vcEnd);
            res.AppendLine("} \\par");
            List<GPLocation> locList = inEvents.getLocationList();
            foreach (GPLocation loc in locList)
            {
                res.Append(loc.getFullName());
                res.AppendLine("\\par");
            }
            res.AppendLine(); 
            
            //res.AppendFormat(inEvents.m_location.getFullName());
            //res.AppendLine("\\par");
            res.AppendLine("\\par");

            DateTime prevd = new DateTime(1970, 1, 1);
            int prevt = -1;

            for (i = 0; i < inEvents.getCount(); i++)
            {
                GPCoreEvent dnr = inEvents.get(i);

                if (inEvents.b_sorted)
                {
                    DateTime dt = dnr.Time.getLocalTime();
                    if (prevd.Day != dt.Day || prevd.Month != dt.Month || prevd.Year != dt.Year)
                    {
                        res.AppendLine("\\par");
                        res.Append(GPAppHelper.CenterString(dnr.Time.getCompleteLongDateString(), 60, '-'));
                        res.AppendLine("\\par");
                        res.AppendLine("\\par");
                    }
                    prevd = dt;
                }
                else
                {
                    if (prevt != dnr.nType)
                    {
                        string s = " " + dnr.getTypeTitle() + " ";
                        res.AppendLine("\\par");
                        res.Append(GPAppHelper.CenterString(s, 60, '-'));
                        res.AppendLine("\\par");
                        res.AppendLine("\\par");
                    }
                    prevt = dnr.nType;
                }

                if (!inEvents.b_sorted)
                {
                    res.Append(dnr.Time.ToString().PadLeft(20));
                }

                GPLocation loc = dnr.Time.getLocation();
                res.AppendFormat("  {0}  {1} {2} {3} {4}", dnr.Time.getLongTimeString(), dnr.getEventTitle().PadRight(45),
                    loc.getTimeZoneName().PadRight(32), loc.getLongitudeString().PadLeft(6), loc.getLatitudeString().PadLeft(6));
                res.AppendLine("\\par");

            }

            res.AppendLine("\\par");

            AddNoteRtf(res);

            res.AppendLine("}");


            return 1;
        }

        public static void AppendColorTable(StringBuilder str)
        {
            str.Append("\\red16\\green16\\blue16; ");
            str.Append("\\red32\\green32\\blue32; ");
            str.Append("\\red48\\green48\\blue48; ");
            str.Append("\\red64\\green64\\blue64; ");
            str.Append("\\red80\\green80\\blue80; ");
            str.Append("\\red96\\green96\\blue96; ");
            str.Append("\\red112\\green112\\blue112; ");
            str.Append("\\red128\\green128\\blue128; ");
            str.Append("\\red143\\green143\\blue143; ");
            str.Append("\\red159\\green159\\blue159; ");
            str.Append("\\red175\\green175\\blue175; ");
            str.Append("\\red191\\green191\\blue191; ");
            str.Append("\\red207\\green207\\blue207; ");
            str.Append("\\red223\\green223\\blue223; ");
            str.Append("\\red239\\green239\\blue239; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red31\\green0\\blue0; ");
            str.Append("\\red63\\green0\\blue0; ");
            str.Append("\\red95\\green0\\blue0; ");
            str.Append("\\red127\\green0\\blue0; ");
            str.Append("\\red159\\green0\\blue0; ");
            str.Append("\\red191\\green0\\blue0; ");
            str.Append("\\red223\\green0\\blue0; ");
            str.Append("\\red255\\green0\\blue0; ");
            str.Append("\\red255\\green31\\blue31; ");
            str.Append("\\red255\\green63\\blue63; ");
            str.Append("\\red255\\green95\\blue95; ");
            str.Append("\\red255\\green127\\blue127; ");
            str.Append("\\red255\\green159\\blue159; ");
            str.Append("\\red255\\green191\\blue191; ");
            str.Append("\\red255\\green223\\blue223; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red31\\green16\\blue0; ");
            str.Append("\\red63\\green32\\blue0; ");
            str.Append("\\red95\\green48\\blue0; ");
            str.Append("\\red127\\green64\\blue0; ");
            str.Append("\\red159\\green80\\blue0; ");
            str.Append("\\red191\\green96\\blue0; ");
            str.Append("\\red223\\green112\\blue0; ");
            str.Append("\\red255\\green128\\blue0; ");
            str.Append("\\red255\\green143\\blue31; ");
            str.Append("\\red255\\green159\\blue63; ");
            str.Append("\\red255\\green175\\blue95; ");
            str.Append("\\red255\\green191\\blue127; ");
            str.Append("\\red255\\green207\\blue159; ");
            str.Append("\\red255\\green223\\blue191; ");
            str.Append("\\red255\\green239\\blue223; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red31\\green31\\blue0; ");
            str.Append("\\red63\\green63\\blue0; ");
            str.Append("\\red95\\green95\\blue0; ");
            str.Append("\\red127\\green127\\blue0; ");
            str.Append("\\red159\\green159\\blue0; ");
            str.Append("\\red191\\green191\\blue0; ");
            str.Append("\\red223\\green223\\blue0; ");
            str.Append("\\red255\\green255\\blue0; ");
            str.Append("\\red255\\green255\\blue31; ");
            str.Append("\\red255\\green255\\blue63; ");
            str.Append("\\red255\\green255\\blue95; ");
            str.Append("\\red255\\green255\\blue127; ");
            str.Append("\\red255\\green255\\blue159; ");
            str.Append("\\red255\\green255\\blue191; ");
            str.Append("\\red255\\green255\\blue223; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red16\\green31\\blue0; ");
            str.Append("\\red32\\green63\\blue0; ");
            str.Append("\\red48\\green95\\blue0; ");
            str.Append("\\red64\\green127\\blue0; ");
            str.Append("\\red80\\green159\\blue0; ");
            str.Append("\\red96\\green191\\blue0; ");
            str.Append("\\red112\\green223\\blue0; ");
            str.Append("\\red128\\green255\\blue0; ");
            str.Append("\\red143\\green255\\blue31; ");
            str.Append("\\red159\\green255\\blue63; ");
            str.Append("\\red175\\green255\\blue95; ");
            str.Append("\\red191\\green255\\blue127; ");
            str.Append("\\red207\\green255\\blue159; ");
            str.Append("\\red223\\green255\\blue191; ");
            str.Append("\\red239\\green255\\blue223; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red0\\green31\\blue0; ");
            str.Append("\\red0\\green63\\blue0; ");
            str.Append("\\red0\\green95\\blue0; ");
            str.Append("\\red0\\green127\\blue0; ");
            str.Append("\\red0\\green159\\blue0; ");
            str.Append("\\red0\\green191\\blue0; ");
            str.Append("\\red0\\green223\\blue0; ");
            str.Append("\\red0\\green255\\blue0; ");
            str.Append("\\red31\\green255\\blue31; ");
            str.Append("\\red63\\green255\\blue63; ");
            str.Append("\\red95\\green255\\blue95; ");
            str.Append("\\red127\\green255\\blue127; ");
            str.Append("\\red159\\green255\\blue159; ");
            str.Append("\\red191\\green255\\blue191; ");
            str.Append("\\red223\\green255\\blue223; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red0\\green31\\blue16; ");
            str.Append("\\red0\\green63\\blue32; ");
            str.Append("\\red0\\green95\\blue48; ");
            str.Append("\\red0\\green127\\blue64; ");
            str.Append("\\red0\\green159\\blue80; ");
            str.Append("\\red0\\green191\\blue96; ");
            str.Append("\\red0\\green223\\blue112; ");
            str.Append("\\red0\\green255\\blue128; ");
            str.Append("\\red31\\green255\\blue143; ");
            str.Append("\\red63\\green255\\blue159; ");
            str.Append("\\red95\\green255\\blue175; ");
            str.Append("\\red127\\green255\\blue191; ");
            str.Append("\\red159\\green255\\blue207; ");
            str.Append("\\red191\\green255\\blue223; ");
            str.Append("\\red223\\green255\\blue239; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red0\\green31\\blue31; ");
            str.Append("\\red0\\green63\\blue63; ");
            str.Append("\\red0\\green95\\blue95; ");
            str.Append("\\red0\\green127\\blue127; ");
            str.Append("\\red0\\green159\\blue159; ");
            str.Append("\\red0\\green191\\blue191; ");
            str.Append("\\red0\\green223\\blue223; ");
            str.Append("\\red0\\green255\\blue255; ");
            str.Append("\\red31\\green255\\blue255; ");
            str.Append("\\red63\\green255\\blue255; ");
            str.Append("\\red95\\green255\\blue255; ");
            str.Append("\\red127\\green255\\blue255; ");
            str.Append("\\red159\\green255\\blue255; ");
            str.Append("\\red191\\green255\\blue255; ");
            str.Append("\\red223\\green255\\blue255; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red0\\green16\\blue31; ");
            str.Append("\\red0\\green32\\blue63; ");
            str.Append("\\red0\\green48\\blue95; ");
            str.Append("\\red0\\green64\\blue127; ");
            str.Append("\\red0\\green80\\blue159; ");
            str.Append("\\red0\\green96\\blue191; ");
            str.Append("\\red0\\green112\\blue223; ");
            str.Append("\\red0\\green128\\blue255; ");
            str.Append("\\red31\\green143\\blue255; ");
            str.Append("\\red63\\green159\\blue255; ");
            str.Append("\\red95\\green175\\blue255; ");
            str.Append("\\red127\\green191\\blue255; ");
            str.Append("\\red159\\green207\\blue255; ");
            str.Append("\\red191\\green223\\blue255; ");
            str.Append("\\red223\\green239\\blue255; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red0\\green0\\blue31; ");
            str.Append("\\red0\\green0\\blue63; ");
            str.Append("\\red0\\green0\\blue95; ");
            str.Append("\\red0\\green0\\blue127; ");
            str.Append("\\red0\\green0\\blue159; ");
            str.Append("\\red0\\green0\\blue191; ");
            str.Append("\\red0\\green0\\blue223; ");
            str.Append("\\red0\\green0\\blue255; ");
            str.Append("\\red31\\green31\\blue255; ");
            str.Append("\\red63\\green63\\blue255; ");
            str.Append("\\red95\\green95\\blue255; ");
            str.Append("\\red127\\green127\\blue255; ");
            str.Append("\\red159\\green159\\blue255; ");
            str.Append("\\red191\\green191\\blue255; ");
            str.Append("\\red223\\green223\\blue255; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red16\\green0\\blue31; ");
            str.Append("\\red32\\green0\\blue63; ");
            str.Append("\\red48\\green0\\blue95; ");
            str.Append("\\red64\\green0\\blue127; ");
            str.Append("\\red80\\green0\\blue159; ");
            str.Append("\\red96\\green0\\blue191; ");
            str.Append("\\red112\\green0\\blue223; ");
            str.Append("\\red128\\green0\\blue255; ");
            str.Append("\\red143\\green31\\blue255; ");
            str.Append("\\red159\\green63\\blue255; ");
            str.Append("\\red175\\green95\\blue255; ");
            str.Append("\\red191\\green127\\blue255; ");
            str.Append("\\red207\\green159\\blue255; ");
            str.Append("\\red223\\green191\\blue255; ");
            str.Append("\\red239\\green223\\blue255; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red31\\green0\\blue31; ");
            str.Append("\\red63\\green0\\blue63; ");
            str.Append("\\red95\\green0\\blue95; ");
            str.Append("\\red127\\green0\\blue127; ");
            str.Append("\\red159\\green0\\blue159; ");
            str.Append("\\red191\\green0\\blue191; ");
            str.Append("\\red223\\green0\\blue223; ");
            str.Append("\\red255\\green0\\blue255; ");
            str.Append("\\red255\\green31\\blue255; ");
            str.Append("\\red255\\green63\\blue255; ");
            str.Append("\\red255\\green95\\blue255; ");
            str.Append("\\red255\\green127\\blue255; ");
            str.Append("\\red255\\green159\\blue255; ");
            str.Append("\\red255\\green191\\blue255; ");
            str.Append("\\red255\\green223\\blue255; ");
            str.Append("\\red255\\green255\\blue255; ");
            str.Append("\\red0\\green0\\blue0; ");
            str.Append("\\red31\\green0\\blue16; ");
            str.Append("\\red63\\green0\\blue32; ");
            str.Append("\\red95\\green0\\blue48; ");
            str.Append("\\red127\\green0\\blue64; ");
            str.Append("\\red159\\green0\\blue80; ");
            str.Append("\\red191\\green0\\blue96; ");
            str.Append("\\red223\\green0\\blue112; ");
            str.Append("\\red255\\green0\\blue128; ");
            str.Append("\\red255\\green31\\blue143; ");
            str.Append("\\red255\\green63\\blue159; ");
            str.Append("\\red255\\green95\\blue175; ");
            str.Append("\\red255\\green127\\blue191; ");
            str.Append("\\red255\\green159\\blue207; ");
            str.Append("\\red255\\green191\\blue223; ");
            str.Append("\\red255\\green223\\blue239; ");
            str.Append("\\red255\\green255\\blue255; ");

        }

        public static void AppendRtfHeader(StringBuilder m_text)
        {
            m_text.Append("{\\rtf1\\ansi\\ansicpg1252\\deff2\\deflang1033{\\fonttbl{\\f0\\fswiss\\fcharset0 Lucida Console;}" +
                    "{\\f1\\fswiss\\fcharset0 Arial;}{\\f2\\froman\\fprq2\\fcharset0 Book Antiqua;}}" +
                    "{\\colortbl ;");
            AppendColorTable(m_text);
            m_text.Append("}{\\*\\generator GCAL;}\\viewkind4\\uc1\\pard\\f0\\fs20 ");
        }

        public static int FormatCalendarRtf(GPCalendarResults daybuff, StringBuilder m_text)
        {
            int k;
            int bShowColumnHeaders = 0;
            String str;
            StringBuilder dayText = new StringBuilder();

            GPCalendarDay pvd, prevd, nextd;
            int lastmasa = -1;
            int lastmonth = -1;
            bool bCalcMoon = GPDisplays.Calendar.TimeMoonriseVisible() || GPDisplays.Calendar.TimeMoonsetVisible();

            m_text.Remove(0, m_text.Length);

            AppendRtfHeader(m_text);

            for (k = 0; k < daybuff.m_vcCount; k++)
            {

                prevd = daybuff.get(k - 1);
                pvd = daybuff.get(k);
                nextd = daybuff.get(k + 1);

                if (pvd != null)
                {
                    bShowColumnHeaders = 0;
                    if (GPDisplays.Calendar.MasaHeader() && (pvd.astrodata.nMasa != lastmasa))
                    {
                        if (bShowColumnHeaders == 0)
                            m_text.Append("\\par ");
                        bShowColumnHeaders = 1;
                        //				m_text.Append("\\par\r\n";
                        str = string.Format("\\par \\pard\\f2\\fs{0}\\qc {1}, {2}", g_Header2Size, pvd.getMasaLongName(), pvd.getGaurabdaYearLongString());
                        if ((pvd.astrodata.nMasa == GPMasa.ADHIKA_MASA) && ((lastmasa >= GPMasa.SRIDHARA_MASA) && (lastmasa <= GPMasa.DAMODARA_MASA)))
                        {
                            str += "\\line ";
                            str += getSharedStringRtf(128);
                        }
                        m_text.Append(str);
                        lastmasa = pvd.astrodata.nMasa;
                    }

                    if (GPDisplays.Calendar.MonthHeader() && (pvd.date.getMonth() != lastmonth))
                    {
                        if (bShowColumnHeaders == 0)
                            m_text.Append("\\par ");
                        bShowColumnHeaders = 1;
                        m_text.AppendFormat("\\par\\pard\\f2\\qc\\fs{0}\r\n", g_Header2Size);
                        m_text.AppendFormat("{0} {1}", getSharedStringRtf(759 + pvd.date.getMonth()), pvd.date.getYear());
                        lastmonth = pvd.date.getMonth();
                    }

                    if (pvd.Travelling != null)
                    {
                        m_text.AppendFormat("\\par\\pard\\f2\\qc\\fs{0}\r\n", g_Header2Size);
                        m_text.AppendFormat("{0}", getSharedStringRtf(1030));
                        GPLocationChange lastLocChange = null;
                        foreach (GPLocationChange lc in pvd.Travelling)
                        {
                            if (lastLocChange != lc)
                            {
                                m_text.Append("\\par\\pard\\qc\\cf2\\fs22 ");
                                m_text.AppendFormat("{0} -> {1}", lc.LocationA.getFullName(), lc.LocationB.getFullName());
                                lastLocChange = lc;
                            }
                        }
                    }
                    if (pvd.FlagNewLocation)
                    {
                        m_text.AppendFormat("\\par\\pard\\f2\\qc\\fs{0}\r\n", g_Header2Size);
                        m_text.Append(GPStrings.getSharedStrings().getString(9));
                        m_text.Append("\\par\\pard\\qc\\cf2\\fs22 ");
                        m_text.Append(pvd.date.getLocation().getFullName());
                    }

                    // print location text
                    if (bShowColumnHeaders != 0)
                    {
                        m_text.Append("\\par\\pard\\qc\\cf2\\fs22 ");
                        m_text.Append(pvd.date.getLocation().getFullName());
                    }

                    if (bShowColumnHeaders != 0)
                    {
                        m_text.AppendFormat("\\par\\pard\\fs{0}\\qc {1}", g_NoteSize, GPFileHelper.FileVersion);
                        m_text.AppendLine("\\par\\par");
                    }


                    if (bShowColumnHeaders != 0)
                    {
                        int tabStop = 5760 * g_TextSize / 24;
                        str = string.Format("\\pard\\tx{0}\\tx{1} ", 2000 * g_TextSize / 24, tabStop);
                        m_text.Append(str);
                        if (GPDisplays.Calendar.PaksaInfoVisible())
                        {
                            tabStop += 990 * g_TextSize / 24;
                            str = string.Format("\\tx{0}", tabStop);
                            m_text.Append(str);
                        }
                        if (GPDisplays.Calendar.YogaVisible())
                        {
                            tabStop += 1720 * g_TextSize / 24;
                            str = string.Format("\\tx{0}", tabStop);
                            m_text.Append(str);
                        }
                        if (GPDisplays.Calendar.NaksatraVisible())
                        {
                            tabStop += 1800 * g_TextSize / 24;
                            str = string.Format("\\tx{0}", tabStop);
                            m_text.Append(str);
                        }
                        if (GPDisplays.Calendar.FastingFlagVisible())
                        {
                            tabStop += 750 * g_TextSize / 24;
                            str = string.Format("\\tx{0}", tabStop);
                            m_text.Append(str);
                        }
                        if (GPDisplays.Calendar.RasiVisible())
                        {
                            tabStop += 1850 * g_TextSize / 24;
                            str = string.Format("\\tx{0}", tabStop);
                            m_text.Append(str);
                        }
                        // paksa width 990
                        // yoga width 1720
                        // naks width 1800
                        // fast width 990
                        // rasi width 1850
                        m_text.Append(str);
                        str = string.Format("{{\\highlight15\\cf7\\fs{0}\\b {1}\\tab {2}", g_NoteSize, getSharedStringRtf(985).ToUpper(), getSharedStringRtf(986).ToUpper());
                        m_text.Append(str);
                        if (GPDisplays.Calendar.PaksaInfoVisible())
                        {
                            m_text.Append("\\tab ");
                            m_text.Append(getSharedStringRtf(20).ToUpper());
                        }
                        if (GPDisplays.Calendar.YogaVisible())
                        {
                            m_text.Append("\\tab ");
                            m_text.Append(getSharedStringRtf(104).ToUpper());
                        }
                        if (GPDisplays.Calendar.NaksatraVisible())
                        {
                            m_text.Append("\\tab ");
                            m_text.Append(getSharedStringRtf(15).ToUpper());
                        }
                        if (GPDisplays.Calendar.FastingFlagVisible())
                        {
                            m_text.Append("\\tab ");
                            m_text.Append(getSharedStringRtf(987).ToUpper());
                        }
                        if (GPDisplays.Calendar.RasiVisible())
                        {
                            m_text.Append("\\tab ");
                            m_text.Append(getSharedStringRtf(105).ToUpper());
                        }
                        m_text.Append("}");
                    }
                    str = string.Format("\\fs{0} ", g_TextSize);
                    m_text.Append(str);

                    FormatCalendarDayRtf(pvd, dayText, prevd, nextd);

                    if (!GPDisplays.Calendar.HideEmptyDays() || dayText.Length > 90)
                        m_text.Append(dayText);


                }
            }

            AddNoteRtf(m_text);

            m_text.AppendLine();
            m_text.AppendLine("}");

            return 1;
        }

        public static int FormatMasaListRtf(GPMasaListResults mlist, StringBuilder str)
        {
            string stt;
            string stt2;

            str.Remove(0, str.Length);
            AppendRtfHeader(str);

            stt = string.Format("{{\\fs{0}\\f2 {1}}\\par\\tx{2}\\tx{3}\\f2\\fs{4}\r\n\\par\r\n{5}: {6}\\par\r\n"
                , g_HeaderSize
                , getSharedStringRtf(39), 1000 * g_TextSize / 24, 4000 * g_TextSize / 24
                , g_TextSize, getSharedStringRtf(40), mlist.m_location.getFullName());
            str.Append(stt);
            str.AppendFormat(getSharedStringRtf(41), mlist.vc_start, mlist.vc_end);
            str.AppendLine("\\par");
            str.AppendLine("==================================================================");
            str.AppendLine("\\par");
            str.AppendLine("\\par");

            int i;

            for (i = 0; i < mlist.n_countMasa; i++)
            {
                stt2 = string.Format("\\tab {0} {1}\\tab ", GPMasa.GetName(mlist.arr[i].masa), mlist.arr[i].year);
                str.Append(stt2);
                stt2 = string.Format("{0} - ", mlist.arr[i].vc_start);
                str.Append(stt2);
                str.AppendFormat("{0}\\par", mlist.arr[i].vc_end);
                str.AppendLine();
            }

            AddNoteRtf(str);

            str.Append("}");

            return 1;
        }


        public static string GetTextRtf(GPCalendarDay pvd)
        {
            String str;

            str = string.Format("\\par {0} {1} {2} {3}\\tab {4}\\tab {5} ", pvd.date.getDay().ToString().PadLeft(2), GPAppHelper.getMonthAbr(pvd.date.getMonth()), pvd.date.getYear()
                , pvd.getWeekdayAbbr(), pvd.getTithiNameExtended(), (GPDisplays.Calendar.PaksaInfoVisible() ? GPPaksa.getAbbreviation(pvd.astrodata.nPaksa) : " "));

            if (GPDisplays.Calendar.YogaVisible())
            {
                str += string.Format("\\tab {0}", GPYoga.getName(pvd.astrodata.nYoga));
            }

            if (GPDisplays.Calendar.NaksatraVisible())
            {
                str += string.Format("\\tab {0}", GPNaksatra.getName(pvd.astrodata.nNaksatra));
            }

            if (pvd.nFastType != GPConstants.FAST_NULL && GPDisplays.Calendar.FastingFlagVisible())
                str += "\\tab *";
            else if (GPDisplays.Calendar.FastingFlagVisible())
                str += "\\tab ";

            if (GPDisplays.Calendar.RasiVisible())
            {
                str += string.Format("\\tab {0}", GPSankranti.getName(pvd.astrodata.moon.GetRasi(pvd.astrodata.msAyanamsa)));
            }

            str += "\r\n";

            return str;
        }



        public static int FormatCalendarDayRtf(GPCalendarDay pvd, StringBuilder dayText, GPCalendarDay prevd, GPCalendarDay nextd)
        {
            string str;

            dayText.Remove(0, dayText.Length);

            str = GetTextRtf(pvd);

            if (pvd.astrodata.sun.eclipticalLongitude < 0.0)
            {
                dayText.Append("\\par\\tab ");
                dayText.Append(GPStrings.getSharedStrings().getString(974));
                return 1;
            }
            dayText.Append(str);


            foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(prevd, nextd))
            {
                if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                {
                    if (fest.ShowSettingItem == GPDisplays.Keys.CalendarSankranti)
                    {
                        dayText.Append("\\par ");
                        dayText.Append(fest.Text.PadLeft((80 + str.Length) / 2, '-').PadRight(80, '-'));
                    }
                    else
                    {
                        AddListRtf(dayText, fest.Text);
                    }
                }
            }

            return 0;
        }

        public static void FormatAppDayRtf(GPAppDayResults app, StringBuilder strResult)
        {
            //MOONDATA moon;
            //SUNDATA sun;
            GPAstroData d = app.details;
            string str;
            GPGregorianTime vc = app.evente;
            StringBuilder strText = strResult;

            strText.Remove(0, strText.Length);
            AppendRtfHeader(strText);


            foreach (GPStringPair rec in app.output)
            {
                if (rec.Header)
                {
                    strText.AppendFormat("\\par\\pard{{\\f2\\fs{0} {1}}", g_HeaderSize, rec.Name);
                    strText.AppendFormat("\\par\\pard\\f2\\fs{0}\r\n", g_TextSize);
                }
                else
                {
                    str = string.Format("\\tab {0} : {{\\b {1}}", rec.Name, rec.Value);
                    AddTextLineRtf(strText, str);
                }
            }


            strText.Append("}");
        }

        public static void FormatTodayInfoRtf(GPGregorianTime vc, GPLocationProvider loc, StringBuilder str)
        {
            string str2, str3 = string.Empty;

            GPCalendarResults db = new GPCalendarResults();

            GPGregorianTime vc2 = new GPGregorianTime(vc);
            vc2.PreviousDay();
            vc2.PreviousDay();
            vc2.PreviousDay();
            vc2.PreviousDay();
            db.CalculateCalendar(vc2, 9);

            int i = db.FindDate(vc);
            GPCalendarDay p = db.get(i);

            if (p == null)
                return;

            str.Remove(0, str.Length);
            AppendRtfHeader(str);
            str2 = string.Format("\\f2\\fs{0} {1} ", g_HeaderSize, GPAppHelper.getDateText(vc));
            str.Append(str2);

            str.AppendFormat("\\par\\f2\\fs{0} {{\\fs{1} {2}}\\line {3} ({4}, {5}, {6}: {7})",
                g_TextSize, g_TextSize + 4, getSharedStringRtf(p.date.getDayOfWeek()), loc.getFullName(), loc.getLocation(0).getLatitudeString(), 
                loc.getLocation(0).getLongitudeString(), getSharedStringRtf(12), 
                loc.getLocation(0).getTimeZoneName());
            str.AppendLine("\\par");
            str.AppendLine("\\par");
            str.AppendFormat("  {0}, {1}", p.getTithiName(), p.getPaksaName());
            str.AppendLine("\\par");
            str.AppendFormat("  {0}, {1}", p.getMasaLongName(), p.getGaurabdaYearLongString());
            str.AppendLine("\\par");
            str.AppendLine("\\par");


            // adding mahadvadasi
            // adding spec festivals

            foreach (GPCalendarDay.Festival fest in p.CompleteFestivalList(db.get(i-1), db.get(i+1)))
            {
                if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                {
                    if (fest.ShowSettingItem == GPDisplays.Keys.CalendarSankranti)
                    {
                        str.AppendLine(fest.Text.PadLeft((80 + str2.Length) / 2, '-').PadRight(80, '-'));
                        str.AppendLine("\\par");
                    }
                    else
                    {
                        str.Append("\\tab");
                        str.Append(fest.Text);
                        str.AppendLine("\\par");
                    }
                }
            }

            str.AppendLine("\\par");


            if (GPDisplays.Today.BrahmaMuhurtaVisible())
            {
                str.AppendLine("\\par");
                str.AppendFormat("{0} {1}", getSharedStringRtf(988), p.astrodata.sun.arunodaya.getShortMuhurtaRange(0));
            }

            if (GPDisplays.Today.SunriseVisible())
            {
                str.AppendLine("\\par");
                str.AppendFormat("{0} {1} ", getSharedStringRtf(51), p.astrodata.sun.rise.getShortTimeString());
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1} ", getSharedStringRtf(989), p.astrodata.sun.rise.getShortSandhyaRange());
                }
                str.AppendFormat(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.AppendLine("\\par");
            }
            if (GPDisplays.Today.NoonVisible())
            {
                str2 = string.Format("{0} {1} ", getSharedStringRtf(857), p.astrodata.sun.noon.getShortTimeString());
                str.Append(str2);
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1} ", getSharedStringRtf(989), p.astrodata.sun.noon.getShortSandhyaRange());
                }
                str.AppendFormat(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.AppendLine("\\par");
            }
            if (GPDisplays.Today.SunsetVisible())
            {
                str.AppendFormat("{0}  {1} ", getSharedStringRtf(52), p.astrodata.sun.set.getShortTimeString());
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1} ", getSharedStringRtf(989), p.astrodata.sun.set.getShortSandhyaRange());
                }
                str.AppendFormat(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.AppendLine("\\par");
            }
            if (GPDisplays.Today.SunriseInfo())
            {
                str.AppendLine("\\par");
                str.Append(getSharedStringRtf(990));
                str.AppendLine("\\par");
                str.AppendFormat("   {1} {2}", GPNaksatra.getName(p.astrodata.nNaksatra), getSharedStringRtf(15));
                if (GPDisplays.Today.NaksatraPadaVisible())
                {
                    str.AppendFormat(", {0} {1} ({2})", p.astrodata.nNaksatraElapse, getSharedStringRtf(993), getSharedStringRtf(811 + p.getNaksatraPada()));
                }
                if (GPDisplays.Today.RasiOfMoonVisible())
                {
                    str.AppendFormat(", {0}: {1}", getSharedStringRtf(991), GPSankranti.getName(p.astrodata.nMoonRasi));
                }
                str.AppendFormat(", {0} {1}", GPYoga.getName(p.astrodata.nYoga), getSharedStringRtf(104));
                str.AppendLine("\\par");
                str.AppendFormat("  {0} {1}", getSharedStringRtf(992), GPSankranti.getName(p.astrodata.nSunRasi));
                str.AppendLine("\\par");
            }
            /* END GCAL 1.4.3 */

            AddNoteRtf(str);
        }



    }
}
