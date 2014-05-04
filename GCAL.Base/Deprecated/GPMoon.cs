using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMoon
    {
        /*// illumination (synodic) phase
        double ip;
        // age of moon
        int age;
        // distance from anomalistic phase
        double di;*/
        // latitude from nodal (draconic) phase
        public double latitude_deg;
        // longitude from sidereal motion
        public double longitude_deg;

        public double radius; //(* lambda, beta, R *)
        public double right_ascession;
        public double declination;  //(* alpha, delta *)
        public double parallax;
        public double elevation;
        public double azimuth;//          (* h, A *)
        public double distance = -1;

        private GPCelestialBodyCoordinates coord = null;

        public GPMoon()
        {
        }

        public GPMoon(GPMoon m)
        {
            latitude_deg = m.latitude_deg;
            longitude_deg = m.longitude_deg;
            radius = m.radius;
            right_ascession = m.right_ascession;
            declination = m.declination;
            parallax = m.parallax;
            elevation = m.elevation;
            azimuth = m.azimuth;
            distance = m.distance;
        }

        public int GetRasi(double ayanamsa)
        {
            return Convert.ToInt32(Math.Floor(GPMath.putIn360(longitude_deg - ayanamsa) / 30.0));
        }

        public static void calc_epsilon_phi(double date, out double delta_phi, out double epsilon)
        {
            GPAstroEngine.calc_epsilon_phi(date, out delta_phi, out epsilon);
        }

        public static void calc_geocentric(ref double longitude, ref double latitude, ref double rektaszension, ref double declination, double date)
        {
            //var
            double epsilon; //: extended;
            double delta_phi; //: extended;
            double alpha, delta; //: extended;

            calc_epsilon_phi(date, out delta_phi, out epsilon);
            longitude = GPMath.putIn360(longitude + delta_phi);

            alpha = GPMath.arctan2Deg(GPMath.sinDeg(longitude) * GPMath.cosDeg(epsilon) - GPMath.tanDeg(latitude) * GPMath.sinDeg(epsilon), GPMath.cosDeg(longitude));

            delta = GPMath.arcsinDeg(GPMath.sinDeg(latitude) * GPMath.cosDeg(epsilon) + GPMath.cosDeg(latitude) * GPMath.sinDeg(epsilon) * GPMath.sinDeg(longitude));

            rektaszension = alpha;
            declination = delta;

            double xg, yg, zg;

            xg = GPMath.cosDeg(longitude) * GPMath.cosDeg(latitude);
            yg = GPMath.sinDeg(longitude) * GPMath.cosDeg(latitude);
            zg = GPMath.sinDeg(latitude);

            alpha = GPMath.arctan2Deg(yg * GPMath.cosDeg(epsilon) - zg * GPMath.sinDeg(epsilon), GPMath.cosDeg(longitude) * GPMath.cosDeg(latitude));
        }


        public void MoonCalc(double jdate)
        {
            coord = GPAstroEngine.moon_coordinate(jdate);
            radius = coord.distanceFromEarth;
            longitude_deg = coord.eclipticalLongitude;
            latitude_deg = coord.eclipticalLatitude;
            declination = coord.declination;
            right_ascession = coord.right_ascession;

        }

        public static double star_time(double date)
        {
            return GPAstroEngine.GetSiderealTime(date);
        }

        public const double MOON_RADIUS = 1737.4;

        public double getAngleRadius()
        {
            return GPMath.rad2deg(2 * Math.Asin(MOON_RADIUS / distance));
        }

        public override string ToString()
        {
            return String.Format("Azimut:{0}, Elevation:{1}", azimuth, elevation);
        }
        public static void CalcMoonTimes(GPLocationProvider e, GPGregorianTime vc, out GPGregorianTime rise, out GPGregorianTime set)
        {
            double UT;

            rise = null;
            set = null;

            // inicializacia prvej hodnoty ELEVATION
            vc.setDayHours(0.0);
            vc.normalizeValues();
            UT = vc.getJulianGreenwichTime();
            GPJulianTime start = new GPJulianTime();
            GPJulianTime time;
            start.setLocalJulianDay(vc.getJulianGreenwichTime());
            TRiseSet rs;
            set = null;
            rise = null;
            time = GPAstroEngine.GetNextMoonEvent(start, e, out rs);
            while (time.getLocalJulianDay() < UT + 1)
            {
                GPGregorianTime gt = new GPGregorianTime(e, time);
                if (rs == TRiseSet.RISE && rise == null)
                {
                    rise = gt;
                }
                else if (rs == TRiseSet.SET && set == null)
                {
                    set = gt;
                }
                time = GPAstroEngine.GetNextMoonEvent(time, e, out rs);
            }

        }


    }
}
