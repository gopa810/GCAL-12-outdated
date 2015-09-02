using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterVCAL : Formatter
    {
        public static int FormatCalendarVCAL(GPCalendarResults daybuff, StringBuilder m_text)
        {
            int k;
            int initialLength = 0;
            int lastmasa = -1;
            string str, str2;
            StringBuilder dayText = new StringBuilder();
            GPCalendarDay pvd, prevd, nextd;
            string SPACE_BEFORE_LINE = " , ";

            DateTime st = new DateTime();

            m_text.Remove(0, m_text.Length);
            m_text.Append("BEGIN:VCALENDAR\nVERSION:1.0\nX-WR-CALNAME:VAISNAVA\nPRODID:-//GBC Calendar Comitee//GCAL//EN\n");
            m_text.Append("X-WR-RELCALID:");
            str2 = string.Format("{0:00000000}-{1:0000}-{2:0000}-{3:0000}-{4:0000}{5:00000000}", st.Year + st.Millisecond, st.Day * Convert.ToInt32(st.DayOfWeek), st.Month,
                st.Hour, st.Minute + st.Millisecond);
            m_text.Append(str2);
            m_text.Append("\nX-WR-TIMEZONE:");

            m_text.Append(daybuff.CurrentLocation.getLocation(0).getTimeZoneName());
            m_text.Append("\n");

            m_text.Append("CALSCALE:GREGORIAN\nMETHOD:PUBLISH\n");

            for (k = 0; k < daybuff.m_PureCount; k++)
            {


                prevd = daybuff.get(k - 1);
                pvd = daybuff.get(k);
                nextd = daybuff.get(k + 1);

                if (pvd != null)
                {
                    dayText.Remove(0, dayText.Length);

                    if (pvd.astrodata.nMasa != lastmasa)
                    {
                        str = string.Format("{0}, {1}", pvd.getMasaLongName(), pvd.getGaurabdaYearLongString());
                        dayText.Append(str);
                        dayText.AppendLine();
                        if ((pvd.astrodata.nMasa == GPMasa.ADHIKA_MASA) && ((lastmasa >= GPMasa.SRIDHARA_MASA) && (lastmasa <= GPMasa.DAMODARA_MASA)))
                        {
                            if (dayText.Length > 0)
                                dayText.Append(SPACE_BEFORE_LINE);
                            dayText.Append(getSharedStringHtml(128));
                            dayText.AppendLine();
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
                    dayText.AppendLine();
                    initialLength += dayText.Length;

                    if (pvd.astrodata.sun.eclipticalLongitude >= 0.0)
                    {

                        if (pvd.hasEkadasiParana())
                        {
                            m_text.AppendLine("BEGIN:VEVENT");
                            if (pvd.ekadasiParanaStart != null)
                            {
                                str2 = string.Format("DTSTART:{0:0000}{1:00}{2:00}T{3:00}{4:00}00", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay(), pvd.ekadasiParanaStart.getHour(), pvd.ekadasiParanaStart.getMinute());
                            }
                            else
                            {
                                str2 = string.Format("DTSTART:{0:0000}{1:00}{2:00}T000000", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay());
                            }
                            m_text.AppendLine(str2);
                            if (pvd.ekadasiParanaEnd != null)
                            {
                                str2 = string.Format("DTEND:{0:0000}{1:00}{2:00}T{3:00}{4:00}00", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay(), pvd.ekadasiParanaEnd.getHour(), pvd.ekadasiParanaEnd.getMinute());
                            }
                            else
                            {
                                str2 = string.Format("DTEND:{0:0000}{1:00}{2:00}T{3:00}{4:00}00", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay(), pvd.astrodata.sun.set.getHour(), pvd.astrodata.sun.set.getMinute());
                            }
                            m_text.AppendLine(str2);
                            m_text.Append("SUMMARY:");
                            m_text.Append(getSharedStringHtml(60));
                            m_text.Append("\nSEQUENCE:1\nEND:VEVENT\n");

                        }

                        foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(prevd, nextd))
                        {
                            if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                            {
                                dayText.Append(SPACE_BEFORE_LINE);
                                dayText.AppendLine(fest.Text);
                            }
                        }
                    }


                    if (dayText.Length > initialLength)
                    {
                        m_text.Append("BEGIN:VEVENT\n");
                        str2 = string.Format("DTSTART:{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}\n", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay(),
                            pvd.astrodata.sun.rise.getHour(), pvd.astrodata.sun.rise.getMinute(), pvd.astrodata.sun.rise.getSecond());
                        m_text.Append(str2);
                        str2 = string.Format("DTEND:{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}\n", pvd.date.getYear(), pvd.date.getMonth(), pvd.date.getDay(),
                            pvd.astrodata.sun.set.getHour(), pvd.astrodata.sun.set.getMinute(), pvd.astrodata.sun.set.getSecond());
                        m_text.Append(str2);
                        str2 = string.Format("LOCATION:{0}\n", pvd.date.getLocation().getFullName());
                        str2.Replace(",", "\\,");
                        m_text.Append(str2);
                        m_text.Append("SUMMARY:");
                        dayText.Replace(",", "\\,");
                        m_text.Append(dayText.ToString().TrimStart());
                        str2 = string.Format("UID:{0:00000000}-{1:0000}-{2:0000}-{3:0000}-{4:00000000}{5:0000}\n", st.Year, st.Month * 30 + st.Day, st.Hour * 60 + st.Minute, st.Second, st.Millisecond, k);
                        m_text.Append(str2);
                        m_text.Append("SEQUENCE:1\nEND:VEVENT\n");
                    }
                }
            }

            m_text.Append("END:VCALENDAR\n");
            return 1;
        }


    }
}
