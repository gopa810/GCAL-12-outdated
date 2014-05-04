using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPFunctorSunMoon: GPFunctor
    {
        public enum ValueType
        {
            None,
            Longitude,
            Latitude,
            RightAscession,
            RightAscessionAbs,
            RightAscessionTopo,
            Declination,
            DeclinationTopo,
            Azimuth,
            Elevation,
            Nutation,
            Obliquity,
            ApparentSiderealTime,
            MeanSiderealTime,
            DynamicTime,
            UniversalTime,
            AzimuthElevationDistance,
            RigthAscessionDeclinationDistance,
            RigthAscessionDeclinationOpositeDistance,
            TithiDistance,
            Tithi,
            Yoga,
            YogaDistance
        }

        ValueType valueType = ValueType.None;
        GPObserver obs = null;
        GPCelestialBodyCoordinates coordMoon = null;
        GPCelestialBodyCoordinates coordSun = null;

        public GPFunctorSunMoon(ValueType vt)
        {
            valueType = vt;
        }

        public GPFunctorSunMoon(ValueType vt, GPObserver o)
        {
            valueType = vt;
            obs = o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg">Julian Ephemeris (Dynamical) Time</param>
        /// <returns></returns>
        public override double getDoubleValue(double arg)
        {
            coordMoon = GPAstroEngine.moon_coordinate(arg);
            coordSun = GPAstroEngine.sun_coordinate(arg);

            switch (valueType)
            {
                case ValueType.Longitude:
                    return GPMath.putIn180(coordMoon.eclipticalLongitude - coordSun.eclipticalLongitude);
                case ValueType.Latitude:
                    return GPMath.putIn180(coordMoon.eclipticalLatitude - coordSun.eclipticalLatitude);
                case ValueType.Azimuth:
                    if (obs != null)
                    {
                        coordMoon.makeTopocentric(obs);
                        GPAstroEngine.calcHorizontal(coordMoon, obs);

                        GPAstroEngine.calcHorizontal(coordSun, obs);
                        return GPMath.putIn180(coordMoon.azimuth - coordSun.azimuth);
                    }
                    break;
                case ValueType.Elevation:
                    if (obs != null)
                    {
                        coordMoon.makeTopocentric(obs);
                        GPAstroEngine.calcHorizontal(coordMoon, obs);
                        GPAstroEngine.calcHorizontal(coordSun, obs);
                        return GPMath.putIn180(coordMoon.elevation - coordSun.elevation);
                    }
                    break;
                case ValueType.DeclinationTopo:
                    if (obs != null)
                    {
                        coordMoon.makeTopocentric(obs);
                        return GPMath.putIn180(coordMoon.declination - coordSun.declination);
                    }
                    break;
                case ValueType.RightAscessionTopo:
                    if (obs != null)
                    {
                        coordMoon.makeTopocentric(obs);
                        return GPMath.putIn180(coordMoon.right_ascession - coordSun.right_ascession);
                    }
                    break;
                case ValueType.Declination:
                    return GPMath.putIn180(coordMoon.declination - coordSun.declination);
                case ValueType.TithiDistance:
                    {
                        double tithi = getTithiDouble();
                        tithi = 0.51 - Math.Abs(tithi - Math.Floor(tithi) - 0.5);
                        return tithi;
                    }
                case ValueType.YogaDistance:
                    {
                        double yoga = getYogaDouble(arg);
                        yoga = 0.51 - Math.Abs(yoga - Math.Floor(yoga) - 0.5);
                        return yoga;
                    }
                case ValueType.RightAscession:
                    return GPMath.putIn180(coordMoon.right_ascession - coordSun.right_ascession);
                case ValueType.RightAscessionAbs:
                    return Math.Abs(GPMath.putIn180(coordMoon.right_ascession - coordSun.right_ascession));
                case ValueType.ApparentSiderealTime:
                    return coordMoon.apparent_sidereal_time;
                case ValueType.MeanSiderealTime:
                    return coordMoon.mean_sidereal_time;
                case ValueType.DynamicTime:
                    return coordMoon.getDynamicTime();
                case ValueType.Nutation:
                    return coordMoon.getNutation();
                case ValueType.Obliquity:
                    return coordMoon.getObliquity();
                case ValueType.AzimuthElevationDistance:
                    if (obs != null)
                    {
                        coordMoon.makeTopocentric(obs);
                        GPAstroEngine.calcHorizontal(coordMoon, obs);
                        GPAstroEngine.calcHorizontal(coordSun, obs);
                        return GPMath.arcDistanceDeg(coordMoon.azimuth, coordMoon.elevation, coordSun.azimuth, coordSun.elevation);
                    }
                    break;
                case ValueType.RigthAscessionDeclinationDistance:
                    return GPMath.arcDistanceDeg(coordMoon.right_ascession, coordMoon.declination, coordSun.right_ascession, coordSun.declination);
                case ValueType.RigthAscessionDeclinationOpositeDistance:
                    return GPMath.arcDistanceDeg(180 + coordMoon.right_ascession, -coordMoon.declination, coordSun.right_ascession, coordSun.declination);
                case ValueType.Tithi:
                    return getTithiDouble();
                case ValueType.Yoga:
                    return getYogaDouble(arg);
                default:
                    break;
            }

            return 0;
        }


        public GPCelestialBodyCoordinates getSunCoordinates()
        {
            return coordSun;
        }

        public GPCelestialBodyCoordinates getMoonCoordinates()
        {
            return coordMoon;
        }

        public double getTithiDouble()
        {
            return GPMath.putIn360(coordMoon.eclipticalLongitude - coordSun.eclipticalLongitude-180) / 30;
        }

        public double getYogaDouble(double day)
        {
            double d = GPMath.putIn360(coordMoon.eclipticalLongitude + coordSun.eclipticalLongitude - 2 * GPAyanamsa.GetAyanamsa(day));
            return (d * 3.0) / 40.0;
        }

    }
}
