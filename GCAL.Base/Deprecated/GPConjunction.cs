using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class GPConjunction
    {
        protected GPGregorianTime pStartDate = null;
        protected double pJulianDate = 0;
        protected GPMoon pMoon = new GPMoon();
        protected GPSunMethodSimple pSun = new GPSunMethodSimple();
        protected double pLastSunLongitude = 0;
        protected double pLastMoonLongitude = 0;
        protected bool pTrackStarted = false;
        protected double opositeLongitude = 0.0;

        public void setStartDate(GPGregorianTime st)
        {
            pStartDate = new GPGregorianTime(st);
            pJulianDate = pStartDate.getJulianGreenwichTime();
            pJulianDate = GPAstroEngine.ConvertUniversalToDynamic(pJulianDate);
        }

        public void setOpositeConjunction(bool opo)
        {
            opositeLongitude = opo ? 180.0 : 0.0;
        }

        protected double calculatePosition()
        {
            pMoon.MoonCalc(pJulianDate);
            pLastMoonLongitude = pMoon.longitude_deg;
            pLastSunLongitude = pSun.getLongitude(pStartDate.getYear(), pStartDate.getMonth(),
                pStartDate.getDay(), pStartDate.getHour(), pStartDate.getMinute(), pStartDate.getSecond());
            return GPMath.putIn180(pMoon.longitude_deg - pLastSunLongitude - opositeLongitude);
            //return GPMath.putIn180(l1);
        }

        public int getCurrentPosition()
        {
            return GPEngine.GetRasi(pLastSunLongitude, GPAyanamsa.GetAyanamsa(pJulianDate));
        }

        public GPGregorianTime getNext()
        {
            if (pTrackStarted)
            {
                pJulianDate += 30;
                return movePosition();
            }
            else
            {
                double orig = pJulianDate;
                pJulianDate -= 30;
                pTrackStarted = true;
                pStartDate = movePosition();
                while (pJulianDate < orig)
                {
                    pJulianDate += 30;
                    pStartDate = movePosition();
                }
                return new GPGregorianTime(pStartDate);
            }
        }

        public GPGregorianTime getPrev()
        {
            if (pTrackStarted)
            {
                pJulianDate -= 30;
                return movePosition();
            }
            else
            {
                double orig = pJulianDate;
                pStartDate = movePosition();
                pTrackStarted = true;
                if (orig < pJulianDate)
                    return getPrev();
                else
                    return pStartDate;
            }
        }

        /// <summary>
        /// this will move date to the nearest conjunction
        /// whether it is in future or in the past
        /// </summary>
        /// <returns></returns>
        protected GPGregorianTime movePosition()
        {
            double position = 0;
            double diff = 1.0;
            double seconds = 0.5 / 86400.0;

            for (int i = 0; i < 10 && Math.Abs(diff) > seconds; i++)
            {
                position = calculatePosition() / 360;
                diff = position * 30;
                //diff = position;
                pJulianDate -= diff;
                pStartDate.addDayHours(-diff);
            }

            /*GPGregorianTime nt = new GPGregorianTime(pStartDate.getLocation());
            nt.setJulianGreenwichTime(new GPJulianTime(GPAstroEngine.ConvertDynamicToUniversal(pJulianDate), 0.0));
            return nt;*/

            return pStartDate;
        }


        /*********************************************************************/
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*********************************************************************/

        public static double GetPrevConjunction(GPGregorianTime test_date, out GPGregorianTime found, bool this_conj, GPObserver earth)
        {
	        double sunl = 0;
            GPGregorianTime d = new GPGregorianTime(test_date);
            
            if (this_conj)
	        {
		        d.addDayHours(-0.2);
	        }

            GPLocation loc = d.getLocation();
            d.setLocation(GPLocation.Zero);

	
	        GPMoon moon = new GPMoon();
            GPSun sun = new GPSun();
	        double scan_step = 1.0;
	        int prev_tit = 0;
	        int new_tit = -1;

	        prev_tit = getCurrentRasi(moon, sun, d);

	        int counter = 0;
	        while(counter < 20)
	        {
		        d.addDayHours(-scan_step);

		        new_tit = getCurrentRasi(moon, sun, d);

		        if (prev_tit >= 0 && new_tit < 0)
		        {
			        d.addDayHours(scan_step);
			        scan_step *= 0.5;
			        counter++;
			        continue;
		        }
		        else
		        {
			        prev_tit = new_tit;
		        }

	        }
	        found = d;
            sunl = sun.getLongitude(d);
            found.setLocation(loc);
	        return sunl;

        }

        private static int getCurrentRasi(GPMoon moon, GPSun sun, GPGregorianTime d)
        {
            moon.MoonCalc(d.getJulianLocalTime());
            double sunl = sun.getLongitude(d);
            double l1 = GPMath.putIn180(moon.longitude_deg - sunl);
            /*Debugger.Log(0, "", "    FIND CONJ:  Moon " + moon.longitude_deg + " Sun:" + sunl + "  Date: " 
                + d.getLongDateString() + " " + d.getLongTimeString() + "  JD:" + d.getJulianLocalTime() + "\n");*/
            return Convert.ToInt32(Math.Floor(l1 / 6.0));
        }

        /*********************************************************************/
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*                                                                   */
        /*********************************************************************/

        public static double GetNextConjunction(GPGregorianTime test_date, out GPGregorianTime found, bool this_conj, GPObserver earth)
        {
	        double sunl = 0;

            GPGregorianTime d = new GPGregorianTime(test_date);
            
            if (this_conj)
	        {
		        d.addDayHours(+0.2);
	        }

            GPLocation loc = d.getLocation();
            d.setLocation(GPLocation.Zero);

	
	        GPMoon moon = new GPMoon();
            GPSun sun = new GPSun();
	        double scan_step = 1.0;
	        int prevRasi = 0;
	        int newRasi = -1;

            prevRasi = getCurrentRasi(moon, sun, d);

	        int counter = 0;
	        while(counter < 20)
	        {
		        d.addDayHours(scan_step);

                newRasi = getCurrentRasi(moon, sun, d);

		        if (prevRasi < 0 && newRasi >= 0)
		        {
                    d.addDayHours(-scan_step);
			        scan_step *= 0.5;
			        counter++;
			        continue;
		        }

		        prevRasi = newRasi;
	        }
            //Debugger.Log(0, "", " -- PRE DATE: " + d.getLongDateString() + " " + d.getLongTimeString() + " \n");
	        found = d;
            sunl = sun.getLongitude(d);
            found.setLocation(loc);
            //Debugger.Log(0, "", " -- RET DATE: " + d.getLongDateString() + " " + d.getLongTimeString() + " \n");
            return sunl;
    
        }

    }
}
