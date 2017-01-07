using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    /// <summary>
    /// Encapsulation class for providing sun calculations
    /// For now, the selection of method of the calculation
    /// is hardcoded by inheritance from one of these classes:
    ///    GPSunMethodSimple
    ///    GPSunMethodMeeus
    ///    
    /// </summary>
    public class GPSun: GPSunMethodSimple
    {
        public GPSun()
        {
        }

        public GPSun(GPSun sun) : base(sun)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jdate">Julian day (Universal Time)</param>
        /// <returns>Returns value of ecliptic longitude</returns>
        public static double GetSunLongitude(double jdate)
        {
            GPSun sun = new GPSun();
            GPGregorianTime gt = new GPGregorianTime((GPLocation)null);
            gt.setJulianGreenwichTime(jdate);
            return sun.getLongitude(gt.getYear(), gt.getMonth(), gt.getDay(), 
                gt.getHour(), gt.getMinute(), gt.getSecond());

            /*GPCelestialBodyCoordinates coord = GPAstroEngine.sun_coordinate(jdate);
            return coord.eclipticalLongitude;*/
        }
    }
}
