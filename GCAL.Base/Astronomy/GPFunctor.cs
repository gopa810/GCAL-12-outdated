using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    /// <summary>
    /// Base class for functors. These entities are used for searching limits, transitions and other nodes
    /// for some functions of argument.
    /// Generaly argument for getValue function is whatever continuous value,
    /// but usually it is used with argument of julian day
    /// </summary>
    public class GPFunctor
    {
        /// <summary>
        /// Function for returning value
        /// </summary>
        /// <param name="arg">Value of argument</param>
        /// <returns>Value of result</returns>
        public virtual double getDoubleValue(double arg)
        {
            return 0;
        }
    }

    public enum GPValueType
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
        SiderealTime,
        DynamicTime,
        UniversalTime,
        Distance
    }
}
