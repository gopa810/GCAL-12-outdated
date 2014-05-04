using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPObserver
    {
        private double longitude = 0;
        private double latitude = 0;
        private double altitude = 0;

        public double GetAltitude()
        {
            return altitude;
        }

        public GPObserver SetAltitude(double A)
        {
            altitude = A;
            return this;
        }

        public GPObserver SetLatitudeSouthPositive(double L)
        {
            latitude = -L;
            return this;
        }

        public GPObserver setLatitudeNorthPositive(double L)
        {
            latitude = L;
            return this;
        }

        public double GetLatitudeNorthPositive()
        {
            return latitude;
        }

        public double GetLatitudeSouthPositive()
        {
            return -latitude;
        }

        public GPObserver SetLongitudeWestPositive(double L)
        {
            longitude = -L;
            return this;
        }

        public GPObserver setLongitudeEastPositive(double L)
        {
            longitude = L;
            return this;
        }

        public double GetLongitudeEastPositive()
        {
            return longitude;
        }

        public double GetLongitudeWestPositive()
        {
            return -longitude;
        }


    }
}
