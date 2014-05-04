using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPAstroIterator
    {
        protected GPGregorianTime pStartDate = null;
        protected double pJulianDate = 0;
        protected int currentPosition = -1;
        protected GPMoon pMoon = new GPMoon();
        protected GPSunData pSun = new GPSunData();

        public void setStartDate(GPGregorianTime st)
        {
            pStartDate = new GPGregorianTime(st);
            pJulianDate = pStartDate.getJulianGreenwichTime();
            pJulianDate = GPAstroEngine.ConvertUniversalToDynamic(pJulianDate);
        }


        public int getCurrentPosition()
        {
            if (currentPosition < 0)
            {
                currentPosition = Convert.ToInt32(Math.Floor(calculatePosition()));
            }

            return currentPosition;
        }

        public virtual double calculatePosition()
        {
            return 0;
        }

        public virtual double getUnitAverageLength()
        {
            return 1;
        }

        public virtual int getUnitCount()
        {
            return 1;
        }

        public GPGregorianTime getNext()
        {
            return movePosition(1);
        }

        public GPGregorianTime getPrev()
        {
            return movePosition(-1);
        }

        public GPGregorianTime movePosition(int dir)
        {
            double unitLength = getUnitAverageLength();
            int unitCount = getUnitCount();

            double lowerLimit = 0.5;
            double upperLimit = unitCount - 0.5;
            double position = 0;
            bool over = false;
            bool under = false;
            double nextTithi = getCurrentPosition() + dir;
            double diff;
            double seconds = 0.5 / 86400.0;
            double prevSeconds = -1;
            if (dir > 0)
            {
                over = (nextTithi > upperLimit);
                under = (currentPosition < lowerLimit);
            }
            else
            {
                over = (currentPosition > upperLimit);
                under = (nextTithi < lowerLimit);
            }
            for (int i = 0; i < 10; i++)
            {
                position = calculatePosition();
                //seconds = Convert.ToInt32((pJulianDate - Math.Floor(pJulianDate)) * 86400);
                if (Math.Abs(pJulianDate - prevSeconds) < seconds)
                    break;
                if (over && position < lowerLimit)
                    position += unitCount;
                else if (under && position > upperLimit)
                    position -= unitCount;

                prevSeconds = pJulianDate;

                diff = (nextTithi - position) * unitLength;
                pJulianDate += diff;
                pStartDate.addDayHours(diff);
            }

            currentPosition = Convert.ToInt32(nextTithi) % unitCount;

            GPGregorianTime nt = new GPGregorianTime(pStartDate.getLocation());
            nt.setJulianGreenwichTime(new GPJulianTime(GPAstroEngine.ConvertDynamicToUniversal(pJulianDate), 0.0));
            return nt;
        }

    }
}
