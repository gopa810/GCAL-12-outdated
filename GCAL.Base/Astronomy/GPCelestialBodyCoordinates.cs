using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCelestialBodyCoordinates
    {
        public double eclipticalLongitude;
        public double eclipticalLatitude;
        public double distanceFromEarth;
        public double right_ascession;
        public double declination;
        public double azimuth;
        public double elevation;
        public double parallax;

        public double delta_phi;
        public double epsilon;
        public double apparent_sidereal_time;
        public double mean_sidereal_time;
        private double julianDay;
        private double deltaT;
        private double julianEphemerisDay;

        private const int JULIAN = 0x0002;
        private const int JDE = 0x0004;
        private const int DELTAT = 0x0008;

        private int init_values = 0;

        /// <summary>
        /// Calculates sidereal time at Greenwich. 
        /// Based on Chapter 11 of Astronomical Algorithms.
        /// </summary>
        /// <param name="date">Julian Ephemeris Day</param>
        /// <returns>Sidereal time in degrees.</returns>
        public void SetSiderealTime(double date)
        {
            double t;
            //date = 2446896.30625;
            //jd = date;
            t = (date - GPAstroEngine.J2000) / 36525.0;
            GPAstroEngine.calc_epsilon_phi(date, out delta_phi, out epsilon);

            // 11.2
            mean_sidereal_time = GPMath.putIn360(280.46061837 + 360.98564736629 * (date - GPAstroEngine.J2000) +
                             t * t * (0.000387933 - t / 38710000));

            apparent_sidereal_time = GPMath.putIn360(mean_sidereal_time + delta_phi * GPMath.cosDeg(epsilon));
        }

        public double getDynamicTime()
        {
            return julianEphemerisDay;
        }

        /// <summary>
        /// Calculates visibility angle of celectial body, for given physical radius.
        /// </summary>
        /// <param name="physicalRadius">Radius in kilometers</param>
        /// <returns></returns>
        public double getVisibleAngle(double physicalRadius)
        {
            return GPMath.rad2deg(Math.Asin(physicalRadius / distanceFromEarth));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jd">Julian Day value equivalent to Universal Time</param>
        /// <returns></returns>
        public void SetJulianDay(double jd)
        {
            julianDay = jd;
            deltaT = GPDynamicTime.GetDeltaT(jd);
            julianEphemerisDay = julianDay + deltaT;

            init_values |= (JULIAN | JDE | DELTAT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jd">Julian Day value equivalent to Dynamic Time</param>
        /// <returns></returns>
        public void SetJulianEphemerisDay(double jde)
        {
            julianEphemerisDay = jde;
            deltaT = GPDynamicTime.GetDeltaT(jde);
            julianDay = julianEphemerisDay - deltaT;

            init_values |= (JULIAN | JDE | DELTAT);
        }

        /// <summary>
        /// Based on Chapter 39, Meeus and Chapter 10
        /// </summary>
        /// <param name="obs"></param>
        public void makeTopocentric(GPObserver obs)
        {
            double u, h, delta_alpha;
            double rho_sin, rho_cos;
            const double b_a = 0.99664719;

            // geocentric position of observer on the earth surface
            // 10.1 - 10.3
            u = GPMath.arctanDeg(b_a * b_a * GPMath.tanDeg(obs.GetLatitudeNorthPositive()));
            rho_sin = b_a * GPMath.sinDeg(u) + obs.GetAltitude() / 6378140.0 * GPMath.sinDeg(obs.GetLatitudeNorthPositive());
            rho_cos = GPMath.cosDeg(u) + obs.GetAltitude() / 6378140.0 * GPMath.cosDeg(obs.GetLatitudeNorthPositive());

            // equatorial horizontal paralax
            // 39.1
            this.parallax = GPMath.arcsinDeg(GPMath.sinDeg(8.794 / 3600) / (this.distanceFromEarth / GPAstroEngine.AU));

            // geocentric hour angle of the body
            h = apparent_sidereal_time - obs.GetLongitudeWestPositive() - right_ascession;


            // 39.2
            delta_alpha = GPMath.arctanDeg(
                        (-rho_cos * GPMath.sinDeg(this.parallax) * GPMath.sinDeg(h)) /
                        (GPMath.cosDeg(this.declination) - rho_cos * GPMath.sinDeg(this.parallax) * GPMath.cosDeg(h)));

            this.right_ascession += delta_alpha;
            this.declination = GPMath.arctanDeg(
              ((GPMath.sinDeg(this.declination) - rho_sin * GPMath.sinDeg(this.parallax)) * GPMath.cosDeg(delta_alpha)) /
              (GPMath.cosDeg(this.declination) - rho_cos * GPMath.sinDeg(this.parallax) * GPMath.cosDeg(h)));
        }

        public double getNutation()
        {
            return delta_phi;
        }

        public double getObliquity()
        {
            return epsilon;
        }
    }
}
