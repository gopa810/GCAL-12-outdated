using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterCSV : Formatter
    {
        public static int FormatCalendarCSV(GPCalendarResults daybuff, StringBuilder m_text)
        {
            int k;
            int initialLength = 0;
            int lastmasa = -1;
            string str, str2;
            StringBuilder dayText = new StringBuilder();
            GPCalendarDay pvd, prevd, nextd;
            string SPACE_BEFORE_LINE = " , ";

            m_text.Remove(0, m_text.Length);
            m_text.Append("\"Subject\",\"Begin Date\",\"Start\",\"End Date\",\"End\",\"WholeDay\",\"Alarm\"\n");

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
                    dayText.Append("; ");
                    initialLength = dayText.Length;

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

                            dayText.Append(str);
                            dayText.Append("; ");


                        }
                    }

                    //			if (0 != GetShowSetVal(6) == 1)
                    {
                        foreach (GPCalendarDay.Festival fest in pvd.Festivals)
                        {
                            if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                            {
                                dayText.Append(fest.Text);
                                dayText.Append("; ");
                            }
                        }
                    }

                    if (/*GetShowSetVal(16) == 1 &&*/ pvd.sankranti_zodiac >= 0)
                    {
                        str = string.Format("{0} {1}; ", GPSankranti.getName(pvd.sankranti_zodiac), getSharedStringHtml(56));
                        dayText.Append(str);
                    }


                    //"Sunrise Time",//2
                    //"Sunset Time",//3
                    if (GPDisplays.Calendar.TimeSunriseVisible())
                    {
                        str = string.Format("Sunrise {0}; ", pvd.astrodata.sun.rise.getShortTimeString());
                        dayText.Append(str);
                    }
                    if (GPDisplays.Calendar.TimeSunsetVisible())
                    {
                        str = string.Format("Sunset {0}; ", pvd.astrodata.sun.set.getShortTimeString());
                        dayText.Append(str);
                    }


                    {
                        if (prevd != null)
                        {
                            if (prevd.astrodata.nMasa != pvd.astrodata.nMasa)
                            {
                                str = string.Format("{0} {1} {2}", getSharedStringHtml(780), GPMasa.GetName(pvd.astrodata.nMasa), getSharedStringHtml(22));
                                dayText.Append(str);
                                dayText.Append("; ");
                            }
                        }
                        if (nextd != null)
                        {
                            if (nextd.astrodata.nMasa != pvd.astrodata.nMasa)
                            {
                                str = string.Format("{0} {1} {2}", getSharedStringHtml(781), GPMasa.GetName(pvd.astrodata.nMasa), getSharedStringHtml(22));
                                dayText.Append(str);
                                dayText.Append("; ");
                            }
                        }
                    }

                _resolve_text:
                    if (dayText.Length > initialLength || !GPDisplays.Calendar.HideEmptyDays())
                    {
                        m_text.Append("\"");
                        m_text.Append(dayText);
                        m_text.Append("\",");

                        str2 = string.Format("\"{0}.{1}.{2}\",\"0:00:00\",\"{3}.{4}.{5}\",\"0:00:00\",\"True\",\"false\"\n",
                            pvd.date.getDay(), pvd.date.getMonth(), pvd.date.getYear(), nextd.date.getDay(),
                            nextd.date.getMonth(), nextd.date.getYear());
                        m_text.Append(str2);
                    }
                }
            }

            return 1;
        }

    }
}
