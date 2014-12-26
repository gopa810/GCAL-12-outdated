using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

        public const int SUNPOSMETHOD_CALCULATOR = 1;
        public const int SUNPOSMETHOD_CALCULATOREX = 2;

        public static int sunPosMethod = SUNPOSMETHOD_CALCULATOR;


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
        // julianDay
        private double julianDay;
        private double sunEquationCenter;
        private double sunMeanAnomaly;

        public double length_deg;
        public double arunodaya_deg;
        public double sunrise_deg;
        public double sunset_deg;
        public double noon_deg;


        public double longitude_set_deg;
        public double longitude_arun_deg;

        public double julianDayRise;
        public double julianDayNoon;
        public double julianDaySet;

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

        public void calculateCoordinatesMethodM(double julian)
        {
            double DG = GPMath.rads;
            double RAD = 180 / GPMath.pi;

            double t = (julian - 2451545.0) / 36525;
            double t2 = t * t;
            double t3 = t2 * t;
            double t4 = t2 * t2;

            // mean ecliptic longitude of the sun 
            double L0 = 280.4664567 + 36000.76982779*t + 0.0003032028*t2 + t3/49931000;

            // mean anomaly of the sun
            double M = 357.5291 + 35999.05030 * t - 0.0001559 * t2 - 0.00000048 * t3;

            L0 = GPMath.putIn360(L0);
            M = GPMath.putIn360(M);
            sunMeanAnomaly = M;

            double C = (1.9146 - 0.004817*t - 0.000014*t2) * GPMath.sinDeg(M)
                     + (0.019993 - 0.000101 * t) * GPMath.sinDeg(2 * M)
                     + 0.00029 * GPMath.sinDeg(3 * M);
            sunEquationCenter = C;

            // ecliptic longitude of the sun
            //double els = 0;
            eclipticalLongitude = GPMath.putIn360(L0 + C);

            double e = 0.016708617 - 0.000042037 * t - 0.0000001236 * t2;
            double trueAnomaly = M + C;


//            double epsilon;
//            double deltaPhi;

//            GPAstroEngine.calc_epsilon_phi(julianDay, out deltaPhi, out epsilon);
//            = 23.4391 - 0.013 * t - t2/6101694;
            double omega = 125.04 - 1934.136 * t;
            double lambda = eclipticalLongitude - 0.00569 - 0.00478 * GPMath.sinDeg(omega);
            double epsilon0 = 23.4392911 - 0.01300416 * t - 1.638e-7 * t2;
            double epsilon1 = epsilon0 + 0.00256 * GPMath.cosDeg(omega);

            // right ascension of the sun
            this.rightAscession = RAD * Math.Atan2(GPMath.cosDeg(epsilon1) * GPMath.sinDeg(lambda), GPMath.cosDeg(lambda));
            this.rightAscession = GPMath.putIn360(rightAscession);

            // declination of the sun
            this.declination = GPMath.arcsinDeg(GPMath.sinDeg(epsilon1) * GPMath.sinDeg(lambda));

            // equation of time
            equationOfTime =  GPAstroEngine.getEquationOfTime(julian, this.rightAscession);
        }

        public void calculateCoordinatesMethodC(GPGregorianTime vct, double DayHours)
        {
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;

            // mean ecliptic longitude of the sun 
            double mel = SunGetMeanLong(vct.getYear(), vct.getMonth(), vct.getDay()) + (360 / 365.25) * DayHours / 360.0;
            // ecliptic longitude of perigee 
            double elp = SunGetPerigee(vct.getYear(), vct.getMonth(), vct.getDay());

            // mean anomaly of the sun
            double M = mel - elp;

            // equation of center
            double C = 1.9148 * GPMath.sinDeg(M)
                     + 0.02   * GPMath.sinDeg(2 * M)
                     + 0.0003 * GPMath.sinDeg(3 * M);

            // ecliptic longitude of the sun
            double els = 0;
            eclipticalLongitude = els = mel + C;

            // declination of the sun
            declination = GPMath.arcsinDeg(0.397948 * GPMath.sinDeg(eclipticalLongitude));

            // right ascension of the sun
            rightAscession = els - RAD * Math.Atan2(Math.Sin(2 * els * DG), 23.2377 + Math.Cos(2 * DG * els));

            // equation of time
            equationOfTime = rightAscession - mel;
            //equationOfTime = GPAstroEngine.getEquationOfTime(julianDay, right_asc_deg);
            //Debugger.Log(0,"", String.Format("{1}: EoTdiff = {0}\n", vct.getShortDateString(), equationOfTime - (right_asc_deg - mel)));
        }

        // VCTIME vct [in] - valid must be each member of this structure
        //

        // from vct uses members: year, month, day
        // DayHours is in degrees (360deg = 24hours)

        public void calculateRiseSet(GPGregorianTime vct, GPLocationProvider ed, double DayHours)
        {
            GPSun sun = this;
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;

            julianDay = vct.getJulianGreenwichNoon() - 0.5 + DayHours / 360;

            if (sunPosMethod == SUNPOSMETHOD_CALCULATOR)
            {
                calculateCoordinatesMethodC(vct, DayHours);
                calculateRiseSetMethodA(vct, ed, DayHours, sun, DG, RAD);
            }
            else if (sunPosMethod == SUNPOSMETHOD_CALCULATOREX)
            {
                calculateCoordinatesMethodM(julianDay);
                calculateRiseSetMethodM(julianDay, ed);
            }

            //calculateRiseSetMethodA(vct, ed, DayHours, sun, DG, RAD);
        }

        private void calculateRiseSetMethodM(double D, GPLocationProvider ed)
        {
            GPLocation obs = ed.getLocation(D);
            double a1, a2, a3;
            double d1, d2, d3;
            double siderealTime = GPAstroEngine.GetSiderealTime(D);
            double h0 = -0.833333;
            calculateCoordinatesMethodM(D - 1);
            a1 = rightAscession;
            d1 = declination;
            calculateCoordinatesMethodM(D);
            a2 = rightAscession;
            d2 = declination;
            calculateCoordinatesMethodM(D + 1);
            a3 = rightAscession;
            d3 = declination;
            double longitude = -ed.GetLongitudeEastPositive();
            double latitude = ed.GetLatitudeNorthPositive();
            double cosH0 = (GPMath.sinDeg(h0) - GPMath.sinDeg(latitude)*GPMath.sinDeg(d2)) 
                / (GPMath.cosDeg(latitude)*GPMath.cosDeg(d2));
            double H0 = GPMath.arccosDeg(cosH0);

            H0 = GPMath.putIn180(H0);

            double m0 = (a2 + longitude - siderealTime) / 360;
            double m1 = m0 - H0 / 360;
            double m2 = m0 + H0 / 360;
            double deltaM = 0;

            deltaM = getCorrection(D, a1, a2, a3, d1, d2, d3, siderealTime, h0, longitude, latitude, m0, true);
            m0 += deltaM;
            deltaM = getCorrection(D, a1, a2, a3, d1, d2, d3, siderealTime, h0, longitude, latitude, m1, false);
            m1 += deltaM;
            deltaM = getCorrection(D, a1, a2, a3, d1, d2, d3, siderealTime, h0, longitude, latitude, m2, false);
            m2 += deltaM;

            julianDayRise = julianDay + m1;
            julianDayNoon = julianDay + m0;
            julianDaySet = julianDay + m2;

            sunrise_deg = GPMath.putIn360(m1 * 360);
            noon_deg = GPMath.putIn360(m0 * 360);
            sunset_deg = GPMath.putIn360(m2 * 360);
        }

        private static double getCorrection(double D, double a1, double a2, double a3, double d1, double d2, double d3, double siderealTime, double h0, double longitude, double latitude, double m0, bool transit)
        {
            double deltaM = 0;
            double PHI = siderealTime + 360.985647 * m0;
            double n = m0 + GPDynamicTime.GetDeltaT(D) / 86400;

            double alpha = GPAstroEngine.interpolation(a1, a2, a3, n);
            double delta = GPAstroEngine.interpolation(d1, d2, d3, n);

            double H = GPMath.putIn180(PHI - longitude - alpha);
            double h = 0;
            if (transit)
            {
                deltaM = -H / 360;
            }
            else
            {
                double sinH = GPMath.sinDeg(latitude) * GPMath.sinDeg(delta) + GPMath.cosDeg(latitude) * GPMath.cosDeg(delta) * GPMath.cosDeg(H);
                h = GPMath.arcsinDeg(sinH);
                deltaM = (h - h0) / (360 * GPMath.cosDeg(delta) * GPMath.cosDeg(latitude) * GPMath.sinDeg(H));
            }
            return deltaM;
        }

        private void calculateRiseSetMethodA(GPGregorianTime vct, GPLocationProvider ed, double DayHours, GPSun sun, double DG, double RAD)
        {
            double time = vct.getJulianLocalNoon() - 0.5 + DayHours - vct.getTimeZoneOffsetHours() / 24.0;
            double dLatitude = ed.getLocation(time).GetLatitudeNorthPositive();
            double dLongitude = ed.getLocation(time).GetLongitudeEastPositive();

            // definition of event
            // eventdef = 0.0;
            // civil twilight eventdef = 0.10453;
            // nautical twilight eventdef = 0.20791;
            // astronomical twilight eventdef = 0.30902;
            // center of the sun on the horizont eventdef = 0.01454;
            double eventdef = 0.01454;

            double x = GPMath.tanDeg(dLatitude) * GPMath.tanDeg(sun.declination) + eventdef / (GPMath.cosDeg(dLatitude) * GPMath.cosDeg(sun.declination));

            if (x < -1.0 || x > 1.0)
            {
                // initial values for the case
                // that no rise no set for that day
                sun.sunrise_deg = -360;
                sun.noon_deg = -360;
                sun.sunset_deg = -360;
                return;
            }

            double hourAngle = GPMath.arcsinDeg(x);

            // time of sunrise
            sun.sunrise_deg = 90.0 - dLongitude - hourAngle + equationOfTime;
            // time of noon
            sun.noon_deg = 180.0 - dLongitude + equationOfTime;
            // time of sunset
            sun.sunset_deg = 270.0 - dLongitude + hourAngle + equationOfTime;

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

            if (sunPosMethod == SUNPOSMETHOD_CALCULATOR)
            {
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
                sun.rightAscession = s_rise.rightAscession;
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
            }
            else
            {
                calculateRiseSet(vct, earth, 180);
                arunodaya_deg = sunrise_deg - 24.0;
                double gmt = vct.getJulianGreenwichNoon();

                longitude_arun_deg = GPAstroEngine.sunLongitudeMethodM(julianDayRise - 96/1440.0);
                eclipticalLongitude = GPAstroEngine.sunLongitudeMethodM(julianDayRise);
                longitude_set_deg = GPAstroEngine.sunLongitudeMethodM(julianDaySet);

                sun.arunodaya = new GPGregorianTime(vct);
                sun.arunodaya.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayRise - 96/1440.0));

                sun.rise = new GPGregorianTime(vct);
                sun.rise.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayRise));

                sun.noon = new GPGregorianTime(vct);
                sun.noon.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayNoon));

                sun.set = new GPGregorianTime(vct);
                sun.set.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDaySet));
            }




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
