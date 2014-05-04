using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPFunctorMoon: GPFunctor
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
            Naksatra,
            NaksatraDistance
        }

        ValueType valueType = ValueType.None;
        GPObserver obs = null;
        GPCelestialBodyCoordinates coord = null;

        public GPFunctorMoon(ValueType vt)
        {
            valueType = vt;
        }

        public GPFunctorMoon(ValueType vt, GPObserver o)
        {
            valueType = vt;
            obs = o;
        }

        public override double getDoubleValue(double arg)
        {
            coord = GPAstroEngine.moon_coordinate(arg);

            switch (valueType)
            {
                case ValueType.Longitude:
                    return coord.eclipticalLongitude;
                case ValueType.Latitude:
                    return coord.eclipticalLatitude;
                case ValueType.Azimuth:
                    if (obs != null)
                    {
                        coord.makeTopocentric(obs);
                        GPAstroEngine.calcHorizontal(coord, obs);
                        return coord.azimuth;
                    }
                    break;
                case ValueType.Elevation:
                    if (obs != null)
                    {
                        coord.makeTopocentric(obs);
                        GPAstroEngine.calcHorizontal(coord, obs);
                        return coord.elevation;
                    }
                    break;
                case ValueType.DeclinationTopo:
                    if (obs != null)
                    {
                        coord.makeTopocentric(obs);
                        return coord.declination;
                    }
                    break;
                case ValueType.RightAscessionTopo:
                    if (obs != null)
                    {
                        coord.makeTopocentric(obs);
                        return coord.right_ascession;
                    }
                    break;
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
                case ValueType.NaksatraDistance:
                    {
                        double naksatraReal = getNaksatraDouble(arg);
                        naksatraReal = 0.51 - Math.Abs(naksatraReal - Math.Floor(naksatraReal) - 0.5);
                        return naksatraReal;
                    }
                case ValueType.Naksatra:
                    return getNaksatraDouble(arg);
                default:
                    break;
            }

            return 0;
        }

        public GPCelestialBodyCoordinates getMoonCoordinates()
        {
            return coord;
        }

        public double getNaksatraDouble(double day)
        {
            double d = GPMath.putIn360(coord.eclipticalLongitude - GPAyanamsa.GetAyanamsa(day));
            return (d * 3.0) / 40.0;
        }

    }
}
