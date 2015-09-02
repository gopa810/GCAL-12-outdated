using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterICAL : Formatter
    {
        public static int FormatCalendarICAL(GPCalendarResults daybuff, StringBuilder m_text)
        {
            int k;
            int initialLength = 0;
            int lastmasa = -1;
            int tzoffset = 0, tzoff;
            string str, str2;
            StringBuilder dayText = new StringBuilder();
            GPCalendarDay pvd, prevd, nextd;
            string SPACE_BEFORE_LINE = " , ";
            GPGregorianTime vc = new GPGregorianTime(daybuff.m_vcStart);
            GPLocation loc = daybuff.CurrentLocation.getLocation(0);

            DateTime st = new DateTime();

            m_text.Remove(0, m_text.Length); 
            m_text.Append("BEGIN:VCALENDAR\nVERSION:2.0\nX-WR-CALNAME:VAISNAVA\nPRODID:-//GBC Calendar Comitee//GCAL//EN\n");
            m_text.Append("X-WR-RELCALID:");
            str2 = string.Format("{0:00000000}-{1:0000}-{2:0000}-{3:0000}-{4:0000}{5:00000000}", st.Year + st.Millisecond, st.Day, st.Month,
                st.Hour, st.Minute + st.Millisecond);
            m_text.Append(str2);
            m_text.Append("\nX-WR-TIMEZONE:");

            m_text.Append(loc.getTimeZoneName());
            m_text.Append("\n");

            m_text.Append("CALSCALE:GREGORIAN\nMETHOD:PUBLISH\n");
            m_text.Append("BEGIN:VTIMEZONE\nTZID:");
            m_text.Append(loc.getTimeZoneName());
            str2 = string.Format("\nLAST-MODIFIED:{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", st.Year, st.Month, st.Day, st.Hour, st.Minute, st.Second);
            m_text.Append(str2);

            tzoffset = Convert.ToInt32(loc.getTimeZone().OffsetSeconds / 60);
            tzoff = Convert.ToInt32(loc.getTimeZone().getMaximumOffsetSeconds() / 60);

            if (loc.getTimeZone().hasDstInYear(st.Year))
            {
                DateTime dta = loc.getTimeZone().StartDateInYear(st.Year);
                str2 = string.Format("\nBEGIN:DAYLIGHT\nDTSTART:{0:0000}{1:00}{2:00}T{3:00}0000", dta.Year, dta.Month, dta.Day, dta.Hour);
                m_text.Append(str2);

                str2 = string.Format("\nTZOFFSETTO:{0}{1:00}{2:00}", (tzoff > 0 ? '+' : '-'), Math.Abs(tzoff) / 60, Math.Abs(tzoff) % 60);
                m_text.Append(str2);

                str2 = string.Format("\nTZOFFSETFROM:{0}{1:00}{2:00}", '+', 0, 0);
                m_text.Append(str2);

                dta = loc.getTimeZone().EndDateInYear(st.Year);
                m_text.Append("\nEND:DAYLIGHT\nBEGIN:STANDARD\nDTSTART:");
                str2 = string.Format("{0:0000}{1:00}{2:00}T{3:00}0000", dta.Year, dta.Month, dta.Day, dta.Hour);
                m_text.Append(str2);

                str2 = string.Format("\nTZOFFSETTO:{0}{1:00}{2:00}", (tzoffset > 0 ? '+' : '-'), Math.Abs(tzoffset) / 60, Math.Abs(tzoffset) % 60);
                m_text.Append(str2);
                str2 = string.Format("\nTZOFFSETFROM:{0}{1:00}{2:00}", (tzoff > 0 ? '+' : '-'), Math.Abs(tzoff) / 60, Math.Abs(tzoff) % 60);
                m_text.Append(str2);
                m_text.Append("\nEND:STANDARD\n");
            }
            else
            {
                m_text.Append("\nBEGIN:STANDARD\nDTSTART:");
                str2 = string.Format("{0:0000}0101T000000", vc.getYear(), vc.getMonth(), vc.getDay(), vc.getHour());
                m_text.Append(str2);

                str2 = string.Format("\nTZOFFSETTO:%+02d{0:00}", Math.Abs(tzoffset) / 60, Math.Abs(tzoffset) % 60);
                m_text.Append(str2);
                str2 = string.Format("\nTZOFFSETFROM:+0000");
                m_text.Append(str2);
                m_text.Append("\nEND:STANDARD\n");
            }

            m_text.Append("END:VTIMEZONE\n");

            for (k = 0; k < daybuff.m_PureCount; k++)
            {
                //		date.shour = 0.0;
                //		date.TimeZone = earth.tzone;

                prevd = daybuff.get(k - 1);
                pvd = daybuff.get(k);
                nextd = daybuff.get(k + 1);

                if (pvd != null)
                {
                    dayText.Remove(0, dayText.Length);

                    if (pvd.astrodata.nMasa != lastmasa)
                    {
                        str = string.Format("{0} {1}, Gaurabda {2}", GPMasa.GetName(pvd.astrodata.nMasa), getSharedStringHtml(22), pvd.astrodata.nGaurabdaYear);
                        dayText.Append(str);
                        dayText.Append("\n");
                        if ((pvd.astrodata.nMasa == GPMasa.ADHIKA_MASA) && ((lastmasa >= GPMasa.SRIDHARA_MASA) && (lastmasa <= GPMasa.DAMODARA_MASA)))
                        {
                            if (dayText.Length > 0)
                                dayText.Append(SPACE_BEFORE_LINE);
                            dayText.Append(getSharedStringHtml(128));
                            dayText.Append("\n");
                        }

                        lastmasa = pvd.astrodata.nMasa;
                        initialLength = -1;
                    }
                    else
                    {
                        initialLength = 0;
                    }

                    if (dayText.Length > 0)
                        dayText.Append(SPACE_BEFORE_LINE);
                    dayText.Append(GPTithi.getName(pvd.astrodata.nTithi));

                    if ((pvd.astrodata.nTithi == 10) || (pvd.astrodata.nTithi == 25) || (pvd.astrodata.nTithi == 11) || (pvd.astrodata.nTithi == 26))
                    {
                        if (pvd.hasEkadasiParana() == false)
                        {
                            dayText.Append(" ");
                            if (pvd.nMahadvadasiType == GPConstants.EV_NULL)
                            {
                                dayText.Append(getSharedStringHtml(58));
                            }
                            else
                            {
                                dayText.Append(getSharedStringHtml(59));
                            }
                        }
                    }
                    dayText.Append("\n");
                    initialLength += dayText.Length;

                    if (pvd.astrodata.sun.eclipticalLongitude < 0.0)
                    {
                        goto _resolve_text;
                    }

                    //			if (0 != GetShowSetVal(17) == 1)
                    {
                        //				double h1, m1;
                        if (pvd.hasEkadasiParana())
                        {
                            str = pvd.getEkadasiParanaString();

                            dayText.Append(SPACE_BEFORE_LINE);
                            dayText.Append(str);
                            dayText.Append("\n");


                        }
                    }

                    //			if (0 != GetShowSetVal(6) == 1)
                    {
                        foreach (GPCalendarDay.Festival fest in pvd.Festivals)
                        {
                            if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                            {
                                        dayText.Append(SPACE_BEFORE_LINE);
                                        dayText.Append(fest.Text);
                                        dayText.Append("\n");
                            }
                        }
                    }

                    if (/*GetShowSetVal(16) == 1 &&*/ pvd.sankranti_zodiac >= 0)
                    {
                        str = string.Format(" {0} {1}", GPSankranti.getName(pvd.sankranti_zodiac), getSharedStringHtml(56));
                        dayText.Append(SPACE_BEFORE_LINE);
                        dayText.Append(str);
                        dayText.Append("\n");
                    }



                    //"Sunrise Time",//2
                    //"Sunset Time",//3
                    if (GPDisplays.Calendar.TimeSunriseVisible())
                    {
                        str = string.Format("{0} {1}", getSharedStringHtml(51), pvd.astrodata.sun.rise.getShortTimeString());
                        dayText.Append(SPACE_BEFORE_LINE);
                        dayText.Append(str);
                        dayText.Append("\n");
                    }
                    if (GPDisplays.Calendar.TimeSunsetVisible())
                    {
                        str = string.Format("{0} {1}", getSharedStringHtml(52), pvd.astrodata.sun.set.getShortTimeString());
                        dayText.Append(SPACE_BEFORE_LINE);
                        dayText.Append(str);
                        dayText.Append("\n");

                    }

                    {
                        if (prevd != null)
                        {
                            if (prevd.astrodata.nMasa != pvd.astrodata.nMasa)
                            {
                                str = string.Format("{0} {1} {2}", getSharedStringHtml(780), GPMasa.GetName(pvd.astrodata.nMasa), getSharedStringHtml(22));
                                dayText.Append(SPACE_BEFORE_LINE);
                                dayText.Append(str);
                                dayText.Append("\n");
                            }
                        }
                        if (nextd != null)
                        {
                            if (nextd.astrodata.nMasa != pvd.astrodata.nMasa)
                            {
                                str = string.Format("{0} {1} {2}", getSharedStringHtml(781), GPMasa.GetName(pvd.astrodata.nMasa), getSharedStringHtml(22));
                                dayText.Append(SPACE_BEFORE_LINE);
                                dayText.Append(str);
                                dayText.Append("\n");
                            }
                        }
                    }

                _resolve_text:
                    if (dayText.Length > initialLength)
                    {
                        m_text.Append("BEGIN:VEVENT\n");
                        str2 = string.Format("DTSTART;VALUE=DATE:{0:0000}{1:00}{2:00}\n", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay());
                        m_text.Append(str2);
                        /*str2 = string.Format("DTEND;VALUE=DATE:{0:0000}{0:00}{0:00}T{0:00}{0:00}{0:00}\n", pvd.date.year, pvd.date.month, pvd.date.day,
                            pvd.astrodata.sun.set.hour, pvd.astrodata.sun.set.min, pvd.astrodata.sun.set.sec);
                        m_text.Append(str2);*/
                        str2 = string.Format("LOCATION:{0}\n", loc.getFullName());
                        str2.Replace(",", "\\,");
                        m_text.Append(str2);
                        m_text.Append("SUMMARY:");
                        dayText.Replace(",", "\\,");
                        m_text.Append(dayText.ToString().TrimStart());
                        str2 = string.Format("UID:{0:00000000}-{1:0000}-{2:0000}-{3:0000}-{4:00000000}{5:0000}\n", st.Year, st.Month * 30 + st.Day, st.Hour * 60 + st.Minute, st.Second, st.Millisecond, k);
                        m_text.Append(str2);
                        m_text.Append("DURATION:P1D\nSEQUENCE:1\nEND:VEVENT\n");
                    }
                }
            }

            m_text.Append("END:VCALENDAR\n");
            return 1;
        }













    }
}
