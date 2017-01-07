using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSunMethodMeeus: GPSunMethod
    {

        public const int SUNRISE_TYPE_CENTER = 0;
        public const int SUNRISE_TYPE_TOPTIP = 1;
        public const int SUNRISE_TYPE_BOTTOMTIP = 2;
        public const int SUNRISE_TYPE_CIVIL = 3;
        public const int SUNRISE_TYPE_NAUTICAL = 4;
        public const int SUNRISE_TYPE_ASTRONOMICAL = 5;

        public static int sunriseType = SUNRISE_TYPE_TOPTIP;

        // julianDay
        private double julianDay;

        public double julianDayRise;
        public double julianDayNoon;
        public double julianDaySet;

        public GPSunMethodMeeus()
        {
        }

        public GPSunMethodMeeus(GPSunMethodMeeus sn)
            : base(sn)
        {
        }

        // VCTIME vct [in] - valid must be each member of this structure
        //

        // from vct uses members: year, month, day
        // DayHours is in degrees (360deg = 24hours)



        public void calculateCoordinatesMethodM(double julian, SunCoords coords)
        {
            double DG = GPMath.rads;
            double RAD = 180 / GPMath.pi;

            double t = (julian - 2451545.0) / 36525;
            double t2 = t * t;
            double t3 = t2 * t;
            double t4 = t2 * t2;

            // mean ecliptic longitude of the sun 
            double L0 = 280.4664567 + 36000.76982779 * t + 0.0003032028 * t2 + t3 / 49931000;

            // mean anomaly of the sun
            double M = 357.5291 + 35999.05030 * t - 0.0001559 * t2 - 0.00000048 * t3;

            L0 = GPMath.putIn360(L0);
            M = GPMath.putIn360(M);

            double C = (1.9146 - 0.004817 * t - 0.000014 * t2) * GPMath.sinDeg(M)
                     + (0.019993 - 0.000101 * t) * GPMath.sinDeg(2 * M)
                     + 0.00029 * GPMath.sinDeg(3 * M);

            // ecliptic longitude of the sun
            //double els = 0;
            coords.eclipticalLongitude = GPMath.putIn360(L0 + C);

            double e = 0.016708617 - 0.000042037 * t - 0.0000001236 * t2;
            double trueAnomaly = M + C;


            //            double epsilon;
            //            double deltaPhi;

            //            GPAstroEngine.calc_epsilon_phi(julianDay, out deltaPhi, out epsilon);
            //            = 23.4391 - 0.013 * t - t2/6101694;
            double omega = 125.04 - 1934.136 * t;
            double lambda = coords.eclipticalLongitude - 0.00569 - 0.00478 * GPMath.sinDeg(omega);
            double epsilon0 = 23.4392911 - 0.01300416 * t - 1.638e-7 * t2;
            double epsilon1 = epsilon0 + 0.00256 * GPMath.cosDeg(omega);

            // right ascension of the sun
            coords.rightAscession = RAD * Math.Atan2(GPMath.cosDeg(epsilon1) * GPMath.sinDeg(lambda), GPMath.cosDeg(lambda));
            coords.rightAscession = GPMath.putIn360(coords.rightAscession);

            // declination of the sun
            coords.declination = GPMath.arcsinDeg(GPMath.sinDeg(epsilon1) * GPMath.sinDeg(lambda));

            // equation of time
            coords.equationOfTime = GPAstroEngine.getEquationOfTime(julian, coords.rightAscession);
        }


        private void calculateRiseSetMethodM(double D, GPLocation ed)
        {
            GPLocation obs = ed;
            double a1, a2, a3;
            double d1, d2, d3;
            double siderealTime = GPAstroEngine.GetSiderealTime(D);
            double h0 = -0.833333;
            SunCoords coords = new SunCoords();
            calculateCoordinatesMethodM(D - 1, coords);
            a1 = coords.rightAscession;
            d1 = coords.declination;
            calculateCoordinatesMethodM(D, coords);
            a2 = coords.rightAscession;
            d2 = coords.declination;
            calculateCoordinatesMethodM(D + 1, coords);
            a3 = coords.rightAscession;
            d3 = coords.declination;
            double longitude = -ed.GetLongitudeEastPositive();
            double latitude = ed.GetLatitudeNorthPositive();
            double cosH0 = (GPMath.sinDeg(h0) - GPMath.sinDeg(latitude) * GPMath.sinDeg(d2))
                / (GPMath.cosDeg(latitude) * GPMath.cosDeg(d2));
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


        // return values are in sun.arunodaya, sun.rise, sun.set, sun.noon, sun.length
        // if values are less than zero, that means, no sunrise, no sunset in that day
        //
        // brahma 1 = calculation at brahma muhurta begining
        // brahma 0 = calculation at sunrise



        public void SunCalc(GPGregorianTime vct, GPLocation earth)
        {
            julianDay = vct.getJulianGreenwichNoon() - 0.5;

            SunCoords coords = new SunCoords();

            //calculateCoordinatesMethodM(julianDay, coords);
            calculateRiseSetMethodM(julianDay, earth);

            double gmt = vct.getJulianGreenwichNoon();

            arunodaya = new GPBodyData(vct);
            rise = new GPBodyData(vct);
            noon = new GPBodyData(vct);
            set = new GPBodyData(vct);

            arunodaya.eclipticalLongitude = GPAstroEngine.sunLongitudeMethodM(julianDayRise - 96 / 1440.0);
            rise.eclipticalLongitude = GPAstroEngine.sunLongitudeMethodM(julianDayRise);
            set.eclipticalLongitude = GPAstroEngine.sunLongitudeMethodM(julianDaySet);

            arunodaya.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayRise - 96 / 1440.0));
            rise.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayRise));
            noon.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDayNoon));
            set.setJulianGreenwichTime(GPAstroEngine.ConvertDynamicToUniversal(julianDaySet));

            // finally calculate length of the daylight
            DayLength = (set.getJulianGreenwichTime() - rise.getJulianGreenwichTime()) * 24.0;

        }
    }
}
