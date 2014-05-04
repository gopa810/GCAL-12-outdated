using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPLocation : GPObserver
    {
        private string city = string.Empty;
        //private double latitude = 0.0;
        //private double longitude = 0.0;
        private string countryCode = string.Empty;
        private string timezoneName = "GMT";
        private GPTimeZone timezone = null;


        public GPLocation()
        {
        }

        public GPLocation(GPLocation loc)
        {
            city = loc.city;
            setLatitudeNorthPositive(loc.GetLatitudeNorthPositive());
            setLongitudeEastPositive(loc.GetLongitudeEastPositive());
            countryCode = loc.countryCode;
            timezoneName = loc.timezoneName;
            timezone = loc.timezone;
        }

        public static GPLocation getEmptyLocation()
        {
            return new GPLocation();
        }

        public bool Equals(GPLocation loc)
        {
            return (loc.getCity().Equals(getCity())
                && Math.Abs(loc.GetLatitudeNorthPositive() - GetLatitudeNorthPositive()) < 0.01
                && Math.Abs(loc.GetLongitudeEastPositive() - GetLongitudeEastPositive()) < 0.01) ;
        }

        public string getCity()
        {
            return city;
        }

        public void setCity(string value)
        {
            city = value;
        }

        public string getCountryCode()
        {
            return countryCode;
        }

        public void setCountryCode(string value)
        {
            countryCode = value;
        }

        public string getCountryName()
        {
            GPCountry ctr = GPCountryList.getShared().GetCountryByCode(countryCode);
            if (ctr != null)
                return ctr.getName();
            return string.Empty;
        }

        public string getName()
        {
            return string.Format("{0} ({1})", getCity(), getCountryName());
        }

        public string getFullName()
        {
            return string.Format("{0} {1}, {2} {3}", getCity(), getCountryName(), getLatitudeString(), getLongitudeString());
        }

        public string getTimeZoneName()
        {
            return timezoneName;
        }

        public void setTimeZoneName(string value)
        {
            timezoneName = value;
            timezone = GPTimeZoneList.sharedTimeZones().GetTimezoneByName(timezoneName);
        }

        public GPTimeZone getTimeZone()
        {
            return timezone;
        }

        public void setTimeZone(GPTimeZone value)
        {
            timezone = value;
            if (timezone != null) timezoneName = timezone.Name;
        }

        public string getTimeZoneString()
        {
            return string.Format("{0} {1}", GPAppHelper.GetTextTimeZone(getTimeZone().OffsetSeconds), getTimeZoneName());
        }

        public string getLatitudeString()
        {
            double abslat = Math.Abs(GetLatitudeNorthPositive());
            return string.Format("{0:00}{1}{2:00}", Math.Floor(abslat), (Math.Sign(GetLatitudeNorthPositive()) > 0 ? 'N' : 'S'), Math.Floor(abslat * 60) % 60);
        }

        public void setLatitudeString(string value)
        {
            double l;
            ConvertStringToCoordinate(value, out l);
            setLatitudeNorthPositive(l);
        }

        /// <summary>
        /// String representation of longitude
        /// e.g. 10E20, 18W55
        /// </summary>
        public string getLongitudeString()
        {
            double abslat = Math.Abs(GetLongitudeEastPositive());
            return string.Format("{0:00}{1}{2:00}", Math.Floor(abslat), (Math.Sign(GetLongitudeEastPositive()) > 0 ? 'E' : 'W'), Math.Floor(abslat * 60) % 60);
        }

        public void setLongitudeString(string value)
        {
            double l;
            ConvertStringToCoordinate(value, out l);
            setLongitudeEastPositive(l);
        }

        /// <summary>
        /// Converts string with geographical coordinate to double number
        /// </summary>
        /// <param name="s">string with value</param>
        /// <param name="d">converted value</param>
        /// <returns>Returns true, if conversion was successful, false if conversion failed.</returns>
        public static bool ConvertStringToCoordinate(string s, out double d)
        {
            if (double.TryParse(s, out d))
                return true;

            string a = "NESW";
            for (int i = 0; i < 4; i++)
            {
                if (s.IndexOf(a[i]) > 0)
                {
                    return ConvertStringDegToCoordinate(s, a[i], out d, ((i < 2) ? 1 : -1));
                }
            }

            return false;
        }

        /// <summary>
        /// Helper function for ConevrtStringToCoordinate
        /// </summary>
        /// <param name="s">string with value</param>
        /// <param name="div">separator character</param>
        /// <param name="d">output: converted value</param>
        /// <param name="mult">multiplier, if separator character is dedicated to 
        /// negative values, this parameter is -1, otherwise it is 1</param>
        /// <returns>Returns true on success, false if not success</returns>
        protected static bool ConvertStringDegToCoordinate(string s, char div, out double d, int mult)
        {
            int index = s.IndexOf(div);
            if (index >= 0)
            {
                int a, b;
                if (int.TryParse(s.Substring(0, index), out a) && int.TryParse(s.Substring(index + 1), out b))
                {
                    while (b > 100)
                        b = b / 10;
                    while (b < 10)
                        b = b * 10;
                    if (b >= 60)
                    {
                        d = 0.0;
                        return false;
                    }
                    d = Convert.ToDouble(mult * a) + Convert.ToDouble(b) / 60.0;
                    return true;
                }
            }
            d = 0.0;
            return false;
        }

        public double getTimeZoneOffsetHours()
        {
            if (timezone == null)
                timezone = GPTimeZoneList.sharedTimeZones().GetTimezoneByName(timezoneName);
            if (timezone != null)
                return Convert.ToDouble(timezone.OffsetSeconds) / 3600.0;
            return 0.0;
        }
    }
}
