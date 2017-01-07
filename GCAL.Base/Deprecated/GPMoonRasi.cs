using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMoonRasi
    {

        public static int GetNextRasi(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double jday = startDate.getJulianGreenwichTime();
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            double ayanamsa = GPAyanamsa.GetAyanamsa(jday);
            double scan_step = 0.5;
            int prev_naks = 0;
            int new_naks = -1;

            double xj;
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocation());

            moon.MoonCalc(jday);
            //l1 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
            prev_naks = GPEngine.GetRasi(moon.longitude_deg, ayanamsa);

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
                //l2 = GPMath.putIn360(moon.longitude_deg - ayanamsa);
                new_naks = GPEngine.GetRasi(moon.longitude_deg, ayanamsa);
                if (prev_naks != new_naks)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
            }
            nextDate = d;
            return new_naks;
        }


        public static int GetPrevRasi(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
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
            prev_naks = GPEngine.GetRasi(moon.longitude_deg, ayanamsa);

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
                new_naks = GPEngine.GetRasi(moon.longitude_deg, ayanamsa);

                if (prev_naks != new_naks)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
            }

            nextDate = d;
            return new_naks;
        }

    }
}
