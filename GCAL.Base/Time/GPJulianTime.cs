using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPJulianTime
    {
        private const int JULIAN = 0x0001;
        private const int JULIAN_EPHEMERIS = 0x0002;

        private double timezone = 0;

        private double julianDay = 0;
        private double deltaT = 0;
        private double julianEphemerisDay = 0;
        private int mask = 0;
        private int lastType = 0;

        public GPJulianTime()
        {
        }

        public GPJulianTime(double julianTime, double offset)
        {
            setLocalJulianDay(julianTime, offset);
        }

        public override string ToString()
        {
            return String.Format("{0}  Timezone:{1}", julianDay, timezone);
        }

        public GPJulianTime AddSeconds(double s)
        {
            if (lastType == JULIAN)
            {
                julianDay += s / 86400.0;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += s / 86400.0;
            }
            mask = lastType;
            return this;
        }

        public GPJulianTime AddMinutes(int m)
        {
            if (lastType == JULIAN)
            {
                julianDay += m / 1440.0;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += m / 1440.0;
            }

            mask = lastType;
            return this;
        }

        public GPJulianTime AddHours(int h)
        {
            if (lastType == JULIAN)
            {
                julianDay += h / 24.0;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += h / 24.0;
            }

            mask = lastType;
            return this;
        }

        public GPJulianTime AddDays(int d)
        {
            if (lastType == JULIAN)
            {
                julianDay += d;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += d;
            }

            mask = lastType;
            return this;
        }

        public GPJulianTime AddMonths(int m)
        {
            if (lastType == JULIAN)
            {
                julianDay += 30;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += 30;
            }

            mask = lastType;
            return this;
        }

        public GPJulianTime AddYears(int y)
        {
            if (lastType == JULIAN)
            {
                julianDay += 365;
            }
            else if (lastType == JULIAN_EPHEMERIS)
            {
                julianEphemerisDay += 365;
            }

            mask = lastType;
            return this;
        }

        private void UpdateJulianFromJulianEphemeris()
        {
            deltaT = GPDynamicTime.GetDeltaT(julianEphemerisDay);
            julianDay = julianEphemerisDay - deltaT/86400;
        }

        private void UpdateJulianEphemerisFromJulian()
        {
            deltaT = GPDynamicTime.GetDeltaT(julianDay);
            julianEphemerisDay = julianDay + deltaT/86400;
        }

        public void setLocalJulianDay(double d)
        {
            julianDay = d;
            mask = JULIAN;
            lastType = JULIAN;
        }

        public void setGreenwichJulianDay(double d)
        {
            julianDay = d + getLocalTimezoneOffset() / 24.0;
            mask = JULIAN;
            lastType = JULIAN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localTime">Julian day value in local coordinates and timezone</param>
        /// <param name="offset">Offset in hours.</param>
        public void setLocalJulianDay(double localTime, double offset)
        {
            julianDay = localTime;
            timezone = offset;
            mask = JULIAN;
            lastType = JULIAN;
        }

        public void setLocalJulianEphemerisDay(double d)
        {
            julianEphemerisDay = d;
            mask = JULIAN_EPHEMERIS;
            lastType = JULIAN_EPHEMERIS;
        }

        public void setGreenwichJulianEphemerisDay(double d)
        {
            julianEphemerisDay = d + getLocalTimezoneOffset() / 24.0;
            mask = JULIAN_EPHEMERIS;
            lastType = JULIAN_EPHEMERIS;
        }

        public double getLocalJulianDay()
        {
            ValidateJulianDay();
            return julianDay;
        }

        public double getGreenwichJulianDay()
        {
            ValidateJulianDay();
            return julianDay - getLocalTimezoneOffset() / 24.0;
        }

        public double getLocalJulianEphemerisDay()
        {
            ValidateJulianEphemerisDay();
            return julianEphemerisDay;
        }

        public double getGreenwichJulianEphemerisDay()
        {
            ValidateJulianEphemerisDay();
            return julianEphemerisDay - getLocalTimezoneOffset() / 24.0;
        }

        /// <summary>
        /// This function does not invalidates julian day.
        /// </summary>
        /// <param name="hours">Hours offset from GMT. Eastward positive values, westward negative values.</param>
        public void setLocalTimezoneOffset(double hours)
        {
            timezone = hours;
        }
        
        /// <summary>
        /// Local timezone offset in fraction of hours.
        /// </summary>
        /// <returns></returns>
        public double getLocalTimezoneOffset()
        {
            return timezone;
        }

        private void ValidateJulianDay()
        {
            if ((mask & JULIAN) == 0)
            {
                if ((mask & JULIAN_EPHEMERIS) > 0)
                {
                    UpdateJulianFromJulianEphemeris();
                    mask |= JULIAN;
                }
            }
        }

        private void ValidateJulianEphemerisDay()
        {
            if ((mask & JULIAN_EPHEMERIS) == 0)
            {
                if ((mask & JULIAN) > 0)
                {
                    UpdateJulianEphemerisFromJulian();
                    mask |= JULIAN_EPHEMERIS;
                }
            }
        }
    }
}
