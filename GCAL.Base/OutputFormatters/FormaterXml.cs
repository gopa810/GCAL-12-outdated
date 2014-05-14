using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterXml : Formatter
    {
        public static int FormatEventsXML(GPCoreEventResults inEvents, StringBuilder strXml)
        {

            int i;

            strXml.AppendFormat("<xml>\r\n<program version=\"{0}\">\r\n<location longitude=\"{1}\" latitude=\"{2}\" timezone=\"{3}\"/>\n"
                , GPFileHelper.FileVersion, inEvents.m_location.getLocation(0).GetLongitudeEastPositive(), 
                inEvents.m_location.getLocation(0).GetLatitudeNorthPositive()
                , inEvents.m_location.getLocation(0).getTimeZoneName());
            GPGregorianTime prevd = new GPGregorianTime(inEvents.m_location);

            prevd.setDate(1800, 1, 1);
            for (i = 0; i < inEvents.getCount(); i++)
            {
                GPCoreEvent dnr = inEvents.get(i);

                if (inEvents.b_sorted)
                {
                    if (prevd.getDay() != dnr.Time.getDay() || prevd.getMonth() != dnr.Time.getMonth() || prevd.getYear() != dnr.Time.getYear())
                    {
                        strXml.AppendFormat("\t<day date=\"{0}\" />\n", dnr.Time);
                    }
                    prevd = dnr.Time;
                }

                strXml.AppendFormat("\t<event type=\"{0}\" time=\"{1}\" />\n", dnr.getEventTitle(), dnr.Time.getLongTimeString());

            }

            strXml.Append("</xml>\n");

            return 1;
        }

        public static void FormatAppDayXML(GPAppDayResults app, StringBuilder strResult)
        {
            GPAstroData d = app.details;
            string str;
            GPGregorianTime vc = app.evente;
            StringBuilder strText = strResult;
            int npada;
            bool bDuringAdhika = false;

            strText.AppendFormat(
                "<xml>\n" +
                "\t<request name=\"AppDay\" version=\"{0}\">\n" +
                "\t\t<arg name=\"longitude\" value=\"{1}\" />\n" +
                "\t\t<arg name=\"latitude\" value=\"{2}\" />\n" +
                "\t\t<arg name=\"timezone\" value=\"{3}\" />\n" +
                "\t\t<arg name=\"year\" value=\"{4}\" />\n" +
                "\t\t<arg name=\"month\" value=\"{5}\" />\n" +
                "\t\t<arg name=\"day\" value=\"{6}\" />\n" +
                "\t\t<arg name=\"hour\" value=\"{7}\" />\n" +
                "\t\t<arg name=\"minute\" value=\"{8}\" />\n" +
                "\t</request>\n", GPFileHelper.FileVersion,
                app.evente.getLocation().GetLongitudeEastPositive(), app.evente.getLocation().GetLatitudeNorthPositive(), 
                app.evente.getLocation().getTimeZoneName(),
                app.evente.getYear(), app.evente.getMonth(), app.evente.getDay(), app.evente.getHour(), app.evente.getMinuteRound()
                );


            npada = Convert.ToInt32(d.nNaksatraElapse / 25.0) + 1;
            if (npada > 4)
                npada = 4;

            str = string.Format("\t<result name=\"AppDay\" >\n" +
                "\t\t<tithi name=\"{0}\" elapse=\"{1}\" />\n" +
                "\t\t<naksatra name=\"{2}\" elapse=\"{3}\" pada=\"{4}\"/>\n" +
                "\t\t<paksa name=\"{5}\" />\n" +
                "\t\t<masa name=\"{6}\" adhikamasa=\"{7}\"/>\n" +
                "\t\t<gaurabda value=\"{8}\" />\n"

                , GPTithi.getName(d.nTithi), d.nTithiElapse
                , GPNaksatra.getName(d.nNaksatra), d.nNaksatraElapse, npada
                , GPPaksa.getName(d.nPaksa)
                , GPMasa.GetName(d.nMasa), (bDuringAdhika ? "yes" : "no")
                , d.nGaurabdaYear
                );

            strText.Append(str); ;
            string prefix = string.Format("{0} ", getSharedString(994));
            strText.Append("\t\t<celebrations>\n");
            foreach(GPStringPair rec in app.output)
            {
                if (rec.Name.StartsWith(prefix))
                {
                    str = string.Format("\t\t\t<celebration gaurabda=\"{0}\" date=\"{1}\" />\n", rec.Name, rec.Value);
                    strText.Append(str);
                }
            }

            strText.Append("\t\t</celebrations>\n\t</result>\n</xml>\n");
        }

        public static int WriteXML_FirstDay_Year(StringBuilder doc, GPGregorianTime vcStart)
        {
            vcStart = GPGaurabdaYear.getFirstDayOfYear(vcStart.getLocationProvider(), vcStart.getYear());

            // write
            doc.Append("<xml>\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + vcStart.getLocation().GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + vcStart.getLocation().GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"year\" val=\"" + vcStart.getYear() + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"FirstDay_of_GaurabdaYear\">\n");
            doc.Append("\t\t<firstday date=\"" + vcStart
                + "\" dayweekid = \"" + vcStart.getDayOfWeek() + "\" dayweek=\"" + getSharedString(vcStart.getDayOfWeek()) + "\" />\n");
            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");

            return 0;
        }

        public static int WriteXML_Sankrantis(StringBuilder doc, GPLocationProvider loc, GPGregorianTime vcStart, GPGregorianTime vcEnd)
        {
            GPGregorianTime d = new GPGregorianTime(loc);
            int zodiac;

            d.Copy(vcStart);

            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Sankranti\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + loc.GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + loc.GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + loc.getTimeZone().OffsetSeconds / 60 + "\" />\n");
            doc.Append("\t\t<arg name=\"location\" val=\"" + loc.getName() + "\" />\n");
            doc.Append("\t\t<arg name=\"startdate\" val=\"" + vcStart + "\" />\n");
            doc.Append("\t\t<arg name=\"enddate\" val=\"" + vcEnd + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"SankrantiList\">\n");

            while (d.IsBeforeThis(vcEnd))
            {
                d = GPSankranti.GetNextSankranti(d, out zodiac);
                doc.Append("\t\t<sank date=\"" + d + "\" ");
                doc.Append("dayweekid=\"" + d.getDayOfWeek() + "\" dayweek=\"" + getSharedString(d.getDayOfWeek()) + "\" ");
                doc.Append(" time=\"" + d.getShortTimeString() + "\" >\n");
                doc.Append("\t\t<zodiac sans=\"" + GPSankranti.GetNameSan(zodiac));
                doc.Append("\" eng=\"" + GPSankranti.GetNameEng(zodiac) + "\" id=\"" + zodiac + "\" />\n");
                doc.Append("\t\t</sank>\n\n");

                d.NextDay();
                d.NextDay();
            }

            doc.Append("\t</result>\n");
            doc.Append("</xml>");

            return 1;
        }

        public static int WriteCalendarXml(GPCalendarResults daybuff, StringBuilder doc)
        {
            int k;
            string str, st;

            GPCalendarDay pvd;
            int nPrevMasa = -1;

            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Calendar\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + daybuff.m_Location.getLocation(0).GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + daybuff.m_Location.getLocation(0).GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + daybuff.m_Location.getLocation(0).getTimeZone().OffsetSeconds / 60 + "\" />\n");
            doc.Append("\t\t<arg name=\"startdate\" val=\"" + daybuff.m_vcStart + "\" />\n");
            doc.Append("\t\t<arg name=\"daycount\" val=\"" + daybuff.m_vcCount + "\" />\n");
            doc.Append("\t\t<arg name=\"dst\" val=\"" + daybuff.m_Location.getLocation(0).getTimeZoneName() + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"Calendar\">\n");
            doc.Append("\t<dstsystem name=\"" + daybuff.m_Location.getLocation(0).getTimeZoneName() + "\" />\n");

            for (k = 0; k < daybuff.m_vcCount; k++)
            {
                pvd = daybuff.get(k);
                if (pvd != null)
                {
                    if (nPrevMasa != pvd.astrodata.nMasa)
                    {
                        if (nPrevMasa != -1)
                            doc.Append("\t</masa>\n");
                        doc.Append("\t<masa name=\"" + GPMasa.GetName(pvd.astrodata.nMasa) + " Masa");
                        if (nPrevMasa == GPMasa.ADHIKA_MASA)
                            doc.Append(" " + getSharedString(109));
                        doc.Append("\"");
                        doc.Append(" gyear=\"Gaurabda " + pvd.getGaurabdaYearLongString() + "\"");
                        doc.Append(">\n");
                    }

                    nPrevMasa = pvd.astrodata.nMasa;

                    // date data
                    doc.Append("\t<day date=\"" + pvd.date + "\" dayweekid=\"" + pvd.date.getDayOfWeek());
                    doc.Append("\" dayweek=\"");
                    st = getSharedString(pvd.date.getDayOfWeek()).Substring(0, 2);
                    doc.Append(st + "\">\n");

                    // sunrise data
                    doc.Append("\t\t<sunrise time=\"" + pvd.astrodata.sun.rise + "\">\n");

                    doc.Append("\t\t\t<tithi name=\"");
                    doc.Append(GPTithi.getName(pvd.astrodata.nTithi));
                    if ((pvd.astrodata.nTithi == 10) || (pvd.astrodata.nTithi == 25)
                        || (pvd.astrodata.nTithi == 11) || (pvd.astrodata.nTithi == 26))
                    {
                        if (pvd.hasEkadasiParana() == false)
                        {
                            if (pvd.nMahadvadasiType == GPConstants.EV_NULL)
                            {
                                doc.Append(" " + getSharedString(58));
                            }
                            else
                            {
                                doc.Append(" " + getSharedString(59));
                            }
                        }
                    }
                    str = string.Format("\" elapse=\"{0}\" index=\"{1}\"/>\n"
                        , pvd.astrodata.nTithiElapse, pvd.astrodata.nTithi % 30 + 1);
                    doc.Append(str);

                    str = string.Format("\t\t\t<naksatra name=\"{0}\" elapse=\"{1}\" />\n"
                        , GPNaksatra.getName(pvd.astrodata.nNaksatra), pvd.astrodata.nNaksatraElapse);
                    doc.Append(str);

                    str = string.Format("\t\t\t<yoga name=\"{0}\" />\n", GPYoga.getName(pvd.astrodata.nYoga));
                    doc.Append(str);

                    str = string.Format("\t\t\t<paksa id=\"{0}\" name=\"{1}\"/>\n", GPPaksa.getAbbreviation(pvd.astrodata.nPaksa), GPPaksa.getName(pvd.astrodata.nPaksa));
                    doc.Append(str);

                    doc.Append("\t\t</sunrise>\n");

                    doc.Append("\t\t<dst offset=\"" + pvd.date.getDaylightTimeBias() + "\" />\n");
                    // arunodaya data
                    doc.Append("\t\t<arunodaya time=\"" + pvd.astrodata.sun.arunodaya + "\">\n");
                    doc.Append("\t\t\t<tithi name=\"" + GPTithi.getName(pvd.astrodata.getTithiAtArunodaya()) + "\" />\n");
                    doc.Append("\t\t</arunodaya>\n");

                    str = string.Empty;

                    doc.Append("\t\t<noon time=\"" + pvd.astrodata.sun.noon + "\" />\n");

                    doc.Append("\t\t<sunset time=\"" + pvd.astrodata.sun.set + "\" />\n");

                    // moon data
                    doc.Append("\t\t<moon rise=\"" + pvd.moonrise + "\" set=\"" + pvd.moonset + "\" />\n");

                    if (pvd.hasEkadasiParana() && pvd.ekadasiParanaStart != null)
                    {
                        if (pvd.ekadasiParanaEnd != null)
                        {
                            str = string.Format("\t\t<parana from=\"{0}\" to=\"{1}\" />\n", pvd.ekadasiParanaStart.getShortTimeString(), pvd.ekadasiParanaEnd.getShortTimeString());
                        }
                        else
                        {
                            str = string.Format("\t\t<parana after=\"{0}\" />\n",pvd.ekadasiParanaStart.getShortTimeString());
                        }
                        doc.Append(str);
                    }
                    str = string.Empty;

                    foreach (GPCalendarDay.Festival fest in pvd.Festivals)
                    {
                        doc.AppendFormat("\t\t<festival name=\"{0}\" class=\"{1}\" />\n", fest.Text, fest.ShowSettingItem);
                    }

                    if (pvd.nFastType != GPConstants.FAST_NULL)
                    {
                        if (pvd.nFastType == GPConstants.FAST_EKADASI)
                        {
                            doc.Append("\t\t<fast type=\"" + pvd.nFastType + "\" mark=\"*\" />\n");
                        }
                        else
                        {
                            doc.Append("\t\t<fast type=\"\" mark=\"\" />\n");
                        }
                    }

                    if (pvd.sankranti_zodiac >= 0)
                    {
                        //double h1, m1, s1;
                        //m1 = modf(pvd.sankranti_day.shour*24, &h1);
                        //				s1 = modf(m1*60, &m1);
                        str = string.Format("\t\t<sankranti rasi=\"{0}\" time=\"{1}\" />\n"
                            , GPSankranti.getName(pvd.sankranti_zodiac), pvd.sankranti_day.getLongTimeString());
                        doc.Append(str);
                    }

                    if (pvd.hasKsayaTithi())
                    {
                        GPGregorianTime vcStart = pvd.ksayaTithi.getStartTime();
                        GPGregorianTime vcEnd = pvd.ksayaTithi.getEndTime();
                        str = string.Format("\t\t<ksaya from=\"{0} {1}\" to=\"{2} {3}\" />\n", vcStart.ToString(), vcStart.getShortTimeString(), vcEnd.ToString(), vcEnd.getShortTimeString());
                        doc.Append(str);
                    }

                    if (pvd.IsSecondDayTithi)
                    {
                        doc.Append("\t\t<vriddhi sd=\"yes\" />\n");
                    }
                    else
                    {
                        doc.Append("\t\t<vriddhi sd=\"no\" />\n");
                    }

                    doc.Append("\t</day>\n\n");

                }
            }
            doc.Append("\t</masa>\n");
            doc.Append("</result>\n" + "</xml>\n");

            return 1;
        }

        public static int WriteXML_Naksatra(StringBuilder doc, GPLocationProvider loc, GPGregorianTime vc, int nDaysCount)
        {
            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Naksatra\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + loc.GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + loc.GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + loc.getTimeZone().OffsetSeconds / 60 + "\" />\n");
            doc.Append("\t\t<arg name=\"startdate\" val=\"" + vc + "\" />\n");
            doc.Append("\t\t<arg name=\"daycount\" val=\"" + nDaysCount + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"Naksatra\">\n");

            GPGregorianTime d = new GPGregorianTime(vc);
            GPGregorianTime dn = new GPGregorianTime(loc);
            GPSun sun = new GPSun();
            int nak;

            for (int i = 0; i < 30; i++)
            {
                nak = GPNaksatra.GetNextNaksatra(d, out dn);
                d.Copy(dn);
                doc.Append("\t\t<day date=\"" + d + "\">\n");
                //str = string.Format("{}.{}.{}", d.day, d.month, d.year);
                //n = m_list.InsertItem(50, GPNaksatra.GetName(nak));
                //m_list.SetItemText(n, 1, str);
                doc.Append("\t\t\t<naksatra id=\"" + nak + "\" name=\"" + GPNaksatra.getName(nak) + "\"\n");
                //dt.SetDegTime(d.getDayHours() * 360);
                //time_print(str, dt);
                doc.Append("\t\t\t\tstarttime=\"" + d.getShortTimeString() + "\" />\n");
                //m_list.SetItemText(n, 2, str);

                // sunrise time get
                sun.SunCalc(d, loc);
                //time_print(str, sun.rise);
                //m_list.SetItemText(n, 3, str);
                doc.Append("\t\t\t<sunrise time=\"" + sun.rise + "\" />\n");

                doc.Append("\t\t</day>\n");
                // increment for non-duplication of naksatra
                d.Copy(dn);
                d.setDayHours( d.getDayHours() + 1.0 / 8.0);
            }


            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");


            return 1;
        }

        public static int WriteXML_Tithi(StringBuilder doc, GPLocationProvider loc, GPGregorianTime vc)
        {
            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Tithi\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + vc.getLocation().GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + vc.getLocation().GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + vc.getLocation().getTimeZone().OffsetSeconds / 60 + "\" />\n");
            doc.Append("\t\t<arg name=\"startdate\" val=\"" + vc + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"Tithi\">\n");

            GPGregorianTime d = new GPGregorianTime(vc);
            GPGregorianTime d1 = new GPGregorianTime(loc);
            GPGregorianTime d2 = new GPGregorianTime(loc);


            GPAstroData day = new GPAstroData();

            day.calculateDayData(vc, loc);

            d.setDayHours(day.sun.getSunriseDayHours());

            GPTithi.GetPrevTithiStart(d, out d1);
            GPTithi.GetNextTithiStart(d, out d2);


            //dt.SetDegTime(d1.getDayHours() * 360);
            // start tithi at t[0]
            doc.Append("\t\t<tithi\n\t\t\tid=\"" + day.nTithi + "\"\n");
            doc.Append("\t\t\tname=\"" + GPTithi.getName(day.nTithi) + "\"\n");
            doc.Append("\t\t\tstartdate=\"" + d1.getShortDateString() + "\"\n");
            doc.Append("\t\t\tstarttime=\"" + d1.getShortTimeString() + "\"\n");

            //dt.SetDegTime(d2.getDayHours() * 360);
            doc.Append("\t\t\tenddate=\"" + d2.getShortDateString() + "\"\n");
            doc.Append("\t\t\tendtime=\"" + d2.getShortTimeString() + "\"\n />");


            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");

            return 1;
        }

        public static int WriteXML_GaurabdaTithi(StringBuilder doc, GPLocationProvider loc, GPVedicTime vaStart, GPVedicTime vaEnd)
        {
            int gyearA = vaStart.gyear;
            int gyearB = vaEnd.gyear;
            int gmasa = vaStart.masa;
            int gpaksa = vaStart.tithi / 15;
            int gtithi = vaStart.tithi % 15;

            if (gyearB < gyearA)
                gyearB = gyearA;



            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Tithi\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + loc.getLocation(0).GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + loc.getLocation(0).GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + loc.getLocation(0).getTimeZone().OffsetSeconds / 60 + "\" />\n");
            if (gyearA > 1500)
            {
                doc.Append("\t\t<arg name=\"year-start\" val=\"" + gyearA + "\" />\n");
                doc.Append("\t\t<arg name=\"year-end\" val=\"" + gyearB + "\" />\n");
            }
            else
            {
                doc.Append("\t\t<arg name=\"gaurabdayear-start\" val=\"" + gyearA + "\" />\n");
                doc.Append("\t\t<arg name=\"gaurabdayear-end\" val=\"" + gyearB + "\" />\n");
            }
            doc.Append("\t\t<arg name=\"masa\" val=\"" + gmasa + "\" />\n");
            doc.Append("\t\t<arg name=\"paksa\" val=\"" + gpaksa + "\" />\n");
            doc.Append("\t\t<arg name=\"tithi\" val=\"" + gtithi + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"Tithi\">\n");


            GPGregorianTime vcs = new GPGregorianTime(loc), vce = new GPGregorianTime(loc), today = new GPGregorianTime(loc);
            GPSun sun = new GPSun();
            int A, B;
            double sunrise;
            GPAstroData day = new GPAstroData();
            int oTithi, oPaksa, oMasa, oYear;

            if (gyearA > 1500)
            {
                A = gyearA - 1487;
                B = gyearB - 1485;
            }
            else
            {
                A = gyearA;
                B = gyearB;
            }

            for (; A <= B; A++)
            {
                vcs = GPTithi.CalcTithiEnd(A, gmasa, gpaksa, gtithi, loc, out vce);
                if (gyearA > 1500)
                {
                    if ((vcs.getYear() < gyearA) || (vcs.getYear() > gyearB))
                        continue;
                }
                oTithi = gpaksa * 15 + gtithi;
                oMasa = gmasa;
                oPaksa = gpaksa;
                oYear = 0;
                doc.Append("\t<celebration\n");
                doc.Append("\t\trtithi=\"" + GPTithi.getName(oTithi) + "\"\n");
                doc.Append("\t\trmasa=\"" + GPMasa.GetName(oMasa) + "\"\n");
                doc.Append("\t\trpaksa=\"" + GPPaksa.getName(oPaksa) + "\"\n");
                // test ci je ksaya
                today.Copy(vcs);
                today.setDayHours(0.5);
                sun.SunCalc(today, loc);
                sunrise = sun.getSunriseDayHours();
                if (sunrise < vcs.getDayHours())
                {
                    today.Copy(vce);
                    sun.SunCalc(today, loc);
                    sunrise = sun.getSunriseDayHours();
                    if (sunrise < vce.getDayHours())
                    {
                        // normal type
                        vcs.NextDay();
                        doc.Append("\t\ttype=\"normal\"\n");
                    }
                    else
                    {
                        // ksaya
                        vcs.NextDay();
                        day.calculateDayData(vcs, loc);
                        oTithi = day.nTithi;
                        oPaksa = day.nPaksa;
                        oMasa = day.determineMasa(vcs, out oYear);
                        doc.Append("\t\ttype=\"ksaya\"\n");
                    }
                }
                else
                {
                    // normal, alebo prvy den vriddhi
                    today.Copy(vce);
                    sun.SunCalc(today, loc);
                    if (sun.getSunriseDayHours() < vce.getDayHours())
                    {
                        // first day of vriddhi type
                        doc.Append("\t\ttype=\"vriddhi\"\n");
                    }
                    else
                    {
                        // normal
                        doc.Append("\t\ttype=\"normal\"\n");
                    }
                }
                doc.Append("\t\tdate=\"" + vcs + "\"\n");
                doc.Append("\t\totithi=\"" + GPTithi.getName(oTithi) + "\"\n");
                doc.Append("\t\tomasa=\"" + GPMasa.GetName(oMasa) + "\"\n");
                doc.Append("\t\topaksa=\"" + GPPaksa.getName(oPaksa) + "\"\n");
                doc.Append("\t/>\n");
                //		doc.Append( "\t\t</celebration>\n");

            }


            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");

            return 1;
        }

        public static int WriteXML_GaurabdaNextTithi(StringBuilder doc, GPLocationProvider loc, GPGregorianTime vcStart, GPVedicTime vaStart)
        {
            int gmasa, gpaksa, gtithi;


            gmasa = vaStart.masa;
            gpaksa = vaStart.tithi / 15;
            gtithi = vaStart.tithi % 15;

            doc.Append("<xml>\n");
            doc.Append("\t<request name=\"Tithi\" version=\"" + GPFileHelper.FileVersion + "\">\n");
            doc.Append("\t\t<arg name=\"longitude\" val=\"" + loc.GetLongitudeEastPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"latitude\" val=\"" + loc.GetLatitudeNorthPositive() + "\" />\n");
            doc.Append("\t\t<arg name=\"timezone\" val=\"" + loc.getTimeZone().OffsetSeconds / 60 + "\" />\n");
            doc.Append("\t\t<arg name=\"start date\" val=\"" + vcStart + "\" />\n");
            doc.Append("\t\t<arg name=\"masa\" val=\"" + gmasa + "\" />\n");
            doc.Append("\t\t<arg name=\"paksa\" val=\"" + gpaksa + "\" />\n");
            doc.Append("\t\t<arg name=\"tithi\" val=\"" + gtithi + "\" />\n");
            doc.Append("\t</request>\n");
            doc.Append("\t<result name=\"Tithi\">\n");

            GPGregorianTime vcs = new GPGregorianTime(loc);
            GPGregorianTime vce = new GPGregorianTime(loc);
            GPGregorianTime today = new GPGregorianTime(loc);
            GPSun sun = new GPSun();
            int A;
            double sunrise;
            GPAstroData day = new GPAstroData();
            int oTithi, oPaksa, oMasa, oYear;

            today.Copy(vcStart);
            today.PreviousDay();
            vcStart.SubDays(15);
            for (A = 0; A <= 3; A++)
            {
                vcs = GPTithi.CalcTithiEndEx(vcStart, 0, gmasa, gpaksa, gtithi, loc, out vce);
                if (!vcs.IsBeforeThis(today))
                {
                    oTithi = gpaksa * 15 + gtithi;
                    oMasa = gmasa;
                    oPaksa = gpaksa;
                    oYear = 0;
                    doc.Append("\t<celebration\n");
                    //		doc.Append( "\t\t<tithi\n");
                    doc.Append("\t\trtithi=\"" + GPTithi.getName(oTithi) + "\"\n");
                    doc.Append("\t\trmasa=\"" + GPMasa.GetName(oMasa) + "\"\n");
                    doc.Append("\t\trpaksa=\"" + GPPaksa.getName(oPaksa) + "\"\n");
                    // test ci je ksaya
                    today.Copy(vcs);
                    today.setDayHours(0.5);
                    sun.SunCalc(today, loc);
                    sunrise = sun.getSunriseDayHours();
                    if (sunrise < vcs.getDayHours())
                    {
                        today.Copy(vce);
                        sun.SunCalc(today, loc);
                        sunrise = sun.getSunriseDayHours();
                        if (sunrise < vce.getDayHours())
                        {
                            // normal type
                            vcs.NextDay();
                            doc.Append("\t\ttype=\"normal\"\n");
                        }
                        else
                        {
                            // ksaya
                            vcs.NextDay();
                            day.calculateDayData(vcs, loc);
                            oTithi = day.nTithi;
                            oPaksa = day.nPaksa;
                            oMasa = day.determineMasa(vcs, out oYear);
                            doc.Append("\t\ttype=\"ksaya\"\n");
                        }
                    }
                    else
                    {
                        // normal, alebo prvy den vriddhi
                        today.Copy(vce);
                        sun.SunCalc(today, loc);
                        if (sun.getSunriseDayHours() < vce.getDayHours())
                        {
                            // first day of vriddhi type
                            doc.Append("\t\ttype=\"vriddhi\"\n");
                        }
                        else
                        {
                            // normal
                            doc.Append("\t\ttype=\"normal\"\n");
                        }
                    }
                    doc.Append("\t\tdate=\"" + vcs + "\"\n");
                    doc.Append("\t\totithi=\"" + GPTithi.getName(oTithi) + "\"\n");
                    doc.Append("\t\tomasa=\"" + GPMasa.GetName(oMasa) + "\"\n");
                    doc.Append("\t\topaksa=\"" + GPPaksa.getName(oPaksa) + "\"\n");
                    doc.Append("\t/>\n");
                    break;
                }
                else
                {
                    vcStart.Copy(vcs);
                    vcs.NextDay();
                }
            }

            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");


            return 1;
        }


    }
}
