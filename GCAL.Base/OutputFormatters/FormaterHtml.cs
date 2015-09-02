using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class FormaterHtml: Formatter
    {

        public static int DAYS_TO_ENDWEEK(int i)
        {
            return (21 - (i - GPDisplays.General.FirstDayOfWeek())) % 7;
        }

        public static int DAYS_FROM_BEGINWEEK(int i)
        {
            return (i - GPDisplays.General.FirstDayOfWeek() + 14) % 7;
        }

        public static int DAY_INDEX(int i)
        {
            return (i + GPDisplays.General.FirstDayOfWeek()) % 7;
        }

        public static void fprintf(StringBuilder sb, string str)
        {
            sb.Append(str);
        }

        public static void fprintf(StringBuilder sb, string format, params object[] pars)
        {
            sb.AppendFormat(format, pars);
        }

        public static void WriteAppDayHTML(GPAppDayResults app, StringBuilder builder)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            GPAstroData d = app.details;
            GPGregorianTime vc = app.evente;
            builder.AppendFormat("<html><head><title>{0}</title>", GPStrings.getString(45));
            builder.Append("<style>\n<!--\n");
            builder.AppendLine(CurrentFormattingStyles);
            builder.Append("-->\n</style>\n");
            builder.AppendFormat("</head>\n\n<body>");

            WriteAppDayHTML_BodyTable(app, builder);

            WriteVersionInfo(builder);
            builder.AppendLine("</body></html>");

        }

        public static void WriteAppDayHTML_BodyTable(GPAppDayResults app, StringBuilder builder)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            GPAstroData d = app.details;
            GPGregorianTime vc = app.evente;
            bool evline = false;

            builder.AppendFormat("<p class=Header1>{0}</p>", GPStrings.getString(45));
            builder.AppendFormat("<table align=center cellpadding=4 cellspacing=0>");

            foreach (GPStringPair rec in app.output)
            {
                if (rec.Header)
                {
                    builder.AppendFormat("<tr><td colspan=3 class=Header2>{0}</td></tr>", rec.Name);
                    evline = false;
                }
                else
                {
                    if (rec.Name.Length == 0)
                        evline = false;
                    if (evline && GPDisplays.General.HighlightEvenLines())
                    {
                        builder.Append("<tr class=evenLine>");
                    }
                    else
                    {
                        builder.Append("<tr>");
                    }
                    evline = !evline;
                    builder.AppendFormat("<td colspan=2>{0}&nbsp;</td><td>{1}</td>", rec.Name, rec.Value);
                    builder.Append("</tr>");
                }
            }

            builder.AppendLine("</table>");
        }


        public static int WriteEventsHTML(GPCoreEventResults inEvents, StringBuilder f)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;

            fprintf(f, "<html>\n<head>\n<title>{0}</title>\n\n", GPStrings.getString(46));
            fprintf(f, "<style>\n<!--\n");
            f.AppendLine(CurrentFormattingStyles);
            fprintf(f, "-->\n</style>\n");
            fprintf(f, "</head>\n");
            fprintf(f, "<body>\n\n");

            WriteEventsHTML_BodyTable(inEvents, f);

            WriteVersionInfo(f);
            fprintf(f, "</body>\n</html>\n");

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inEvents"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int WriteEventsHTML_BodyTable(GPCoreEventResults inEvents, StringBuilder f)
        {
            int i;

            fprintf(f, "<p class=Header1>{0}</p>", GPStrings.getString(46));
            fprintf(f, "<p class=Header2>{0}: {1}, {2}: {3}", GPStrings.getString(261),
                inEvents.m_vcStart.ToString(), GPStrings.getString(262),
                inEvents.m_vcEnd.ToString());
            fprintf(f, "</p>");

            List<GPLocation> locList = inEvents.getLocationList();
            fprintf(f, "<p class=HeaderLocation>");
            foreach (GPLocation loc in locList)
            {
                fprintf(f, "{0}: {1}<br>", GPStrings.getString(9), loc.getFullName());
            }
            fprintf(f, "</p>\n");
            
            //fprintf(f, "<p class=HeaderLocation>{0}</p>\n", inEvents.m_location.getFullName());

            DateTime prevd = new DateTime(1970,1,1);
            string prevt = string.Empty;
            bool evline = true;

            fprintf(f, "<table align=center cellspacing=0 cellpadding=4><tr>\n");
            for (i = 0; i < inEvents.getCount(); i++)
            {
                GPCoreEvent dnr = inEvents.get(i);

                if (inEvents.b_sorted)
                {
                    DateTime dt = dnr.Time.getLocalTime();
                    if (prevd.Day != dt.Day || prevd.Month != dt.Month || prevd.Year != dt.Year)
                    {
//                        int wd = dnr.Time.getDayOfWeek();
//                        Debugger.Log(0, "", string.Format("Date: {0}, Julian: {1}, Weekday: {2}\n", dnr.Time.getLongDateString(), dnr.Time.getJulianLocalNoon(), wd));
                        fprintf(f, "<td class=hed colspan=2>{0}</td><td class=hed>{1}</td><td class=hed>{2}</td></tr>", dnr.Time.getCompleteLongDateString(), GPStrings.getString(12), GPStrings.getString(9));
                    }
                    prevd = dt;
                }
                else
                {
                    if (prevt != dnr.getTypeTitle())
                    {
                        fprintf(f, "<td class=hed colspan=2>{0}</td><td class=hed>{1}</td><td class=hed>{2}</td></tr>\n", dnr.getTypeTitle(), GPStrings.getString(12), GPStrings.getString(9));
                    }
                    prevt = dnr.getTypeTitle();
                }

                evline = !evline;
                if (evline && GPDisplays.General.HighlightEvenLines())
                {
                    f.Append("<tr class=evenLine>");
                }
                else
                {
                    f.Append("<tr>");
                }

                if (!inEvents.b_sorted)
                {
                    fprintf(f, "<td>{0} </td>", dnr.Time.ToString());
                }

                GPLocation loc = dnr.Time.getLocation();
                fprintf(f, "<td>{0}</td><td>{1}</td><td>{2}</td><td>{3} {4}</td></tr>\n", 
                    dnr.getEventTitle(), dnr.Time.getLongTimeString(),
                    loc.getTimeZoneName(), loc.getLongitudeString(), loc.getLatitudeString());
            }

            fprintf(f, "</tr></table>\n");

            return 1;
        }

        /******************************************************************************************/
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /******************************************************************************************/

        public static int WriteMasaListHTML(GPMasaListResults mlist, StringBuilder f)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;

            fprintf(f, "<html>\n<head>\n<title>{0}</title>\n\n", GPStrings.getString(48));
            fprintf(f, "<style>\n<!--\n");
            f.AppendLine(CurrentFormattingStyles);
            fprintf(f, "-->\n</style>\n");
            fprintf(f, "</head>\n");
            fprintf(f, "<body>\n\n");

            WriteMasaListHTML_BodyTable(mlist, f);

            fprintf(f, "<hr>");
            WriteVersionInfo(f);
            fprintf(f, "</body></html>");
            return 1;
        }

        public static int WriteMasaListHTML_BodyTable(GPMasaListResults mlist, StringBuilder f)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;

            fprintf(f, "<p class=Header1>{0}</p>\n", GPStrings.getString(48));
            fprintf(f, "<p class=HeaderLocation>{0}</p>", mlist.m_location.getLocation(0).getFullName());
            fprintf(f, "<p class=HeaderTimezone>{0}</p>", mlist.m_location.getLocation(0).getTimeZone().getFullName());
            fprintf(f, "<p class=HeaderTimezone>");
            fprintf(f, GPStrings.getString(41), mlist.vc_start, mlist.vc_end);
            fprintf(f, "</p>\n");
            fprintf(f, "<hr width=\"50%\">");

            fprintf(f, "<table align=center cellpadding=4 cellspacing=0>");
            fprintf(f, "<tr><td class=\"hed\" style=\'text-align:left\'>{0}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td class=\"hed\">{1}</td><td class=\"hed\">{2}</td></tr>",
                GPStrings.getUpperString(1002), GPStrings.getUpperString(1003), GPStrings.getUpperString(1004));
            int i;
            bool evline = false;
            for (i = 0; i < mlist.n_countMasa; i++)
            {
                if (evline && GPDisplays.General.HighlightEvenLines())
                {
                    fprintf(f, "<tr class=evenLine>");
                }
                else
                {
                    fprintf(f, "<tr>");
                }
                evline = !evline;
                fprintf(f, "<td>{0} {1}&nbsp;&nbsp;&nbsp;&nbsp;</td>", GPMasa.GetName(mlist.arr[i].masa), mlist.arr[i].year);
                fprintf(f, "<td>{0} {1} {2}</td>", mlist.arr[i].vc_start.getDay(), GPAppHelper.getMonthAbr(mlist.arr[i].vc_start.getMonth()), mlist.arr[i].vc_start.getYear());
                fprintf(f, "<td>{0} {1} {2}</td>", mlist.arr[i].vc_end.getDay(), GPAppHelper.getMonthAbr(mlist.arr[i].vc_end.getMonth()), mlist.arr[i].vc_end.getYear());
                fprintf(f, "</tr>");
            }
            fprintf(f, "</table>");
            return 1;
        }


        public static int WriteCalendarPlusCoreHTML(GPCalendarPlusEventsResults calev, StringBuilder fout)
        {
            GPCalendarResults daybuff = calev.theCalendar;
            GPCoreEventResults events = calev.theEvents;

            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            //int k;
            //string str;
            GPGregorianTime date = new GPGregorianTime(daybuff.CurrentLocation);
            //GPCalendarDay pvd;
            //int nPrevMasa = -1;
            //int nPrevMonth = -1;
            //bool evline = false;

            if (events.b_sorted == false)
            {
                events.Sort(true);
            }

            string columnHeader;
            int columnHeaderCount = 0;

            PrepareHtmlCalendarColumnHeader(out columnHeader, out columnHeaderCount);

            fout.AppendLine("<html><head>");
            fout.AppendFormat("<title>Calendar {0}</title>", daybuff.m_vcStart.getYear());
            fout.AppendLine("<style>");
            fout.AppendLine("<!--");
            fout.AppendLine(CurrentFormattingStyles);
            fout.AppendLine("-->");
            fout.AppendLine("</style>");
            fout.AppendLine("</head>");
            fout.AppendLine("<body>");


            WriteCalendarPlusCoreHTML_BodyTable(calev, fout);

            WriteVersionInfo(fout);

            fprintf(fout, "</body>\n</html>\n");

            return 1;
        }

        public static int WriteCalendarPlusCoreHTML_BodyTable(GPCalendarPlusEventsResults calev, StringBuilder fout)
        {
            GPCalendarResults daybuff = calev.theCalendar;
            GPCoreEventResults events = calev.theEvents;

            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            int k;
            string str;
            GPGregorianTime date = new GPGregorianTime(daybuff.CurrentLocation);
            GPCalendarDay pvd;
            int nPrevMasa = -1;
            int nPrevMonth = -1;
            bool evline = false;

            if (events.b_sorted == false)
            {
                events.Sort(true);
            }

            string columnHeader;
            int columnHeaderCount = 0;

            PrepareHtmlCalendarColumnHeader(out columnHeader, out columnHeaderCount);

            fout.AppendLine("<table cellpadding=4 cellspacing=0 align=center>");

            for (k = 0; k < daybuff.getCount(); k++)
            {
                pvd = daybuff.get(k);
                if (pvd != null)
                {
                    bool writeHeaders = false;

                    if (nPrevMasa != pvd.astrodata.nMasa && GPDisplays.Calendar.MasaHeader())
                    {
                        fout.Append("<tr>");
                        fprintf(fout, "<td colspan={0}>", columnHeaderCount);
                        fprintf(fout, "<p class=MasaHeader><span class=HeaderTitle>" + GPMasa.GetName(pvd.astrodata.nMasa) + " Masa");
                        if (nPrevMasa == GPMasa.ADHIKA_MASA)
                            fprintf(fout, " " + GPStrings.getString(109));
                        fprintf(fout, "</span>");
                        fprintf(fout, "<br><span class=HeaderLocation>{0}</span>", pvd.getGaurabdaYearLongString());
                        fprintf(fout, "<br><span class=HeaderLocation>{0}</span>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<br><span class=HeaderTimezone>{0}: {1}</span>", gstr[12], pvd.date.getLocation().getTimeZone().getFullName());
                        fprintf(fout, "</p>");
                        fout.Append("</tr>");

                        nPrevMasa = pvd.astrodata.nMasa;
                        writeHeaders = true;
                    }
                    else if (nPrevMonth != pvd.date.getMonth() && GPDisplays.Calendar.MonthHeader())
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0} {1}</span><br>", GPStrings.getString(759 + pvd.date.getMonth()), pvd.date.getYear());
                        fout.AppendFormat("<span class=HeaderLocation>{0}</span><br>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<span class=HeaderTimezone>{0}: {1}</span>", gstr[12], pvd.date.getLocation().getTimeZone().getFullName());
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        nPrevMonth = pvd.date.getMonth();
                        writeHeaders = true;
                    }
                    else if (pvd.Travelling != null)
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0}</span><br>", GPStrings.getString(1030));
                        GPLocationChange lastLocChange = null;
                        foreach (GPLocationChange lc in pvd.Travelling)
                        {
                            if (lastLocChange != lc)
                            {
                                fout.AppendFormat("<span class=HeaderLocation>");
                                fout.AppendFormat("{0}, {1} {2},<br>{3}<br>@ {4}</span><br>",
                                    lc.LocationA.getName(), lc.LocationA.getLongitudeString(),
                                    lc.LocationA.getLatitudeString(), lc.LocationA.getTimeZoneString(), lc.humanStart);
                                fout.AppendFormat("&#10132;<br>");
                                fout.AppendFormat("{0}, {1} {2}<br>{3}<br>@ {4}</span><br>",
                                    lc.LocationB.getName(), lc.LocationB.getLongitudeString(),
                                    lc.LocationB.getLatitudeString(), lc.LocationB.getTimeZoneString(), lc.humanEnd);

                                lastLocChange = lc;
                            }
                        }
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        writeHeaders = true;
                    }
                    else if (pvd.FlagNewLocation)
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0}</span><br>", GPStrings.getString(9));
                        fout.AppendFormat("<span class=HeaderLocation>{0}</span><br>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<span class=HeaderTimezone>Timezone: {0}</span>", pvd.date.getLocation().getTimeZone().getFullName());
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        writeHeaders = true;
                    }


                    if (writeHeaders)
                    {
                        fprintf(fout, "<tr>");
                        fprintf(fout, columnHeader);
                        fprintf(fout, "</tr>");
                    }

                    // date data
                    evline = !evline;
                    if (evline)
                    {
                        fprintf(fout, "<tr class=evenLine>");
                    }
                    else
                    {
                        fprintf(fout, "<tr>");
                    }
                    fprintf(fout, "<td align=right>{0}</td><td>&nbsp;{1}</td>\n", pvd.date.ToString().Replace(" ", "&nbsp;"), GPStrings.getString(150 + pvd.date.getDayOfWeek()));
                    fprintf(fout, "<td>{0}</td>\n", pvd.getTithiNameExtended());
                    if (GPDisplays.Calendar.PaksaInfoVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getPaksaAbbreviation());
                    if (GPDisplays.Calendar.NaksatraVisible())
                        fprintf(fout, "<td>{0}</td>\n", GPNaksatra.getName(pvd.astrodata.nNaksatra));
                    if (GPDisplays.Calendar.YogaVisible())
                        fprintf(fout, "<td>{0}</td>\n", GPYoga.getName(pvd.astrodata.nYoga));
                    if (GPDisplays.Calendar.FastingFlagVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getFastingFlag());
                    if (GPDisplays.Calendar.RasiVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getRasiOfMoonName());
                    fprintf(fout, "</tr>");

                    str = string.Empty;

                    if (evline)
                    {
                        fprintf(fout, "<tr class=evenLine>");
                    }
                    else
                    {
                        fprintf(fout, "<tr>");
                    }
                    fout.AppendFormat("<td></td><td></td><td colspan={0} valign=top>", columnHeaderCount - 2);
                    foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(daybuff.get(k - 1), daybuff.get(k + 1)))
                    {
                        if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                        {
                            fprintf(fout, "{0}<br>\n", fest.Text);
                        }
                    }
                    fout.Append("</td>");
                    fout.AppendLine("</tr>");


                    if (evline)
                    {
                        fprintf(fout, "<tr class=evenLine>");
                    }
                    else
                    {
                        fprintf(fout, "<tr>");
                    }
                    fout.AppendFormat("<td></td><td></td><td colspan={0} valign=top>", columnHeaderCount - 2);
                    fprintf(fout, "<table border=1 bordercolor=#909090 cellpadding=4 cellspacing=0>");
                    fout.AppendLine("<tr>");
                    List<GPStringPair> recs = events.ExtractRecordsForDate(pvd.date);
                    foreach (GPStringPair rec in recs)
                    {
                        fout.AppendFormat("<td><span class=CoreInCalHeader>{0}</span><br>{1}</td>", rec.Name, rec.Value);
                    }
                    fout.AppendLine("</tr>");
                    fout.Append("</table></td>");

                    fprintf(fout, "\t</tr>\n\n");

                }
                date.setDayHours(0.0);
                date.NextDay();
            }
            fprintf(fout, "\t</table>\n\n");

            return 1;
        }

        public static void WriteVersionInfo(StringBuilder fout)
        {
            fprintf(fout, "<hr align=center width=\"65%\">\n");
            fprintf(fout, "<p align=center>");
            fprintf(fout, "{0} {1}", getSharedStringHtml(982), GPAppHelper.getShortVersionText());
            fprintf(fout, "</p>\n");
        }

        public static void WriteCalendarTwoLocItem2(string sa, string sb, StringBuilder colA, StringBuilder colB)
        {
            if (sa != sb)
            {
                fprintf(colA, "<td><span style='color:red;font-weight:bold;'>{0}</style></td>\n", sa);
                fprintf(colB, "<td><span style='color:red;font-weight:bold;'>{0}</style></td>\n", sb);
            }
            else
            {
                fprintf(colA, "<td>{0}</td>\n", sa);
                fprintf(colB, "<td>{0}</td>\n", sb);
            }
        }

        public static void PrepareHtmlCalendarColumnHeader(out string columns, out int columnsCount)
        {
            int i = 0;
            StringBuilder fout = new StringBuilder();

            fprintf(fout, "<td class=hed colspan=2>{0}</td>", GPStrings.getUpperString(985));
            i += 2;
            fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(986));
            i++;

            if (GPDisplays.Calendar.PaksaInfoVisible())
            {
                fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(20));
                i++;
            }
            if (GPDisplays.Calendar.NaksatraVisible())
            {
                fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(15));
                i++;
            }
            if (GPDisplays.Calendar.YogaVisible())
            {
                fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(104));
                i++;
            }
            if (GPDisplays.Calendar.FastingFlagVisible())
            {
                fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(987));
                i++;
            }
            if (GPDisplays.Calendar.RasiVisible())
            {
                fprintf(fout, "<td class=hed>{0}</td>", GPStrings.getUpperString(105));
                i++;
            }

            columns = fout.ToString();
            columnsCount = i;
        }

        public static int WriteCompareCalendarHTML(GPCalendarTwoLocResults cals, StringBuilder fout)
        {
            GPCalendarResults daybuffA = cals.CalendarA;
            GPCalendarResults daybuffB = cals.CalendarB;

            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            //int k;
            GPGregorianTime date = new GPGregorianTime(daybuffA.CurrentLocation);

            fprintf(fout, "<html><head><title>\n");

            fprintf(fout, "Calendar " + daybuffA.m_vcStart.getYear() + "</title>");
            fprintf(fout, "<style>\n");
            fprintf(fout, "<!--\n");
            fout.AppendLine(CurrentFormattingStyles);
            fprintf(fout, "-->\n</style>\n");
            fprintf(fout, "</head>\n<body>");

            WriteCompareCalendarHTML_BodyTable(cals, fout);

            WriteVersionInfo(fout);

            fprintf(fout, "</body>\n</html>\n");

            return 1;

        }

        public static int WriteCompareCalendarHTML_BodyTable(GPCalendarTwoLocResults cals, StringBuilder fout)
        {
            GPCalendarResults daybuffA = cals.CalendarA;
            GPCalendarResults daybuffB = cals.CalendarB;

            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            int k;
            GPGregorianTime date = new GPGregorianTime(daybuffA.CurrentLocation);
            GPCalendarDay pvd;
            GPCalendarDay pve;
            int nPrevMasaA = -1;
            int nPrevMasaB = -1;

            fprintf(fout, "<table align=center cellspacing=0 cellpadding=3>");

            string columnHeaders;
            int columnHeadersCount;

            PrepareHtmlCalendarColumnHeader(out columnHeaders, out columnHeadersCount);


            StringBuilder colA = new StringBuilder();
            StringBuilder colB = new StringBuilder();
            bool evline = false;

            for (k = 0; k < daybuffA.getCount(); k++)
            {
                pvd = daybuffA.get(k);
                pve = daybuffB.get(k);

                if (pvd != null && pvd.date.CompareYMD(pve.date) == 0)
                {
                    colA.Remove(0, colA.Length);
                    colB.Remove(0, colB.Length);

                    if (nPrevMasaA != pvd.astrodata.nMasa)
                    {
                        fprintf(colA, "<td colspan={0} style=\'text-align:center;font-weight:bold\'><span style =\'font-size:14pt\'>{1}", columnHeadersCount,
                            pvd.getMasaLongName());
                        if (nPrevMasaA == GPMasa.ADHIKA_MASA)
                            fprintf(colA, " " + GPStrings.getString(109));
                        fprintf(colA, "</span>");
                        fprintf(colA, "<br><span style=\'font-size:10pt;\'>{0}", pvd.getGaurabdaYearLongString());
                        fprintf(colA, "<br>" + pvd.date.getLocation().getFullName() + "</font>");
                        fprintf(colA, "</span></td>\n");
                    }
                    nPrevMasaA = pvd.astrodata.nMasa;

                    if (nPrevMasaB != pve.astrodata.nMasa)
                    {
                        fprintf(colB, "<td colspan={0} style=\'text-align:center;font-weight:bold\'><span style =\'font-size:14pt\'>{1}",
                            columnHeadersCount, pve.getMasaLongName());
                        if (nPrevMasaB == GPMasa.ADHIKA_MASA)
                            fprintf(colB, " " + GPStrings.getString(109));
                        fprintf(colB, "</span>");
                        fprintf(colB, "<br><span style=\'font-size:10pt;\'>{0}", pve.getGaurabdaYearLongString());
                        fprintf(colB, "<br>" + pve.date.getLocation().getFullName() + "</font>");
                        fprintf(colB, "</span></td>\n");
                    }
                    nPrevMasaB = pve.astrodata.nMasa;

                    if (colA.Length > 0)
                    {
                        if (colB.Length > 0)
                        {
                            fprintf(fout, "<tr>{0}{1}</tr>", colA.ToString(), colB.ToString());
                            fprintf(fout, "<tr>{0}{1}</tr>", columnHeaders, columnHeaders);
                        }
                        else
                        {
                            fprintf(fout, "<tr>{0}{1}</tr>", colA.ToString(), "<td colspan=7></td>");
                            fprintf(fout, "<tr>{0}{1}</tr>", columnHeaders, "<td colspan=7></td>");
                        }
                    }
                    else if (colB.Length > 0)
                    {
                        fprintf(fout, "<tr>{0}{1}</tr>", "<td colspan=7></td>", colB.ToString());
                        fprintf(fout, "<tr>{0}{1}</tr>", "<td colspan=7></td>", columnHeaders);
                    }

                    colA.Remove(0, colA.Length);
                    colB.Remove(0, colB.Length);

                    // date data
                    if (evline)
                    {
                        fprintf(fout, "<tr class=evenLine>");
                    }
                    else
                    {
                        fprintf(fout, "<tr>");
                    }
                    evline = !evline;
                    fprintf(colA, "<td class=CompareCalDate>{0}</td>", pvd.date.ToString().Replace(" ", "&nbsp;"));
                    fprintf(colA, "<td>&nbsp;{0}</td>\n", GPStrings.getString(150 + pvd.date.getDayOfWeek()));
                    fprintf(colB, "<td class=CompareCalDate>{0}</td>", pve.date.ToString().Replace(" ", "&nbsp;"));
                    fprintf(colB, "<td>&nbsp;{0}</td>\n", GPStrings.getString(150 + pve.date.getDayOfWeek()));

                    WriteCalendarTwoLocItem2(pvd.getTithiNameExtended(), pve.getTithiNameExtended(), colA, colB);

                    if (GPDisplays.Calendar.PaksaInfoVisible())
                        WriteCalendarTwoLocItem2(GPPaksa.getAbbreviation(pvd.astrodata.nPaksa), GPPaksa.getAbbreviation(pve.astrodata.nPaksa), colA, colB);

                    if (GPDisplays.Calendar.NaksatraVisible())
                        WriteCalendarTwoLocItem2(GPNaksatra.getName(pvd.astrodata.nNaksatra), GPNaksatra.getName(pve.astrodata.nNaksatra), colA, colB);

                    if (GPDisplays.Calendar.YogaVisible())
                        WriteCalendarTwoLocItem2(GPYoga.getName(pvd.astrodata.nYoga), GPYoga.getName(pve.astrodata.nYoga), colA, colB);

                    if (GPDisplays.Calendar.FastingFlagVisible())
                        WriteCalendarTwoLocItem2(pvd.getFastingFlag(), pve.getFastingFlag(), colA, colB);

                    if (GPDisplays.Calendar.RasiVisible())
                        WriteCalendarTwoLocItem2(pvd.getRasiOfMoonName(), pve.getRasiOfMoonName(), colA, colB);

                    fprintf(fout, colA.ToString());
                    fprintf(fout, colB.ToString());
                    fprintf(fout, "</tr>\n");


                    colA.Remove(0, colA.Length);
                    colB.Remove(0, colB.Length);
                    fprintf(fout, "<tr>\n");
                    colA.AppendFormat("<td></td><td></td><td colspan={0}>", columnHeadersCount - 2);
                    colB.AppendFormat("<td></td><td></td><td colspan={0}>", columnHeadersCount - 2);

                    List<GPCalendarDay.Festival> festA = pvd.CompleteFestivalList(daybuffA.get(k - 1), daybuffA.get(k + 1));
                    List<GPCalendarDay.Festival> festB = pve.CompleteFestivalList(daybuffB.get(k - 1), daybuffB.get(k + 1));
                    int i = 0;
                    int m = Math.Max(festA.Count, festB.Count);

                    while (festA.Count < m)
                    {
                        festA.Add(new GPCalendarDay.Festival(999, ""));
                    }
                    while (festB.Count < m)
                    {
                        festB.Add(new GPCalendarDay.Festival(999, ""));
                    }

                    i = 0;
                    foreach (GPCalendarDay.Festival fest in festA)
                    {
                        if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                        {
                            if (festB[i].Text != festA[i].Text)
                            {
                                fprintf(colA, "<span style='color:red;font-weight:bold;'>{0}</span><br>\n", festA[i].Text);
                                fprintf(colB, "<span style='color:red;font-weight:bold;'>{0}</span><br>\n", festB[i].Text);
                            }
                            else
                            {
                                fprintf(colA, "{0}<br>\n", festA[i].Text);
                                fprintf(colB, "{0}<br>\n", festB[i].Text);
                            }
                        }
                        i++;
                    }

                    fprintf(fout, colA.ToString());
                    fprintf(fout, colB.ToString());
                    fprintf(fout, "\t</tr>\n\n");

                }
                date.setDayHours(0.0);
                date.NextDay();
            }
            fprintf(fout, "\t</table>\n\n");

            return 1;

        }

        public static string CurrentFormattingStyles
        {
            get
            {
                return FormattingStylesWithFontSize(CurrentFontSize);
            }
        }

        public static string FormattingStylesWithFontSize(string aFontSize)
        {
                string fm = @"
body {
    font-family:Verdana;
    font-size:[FS];
}
td {
    font-size:[FS];
}
.Header1 {
    text-align:center;
    font-size:125%;
    font-weight:bold;
}
.Header2 {
    text-align:center;
    font-size:125%;
    font-weight:normal;
    padding:10pt;
}
tr.evenLine {
    background-color:#ddddff;
}
td.hed {
    font-family:Verdana;
    font-size:[FS];
    font-weight:bold;
    background:#aaaaaa;
    color:white;
    text-align:left;
    vertical-align:center;
    padding-left:5pt;
    padding-right:5pt;
    padding-top:4pt;
    padding-bottom:4pt;
}
td.hed2 {
   font-family:Helvetica;
   font-size:[FS];
   background:#999999;
   color:white;
   text-align:center;
   vertical-align:center;
   padding-left:2pt;
   padding-right:2pt;
   padding-top:3pt;
   padding-bottom:3pt;
}
table.TodayFestBorder {
    border-width:1pt;
    border-color:#555555;
    border-style:solid 
}
td.TodayCellBody {
    font-size:[FS];
}
td.TodayFestCell {
    font-size:[FS];
    padding-left:7pt;
    padding-right:7pt;
    padding-top:7pt;
    padding-bottom:7pt;
    vertical-align:center; 
}
.MasaHeader {
    padding:10pt;
    text-align:center;
    font-weight:bold;
}
.HeaderTitle {
    font-size:125%;
}
.HeaderTimezone {
    text-align:center;
    font-size:[FS];
    font-weight:normal;
}
.HeaderLocation {
    text-align:center;
    font-size:[FS];
    font-weight:bold;
}
.SankrantiInfo {
    color:blue;
}
.CompareCalDate {
    background:#dddddd;
    text-align:right;
}
.CoreInCalHeader {
    font-weight:bold;
    font-size:80%;
}

p.MsoNormal, li.MsoNormal, div.MsoNormal {
    margin:0in;
    font-size:80%;
    font-family:Arial;
}

p.month
{
    margin-right:0in;
    margin-left:0in;
    font-size:170%;
    font-family:Arial;
}

.text
{
    margin-right:0in;
    margin-left:0in;
    font-size:60%;
    font-family:Arial;
}

.tnote
{
    margin-right:0in;
    margin-left:0in;
    font-size:70%;
    font-family:Arial;
}

.tithiname
{
    margin-right:0in;
    margin-left:0in;
    font-size:80%;
    font-family:Arial;
}

.dayt
{
    vertical-align:top;
    font-size:200%;
    font-family:Arial;
    font-weight:bold;
}
td.CalTabCell
{
    border:solid windowtext 1.0pt;
    padding:3.0pt 3.0pt 3.0pt 3.0pt;
}
span.SpellE
{
    mso-style-name:"""";
    mso-spl-e:yes;
}
span.GramE
{
    mso-style-name:"""";
    mso-gram-e:yes;
}
";
                return fm.Replace("[FS]", aFontSize);
        }

        public static string CurrentFontSize
        {
            get
            {
                return string.Format("{0}pt", GPUserDefaults.IntForKey("FontSize", 10));
            }
        }

        public static int WriteCalendarHTML_BodyTable(GPCalendarResults daybuff, StringBuilder fout)
        {
            GPCalendarDay pvd;
            int nPrevMasa = -1;
            int nPrevMonth = -1;
            int k;
            string columnHeader;
            int columnHeaderCount;
            GPGregorianTime date = new GPGregorianTime(daybuff.CurrentLocation);
            StringBuilder tempBuilder = new StringBuilder();
            //List<string> gstr = GPStrings.getSharedStrings().gstr;


            PrepareHtmlCalendarColumnHeader(out columnHeader, out columnHeaderCount);

            fprintf(fout, "<table align=center cellspacing=0 cellpadding=4>\n");
            bool shouldHighlight = true;
            for (k = 0; k < daybuff.getCount(); k++)
            {
                pvd = daybuff.get(k);
                if (pvd != null)
                {
                    bool writeHeaders = false;

                    if (nPrevMasa != pvd.astrodata.nMasa && GPDisplays.Calendar.MasaHeader())
                    {
                        fout.Append("<tr>");
                        fprintf(fout, "<td colspan={0}>", columnHeaderCount);
                        fprintf(fout, "<p class=MasaHeader><span class=HeaderTitle>{0}", pvd.getMasaLongName());
                        if (nPrevMasa == GPMasa.ADHIKA_MASA)
                            fprintf(fout, " " + GPStrings.getString(109));
                        fprintf(fout, "</span>");
                        fprintf(fout, "<br><span class=HeaderLocation>{0}</span>", pvd.getGaurabdaYearLongString());
                        fprintf(fout, "<br><span class=HeaderLocation>{0}</span>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<br><span class=HeaderTimezone>{0}: {1}</span>", gstr[12], pvd.date.getLocation().getTimeZone().getFullName());
                        fprintf(fout, "</p>");
                        fout.Append("</tr>");

                        nPrevMasa = pvd.astrodata.nMasa;
                        writeHeaders = true;
                    }
                    else if (nPrevMonth != pvd.date.getMonth() && GPDisplays.Calendar.MonthHeader())
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0} {1}</span><br>", GPStrings.getString(759 + pvd.date.getMonth()), pvd.date.getYear());
                        fout.AppendFormat("<span class=HeaderLocation>{0}</span><br>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<span class=HeaderTimezone>Timezone: {0}</span>", pvd.date.getLocation().getTimeZone().getFullName());
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        nPrevMonth = pvd.date.getMonth();
                        writeHeaders = true;
                    }
                    else if (pvd.Travelling != null)
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0}</span><br>", GPStrings.getString(1030));
                        GPLocationChange lastLocChange = null;
                        foreach (GPLocationChange lc in pvd.Travelling)
                        {
                            if (lastLocChange != lc)
                            {
                                fout.AppendFormat("<span class=HeaderLocation>");
                                fout.AppendFormat("{0}, {1} {2},<br>{3}<br>@ {4}</span><br>",
                                    lc.LocationA.getName(), lc.LocationA.getLongitudeString(),
                                    lc.LocationA.getLatitudeString(), lc.LocationA.getTimeZoneString(), lc.humanStart);
                                fout.AppendFormat("&#10132;<br>");
                                fout.AppendFormat("{0}, {1} {2}<br>{3}<br>@ {4}</span><br>",
                                    lc.LocationB.getName(), lc.LocationB.getLongitudeString(),
                                    lc.LocationB.getLatitudeString(), lc.LocationB.getTimeZoneString(), lc.humanEnd);

                                lastLocChange = lc;
                            }
                        }
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        writeHeaders = true;
                    }
                    else if (pvd.FlagNewLocation)
                    {
                        fout.Append("<tr>");
                        fout.AppendFormat("<td colspan={0}>", columnHeaderCount);
                        fout.Append("<p class=MasaHeader>");
                        fout.AppendFormat("<span class=HeaderTitle>{0}</span><br>", GPStrings.getString(9));
                        fout.AppendFormat("<span class=HeaderLocation>{0}</span><br>", pvd.date.getLocation().getFullName());
                        //fout.AppendFormat("<span class=HeaderTimezone>Timezone: {0}</span>", pvd.date.getLocation().getTimeZone().getFullName());
                        fout.Append("</p>");
                        fout.Append("</td>");
                        fout.Append("</tr>");

                        writeHeaders = true;
                    }


                    if (writeHeaders)
                    {
                        fprintf(fout, "<tr>");
                        fprintf(fout, columnHeader);
                        fprintf(fout, "</tr>");
                    }

                    // date data
                    shouldHighlight = !shouldHighlight;
                    if (shouldHighlight && GPDisplays.General.HighlightEvenLines())
                        fprintf(fout, "<tr>");
                    else
                        fprintf(fout, "<tr class=evenLine>");

                    fprintf(fout, "<td align=right>{0}</td>", pvd.date.ToString().Replace(" ", "&nbsp;"));
                    fprintf(fout, "<td>&nbsp;{0}</td>\n", GPStrings.getString(150 + pvd.date.getDayOfWeek()));
                    fprintf(fout, "<td>{0}</td>\n", pvd.getTithiNameExtended());
                    if (GPDisplays.Calendar.PaksaInfoVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getPaksaAbbreviation());
                    if (GPDisplays.Calendar.NaksatraVisible())
                        fprintf(fout, "<td>{0}</td>\n", GPNaksatra.getName(pvd.astrodata.nNaksatra));
                    if (GPDisplays.Calendar.YogaVisible())
                        fprintf(fout, "<td>{0}</td>\n", GPYoga.getName(pvd.astrodata.nYoga));
                    if (GPDisplays.Calendar.FastingFlagVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getFastingFlag());
                    if (GPDisplays.Calendar.RasiVisible())
                        fprintf(fout, "<td>{0}</td>\n", pvd.getRasiOfMoonName());
                    fprintf(fout, "</tr>\n\n");


                    tempBuilder.Remove(0, tempBuilder.Length);

                    foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(daybuff.get(k - 1), daybuff.get(k + 1)))
                    {
                        if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                        {
                            if (fest.ShowSettingItem == GPDisplays.Keys.CalendarSankranti)
                                fprintf(tempBuilder, "<span class=SankrantiInfo>{0}</span><br>\n", fest.Text);
                            else
                                fprintf(tempBuilder, "{0}<br>\n", fest.Text);
                        }
                    }

                    if (tempBuilder.Length > 0)
                    {
                        if (shouldHighlight && GPDisplays.General.HighlightEvenLines())
                            fprintf(fout, "<tr>");
                        else
                            fprintf(fout, "<tr class=evenLine>");
                        fprintf(fout, "<td></td><td></td><td colspan={0}>", columnHeaderCount - 2);
                        fout.Append(tempBuilder);
                        fprintf(fout, "\t</tr>\n\n");
                    }


                }
                date.setDayHours(0.0);
                date.NextDay();
            }
            fprintf(fout, "\t</table>\n\n");

            return 0;
        }

        /******************************************************************************************/
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /******************************************************************************************/

        public static int WriteCalendarHTML(GPCalendarResults daybuff, StringBuilder fout)
        {
	        fout.Append("<html><head>");

            fout.AppendFormat("<title>Calendar {0}</title>", daybuff.m_vcStart.getYear());
	        fout.AppendLine("<style>");
	        fout.AppendLine("<!--");
            fout.AppendLine(CurrentFormattingStyles);
            fout.AppendLine("-->");
	        fout.AppendLine("</style>");
	        fout.AppendLine("</head>");
            fout.AppendLine("<body>");

            WriteCalendarHTML_BodyTable(daybuff, fout);

            WriteVersionInfo(fout);

            fprintf(fout, "</body>\n</html>\n");

	        return 1;
        }

        /******************************************************************************************/
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /*                                                                                        */
        /******************************************************************************************/

        public static int WriteCalendarHtmlTable(GPCalendarResults daybuff, StringBuilder fout)
        {
            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            int g_firstday_in_week = GPDisplays.General.FirstDayOfWeek();
            int k, y, lwd = 0;
            //VCTIME date;
            GPCalendarDay pvd;
            int nPrevMasa = -1;
            int festCount = 0;

            fprintf(fout, "<html>\n<head>\n<title>Calendar {0}</title>\n", daybuff.m_vcStart);
            fout.AppendLine("<style>");
            fout.AppendLine("<!--");
            fout.AppendLine(CurrentFormattingStyles);
            fout.AppendLine("-->");
            fout.AppendLine("</style>");
            fout.AppendLine("</head>");
            fout.AppendLine("<body>");

            for (k = 0; k < daybuff.m_vcCount; k++)
            {
                pvd = daybuff.get(k);
                if (pvd != null)
                {
                    bool bSemicolon = false;
                    lwd = pvd.date.getDayOfWeek();
                    if (nPrevMasa != pvd.date.getMonth())
                    {
                        if (nPrevMasa != -1)
                        {
                            if (DAYS_TO_ENDWEEK(lwd) > 0)
                            {
                                fprintf(fout, "<td colspan={0} class=CalTabCell>&nbsp;</td>", DAYS_TO_ENDWEEK(lwd));
                            }
                            fprintf(fout, "</tr></table>\n<p>&nbsp;</p>");
                        }
                        fprintf(fout, "\n<table width=\"100%\" border=0 frame=bottom cellspacing=0 cellpadding=0><tr><td width=\"60%\"><p class=month>" + GPStrings.getString(pvd.date.getMonth() + 759) + " " + pvd.date.getYear());
                        fprintf(fout, "</p></td><td><p class=tnote align=right>");
                        fprintf(fout, pvd.date.getLocation().getFullName());
                        fprintf(fout, "<br>Timezone: ");
                        fprintf(fout, pvd.date.getLocation().getTimeZoneName());
                        fprintf(fout, "</p>");
                        fprintf(fout, "</td></tr></table><hr>");
                        nPrevMasa = pvd.date.getMonth();
                        fprintf(fout, "\n<table width=\"100%\" bordercolor=black cellpadding=0 cellspacing=0>\n<tr>\n");
                        for (y = 0; y < 7; y++)
                        {
                            fprintf(fout, "<td width=\"14%\" align=center style=\'font-size:10.0pt;border:none\'>" + GPStrings.getString(DAY_INDEX(y)) + "</td>\n");
                        }
                        fprintf(fout, "<tr>\n");
                        if (DAYS_FROM_BEGINWEEK(pvd.date.getDayOfWeek()) > 0)
                        {
                            fprintf(fout, "<td colspan={0} class=CalTabCell>&nbsp;</td>", DAYS_FROM_BEGINWEEK(pvd.date.getDayOfWeek()));
                        }
                    }
                    else
                    {
                        if (pvd.date.getDayOfWeek() == g_firstday_in_week)
                            fprintf(fout, "<tr>\n");
                    }

                    // date data
                    fout.AppendLine();
                    fout.Append("<td valign=top class=CalTabCell ");
                    fout.AppendFormat(" style='background:\"{0}\";'", GetTodayFestivalBackground(pvd));
                    fout.Append(">");

                    fprintf(fout, "<table width=\"100%\" border=0 cellspacing=0 cellpadding=3>");
                    fprintf(fout, "<tr>");
                    fout.AppendFormat("<td style='vertical-align:top'><p class=text><span class=dayt>{0}</span></p></td>", pvd.date.getDay());

                    fprintf(fout, "<td><span class=\"tithiname\">{0}</span></td>", pvd.getTithiNameExtended());
                    fprintf(fout, "</tr></table>\n");
                    fprintf(fout, "<span class=\"text\">\n");

                    fprintf(fout, "<br>\n");

                    festCount = 5;
                    bSemicolon = false;
                    foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(daybuff.get(k-1), daybuff.get(k+1)))
                    {
                        if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                        {
                            if (bSemicolon)
                                fprintf(fout, "; ");
                            fprintf(fout, fest.Text);
                            bSemicolon = true;
                            festCount--;
                        }
                    }

                    if (festCount > 0)
                    {
                        for (int i = 0; i < festCount; i++)
                        {
                            fout.AppendLine("&nbsp;<br>");
                        }
                    }
                    fprintf(fout, "</span>");
                    fprintf(fout, "</td>\n\n");

                }
            }

            if (DAYS_TO_ENDWEEK(lwd) > 0)
            {
                fprintf(fout, "<td colspan={0} class=CalTabCell>&nbsp;</td>", DAYS_TO_ENDWEEK(lwd));
            }
            fprintf(fout, "</tr>\n</table>\n");

            WriteVersionInfo(fout);

            fprintf(fout, "</body>\n</html>\n");

            return 1;
        }

        public static string GetTodayFestivalBackground(GPCalendarDay p)
        {
            if (p == null)
                return "rgb(242,231,212)";
            if (p.nFastType == GPConstants.FAST_EKADASI)
                return "#FFFFCC";
            if (p.nFastType != GPConstants.EP_TYPE_NULL)
                return "#CCFFCC";
            return "rgb(242,231,212)";
        }

        public static void WriteTodayInfoHTML(GPGregorianTime vc, GPCalendarResults db, StringBuilder f, int fontSize, string baseDir)
        {
            StringBuilder sb = new StringBuilder();
            int i = db.FindDate(vc);
            GPCalendarDay p = db.get(i);

            if (p == null)
                return;

            //
            // info about gregorian calendar day
            //
            f.Append("<p align=center>");
            f.AppendFormat("<span style='font-size:130%;font-weight:bold'>{0}</span>\n", GPAppHelper.getDateText(vc));
            f.AppendFormat("<br>{0}", vc.getLocation().getFullName());
            f.AppendFormat("<br>{0}: {1}\n", GPStrings.getString(12), vc.getLocation().getTimeZoneString());
            f.Append("</p>");

            //
            // info about gaurabda day
            //
            f.AppendFormat("<p align=center><span style='font-size:130%;font-weight:bold'>  {0} {1}</span> <br> {2}, {3}, {4}</p>",
                GPTithi.getName(p.astrodata.nTithi), GPStrings.getString(986),
                p.getPaksaName(), p.getMasaLongName(), p.getGaurabdaYearLongString());

            int prevCountFest = 0;

            List<GPCalendarDay.Festival> allFestivals = p.CompleteFestivalList(db.get(i - 1), db.get(i + 1));
            f.Append("<div>");
            if (allFestivals.Count > 0)
            {
                StringBuilder sbt = new StringBuilder();
                sbt.AppendFormat("<table class=TodayFestBorder><tr><td class=TodayFestCell style='background:{0}'>\n", GetTodayFestivalBackground(p));
                foreach (GPCalendarDay.Festival fest in allFestivals)
                {
                    if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                    {
                        if (prevCountFest > 0)
                            sbt.Append("<br>");
                        sbt.Append(fest.Text);
                        prevCountFest++;
                    }
                }
                sbt.Append("</td></tr></table>\n");
                if (prevCountFest > 0)
                    f.Append(sbt);
            }
            f.Append("</div>");


            //f.Append("<p>&nbsp;</p>");

            /*BEGIN GCAL 1.4.3*/
            //List<string> gstr = GPStrings.getSharedStrings().gstr;

            f.Append("<table border=0 cellpadding=8><tr>");
            if (GPDisplays.Today.SunriseVisible())
            {
                f.AppendFormat("<td class=hed style='text-align:center'>{0}<br> <span style='font-size:110%'>{1}</span></td>",
                    GPStrings.getString(51), p.astrodata.sun.rise.getShortTimeString());
            }
            if (GPDisplays.Today.NoonVisible())
            {
                f.AppendFormat("<td class=hed style='text-align:center'>{0}<br> <span style='font-size:110%'>{1}</span></td>", GPStrings.getString(857), p.astrodata.sun.noon.getShortTimeString());
            }
            if (GPDisplays.Today.SunsetVisible())
            {
                f.AppendFormat("<td class=hed style='text-align:center'>{0}<br> <span style='font-size:110%'>{1}</span></td>", GPStrings.getString(52), p.astrodata.sun.set.getShortTimeString());
            }

            if (GPDisplays.Today.SandhyaTimesVisible())
            {
                f.Append("<tr>");
                if (GPDisplays.Today.SunriseVisible())
                {
                    f.AppendFormat("<td class=hed2 style='text-align:center'><span style='font-size:90%'>{0}</span><br><b>{1}</b></td>", GPStrings.getString(989), p.astrodata.sun.rise.getShortSandhyaRange());
                }
                if (GPDisplays.Today.NoonVisible())
                {
                    f.AppendFormat("<td class=hed2 style='text-align:center'><span style='font-size:90%'>{0}</span><br><b>{1}</b></td>", GPStrings.getString(989), p.astrodata.sun.noon.getShortSandhyaRange());
                }
                if (GPDisplays.Today.SunsetVisible())
                {
                    f.AppendFormat("<td class=hed2 style='text-align:center'><span style='font-size:90%'>{0}</span><br><b>{1}</b></td>", GPStrings.getString(989), p.astrodata.sun.set.getShortSandhyaRange());
                }
            }
            f.AppendLine("</table>");


            //
            // other info
            //

            //f.Append("<p>&nbsp;</p>");


            f.Append("<center><table width=\"80%\" cellpadding=8 cellspacing=0><tr><td style='text-align:center'>");
            if (GPDisplays.Today.SunriseInfo())
            {
                f.AppendFormat("<p style='text-align:left'><b style='color:rgb(0,100,0);'>{0}:</b> ", GPStrings.getString(990));
                //f.AppendFormat("<p><img src=\"{0}/separator.png\" width=320>", (baseDir == null ? "" : baseDir));

                //f.AppendFormat("<p >{0}: <b>{1}</b> ", GPStrings.getString(15), GPNaksatra.getName(p.astrodata.nNaksatra));
                if (GPDisplays.Today.NaksatraPadaVisible())
                {
                    f.AppendFormat("{0} {1} ({2})", p.getNaksatraElapsedString(), GPStrings.getString(993), GPStrings.getString(811 + p.getNaksatraPada()));
                }
                f.Append("; ");
                f.AppendFormat("{0}: <b>{1}</b>; ", GPStrings.getString(104), GPYoga.getName(p.astrodata.nYoga));
                if (GPDisplays.Today.RasiOfMoonVisible())
                {
                    f.AppendFormat("{0}: <b>{1}</b>; ", GPStrings.getString(991), GPSankranti.getName(p.astrodata.nMoonRasi));
                }
                f.AppendFormat("{0}: <b>{1}</b>", GPStrings.getString(992), GPSankranti.getName(p.astrodata.nSunRasi));
                f.Append("</p>");
            }

            sb.Remove(0, sb.Length);
            
            if (GPDisplays.Today.TithiList())
            {
                f.AppendFormat("<p style='text-align:left'><b style='color:rgb(0,100,0);'>{0}: </b>", GPStrings.getString(1005));
                //f.AppendFormat("<p><img src=\"{0}/separator.png\" width=320>", (baseDir == null ? "" : baseDir));

                GPLocalizedTithi current = p.getCurrentTithi();
                GPLocalizedTithi previous = current.getPreviousTithi();
                GPLocalizedTithi next = current.getNextTithi();

                //f.Append("<p style='text-align:left'>");
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4}; ", previous.getName(), 
                    previous.getStartTime().getShortDateString(), 
                    previous.getStartTime().getShortTimeString(),
                    previous.getEndTime().getShortDateString(), 
                    previous.getEndTime().getShortTimeString());
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4}; ", current.getName(), 
                    current.getStartTime().getShortDateString(), 
                    current.getStartTime().getShortTimeString(),
                    current.getEndTime().getShortDateString(), 
                    current.getEndTime().getShortTimeString());
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4};", next.getName(), 
                    next.getStartTime().getShortDateString(), 
                    next.getStartTime().getShortTimeString(),
                    next.getEndTime().getShortDateString(), 
                    next.getEndTime().getShortTimeString());
                f.Append("</p>");
            }

            if (GPDisplays.Today.NaksatraList())
            {
                f.AppendFormat("<p style='text-align:left'><b style='color:rgb(0,100,0);'>{0}: </b>", GPStrings.getString(1006));
//                f.AppendFormat("<p><img src=\"{0}/separator.png\" width=320></p>", (baseDir == null ? "" : baseDir));

                GPLocalizedNaksatra current = p.getCurrentNaksatra();
                GPLocalizedNaksatra previous = current.getPreviousNaksatra();
                GPLocalizedNaksatra next = current.getNextNaksatra();

                //f.Append("<p style='text-align:left'>");
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4}; ", previous.getName(),
                    previous.getStartTime().getShortDateString(),
                    previous.getStartTime().getShortTimeString(),
                    previous.getEndTime().getShortDateString(),
                    previous.getEndTime().getShortTimeString());
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4}; ", current.getName(),
                    current.getStartTime().getShortDateString(),
                    current.getStartTime().getShortTimeString(),
                    current.getEndTime().getShortDateString(),
                    current.getEndTime().getShortTimeString());
                f.AppendFormat("<b>{0}</b> {1}, {2} - {3}, {4};", next.getName(),
                    next.getStartTime().getShortDateString(),
                    next.getStartTime().getShortTimeString(),
                    next.getEndTime().getShortDateString(),
                    next.getEndTime().getShortTimeString());
                f.Append("</p>");

            }

            f.Append("</table>");
            f.Append("</center>");

            /* fprintf(f, "</html>");*/
            /* END GCAL 1.4.3 */
        }

        public static void WriteTodayInfoHTML(GPGregorianTime vc, GPLocationProvider loc, StringBuilder f, int fontSize, string baseDir)
        {
            GPCalendarResults db = new GPCalendarResults();
            GPGregorianTime vc2 = new GPGregorianTime(vc);
            vc2.setLocationProvider(loc);
            vc2.PreviousDay();
            vc2.PreviousDay();
            vc2.PreviousDay();
            vc2.PreviousDay();
            db.CalculateCalendar(vc2, 9);

            WriteTodayInfoHTML(vc, db, f, fontSize, baseDir);

        }

     }
}
