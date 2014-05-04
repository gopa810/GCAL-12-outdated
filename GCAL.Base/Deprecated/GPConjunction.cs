using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPConjunction
    {
        protected GPGregorianTime pStartDate = null;
        protected double pJulianDate = 0;
        protected GPMoon pMoon = new GPMoon();
        protected GPSunData pSun = new GPSunData();
        protected double pLastSunLongitude = 0;
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
            pLastSunLongitude = GPSun.GetSunLongitude(pJulianDate);
            double l1 = GPMath.putIn360(pMoon.longitude_deg - pLastSunLongitude - opositeLongitude);
            return GPMath.putIn180(l1);
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
            double diff;
            double seconds = 0.5 / 86400.0;
            double prevSeconds = -1;

            for (int i = 0; i < 10; i++)
            {
                position = calculatePosition() / 360;
                if (Math.Abs(pJulianDate - prevSeconds) < seconds)
                    break;

                prevSeconds = pJulianDate;

                diff = position * 30;
                pJulianDate -= diff;
                pStartDate.addDayHours(diff);
            }

            GPGregorianTime nt = new GPGregorianTime(pStartDate.getLocation());
            nt.setJulianGreenwichTime(new GPJulianTime(GPAstroEngine.ConvertDynamicToUniversal(pJulianDate), 0.0));
            return nt;
        }

    }
}
