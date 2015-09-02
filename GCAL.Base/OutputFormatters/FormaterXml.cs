using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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

        public static XmlDocument GetAppDayXml(GPAppDayResults app)
        {
            XmlDocument doc = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            FormatAppDayXML(app, sb);
            doc.LoadXml(sb.ToString());
            return doc;
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
            string prefix = string.Format("{0} ", getSharedStringHtml(994));
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
                + "\" dayweekid = \"" + vcStart.getDayOfWeek() + "\" dayweek=\"" + getSharedStringHtml(vcStart.getDayOfWeek()) + "\" />\n");
            doc.Append("\t</result>\n");
            doc.Append("</xml>\n");

            return 0;
        }

        public static XmlDocument GetSankrantiXml(GPLocationProvider loc, GPGregorianTime vcStart, GPGregorianTime vcEnd)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement e1, e2, e3, eday, e5, e6;

            GPGregorianTime d = new GPGregorianTime(loc);
            int zodiac;

            d.Copy(vcStart);


            e1 = doc.CreateElement("xml");
            doc.AppendChild(e1);

            e2 = doc.CreateElement("request");
            e1.AppendChild(e2);
            e2.SetAttribute("name", "Sankranti");
            e2.SetAttribute("version", GPFileHelper.FileVersion);

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "longitude");
            e3.SetAttribute("val", loc.GetLongitudeEastPositive().ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "latitude");
            e3.SetAttribute("val", loc.GetLatitudeNorthPositive().ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "timezone");
            e3.SetAttribute("val", (loc.getTimeZone().OffsetSeconds / 60).ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "startdate");
            e3.SetAttribute("val", vcStart.ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "enddate");
            e3.SetAttribute("val", vcEnd.ToString());

            e2 = doc.CreateElement("result");
            e1.AppendChild(e2);
            e2.SetAttribute("name", "SankrantiList");



            while (d.IsBeforeThis(vcEnd))
            {
                d = GPSankranti.GetNextSankranti(d, out zodiac);

                eday = doc.CreateElement("sank");
                e2.AppendChild(eday);

                eday.SetAttribute("date", d.getLongDateString());
                eday.SetAttribute("time", d.getLongTimeString());
                eday.SetAttribute("dayweekid", d.getDayOfWeek().ToString());
                eday.SetAttribute("dayweek", getSharedStringHtml(d.getDayOfWeek()));

                e5 = doc.CreateElement("zodiac");
                eday.AppendChild(e5);

                e5.SetAttribute("sans", GPSankranti.GetNameSan(zodiac));
                e5.SetAttribute("eng", GPSankranti.GetNameEng(zodiac));
                e5.SetAttribute("id", zodiac.ToString());

                d.NextDay();
                d.NextDay();
            }

            return doc;
        }

        public static int WriteXml(XmlDocument xmlDoc, StringBuilder doc)
        {
            XmlWriter xw = XmlWriter.Create(doc);

            xmlDoc.WriteTo(xw);
            xw.Flush();

            return 1;
        }

        public static XmlDocument GetCalendarXmlDocument(GPCalendarResults daybuff)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement e1, e2, e3, eday, e5, e6;
            int k;
            string str, st;

            GPCalendarDay pvd;
            int nPrevMasa = -1;

            e1 = doc.CreateElement("xml");
            doc.AppendChild(e1);

            e2 = doc.CreateElement("request");
            e1.AppendChild(e2);
            e2.SetAttribute("name", "Calendar");
            e2.SetAttribute("version", GPFileHelper.FileVersion);

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "longitude");
            e3.SetAttribute("val", daybuff.CurrentLocation.getLocation(0).GetLongitudeEastPositive().ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "latitude");
            e3.SetAttribute("val", daybuff.CurrentLocation.getLocation(0).GetLatitudeNorthPositive().ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "timezone");
            e3.SetAttribute("val", (daybuff.CurrentLocation.getLocation(0).getTimeZone().OffsetSeconds / 60).ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "startdate");
            e3.SetAttribute("val", daybuff.m_vcStart.ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "daycount");
            e3.SetAttribute("val", daybuff.m_vcCount.ToString());

            e3 = doc.CreateElement("arg");
            e2.AppendChild(e3);
            e3.SetAttribute("name", "dst");
            e3.SetAttribute("val", daybuff.CurrentLocation.getLocation(0).getTimeZoneName());

            e2 = doc.CreateElement("result");
            e1.AppendChild(e2);
            e2.SetAttribute("name", "Calendar");

            e3 = doc.CreateElement("dstsystem");
            e2.AppendChild(e3);
            e3.SetAttribute("name", daybuff.CurrentLocation.getLocation(0).getTimeZoneName());

            for (k = 0; k < daybuff.m_vcCount; k++)
            {
                pvd = daybuff.get(k);
                if (pvd != null)
                {
                    if (nPrevMasa != pvd.astrodata.nMasa)
                    {
                        e3 = doc.CreateElement("masa");
                        e2.AppendChild(e3);
                        e3.SetAttribute("name", GPMasa.GetName(pvd.astrodata.nMasa) + " Masa" + (nPrevMasa == GPMasa.ADHIKA_MASA ? " " + getSharedStringHtml(109) : ""));
                        e3.SetAttribute("gyear", pvd.getGaurabdaYearLongString());
                    }

                    nPrevMasa = pvd.astrodata.nMasa;

                    eday = doc.CreateElement("day");
                    e3.AppendChild(eday);
                    eday.SetAttribute("date", pvd.date.ToString());
                    eday.SetAttribute("dayweekid", pvd.date.getDayOfWeek().ToString());
                    eday.SetAttribute("dayweek", getSharedStringHtml(150 + pvd.date.getDayOfWeek()));

                    // sunrise data
                    e5 = doc.CreateElement("sunrise");
                    eday.AppendChild(e5);
                    e5.SetAttribute("time", pvd.astrodata.sun.rise.getLongTimeString());

                    e6 = doc.CreateElement("tithi");
                    e5.AppendChild(e6);
                    e6.SetAttribute("name", pvd.getTithiNameExtended());
                    e6.SetAttribute("elapse", pvd.astrodata.nTithiElapse.ToString());
                    e6.SetAttribute("index", (pvd.astrodata.nTithi % 30 + 1).ToString());

                    e6 = doc.CreateElement("naksatra");
                    e5.AppendChild(e6);
                    e6.SetAttribute("name", pvd.getNaksatraName());
                    e6.SetAttribute("elapse", pvd.astrodata.nNaksatraElapse.ToString());

                    e6 = doc.CreateElement("yoga");
                    e5.AppendChild(e6);
                    e6.SetAttribute("name", pvd.getYogaName());

                    e6 = doc.CreateElement("paksa");
                    e5.AppendChild(e6);
                    e6.SetAttribute("id", GPPaksa.getAbbreviation(pvd.astrodata.nPaksa));
                    e6.SetAttribute("name", GPPaksa.getName(pvd.astrodata.nPaksa));

                    e5 = doc.CreateElement("dst");
                    eday.AppendChild(e5);
                    e5.SetAttribute("offset", pvd.date.getDaylightTimeBias().ToString());


                    // arunodaya data
                    e5 = doc.CreateElement("arunodaya");
                    eday.AppendChild(e5);
                    e5.SetAttribute("time", pvd.astrodata.sun.arunodaya.getLongTimeString());
                    
                    e6 = doc.CreateElement("tithi");
                    e5.AppendChild(e6);
                    e6.SetAttribute("name", GPTithi.getName(pvd.astrodata.getTithiAtArunodaya()));

                    e5 = doc.CreateElement("noon");
                    eday.AppendChild(e5);
                    e5.SetAttribute("time", pvd.astrodata.sun.noon.getLongTimeString());

                    e5 = doc.CreateElement("sunset");
                    eday.AppendChild(e5);
                    e5.SetAttribute("time", pvd.astrodata.sun.set.getLongTimeString());

                    if (pvd.hasEkadasiParana() && pvd.ekadasiParanaStart != null)
                    {
                        e5 = doc.CreateElement("parana");
                        eday.AppendChild(e5);
                        if (pvd.ekadasiParanaEnd != null)
                        {
                            e5.SetAttribute("from", pvd.ekadasiParanaStart.getShortTimeString());
                            e5.SetAttribute("to", pvd.ekadasiParanaEnd.getShortTimeString());
                        }
                        else
                        {
                            e5.SetAttribute("after", pvd.ekadasiParanaStart.getShortTimeString());
                        }
                    }
                    str = string.Empty;

                    foreach (GPCalendarDay.Festival fest in pvd.Festivals)
                    {
                        e5 = doc.CreateElement("festival");
                        eday.AppendChild(e5);
                        e5.SetAttribute("name", fest.Text);
                        e5.SetAttribute("class", fest.ShowSettingItem);
                    }

                    if (pvd.nFastType != GPConstants.FAST_NULL)
                    {
                        e5 = doc.CreateElement("fast");
                        eday.AppendChild(e5);

                        e5.SetAttribute("type", pvd.nFastType.ToString());
                        e5.SetAttribute("mark", "*");
                    }

                    if (pvd.sankranti_zodiac >= 0)
                    {
                        e5 = doc.CreateElement("sankranti");
                        eday.AppendChild(e5);
                        e5.SetAttribute("rasi", GPSankranti.getName(pvd.sankranti_zodiac));
                        e5.SetAttribute("time", pvd.sankranti_day.getLongTimeString());
                    }

                    if (pvd.hasKsayaTithi())
                    {
                        GPGregorianTime vcStart = pvd.ksayaTithi.getStartTime();
                        GPGregorianTime vcEnd = pvd.ksayaTithi.getEndTime();

                        e5 = doc.CreateElement("ksaya");
                        eday.AppendChild(e5);
                        e5.SetAttribute("from", vcStart.ToString() + " " + vcStart.getShortTimeString());
                        e5.SetAttribute("to", vcEnd.ToString() + " " + vcEnd.getShortTimeString());
                    }


                    if (pvd.IsSecondDayTithi)
                    {
                        e5 = doc.CreateElement("vriddhi");
                        eday.AppendChild(e5);
                        e5.SetAttribute("sd", "yes");
                    }

                }
            }

            return doc;
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
