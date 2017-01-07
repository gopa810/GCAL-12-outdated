using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSunMethod
    {
        // time of arunodaya - 96 mins before sunrise
        public GPBodyData arunodaya;

        // time of sunrise
        public GPBodyData rise;

        // time of noon
        public GPBodyData noon;

        // time of sunset
        public GPBodyData set;

        // length of the day
        public double DayLength;


        public GPSunMethod()
        {
        }

        public GPSunMethod(GPSunMethod source)
        {
            // TODO: copy from source
            this.arunodaya = new GPBodyData(source.arunodaya);
            this.rise = new GPBodyData(source.rise);
            this.noon = new GPBodyData(source.noon);
            this.set = new GPBodyData(source.set);
            this.DayLength = source.DayLength;
        }

        public static int GetRasi(double SunLongitude, double Ayanamsa)
        {
            return Convert.ToInt32(Math.Floor(GPMath.putIn360(SunLongitude - Ayanamsa) / 30.0));
        }

        public virtual void SunCalc(GPGregorianTime vct, GPLocation earth)
        {
        }

        public double getSunriseDayHours()
        {
            return rise.getDayHours();
        }

        public virtual double getLongitude(GPGregorianTime gt)
        {
            return getLongitude(gt.getYear(), gt.getMonth(), gt.getDay(), gt.getHour(), gt.getMinute(), gt.getSecond());
        }

        /// <summary>
        /// Time is given in UTC+0hr
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public virtual double getLongitude(int year, int month, int day, int hour, int minute, int second)
        {
            return 0.0;
        }

        public class SunCoords
        {
            public double eclipticalLongitude;
            public double declination;
            public double rightAscession;
            public double equationOfTime;
            public double length_deg;
            public double arunodaya_deg;
            public double sunrise_deg;
            public double noon_deg;
            public double sunset_deg;

        }
    }
}
