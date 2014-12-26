using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPYoga
    {
        public static string getName(int i)
        {
            return GPStrings.getString(660 + i);
        }

        public static int PREV_YOGA(int a)
        {
            return (((a) + 26) % 27);
        }
        public static int NEXT_YOGA(int a)
        {
            return (((a) + 1) % 27);
        }

        /*********************************************************************/
        /*                                                                   */
        /*   finds next time when starts next yoga                           */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of yoga 0..26                               */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetNextStart(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 40 / 3.0;
            double l1, l2, sunl;
            double jday = startDate.getJulianGreenwichTime();
            double xj;
            double ayanamsa = GPAyanamsa.GetAyanamsa(jday);
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocationProvider());
            double scan_step = 0.5;
            int prev_tit = 0;
            int new_tit = -1;

            moon.MoonCalc(jday);
            sunl = GPSun.GetSunLongitude(jday);
            l1 = GPMath.putIn360(moon.longitude_deg + sunl - 2*ayanamsa);
            prev_tit = Convert.ToInt32(Math.Floor(l1 * 3 / 40.0));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday += scan_step;
                d.setDayHours(d.getDayHours() + scan_step);
                if (d.getDayHours() > 1.0)
                {
                    d.setDayHours(d.getDayHours() - 1.0);
                    d.NextDay();
                }

                moon.MoonCalc(jday);
                //SunPosition(d, ed, sun, d.shour - 0.5 + d.tzone/24.0);
                //l2 = put_in_360(moon.longitude_deg - sun.longitude_deg - 180.0);
                sunl = GPSun.GetSunLongitude(jday);
                l2 = GPMath.putIn360(moon.longitude_deg + sunl - 2*ayanamsa);
                //Debugger.Log(0, "", "Current position: " + l2/12.0 + "   date: " + jday + "\n");
                new_tit = Convert.ToInt32(Math.Floor(l2 / phi));

                if (prev_tit != new_tit)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
                else
                {
                    l1 = l2;
                }
            }
            nextDate = d;
            //	nextDate.shour += startDate.tzone / 24.0;
            //	nextDate.NormalizeValues();
            return new_tit;
        }

        /*********************************************************************/
        /*                                                                   */
        /*   finds previous time when starts next yoga                       */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of yoga 0..26                               */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetPrevStart(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 12.0;
            double l1, l2, sunl;
            double jday = startDate.getJulianGreenwichTime();
            double xj;
            double ayanamsa = GPAyanamsa.GetAyanamsa(jday);
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocationProvider());
            double scan_step = 0.5;
            int prev_tit = 0;
            int new_tit = -1;

            moon.MoonCalc(jday);
            sunl = GPSun.GetSunLongitude(jday);
            l1 = GPMath.putIn360(moon.longitude_deg + sunl - 2*ayanamsa);
            prev_tit = Convert.ToInt32(Math.Floor(l1 / phi));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday -= scan_step;
                d.setDayHours(d.getDayHours() - scan_step);
                if (d.getDayHours() < 0.0)
                {
                    d.setDayHours(d.getDayHours() + 1.0);
                    d.PreviousDay();
                }

                moon.MoonCalc(jday);
                sunl = GPSun.GetSunLongitude(jday);
                l2 = GPMath.putIn360(moon.longitude_deg + sunl - 2*ayanamsa);
                new_tit = Convert.ToInt32(Math.Floor(l2 / phi));

                if (prev_tit != new_tit)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
                else
                {
                    l1 = l2;
                }
            }
            nextDate = d;
            //	nextDate.shour += startDate.tzone / 24.0;
            //	nextDate.NormalizeValues();
            return new_tit;
        }



    }
}
