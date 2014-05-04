using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPFunctorSun:GPFunctor
    {
        public enum ValueType
        {
            None,
            Longitude,
            Latitude,
            RightAscession,
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
            Distance,
            Sankranti,
            SankrantiDistance
        }

        ValueType valueType = ValueType.None;
        GPObserver obs = null;
        GPCelestialBodyCoordinates coord = null;

        public GPFunctorSun(ValueType vt)
        {
            valueType = vt;
        }

        public GPFunctorSun(ValueType vt, GPObserver o)
        {
            valueType = vt;
            obs = o;
        }

        public override double getDoubleValue(double arg)
        {
            coord = GPAstroEngine.sun_coordinate(arg);

            switch (valueType)
            {
                case ValueType.Longitude:
                    return coord.eclipticalLongitude;
                case ValueType.Latitude:
                    return coord.eclipticalLatitude;
                case ValueType.Azimuth:
                    if (obs != null)
                    {
                        GPAstroEngine.calcHorizontal(coord, obs);
                        return coord.azimuth;
                    }
                    break;
                case ValueType.Elevation:
                    if (obs != null)
                    {
                        GPAstroEngine.calcHorizontal(coord, obs);
                        return coord.elevation;
                    }
                    break;
                case ValueType.DeclinationTopo:
                    return coord.declination;
                case ValueType.RightAscessionTopo:
                    return coord.right_ascession;
                case ValueType.Declination:
                    return coord.declination;
                case ValueType.RightAscession:
                    return coord.right_ascession;
                case ValueType.Distance:
                    return coord.distanceFromEarth;
                case ValueType.ApparentSiderealTime:
                    return coord.apparent_sidereal_time;
                case ValueType.MeanSiderealTime:
                    return coord.mean_sidereal_time;
                case ValueType.DynamicTime:
                    return coord.getDynamicTime();
                case ValueType.Nutation:
                    return coord.getNutation();
                case ValueType.Obliquity:
                    return coord.getObliquity();
                case ValueType.Sankranti:
                    return getSankrantiDouble(arg);
                case ValueType.SankrantiDistance:
                    {
                        double sankranti = getSankrantiDouble(arg);
                        sankranti = 0.51 - Math.Abs(sankranti - Math.Floor(sankranti) - 0.5);
                        return sankranti;
                    }
                default:
                    break;
            }

            return 0;
        }


        public GPCelestialBodyCoordinates getSunCoordinates()
        {
            return coord;
        }

        public double getSankrantiDouble(double jd)
        {
            double ld = GPMath.putIn360(coord.eclipticalLongitude - GPAyanamsa.GetAyanamsa(jd));
            return ld / 30.0;
        }

    }
}
