using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterInternal: Formatter
    {
        public static string getInternalDalendarData(GPCalendarResults calendar)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Location: {0}\n", calendar.m_Location.getLocation(0).ToString());
            sb.AppendFormat("Longitude: {0}\nLatitude: {1}\n\n", calendar.m_Location.getLocation(0).getLongitudeString(),
                calendar.m_Location.getLocation(0).getLatitudeString());

            // julian day
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("jdate {0}\t{1}\n", day.date.ToString(), day.astrodata.jdate);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("msAyanamsa {0}\t{1}\n", day.date.ToString(), day.astrodata.msAyanamsa);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("msDistance {0}\t{1}\n", day.date.ToString(), day.astrodata.msDistance);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nGaurabdaYear {0}\t{1}\n", day.date.ToString(), day.astrodata.nGaurabdaYear);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nMasa {0}\t{1}\n", day.date.ToString(), day.astrodata.nMasa);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nMoonRasi {0}\t{1}\n", day.date.ToString(), day.astrodata.nMoonRasi);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nNaksatra {0}\t{1}\n", day.date.ToString(), day.astrodata.nNaksatra);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nNaksatraElapse {0}\t{1}\n", day.date.ToString(), day.astrodata.nNaksatraElapse);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nPaksa {0}\t{1}\n", day.date.ToString(), day.astrodata.nPaksa);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nSunRasi {0}\t{1}\n", day.date.ToString(), day.astrodata.nSunRasi);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nTithi {0}\t{1}\n", day.date.ToString(), day.astrodata.nTithi);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nTithiElapse {0}\t{1}\n", day.date.ToString(), day.astrodata.nTithiElapse);
            }

            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nYoga {0}\t{1}\n", day.date.ToString(), day.astrodata.nYoga);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("nYogaElapse {0}\t{1}\n", day.date.ToString(), day.astrodata.nYogaElapse);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.latitude_deg {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.eclipticalLatitude);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.longitude_deg {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.eclipticalLongitude);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.rise {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.rise.getDayHours());
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.set {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.set.getDayHours());
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.right_ascession {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.right_asc_deg);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("sun.declination {0}\t{1}\n", day.date.ToString(), day.astrodata.sun.declination_deg);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("moon.longitude_deg {0}\t{1}\n", day.date.ToString(), day.astrodata.moon.longitude_deg);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("moon.latitude_deg {0}\t{1}\n", day.date.ToString(), day.astrodata.moon.latitude_deg);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("moon.right_ascession {0}\t{1}\n", day.date.ToString(), day.astrodata.moon.right_ascession);
            }
            foreach (GPCalendarDay day in calendar.m_pData)
            {
                sb.AppendFormat("moon.declination {0}\t{1}\n", day.date.ToString(), day.astrodata.moon.declination);
            }
            return sb.ToString();
        }


        public static string getInternalEventsText(GPCoreEventResults inEvents)
        {

            int i;

            StringBuilder res = new StringBuilder();

            res.AppendFormat(getSharedStringHtml(983), inEvents.m_vcStart, inEvents.m_vcEnd);
            res.AppendLine();
            res.AppendLine();

            res.Append(inEvents.m_location.getFullName());
            res.AppendLine();
            res.AppendLine();

            DateTime prevd = new DateTime(1970, 1, 1);
            int prevt = -1;

            for (i = 0; i < inEvents.getCount(); i++)
            {
                GPCoreEvent dnr = inEvents.get(i);

                if (!inEvents.b_sorted)
                {
                    res.Append(dnr.Time.ToString().PadLeft(20));
                }

                res.AppendFormat("   {0}  {1}", dnr.Time.getLongTimeString(), dnr.getEventTitle());
                res.AppendLine();

            }

            res.AppendLine();

            return res.ToString();
        }

    }
}
