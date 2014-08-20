using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace GCAL.Base
{
    public class GPCountryList: GPObjectListBase
    {
        public List<GPCountry> countries = new List<GPCountry>();

        private static GPCountryList _sharedList = null;

        /// <summary>
        /// global shared object
        /// </summary>
        public static GPCountryList getShared()
        {
            if (_sharedList == null)
            {
                _sharedList = new GPCountryList();
            }
            return _sharedList;
        }

        public override GPObjectListBase.FileKey[] GetFileKeys()
        {
            return new FileKey[] { FileKey.Primary, FileKey.Secondary};
        }
        public override string GetDefaultResourceForKey(FileKey fk)
        {
            if (fk == FileKey.Primary)
                return GCAL.Base.Properties.Resources.Countries;
            if (fk == FileKey.Secondary)
                return GCAL.Base.Properties.Resources.CountryTimezones;
            return string.Empty;
        }

        public override string GetDefaultFileNameForKey(FileKey fk)
        {
            if (fk == FileKey.Primary)
                return "Countries.txt";
            if (fk == FileKey.Secondary)
                return "CountryTimezones.txt";
            return string.Empty;
        }

        public override void InsertNewObjectFromStrings(string[] parts, FileKey fk)
        {
            if (fk == FileKey.Primary)
            {
                if (parts.Length >= 2 && parts[0].Length > 0)
                {
                    GPCountry location = new GPCountry();
                    location.setCode(parts[0]);
                    location.setName(parts[1]);
                    countries.Add(location);
                }
            }
            else
            {
                if (parts.Length == 2)
                {
                    addTimezone(parts[0], parts[1]);
                }
            }
        }

        public override void SaveData(StreamWriter writer, FileKey fk)
        {
            if (fk == FileKey.Primary)
            {
                foreach (GPCountry country in countries)
                {
                    writer.WriteLine("{0}\t{1}", country.getCode(), country.getName());
                }
            }
            else
            {
                foreach (GPCountry country in countries)
                {
                    foreach (string timezoneName in country.Timezones)
                    {
                        writer.WriteLine("{0}\t{1}", country.getCode(), timezoneName);
                    }
                }
            }
        }

        public GPCountry GetCountryByCode(string str)
        {
            foreach (GPCountry c in countries)
            {
                if (c.getCode().Equals(str))
                    return c;
            }
            return null;
        }

        public bool ExistsCode(string code)
        {
            foreach (GPCountry c in countries)
            {
                if (c.getCode().Equals(code))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public GPCountryList()
        {
            InitializeFromResources();
            //loadTimezones();
        }

        public void addTimezone(string countryCode, string timezoneName)
        {
            GPCountry country = GetCountryByCode(countryCode);
            if (country != null)
            {
                country.addTimezone(timezoneName);
            }
        }

        /*public void loadTimezones()
        {
            using (StringReader sr = new StringReader(GCAL.Base.Properties.Resources.CountryTimezones))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        addTimezone(parts[0], parts[1]);
                    }
                    line = sr.ReadLine();
                }
            }
        }

        public void saveTimezones(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (GPCountry country in countries)
                {
                    foreach (string timezoneName in country.Timezones)
                    {
                        sw.WriteLine("{0}\t{1}", country.getCode(), timezoneName);
                    }
                }
            }
        }*/

        public GPCountry GetCountryByName(string cname)
        {
            foreach (GPCountry c in countries)
            {
                if (c.getName().Equals(cname))
                    return c;
            }
            return null;
        }
    }
}
