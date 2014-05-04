using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPNaksatra: GPAstroIterator
    {
        public const int ASVINI_NAKSATRA = 0;
        public const int BHARANI_NAKSATRA = 1;
        public const int KRITTIKA_NAKSATRA = 2;
        public const int ROHINI_NAKSATRA = 3;
        public const int MRIGASIRA_NAKSATRA = 4;
        public const int ARDRA_NAKSATRA = 5;
        public const int PUNARVASU_NAKSATRA = 6;
        public const int PUSYAMI_NAKSATRA = 7;
        public const int ASLESA_NAKSATRA = 8;
        public const int MAGHA_NAKSATRA = 9;
        public const int PURVA_PHALGUNI_NAKSATRA = 10;
        public const int UTTARA_PHALGUNI_NAKSATRA = 11;
        public const int HASTA_NAKSATRA = 12;
        public const int CITRA_NAKSATRA = 13;
        public const int SWATI_NAKSATRA = 14;
        public const int VISAKHA_NAKSATRA = 15;
        public const int ANURADHA_NAKSATRA = 16;
        public const int JYESTHA_NAKSATRA = 17;
        public const int MULA_NAKSATRA = 18;
        public const int PURVA_ASADHA_NAKSATRA = 19;
        public const int UTTARA_ASADHA_NAKSATRA = 20;
        public const int SRAVANA_NAKSATRA = 21;
        public const int DHANISTA_NAKSATRA = 22;
        public const int SATABHISA_NAKSATRA = 23;
        public const int PURVA_BHADRA_NAKSATRA = 24;
        public const int UTTARA_BHADRA_NAKSATRA = 25;
        public const int REVATI_NAKSATRA = 26;

        public static string getName(int i)
        {
            return GPStrings.getSharedStrings().getString(i + 630);
        }

        public int getCurrentNaksatra()
        {
            return getCurrentPosition();
        }

        public override double getUnitAverageLength()
        {
            return 0.9;
        }

        public override int getUnitCount()
        {
            return 27;
        }

        public override double calculatePosition()
        {
            pMoon.MoonCalc(pJulianDate);
            //pSun.calculateCoordinates(pStartDate, pStartDate.getLocation(), pStartDate.getDayHours());
            //	SunPosition(d, ed, sun, d.shour - 0.5 + d.tzone/24.0);
            double l1 = GPMath.putIn360(pMoon.longitude_deg - GPAyanamsa.GetAyanamsa(pJulianDate));
            return 3 * l1 / 40.0;
        }

        /*********************************************************************/
        /*                                                                   */
        /*   finds next time when starts next naksatra                       */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of naksatra 0..26                           */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetNextNaksatra(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 40.0 / 3.0;
            double l1, l2;
            double jday = startDate.getJulianGreenwichTime();
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            double ayanamsa = GPAyanamsa.GetAyanamsa(jday);
            double scan_step = 0.5;
            int prev_naks = 0;
            int new_naks = -1;

            double xj;
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocationProvider());

            moon.MoonCalc(jday);
            l1 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
            prev_naks = Convert.ToInt32(Math.Floor(l1 / phi));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday += scan_step;
                d.setDayHours( d.getDayHours() + scan_step);
                if (d.getDayHours() > 1.0)
                {
                    d.setDayHours(  d.getDayHours() - 1.0);
                    d.NextDay();
                }

                moon.MoonCalc(jday);
                l2 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
                new_naks = Convert.ToInt32(Math.Floor(l2 / phi));
                if (prev_naks != new_naks)
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
            return new_naks;
        }

        /*********************************************************************/
        /*                                                                   */
        /*   finds previous time when starts next naksatra                   */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of naksatra 0..26                           */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetPrevNaksatra(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 40.0 / 3.0;
            double l1, l2;
            double jday = startDate.getJulianGreenwichTime();
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            double ayanamsa = GPAyanamsa.GetAyanamsa(jday);
            double scan_step = 0.5;
            int prev_naks = 0;
            int new_naks = -1;

            double xj;
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocationProvider());

            moon.MoonCalc(jday);
            l1 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
            prev_naks = Convert.ToInt32(Math.Floor(l1 / phi));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday -= scan_step;
                d.setDayHours( d.getDayHours() - scan_step);
                if (d.getDayHours() < 0.0)
                {
                    d.setDayHours( d.getDayHours() + 1.0);
                    d.PreviousDay();
                }

                moon.MoonCalc(jday);
                l2 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
                new_naks = Convert.ToInt32(Math.Floor(l2 / phi));

                if (prev_naks != new_naks)
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
            return new_naks;

        }

        public static int PREV_NAKSATRA(int n)
        {
            return (n + 27) % 28;
        }
    
        public static int NEXT_NAKSATRA(int n)
        {
            return (n + 1) % 28;
        }

    }
}
