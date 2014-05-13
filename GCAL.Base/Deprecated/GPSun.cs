using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSun: GPSunData
    {

        public const int SUNRISE_TYPE_CENTER = 0;
        public const int SUNRISE_TYPE_TOPTIP = 1;
        public const int SUNRISE_TYPE_BOTTOMTIP = 2;
        public const int SUNRISE_TYPE_CIVIL = 3;
        public const int SUNRISE_TYPE_NAUTICAL = 4;
        public const int SUNRISE_TYPE_ASTRONOMICAL = 5;

        public static int sunriseType = SUNRISE_TYPE_TOPTIP;

        // time of arunodaya - 96 mins before sunrise
        public GPGregorianTime arunodaya;
        // time of sunrise
        public GPGregorianTime rise;
        // time of noon
        public GPGregorianTime noon;
        // time of sunset
        public GPGregorianTime set;
        // length of the day
        public double DayLength;


        public double length_deg;
        public double arunodaya_deg;
        public double sunrise_deg;
        public double sunset_deg;
        public double noon_deg;


        public double longitude_set_deg;
        public double longitude_arun_deg;

        public GPSun()
        {
        }

        public GPSun(GPSun sn)
        {
            copyFrom(sn);
        }

        protected void copyFrom(GPSun sn)
        {
            base.copyFrom(sn);

            this.arunodaya = new GPGregorianTime(sn.arunodaya);
            this.arunodaya_deg = sn.arunodaya_deg;
            this.DayLength = sn.DayLength;
            this.length_deg = sn.length_deg;
            this.longitude_arun_deg = sn.longitude_arun_deg;
            this.longitude_set_deg = sn.longitude_set_deg;
            this.noon = new GPGregorianTime(sn.noon);
            this.rise = new GPGregorianTime(sn.rise);
            this.set = new GPGregorianTime(sn.set);
            this.sunrise_deg = sn.sunrise_deg;
            this.sunset_deg = sn.sunset_deg;
        }


        public void calculateCoordinates(GPGregorianTime vct, double DayHours)
        {
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;

            // mean ecliptic longitude of the sun 
            double mel = SunGetMeanLong(vct.getYear(), vct.getMonth(), vct.getDay()) + (360 / 365.25) * DayHours / 360.0;

            // ecliptic longitude of perigee 
            double elp = SunGetPerigee(vct.getYear(), vct.getMonth(), vct.getDay());

            // mean anomaly of the sun
            double mas = mel - elp;


            // ecliptic longitude of the sun
            double els = 0;
            eclipticalLongitude = els = mel + 1.915 * Math.Sin(mas * DG) + 0.02 * Math.Sin(2 * DG * mas);
            //sun.longitude_deg = GetSunLongitude2(vct.GetJulianComplete());
            // declination of the sun
            declination_deg = RAD * Math.Asin(0.39777 * Math.Sin(els * DG));

            // right ascension of the sun
            right_asc_deg = els - RAD * Math.Atan2(Math.Sin(2 * els * DG), 23.2377 + Math.Cos(2 * DG * els));

            // equation of time
            equationOfTime = right_asc_deg - mel;
        }

        // VCTIME vct [in] - valid must be each member of this structure
        //

        // from vct uses members: year, month, day
        // DayHours is in range 0.0 - 1.0

        public void calculateRiseSet(GPGregorianTime vct, GPLocationProvider ed, double DayHours)
        {
            GPSun sun = this;
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;

            double x;


            calculateCoordinates(vct, DayHours);

            double time = vct.getJulianLocalNoon() - 0.5 + DayHours - vct.getTimeZoneOffsetHours() / 24.0;
            double dLatitude = ed.getLocation(time).GetLatitudeNorthPositive();
            double dLongitude = ed.getLocation(time).GetLongitudeEastPositive();
            /*
            // mean ecliptic longitude of the sun 
            double mel = SunGetMeanLong(vct.getYear(), vct.getMonth(), vct.getDay()) + (360 / 365.25) * DayHours / 360.0;

            // ecliptic longitude of perigee 
            double elp = SunGetPerigee(vct.getYear(), vct.getMonth(), vct.getDay());

            // mean anomaly of the sun
            double mas = mel - elp;


            // ecliptic longitude of the sun
            double els = 0;
            sun.eclipticalLongitude = els = mel + 1.915 * Math.Sin(mas * DG) + 0.02 * Math.Sin(2 * DG * mas);
            //sun.longitude_deg = GetSunLongitude2(vct.GetJulianComplete());
            // declination of the sun
            sun.declination_deg = RAD * Math.Asin(0.39777 * Math.Sin(els * DG));

            // right ascension of the sun
            sun.right_asc_deg = els - RAD * Math.Atan2(Math.Sin(2 * els * DG), 23.2377 + Math.Cos(2 * DG * els));

            // equation of time
            double eqt = 0.0;
            eqt = sun.right_asc_deg - mel;

            */
            // definition of event
            double eventdef = 0.01454;
            //double eventdef = 0.0;
            /*	switch(ed.obs)
                {
                case 1:	// civil twilight
                    eventdef = 0.10453;
                    break;
                case 2:	// nautical twilight
                    eventdef = 0.20791;
                    break;
                case 3:	// astronomical twilight
                    eventdef = 0.30902;
                    break;
                default:// center of the sun on the horizont
                    eventdef = 0.01454;
                    break;
                }*/

            eventdef = (eventdef / Math.Cos(dLatitude * DG)) / Math.Cos(sun.declination_deg * DG);

            x = Math.Tan(dLatitude * DG) * Math.Tan(sun.declination_deg * DG) + eventdef;

            // initial values for the case
            // that no rise no set for that day
            sun.sunrise_deg = sun.sunset_deg = -360.0;

            if ((x >= -1.0) && (x <= 1.0))
            {
                double difference = dLongitude + RAD * Math.Asin(x) - equationOfTime;
                // time of sunrise
                sun.sunrise_deg = 90.0 - difference;
                // time of noon
                sun.noon_deg = 180.0 - difference;
                // time of sunset
                sun.sunset_deg = 270.0 - difference;
            }
        }


        // return values are in sun.arunodaya, sun.rise, sun.set, sun.noon, sun.length
        // if values are less than zero, that means, no sunrise, no sunset in that day
        //
        // brahma 1 = calculation at brahma muhurta begining
        // brahma 0 = calculation at sunrise


        public void SunCalc(GPGregorianTime vct, GPLocationProvider earth)
        {
            GPSun sun = this;
            GPSun s_rise = new GPSun();
            GPSun s_set = new GPSun();

            // first calculation
            // for 12:00 universal time
            s_rise.calculateRiseSet(vct, earth, 0.0);
            // second calculation
            // for time of sunrise
            s_rise.calculateRiseSet(vct, earth, s_rise.sunrise_deg - 180);
            // third (last) calculation
            // for time of sunrise
            s_rise.calculateRiseSet(vct, earth, s_rise.sunrise_deg - 180);
            // first calculation
            // for 12:00 universal time
            s_set.calculateRiseSet(vct, earth, 0.0);
            // second calculation
            // for time of sunrise
            s_set.calculateRiseSet(vct, earth, s_set.sunset_deg - 180);
            // third (last) calculation
            // for time of sunrise
            s_set.calculateRiseSet(vct, earth, s_set.sunset_deg - 180);

            // calculate times
            sun.longitude_arun_deg = s_rise.eclipticalLongitude - (24.0 / 365.25);
            sun.eclipticalLongitude = s_rise.eclipticalLongitude;
            sun.right_asc_deg = s_rise.right_asc_deg;
            sun.longitude_set_deg = s_set.eclipticalLongitude;

            sun.arunodaya_deg = s_rise.sunrise_deg - 24.0;
            sun.sunrise_deg = s_rise.sunrise_deg;
            sun.sunset_deg = s_set.sunset_deg;

            // arunodaya is 96 min before sunrise
            //  sunrise_deg is from range 0-360 so 96min=24deg
            sun.arunodaya = new GPGregorianTime(vct);
            sun.arunodaya.setDayHours(SetDegTime(sun.arunodaya_deg + earth.getTimeZoneOffsetHours() * 15.0));
            // sunrise
            sun.rise = new GPGregorianTime(vct);
            sun.rise.setDayHours(SetDegTime(sun.sunrise_deg + earth.getTimeZoneOffsetHours() * 15.0));

            // noon
            sun.noon = new GPGregorianTime(vct);
            sun.noon.setDayHours(SetDegTime((sun.sunset_deg + sun.sunrise_deg) / 2 + earth.getTimeZoneOffsetHours() * 15.0));

            // sunset
            sun.set = new GPGregorianTime(vct);
            sun.set.setDayHours(SetDegTime(sun.sunset_deg + earth.getTimeZoneOffsetHours() * 15.0));
            // length

            // if there is travelling during arunodaya/sunrise
            // then we need to recalculate exact time of arunodaya for travelling path
            if (earth.hasTravelling(sun.rise.getJulianGreenwichTime()))
            {
                GPLocation arunodayaLocation = new GPLocation(earth.getLocation(sun.arunodaya.getJulianGreenwichTime()));
                GPLocation sunriseLocation = earth.getLocation(sun.rise.getJulianGreenwichTime());
                if (!arunodayaLocation.Equals(sunriseLocation))
                {
                    // recalculate arunodaya
                    GPSun newSun = new GPSun();
                    GPLocationProvider arunodayaLocationProvider = new GPLocationProvider(arunodayaLocation);

                    newSun.arunodaya = new GPGregorianTime(sun.arunodaya);
                    for (int count = 0; count < 3; count++)
                    {
                        newSun.SunCalc(sun.arunodaya, arunodayaLocationProvider);
                        arunodayaLocation = earth.getLocation(newSun.arunodaya.getJulianGreenwichTime());
                        arunodayaLocationProvider.setDefaultLocation(arunodayaLocation);
                    }

                    sun.arunodaya_deg = newSun.arunodaya_deg;
                    sun.arunodaya = new GPGregorianTime(vct);
                    sun.arunodaya.setDayHours(SetDegTime(sun.arunodaya_deg + earth.getTimeZoneOffsetHours() * 15.0));
                }

                //
                // recalculate noon time
                GPSun s_noon = new GPSun();
                // first calculation
                // for 12:00 universal time
                s_noon.calculateRiseSet(vct, earth, 0.0);
                // second calculation
                // for time of sunrise
                s_noon.calculateRiseSet(vct, earth, s_noon.noon_deg - 180);
                // third (last) calculation
                // for time of sunrise
                s_noon.calculateRiseSet(vct, earth, s_noon.noon_deg - 180);

                sun.noon_deg = s_noon.noon_deg;
                sun.noon = new GPGregorianTime(vct);
                sun.noon.setDayHours(SetDegTime(sun.noon_deg + earth.getTimeZoneOffsetHours() * 15.0));
            }

            sun.length_deg = s_set.sunset_deg - s_rise.sunrise_deg;
            sun.DayLength = (sun.length_deg / 360.0) * 24.0;



        }

        public double getSunriseDayHours()
        {
            return rise.getDayHours();
        }
    }
}
