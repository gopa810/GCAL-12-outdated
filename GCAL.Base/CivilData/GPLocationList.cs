using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace GCAL.Base
{
    public class GPLocationList : GPObjectListBase
    {
        public List<GPLocation> locations = new List<GPLocation>();

        public GPLocationList()
        {
            InitializeFromResources();
        }

        public override string GetDefaultResourceName()
        {
            return GCAL.Base.Properties.Resources.Locations;
        }

        public override string GetDefaultFileName()
        {
            return "Locations.txt";
        }

        public override void InsertNewObjectFromStrings(string[] parts)
        {
            if (parts.Length >= 5 && parts[0].Length > 0)
            {
                GPLocation location = new GPLocation();
                location.setCity(parts[0]);
                location.setLatitudeNorthPositive(double.Parse(parts[1]));
                location.setLongitudeEastPositive(double.Parse(parts[2]));
                location.setCountryCode(parts[3]);
                location.setTimeZoneName(parts[4]);
                locations.Add(location);
            }
        }

        public override void SaveData(StreamWriter writer)
        {
            foreach (GPLocation loc in locations)
            {
                writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", loc.getCity(), loc.GetLatitudeNorthPositive(),
                    loc.GetLongitudeEastPositive(), loc.getCountryCode(), loc.getTimeZoneName());
            }
        }

        private static GPLocationList _sharedList = null;
        public static GPLocationList getShared()
        {

            if (_sharedList == null)
            {
                _sharedList = new GPLocationList();
            }
            return _sharedList;
        }

        public int GetLocationCountForCountryCode(string code)
        {
            int count = 0;
            foreach (GPLocation location in locations)
            {
                if (location.getCountryCode() == code)
                    count++;
            }
            return count;
        }

        public int GetLocationCountForTimezone(string tzone)
        {
            int count = 0;
            foreach (GPLocation location in locations)
            {
                if (location.getTimeZoneName() == tzone)
                    count++;
            }
            return count;
        }

        public void ChangeCountryCode(string oldCode, string newCode)
        {
            foreach (GPLocation location in locations)
            {
                if (location.getCountryCode().Equals(oldCode))
                {
                    location.setCountryCode(newCode);
                    Modified = true;
                }
            }
        }

        public GPLocation findLocation(string p)
        {
            foreach (GPLocation location in locations)
            {
                if (location.getCity().Equals(p))
                {
                    return location;
                }
            }
            return null;
        }
    }
}
