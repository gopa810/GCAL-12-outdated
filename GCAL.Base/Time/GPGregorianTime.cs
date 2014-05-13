using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class GPGregorianTime
    {
        static int[] m_months_ovr = new int[] { 0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static int[] m_months = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public static bool timeFormat24 = true;

        private int p_year;
        private int p_month;
        private int p_day;
        private double p_shour;

        // Greenwich Julian day
        private double p_julian = -1;

        private bool dstValid = false;
        private bool p_dst_on = false;
        private int p_dst_bias = 0;

        private GPLocationProvider p_locationProvider = null;

        public GPGregorianTime(GPLocation loc)
        {
            setLocation(loc);
            Today();
            setDayHours(0.0);
        }

        public GPGregorianTime(GPLocationProvider loc)
        {
            p_locationProvider = loc;
            Today();
            setDayHours(0.0);
        }

        public GPGregorianTime(GPLocation loc, GPJulianTime julianTime)
        {
            setLocation(loc);
            setJulianGreenwichTime(julianTime);
        }

        public GPGregorianTime(GPLocationProvider loc, GPJulianTime julianTime)
        {
            p_locationProvider = loc;
            setJulianGreenwichTime(julianTime);
        }

        public GPGregorianTime(GPGregorianTime vc)
        {
            Copy(vc);
        }
 
        public void addDayHours(double hr)
        {
            if (p_julian < 0)
            {
                recalculateJulianGreenwichTime();
            }
            p_shour += hr;
            p_julian += hr;
            normalizeValues();
        }

        public GPGregorianTime TimeByAddingHours(double dHours)
        {
            GPGregorianTime vc = new GPGregorianTime(this);
            vc.addDayHours(dHours/24.0);
            return vc;
        }

        public long getTimestamp()
        {
            return Convert.ToInt64(24 * (getDay() + 32 * (getMonth() + 12 * getYear()))) * 3600 + Convert.ToInt64((getDayHours() - getTimeZoneOffsetHours() / 24.0) * 86400);
        }

        public void Clear()
        {
            p_year = p_month = p_day = -1;
            p_shour = 0;
            p_julian = -1;
        }

        public void setTimestamp(long value)
        {
            long v = value;
            long secs = v % 86400;
            setDayHours(Convert.ToDouble(secs) / 86400.0);
            v = (v - secs) / 86400;
            p_day = Convert.ToInt32(v % 32);
            v = (v - getDay()) / 32;
            p_month = Convert.ToInt32(v % 12);
            v = (v - getMonth()) / 12;
            p_year = Convert.ToInt32(v);
            recalculateJulianGreenwichTime();
        }

        // vracia -1, ak zadany den je den nasledujuci po THIS
        // vracia 1 ak THIS je nasledujuci den po zadanom dni
        public int CompareYMD(GPGregorianTime v)
        {
            if (v.getYear() < getYear())
                return (getYear() - v.getYear()) * 365;
            else if (v.getYear() > getYear())
                return (getYear() - v.getYear()) * 365;

            if (v.getMonth() < getMonth())
                return (getMonth() - v.getMonth()) * 31;
            else if (v.getMonth() > getMonth())
                return (getMonth() - v.getMonth()) * 31;

            return (getDay() - v.getDay());
        }

        public void Today()
        {
            DateTime dt = DateTime.Now;
            setDate(dt.Year, dt.Month, dt.Day);
            setDayHours(dt.TimeOfDay.TotalHours / 24.0);
            recalculateJulianGreenwichTime();
        }

        public override bool Equals(object obj)
        {
            if (obj is GPGregorianTime)
            {
                return getTimestamp() == (obj as GPGregorianTime).getTimestamp();
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return getYear() * 384 + getMonth() * 32 + getDay();
        }

        public bool IsBeforeThis(GPGregorianTime date)
        {
            int y1, y2, m1, m2, d1, d2;
            d1 = this.getDay();
            d2 = date.getDay();
            m1 = this.getMonth();
            m2 = date.getMonth();
            y1 = this.getYear();
            y2 = date.getYear();

            if (y1 > y2)
                return false;
            else if (y1 < y2)
                return true;
            else if (m1 > m2)
                return false;
            else if (m1 < m2)
                return true;
            else if (d1 < d2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// JULIAN(greenwich) = JULIAN(local) - TIMEZONEHOURS
        /// JULIAN(local) = JULIAN(greenwich) + TIMEZONEHOURS
        /// </summary>
        /// <param name="jdate">Julian Time</param>
        public void setJulianGreenwichTime(GPJulianTime jdate)
        {
            double jd = jdate.getGreenwichJulianDay() + getLocation().getTimeZoneOffsetHours()/24.0;

            setJulianLocalTime(jd);

            // greenwich julian day
            p_julian = jdate.getGreenwichJulianDay();
        }

        public void setJulianLocalTime(double jd)
        {
            p_julian = jd - getTimeZoneOffsetHours();
            double z = Math.Floor(jd + 0.5);

            double f = (jd + 0.5) - z;

            double A, B, C, D, E, alpha;

            if (z < 2299161.0)
            {
                A = z;
            }
            else
            {
                alpha = Math.Floor((z - 1867216.25) / 36524.25);
                A = z + 1.0 + alpha - Math.Floor(alpha / 4.0);
            }

            B = A + 1524;
            C = Math.Floor((B - 122.1) / 365.25);
            D = Math.Floor(365.25 * C);
            E = Math.Floor((B - D) / 30.6001);
            p_day = Convert.ToInt32(Math.Floor(B - D - Math.Floor(30.6001 * E) + f));
            p_month = Convert.ToInt32((E < 14) ? E - 1 : E - 13);
            p_year = Convert.ToInt32((p_month > 2) ? C - 4716 : C - 4715);
            double final = jd + 0.5;
            p_shour = final - Math.Floor(final);
        }

        /// <summary>
        /// This is value of Julian Day in the moment of 12:00 (noon)
        /// in local coordinates at local time.
        /// 
        /// It answers question: What is the time when localy is 12:00 noon.
        /// </summary>
        /// <returns></returns>
        public double getJulianLocalNoon()
        {
            int yy = getYear() - Convert.ToInt32((12 - getMonth()) / 10);
            int mm = getMonth() + 9;

            if (mm >= 12)
                mm -= 12;

            int k1, k2, k3;
            int j;

            k1 = Convert.ToInt32(Math.Floor(365.25 * (yy + 4712)));
            k2 = Convert.ToInt32(Math.Floor(30.6 * mm + 0.5));
            k3 = Convert.ToInt32(Math.Floor(Math.Floor(Convert.ToDouble(yy / 100) + 49) * .75)) - 38;
            j = k1 + k2 + getDay() + 59;
            if (j > 2299160)
                j -= k3;

            //Debugger.Log(0, "", String.Format("yy={0}, mm={1}, k1={2}, k2={3}, k3={4}, j={5}; y={6}, m={7}, d={8}\n",
            //    yy, mm, k1, k2, k3, j, getYear(), getMonth(), getDay()));

            return Convert.ToDouble(j);
        }

        public GPJulianTime getJulian()
        {
            return new GPJulianTime(getJulianGreenwichTime(), 0.0);
        }

        /// <summary>
        /// This is value of Julian Day in the moment of (12:00 - TIMEZONEOFFSET)
        /// in local coordinates at local time. That means also it is moment 
        /// of 12:00 local coordinates at greenwich time.
        /// 
        /// It answers question: What is the time on Greenwich when localy is 12:00 noon.
        /// </summary>
        /// <returns></returns>
        public double getJulianGreenwichNoon()
        {
            return getJulianLocalNoon() - getTimeZoneOffsetHours() / 24.0;
        }

        /// <summary>
        /// JULIAN(greenwich) = JULIAN(local) - TIMEZONEHOURS
        /// </summary>
        /// <returns></returns>
        public double getJulianGreenwichTime()
        {
            //if (p_julian < 0)
            //recalculateJulianGreenwichTime();
            return p_julian;
            //return getJulianLocalNoon() - 0.5 + getDayHours() - getTimeZoneOffsetHours() / 24.0;
        }

        private void recalculateJulianGreenwichTime()
        {
            p_julian = getJulianLocalNoon() - 0.5 + getDayHours() - getTimeZoneOffsetHours() / 24.0;
        }

        public void AddYears(int a)
        {
            p_year += a;
        }

        public void AddMonths(int a)
        {
            p_month += a;
            while (p_month < 1)
            {
                p_month += 12;
                p_year--;
            }
            while (p_month > 12)
            {
                p_month -= 12;
                p_year++;
            }

            recalculateJulianGreenwichTime();
        }

        public void AddDays(int a)
        {
            if (a < 0)
            {
                SubDays(-a);
            }
            else
            {
                for (int i = 0; i < a; i++)
                    NextDay();
            }
        }

        public void SubDays(int n)
        {
            if (n < 0)
            {
                AddDays(-n);
            }
            else
            {
                for (int i = 0; i < n; i++)
                    PreviousDay();
            }
        }


        public void NextDay()
        {
            p_day++;
            if (p_day > GetMonthMaxDays(getYear(), getMonth()))
            {
                p_month ++;
                if (p_month > 12)
                {
                    p_month = 1;
                    p_year ++;
                }
                p_day = 1;
            }
            p_julian += 1;
        }

        public void PreviousDay()
        {
            p_day--;
            if (p_day < 1)
            {
                p_month--;
                if (p_month < 1)
                {
                    p_month = 12;
                    p_year --;
                }
                p_day = GetMonthMaxDays(getYear(), getMonth());
            }
            p_julian -= 1;
        }

        public bool IsLeapYear()
        {
            return GPGregorianTime.IsLeapYear(getYear());
        }

        public static bool IsLeapYear(int year)
        {
            if ((year % 4) == 0)
            {
                if ((year % 100 == 0) && (year % 400 != 0))
                    return false;
                else
                    return true;
            }

            return false;
        }

        public static int GetMonthMaxDays(int year, int month)
        {
            if (IsLeapYear(year))
                return m_months_ovr[month];
            else
                return m_months[month];
        }

        public void normalizeValues()
        {
            normalizeValues(ref p_year, ref p_month, ref p_day, ref p_shour);
        }

        public static void normalizeValues(ref int y1, ref int m1, ref int d1, ref double h1)
        {
            while (h1 < 0.0)
            {
                d1--;
                h1 += 1.0;
                if (d1 < 1)
                {
                    m1--;
                    if (m1 < 1)
                    {
                        m1 = 12;
                        y1--;
                    }
                    d1 = GetMonthMaxDays(y1, m1);
                }
            }
            while (h1 >= 1.0)
            {
                h1 -= 1.0;
                d1++;
                if (d1 > GetMonthMaxDays(y1, m1))
                {
                    d1 = 1;
                    m1++;
                    if (m1 > 12)
                    {
                        m1 = 1;
                        y1++;
                    }
                }
            }
        }


        public int getYear()
        {
            return p_year;
        }

        public void setDate(int y, int m, int d)
        {
            if (y > 0)
                p_year = y;
            if (m > 0)
                p_month = m;
            if (d > 0)
                p_day = d;
            normalizeValues();
            recalculateJulianGreenwichTime();
            dstValid = false;
        }

        public int getMonth()
        {
            return p_month;
        }

        public int getDay()
        {
            return p_day;
        }

        public int getDayOfWeek()
        {
            return (Convert.ToInt32(getJulianLocalNoon()) + 1) % 7;
        }

        public double getDayHours()
        {
            return p_shour;
        }

        public void setDayHours(double value)
        {
            p_julian += (value - p_shour);
            p_shour = value;
            dstValid = false;
        }

        public double getTimeZoneOffsetHours()
        {
            GPLocation loc = getLocation();
            if (loc == null)
                return 0;
            GPTimeZone tz = loc.getTimeZone();
            if (tz == null)
                return 0;
            return tz.OffsetSeconds / 3600.0;
        }


        public void Copy(GPGregorianTime vc)
        {
            p_year = vc.getYear();
            p_month = vc.getMonth();
            p_day = vc.getDay();
            setDayHours(vc.getDayHours());
            setLocationProvider(vc.getLocationProvider());
            p_julian = vc.getJulianGreenwichTime();
        }

        public GPGregorianTime Copy()
        {
            return new GPGregorianTime(this);
        }

        public GPLocation getLocation()
        {
            return p_locationProvider.getLocation(p_julian);
        }

        public void setLocation(GPLocation value)
        {
            p_locationProvider = new GPLocationProvider();
            p_locationProvider.setDefaultLocation(value);
        }

        public GPLocationProvider getLocationProvider()
        {
            return p_locationProvider;
        }

        public void setLocationProvider(GPLocationProvider value)
        {
            p_locationProvider = value;
        }


        public bool getDaylightTimeON()
        {
            if (!dstValid)
            {
                GPLocation loc = getLocation();
                if (loc != null && loc.getTimeZone() != null)
                {
                    p_dst_bias = Convert.ToInt32(loc.getTimeZone().BiasHoursForDate(this) * 3600);
                    p_dst_on = (p_dst_bias > 0);
                }
                dstValid = true;
            }
            return p_dst_on;
        }

        /// <summary>
        /// Bias of DST time in seconds (s)
        /// </summary>
        public int getDaylightTimeBias()
        {
            if (!dstValid)
            {
                GPLocation loc = getLocation();
                if (loc != null && loc.getTimeZone() != null)
                {
                    p_dst_bias = Convert.ToInt32(loc.getTimeZone().BiasHoursForDate(this) * 3600);
                    p_dst_on = (p_dst_bias > 0);
                }
                dstValid = true;
            }
            return p_dst_bias;
        }
        public int getSecond()
        {
            return Convert.ToInt32(Math.Floor((getDayHours() * 1440 - Math.Floor(getDayHours() * 1440)) * 60));
        }

        public long getDayInteger()
        {
            return Convert.ToInt64(getYear()) * 384 + getMonth() * 32 + getDay();
        }

        public int getMinute()
        {
            return Convert.ToInt32(Math.Floor((getDayHours() * 24 - Math.Floor(getDayHours() * 24)) * 60));
        }

        public int getMinuteRound()
        {
            return Convert.ToInt32(Math.Floor((getDayHours() * 24 - Math.Floor(getDayHours() * 24)) * 60 + 0.5));
        }

        public int getHour()
        {
            return Convert.ToInt32(Math.Floor(getDayHours() * 24));
        }

        public DateTime getLocalTime()
        {
            DateTime dt = new DateTime(getYear(), getMonth(), getDay(), getHour(), getMinute(), getSecond());
            if (getDaylightTimeON())
            {
                dt = dt.AddSeconds(Convert.ToDouble(getDaylightTimeBias()));
            }
            return dt;
        }

        public void getLocalTimeEx(out DateTime dt, out bool dstOn)
        {
            dstOn = getDaylightTimeON();
            normalizeValues();
            dt = new DateTime(getYear(), getMonth(), getDay(), getHour(), getMinute(), getSecond());
            if (dstOn)
            {
                dt = dt.AddSeconds(Convert.ToDouble(getDaylightTimeBias()));
            }
        }

        public override string ToString()
        {
            bool dst;
            DateTime dt;
            getLocalTimeEx(out dt, out dst);
            return string.Format("{0} {1} {2}", dt.Day, GPAppHelper.getMonthAbr(dt.Month), dt.Year);
        }

        public string getShortDateString()
        {
                bool dst;
                DateTime dt;
                getLocalTimeEx(out dt, out dst);
                return string.Format("{0} {1}", dt.Day, GPAppHelper.getMonthAbr(dt.Month));
        }

        public string getLongDateString()
        {
            bool dst;
            DateTime dt;
            getLocalTimeEx(out dt, out dst);
            return string.Format("{0} {1} {2}", dt.Day, GPAppHelper.getMonthAbr(dt.Month), dt.Year);
        }

        public string getCompleteLongDateString()
        {
            bool dst;
            DateTime dt;
            getLocalTimeEx(out dt, out dst);
            return string.Format("{0} {1} {2} ({3})", dt.Day, GPAppHelper.getMonthAbr(dt.Month), dt.Year, getDayOfWeekName(dt.DayOfWeek));
        }

        public string getDayOfWeekName(DayOfWeek dow)
        {
            int idx = 0;
            switch (dow)
            {
                case DayOfWeek.Sunday:
                    idx = 0;
                    break;
                case DayOfWeek.Monday:
                    idx = 1;
                    break;
                case DayOfWeek.Tuesday:
                    idx = 2;
                    break;
                case DayOfWeek.Wednesday:
                    idx = 3;
                    break;
                case DayOfWeek.Thursday:
                    idx = 4;
                    break;
                case DayOfWeek.Friday:
                    idx = 5;
                    break;
                case DayOfWeek.Saturday:
                    idx = 6;
                    break;
            }

            return GPStrings.getSharedStrings().getString(idx);
        }


        public string getLongTimeString()
        {
                bool dst;
                DateTime dt;
                getLocalTimeEx(out dt, out dst);
                if (getLocation().getTimeZone().isSupportDaylightSaving())
                {
                    if (timeFormat24)
                    {
                        return string.Format("{0:00}:{1:00}:{2:00} ({3})", dt.Hour, dt.Minute, dt.Second, GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                    }
                    else
                    {
                        return string.Format("{0:00}:{1:00}:{2:00} {3} ({4})", (((dt.Hour % 12) + 11) % 12 + 1), dt.Minute, dt.Second, (dt.Hour >= 12 ? "PM" : "AM"), GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                    }
                }
                else
                {
                    if (timeFormat24)
                    {
                        return string.Format("{0:00}:{1:00}:{2:00}", dt.Hour, dt.Minute, dt.Second);
                    }
                    else
                    {
                        return string.Format("{0:00}:{1:00}:{2:00} {3}", (((dt.Hour % 12) + 11) % 12 + 1), dt.Minute, dt.Second, (dt.Hour >= 12 ? "PM" : "AM"));
                    }
                }
        }

        public string getShortTimeString()
        {
                bool dst;
                DateTime dt;
                getLocalTimeEx(out dt, out dst);
                int h = dt.Hour;
                int m = dt.Minute + ((dt.Second > 30) ? 1 : 0);

                if (getLocation().getTimeZone().isSupportDaylightSaving())
                {
                    if (timeFormat24)
                    {
                        return string.Format("{0:00}:{1:00} ({2})", h, m, GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                    }
                    else
                    {
                        return string.Format("{0:00}:{1:00} {2} ({3})", (((h % 12) + 11) % 12 + 1), m, (h >= 12 ? "PM" : "AM"), GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                    }
                }
                else
                {
                    if (timeFormat24)
                    {
                        return string.Format("{0:00}:{1:00}", h, m);
                    }
                    else
                    {
                        return string.Format("{0:00}:{1:00} {2}", (((h % 12) + 11) % 12 + 1), m, (h >= 12 ? "PM" : "AM"));
                    }
                }
        }

        public string getShortSandhyaRange()
        {
                bool dst;
                DateTime dt;
                getLocalTimeEx(out dt, out dst);
                int h = dt.Hour;
                int m = dt.Minute + ((dt.Second > 30) ? 1 : 0);
                int h1, m1, h2, m2;

                h1 = h2 = h;
                m1 = m2 = m;
                m1 -= 24;
                m2 += 24;
                if (m1 < 0)
                {
                    m1 += 60;
                    h1--;
                }
                if (m1 > 59)
                {
                    m1 -= 60;
                    h1++;
                }
                if (m2 < 0)
                {
                    m2 += 60;
                    h2--;
                }
                if (m2 > 59)
                {
                    m2 -= 60;
                    h2++;
                }

                return FormatTimeRange(h1, m1, h1, m2, dst);
        }

        public string getShortMuhurtaRange(int nMuhurta)
        {
            bool dst;
            DateTime dt;
            getLocalTimeEx(out dt, out dst);
            int h = dt.Hour;
            int m = dt.Minute + ((dt.Second > 30) ? 1 : 0);
            int h1, m1, h2, m2;

            h1 = h2 = h;
            m1 = m2 = m;
            m1 += (nMuhurta * 48);
            m2 += (nMuhurta * 48 + 48);
            while (m1 < 0)
            {
                m1 += 60;
                h1--;
            }
            while (m1 > 59)
            {
                m1 -= 60;
                h1++;
            }
            while (m2 < 0)
            {
                m2 += 60;
                h2--;
            }
            while (m2 > 59)
            {
                m2 -= 60;
                h2++;
            }

            return FormatTimeRange(h1, m1, h1, m2, dst);
        }


        public string FormatTimeRange(int h1, int m1, int h2, int m2, bool dst)
        {
            if (timeFormat24)
            {
                if (getLocation().getTimeZone().isSupportDaylightSaving())
                {
                    return string.Format("{0:00}:{1:00} - {2:00}:{3:00} ({4})", h1, m1, h2, m2, GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                }
                else
                {
                    return string.Format("{0:00}:{1:00} - {2:00}:{3:00}", h1, m1, h2, m2);
                }
            }
            else
            {
                if (getLocation().getTimeZone().isSupportDaylightSaving())
                {
                    return string.Format("{0:00}:{1:00} {2} - {3:00}:{4:00} {5} ({6})", (((h1 % 12) + 11) % 12 + 1), m1, (h1 > 12), (((h2 % 12) + 11) % 12 + 1), m2, GPAppHelper.GetDSTSignature(dst ? 1 : 0));
                }
                else
                {
                    return string.Format("{0:00}:{1:00} {2} - {3:00}:{4:00} {5}", (((h1 % 12) + 11) % 12 + 1), m1, (((h2 % 12) + 11) % 12 + 1), m2);
                }
            }
        }

        public void setDateTime(DateTime dateTime)
        {
            setDate(dateTime.Year, dateTime.Month, dateTime.Day);
            setDayHours(((dateTime.Hour * 60 + dateTime.Minute) * 60 + dateTime.Second) / 86400.0);
        }
    }
}
