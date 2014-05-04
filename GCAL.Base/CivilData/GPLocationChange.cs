using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPLocationChange
    {
        public GPLocation LocationA { get; set; }
        public GPLocation LocationB { get; set; }

        public bool TimezoneStart { get; set; }

        public double julianStart { get; set; }
        public double julianEnd { get; set; }

        public String humanStart 
        {
            get
            {
                GPGregorianTime gt = new GPGregorianTime(LocationA);
                gt.setJulianGreenwichTime(new GPJulianTime(julianStart, 0));
                return string.Format("{0} {1} {2}", gt.getShortDateString(), gt.getLongTimeString(), LocationA.getTimeZoneName());
            }
        }
        public String humanEnd
        {
            get
            {
                GPGregorianTime gt = new GPGregorianTime(LocationB);
                gt.setJulianGreenwichTime(new GPJulianTime(julianEnd, 0));
                return string.Format("{0} {1} {2}", gt.getShortDateString(), gt.getLongTimeString(), LocationB.getTimeZoneName());
            }
        }



        public String humanLength
        {
            get
            {
                int len = Convert.ToInt32((julianEnd - julianStart)*1440);
                if (len < 0)
                    len = 30;
                int hr = len / 60;
                len -= hr * 60;
                return String.Format("{0} hr {1:00} min", hr, len);
            }
        }

        private GPLocation temp = new GPLocation();

        public GPLocation getLocation(double jdate)
        {
            if (jdate < julianStart)
                return temp;
            if (jdate > julianEnd)
                return temp;

            double lonA, lonB;
            double latA, latB;

            lonA = LocationA.GetLongitudeEastPositive();
            lonB = LocationB.GetLongitudeEastPositive();
            latA = LocationA.GetLatitudeNorthPositive();
            latB = LocationB.GetLatitudeNorthPositive();

            temp.setTimeZone(TimezoneStart ? LocationA.getTimeZone() : LocationB.getTimeZone());
            temp.setLongitudeEastPositive(lonA + (lonB - lonA) * (jdate - julianStart) / (julianEnd - julianStart));
            temp.setLatitudeNorthPositive(latA + (latB - latA) * (jdate - julianStart) / (julianEnd - julianStart));
            temp.SetAltitude(0);

            return temp;
        }
    }
}
