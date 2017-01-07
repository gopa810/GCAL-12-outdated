using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class GPAstroData
    {
        private GPLocationListRecent p_location = null;

        private GPGregorianTime p_date = null;

        // date of Julian epoch
        public double jdate;
        // sun
        public GPSun sun = new GPSun();
        // moon
        public GPMoon moon = new GPMoon();
        // year of Gaurabda epoch
        public int nGaurabdaYear;
        // value of ayanamsa for this date
        public double msAyanamsa;
        // sign of zodiac
        public int nSunRasi;
        // rasi of the moon
        public int nMoonRasi;
        // tithi #
        public int nTithi;
        // tithi elaps.
        public double nTithiElapse;
        // paksa
        public int nPaksa;
        // yoga
        public int nYoga;
        // yoga elaps.
        public double nYogaElapse;
        // naksatra
        public int nNaksatra;
        // naksatra elaps.
        public double nNaksatraElapse;
        // masa
        public int nMasa;
        // distance of moon and sun in degrees
        public double msDistance;

        public GPAstroData()
        {
        }

        public GPAstroData(GPAstroData dt)
        {
            Copy(dt);
        }

        public void setLocation(GPLocationListRecent value)
        {
            p_location = value;
        }

        public GPLocationListRecent getLocation()
        {
            return p_location;
        }

        public GPGregorianTime getDate()
        {
            return p_date;
        }

        public void setDate(GPGregorianTime value)
        {
            p_date = new GPGregorianTime(value);
        }
        public void Copy(GPAstroData dt)
        {
            this.setLocation(dt.getLocation());
            this.setDate(dt.getDate());
            this.jdate = dt.jdate;
            this.moon = new GPMoon(dt.moon);
            this.msAyanamsa = dt.msAyanamsa;
            this.msDistance = dt.msDistance;
            this.nGaurabdaYear = dt.nGaurabdaYear;
            this.nMasa = dt.nMasa;
            this.nMoonRasi = dt.nMoonRasi;
            this.nNaksatra = dt.nNaksatra;
            this.nNaksatraElapse = dt.nNaksatraElapse;
            this.nPaksa = dt.nPaksa;
            this.nSunRasi = dt.nSunRasi;
            this.nTithi = dt.nTithi;
            this.nTithiElapse = dt.nTithiElapse;
            this.nYoga = dt.nYoga;
            this.nYogaElapse = dt.nYogaElapse;
            this.sun = new GPSun(dt.sun);
        }

        /*********************************************************************/
        /*                                                                   */
        /* Calculation of tithi, paksa, naksatra, yoga for given             */
        /*    Gregorian date                                                 */
        /*                                                                   */
        /*                                                                   */
        /*********************************************************************/

        public int calculateDayData(GPGregorianTime aDate, GPLocation earth)
        {
            double d;
            GPAstroData day = this;
            GPGregorianTime date = new GPGregorianTime(aDate);
            //	SUNDATA sun;

            // sun position on sunrise on that day
            sun.SunCalc(date, earth);
            date.setDayHours(sun.getSunriseDayHours());

            // date.shour is [0..1] time of sunrise in local timezone time
            jdate = date.getJulianGreenwichTime();

            // moon position at sunrise on that day
            day.moon.MoonCalc(jdate);

            // correct_parallax(day.moon, jdate, earth.latitude_deg, earth.longitude_deg);

            day.msDistance = GPMath.putIn360(day.moon.longitude_deg - day.sun.rise.eclipticalLongitude - 180.0);
            day.msAyanamsa = GPAyanamsa.GetAyanamsa(jdate);

            // tithi
            d = day.msDistance / 12.0;
            day.nTithi = Convert.ToInt32(Math.Floor(d));
            day.nTithiElapse = GPMath.frac(d) * 100.0;
            day.nPaksa = (day.nTithi >= 15) ? 1 : 0;


            // naksatra
            d = GPMath.putIn360(day.moon.longitude_deg - day.msAyanamsa);
            d = (d * 3.0) / 40.0;
            day.nNaksatra = Convert.ToInt32(Math.Floor(d) + 0.1);
            day.nNaksatraElapse = GPMath.frac(d) * 100.0;

            // yoga
            d = GPMath.putIn360(day.moon.longitude_deg + day.sun.rise.eclipticalLongitude - 2 * day.msAyanamsa);
            d = (d * 3.0) / 40.0;
            day.nYoga = Convert.ToInt32(Math.Floor(d));
            day.nYogaElapse = GPMath.frac(d) * 100.0;

            // masa
            day.nMasa = -1;

            // rasi
            day.nSunRasi = GPEngine.GetRasi(day.sun.rise.eclipticalLongitude, day.msAyanamsa);
            day.nMoonRasi = GPEngine.GetRasi(day.moon.longitude_deg, day.msAyanamsa);

            setDate(date);

            return 1;
        }

        private int p_tithi_arunodaya = -1;
        private int p_tithi_sunset = -1;

        // tithi at arunodaya
        public int getTithiAtArunodaya()
        {
            if (p_tithi_arunodaya < 0)
            {
                GPCelestialBodyCoordinates moonCoord = GPAstroEngine.moon_coordinate(sun.arunodaya.getJulianGreenwichTime());
                double d = GPMath.putIn360(moonCoord.eclipticalLongitude - sun.arunodaya.eclipticalLongitude - 180) / 12.0;
                p_tithi_arunodaya = Convert.ToInt32(Math.Floor(d));
            }
            return p_tithi_arunodaya;
        }

        // tithi at sunset
        public int getTithiAtSunset()
        {
            if (p_tithi_sunset < 0)
            {
                GPCelestialBodyCoordinates moonCoord = GPAstroEngine.moon_coordinate(sun.set.getJulianGreenwichTime());
                double d = GPMath.putIn360(moonCoord.eclipticalLongitude - sun.set.eclipticalLongitude - 180) / 12.0;
                p_tithi_sunset = Convert.ToInt32(Math.Floor(d));
            }
            return p_tithi_sunset;
        }

        /*********************************************************************/
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*********************************************************************/

        public int determineMasa(GPGregorianTime aDate, out int nGaurabdaYear)
        {
            GPAstroData day = this;
            GPGregorianTime date = new GPGregorianTime(aDate);
            const int PREV_MONTHS = 6;
            GPLocation earth = aDate.getLocation();

            double[] L = new double[8];
            GPGregorianTime[] C = new GPGregorianTime[8];
            int[] R = new int[8];
            int n, rasi;
            int masa = 0;
            int ksaya_from = -1;
            int ksaya_to = -1;

            date.setDayHours(day.sun.getSunriseDayHours());

            // STEP 1: calculate position of the sun and moon
            // it is done by previous call of DayCalc
            // and results are in argument DAYDATA day
            // *DayCalc(date, earth, day, moon, sun);*


            L[1] = /*Long[0] =*/ GPConjunction.GetNextConjunction(date, out C[1], false, earth);
            L[0] = /*LongA   =*/ GPConjunction.GetNextConjunction(C[1], out C[0], true, earth);

            // on Pratipat (nTithi == 15) we need to look for previous conjunction
            // but this conjunction can occur on this date before sunrise
            // so we need to include this very date into looking for conjunction
            // on other days we cannot include it
            // and exclude it from looking for next because otherwise that will cause
            // incorrect detection of Purusottama adhika masa
            L[2] = GPConjunction.GetPrevConjunction(date, out C[2], false, earth);

            for (n = 3; n < PREV_MONTHS; n++)
                L[n] = GPConjunction.GetPrevConjunction(C[n - 1], out C[n], true, earth);

            for (n = 0; n < PREV_MONTHS; n++)
            {
                R[n] = GPEngine.GetRasi(L[n], GPAyanamsa.GetAyanamsa(C[n].getJulianLocalNoon()));
                //if (nr != R[n])
                //    Debugger.Log(0, "", String.Format("Different rasi {0} <=> {1}  for input date: {2}", nr, R[n], date));
            }

            /*	TRACE("TEST Date: %d %d %d\n", date.day, date.month, date.year);
                TRACE("FOUND CONJ Date: %d %d %d rasi: %d\n", C[1].day, C[1].month, C[1].year, R[1]);
                TRACE("FOUND CONJ Date: %d %d %d rasi: %d\n", C[2].day, C[2].month, C[2].year, R[2]);
                TRACE("FOUND CONJ Date: %d %d %d rasi: %d\n", C[3].day, C[3].month, C[3].year, R[3]);
                TRACE("FOUND CONJ Date: %d %d %d rasi: %d\n", C[4].day, C[4].month, C[4].year, R[4]);
                TRACE("---\n");
            */
            // test for Adhika-Ksaya sequence
            // this is like 1-2-2-4-5...
            // second (2) is replaced by rasi(3)
            /*	if ( ((Sank[1] + 2) % 12 == SankA) && ((Sank[1] == Sank[0]) || (Sank[0] == SankA)))
                {
                    Sank[0] = (Sank[1] + 1) % 12;
                }
	
                if ( ((Sank[2] + 2) % 12 == Sank[0]) && ((Sank[2] == Sank[1]) || (Sank[1] == Sank[0])))
                {
                    Sank[1] = (Sank[2] + 1) % 12;
                }*/

            // look for ksaya month
            ksaya_from = -1;
            for (n = PREV_MONTHS - 2; n >= 0; n--)
            {
                if ((R[n + 1] + 2) % 12 == R[n])
                {
                    ksaya_from = n;
                    break;
                }
            }

            if (ksaya_from >= 0)
            {
                for (n = ksaya_from; n > 0; n--)
                {
                    if (R[n] == R[n - 1])
                    {
                        ksaya_to = n;
                        break;
                    }
                }

                if (ksaya_to >= 0)
                {
                    // adhika masa found
                    // now correct succession of rasis
                }
                else
                {
                    // adhika masa not found
                    // there will be some break in masa queue
                    ksaya_to = 0;
                }

                int current_rasi = R[ksaya_from + 1] + 1;
                for (n = ksaya_from; n >= ksaya_to; n--)
                {
                    R[n] = current_rasi;
                    current_rasi = (current_rasi + 1) % 12;
                }
            }

            // STEP 3: test for adhika masa
            // test for adhika masa
            if (R[1] == R[2])
            {
                // it is adhika masa
                masa = 12;
                rasi = R[1];
            }
            else
            {
                // STEP 2. select nearest Conjunction
                if (day.nPaksa == 0)
                {
                    masa = R[1];
                }
                else if (day.nPaksa == 1)
                {
                    masa = R[2];
                }
                rasi = masa;
            }

            // calculation of Gaurabda year
            nGaurabdaYear = date.getYear() - 1486;

            if ((rasi > 7) && (rasi < 11)) // Visnu
            {
                if (date.getMonth() < 6)
                    nGaurabdaYear--;
            }

            Debugger.Log(0, "", String.Format("---- MASA CALC --------------------{0} {1} -\n", date.getLongDateString(), date.getLongTimeString()));
            Debugger.Log(0, "", " No   Rasi   Long     Date\n");
            for (int i = 0; i < PREV_MONTHS; i++)
            {
                Debugger.Log(0, "", String.Format(" {0:#0}   {1:#0}  {2:000.000}    {3}  {4}\n", i, R[i], L[i], C[i].getLongDateString(), C[i].getLongTimeString()));
            }
            Debugger.Log(0, "", "\n\n");

            return masa;

        }

        public static int calculateNaksatraAtMidnight(GPGregorianTime date, GPLocation earth)
        {
            double d;
            double jdate;
            GPMoon moon = new GPMoon();

            jdate = date.getJulianGreenwichNoon() + 0.5;
            moon.MoonCalc(jdate);
            d = GPMath.putIn360(moon.longitude_deg - GPAyanamsa.GetAyanamsa(jdate));
            return Convert.ToInt32(Math.Floor((d * 3.0) / 40.0));
        }

    }
}
