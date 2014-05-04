using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPAyanamsa
    {
        private static int currType = 1;

        public static int CurrentType
        {
            get
            {
                return currType;
            }
        }

        public static string CurrentName
        {
            get
            {
                return GetName(CurrentType);
            }
        }

        public static string GetName(int i)
        {
            return GPStrings.getSharedStrings().getString(940 + i);
        }

        /*********************************************************************/
        /*                                                                   */
        /* Value of Ayanamsha for given Julian day                           */
        /*                                                                   */
        /*********************************************************************/

        /*
        27.8.1900     22-27-54  2415259.000000 22,475
        23.7.1950     23-09-53  2433486.000000 23,16472
        3.9.2000      23-52-13  2451791.000000 23,870277778
        28.8.2010     24-00-04  2455437.000000 24,001111111
        21.6.2050     24-33-29  2469979.000000 24,558055556
        14.6.2100     25-15-29  2488234.000000 25,258055556*/




        public static double GetLahiriAyanamsa(double d)
        {
            double[] h = new double[] { 2415259.000000,22.475,
		            2433486.000000,23.16472,
		            2451791.000000,23.870277778,
		            2455437.000000,24.001111111,
		            2469979.000000,24.558055556,
		            2488234.000000,25.258055556 };

            if (d > h[10])
            {
                return (d - h[10]) * ((h[11] - h[9]) / (h[10] - h[8])) + h[11];
            }
            else if (d > h[8])
            {
                return (d - h[8]) * ((h[11] - h[9]) / (h[10] - h[8])) + h[9];
            }
            else if (d > h[6])
            {
                return (d - h[6]) * ((h[9] - h[7]) / (h[8] - h[6])) + h[7];
            }
            else if (d > h[4])
            {
                return (d - h[4]) * ((h[7] - h[5]) / (h[6] - h[4])) + h[5];
            }
            else if (d > h[2])
            {
                return (d - h[2]) * ((h[5] - h[3]) / (h[4] - h[2])) + h[3];
            }
            else if (d > h[0])
            {
                return (d - h[0]) * ((h[3] - h[1]) / (h[2] - h[0])) + h[1];
            }
            else
            {
                return (d - h[0]) * ((h[3] - h[1]) / (h[2] - h[0])) + h[1];
            }
        }

        //==================================================================
        //
        // precession of the equinoxes http://en.wikipedia.org/wiki/Precession_%28astronomy%29
        //
        //==================================================================

        public static double GetAyanamsa(double jdate)
        {
            double t, d;
            double a1 = 0.0;


            // progress of ayanamsa from 1950 to 2000
            //1.3971948971667
            t = (jdate - 2451545.0) / 36525.0;
            d = (5028.796195 - 1.1054348 * t) * t / 3600.0;
            switch (CurrentType)
            {
                case 0: // Fagan-Bradley
                    a1 = 24.8361111111 - 0.095268987143399 + d;
                    //-69.943382314 + jdate * 3.8263328316687189e-5;
                    break;
                case 1: // Lahiri
                    a1 = 23.85305555555 + d;
                    //a1 = GetLahiriAyanamsa(jdate);
                    // 23-51-14 in 2000
                    //TRACE("Ayan lahiri = %f\n", a1 - GetLahiriAyanamsa(jdate));
                    break;
                case 2: // Krishnamurti
                    a1 = 23.8561111111 - 0.095268987143399 + d;
                    break;
                case 3: // Raman
                    a1 = 22.5066666666 - 0.095268987143399 + d;
                    break;
                default:
                    break;
            }

            return a1;

        }
    }
}
