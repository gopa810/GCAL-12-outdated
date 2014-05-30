using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPStrings: GPObjectListBase
    {
        public string Language = "English";
        public List<string> gstr = new List<string>();
        public List<string> keys = new List<string>();
        public Dictionary<string, int> map = new Dictionary<string, int>();


        public GPStrings()
        {
        }

        public GPStrings(GPStrings str)
        {
            Language = str.Language;
            foreach (string s1 in str.gstr)
            {
                gstr.Add(s1);
            }

            foreach (string sa in str.keys)
            {
                keys.Add(sa);
            }

            foreach (string s2 in str.map.Keys)
            {
                map[s2] = str.map[s2];
            }
        }

        private static GPStrings _sharedStrings = null;
        
        public static GPStrings getSharedStrings()
        {
            if (_sharedStrings == null)
            {
                _sharedStrings = GPLanguageList.getCurrentLanguage().getStrings();
            }
            return _sharedStrings;
        }

        public static void setSharedStrings(GPStrings value)
        {
            _sharedStrings = value;
        }

        public override string GetDefaultResourceName()
        {
            return GCAL.Base.Properties.Resources.Strings;
        }

        public override string GetDefaultFileName()
        {
            return "Strings.txt";
        }

        public override void InsertNewObjectFromStrings(string[] parts)
        {
            if (parts.Length >= 2)
            {
                int index;
                if (parts[0] == "lang")
                {
                    Language = parts[1];
                }
                else if (int.TryParse(parts[0], out index))
                {
                    if (parts.Length > 2)
                        setString(index, parts[1], parts[2]);
                    else
                        setString(index, parts[1]);
                    if (parts.Length >= 3)
                    {
                        map[parts[2]] = index;
                    }
                }
            }
        }

        public void setString(int index, string strValue)
        {
            setString(index, strValue, string.Empty);
        }

        public void setString(int index, string strValue, string strKey)
        {
            while (keys.Count <= index)
            {
                keys.Add(string.Empty);
                gstr.Add(string.Empty);
            }

            keys[index] = strKey;
            gstr[index] = strValue;

        }

        public string getString(int index)
        {
            if (index < 0 || index >= gstr.Count)
                return string.Empty;
            return gstr[index];
        }

        public string getString(string key)
        {
            if (map.ContainsKey(key))
                return getString(map[key]);
            return string.Empty;
        }

        public static int getCount()
        {
            return getSharedStrings().gstr.Count;
        }

        public override void SaveData(System.IO.StreamWriter writer)
        {
            writer.WriteLine("lang\t{0}", Language);
            for (int i = 0; i < gstr.Count; i++)
            {
                writer.WriteLine("{0}\t{1}\t[2}", i, gstr[i], keys[i]);
            }
        }

        private Dictionary<string, string[]> mapPropertyTextValue = null;

        private void initMapProperty()
        {
            mapPropertyTextValue = new Dictionary<string, string[]>();

            mapPropertyTextValue.Add("cal.anniversary", new string[] {
                getString(202),
                getString(203),
                getString(204)
            });

            mapPropertyTextValue.Add("cal.headertype", new string[] {
                getString(198),
                getString(199),
                getString(200)
            });

            mapPropertyTextValue.Add("gen.caturmasya", new string[] {
                getString(228),
                getString(229),
                getString(230),
                getString(231)
            });

            mapPropertyTextValue.Add("gen.week.firstday", new string[] {
                getString(0),
                getString(1),
                getString(2),
                getString(3),
                getString(4),
                getString(5),
                getString(6)
            });

            mapPropertyTextValue.Add("gen.fastingnotation", new string[] {
                getString(223),
                getString(224)
            });

            mapPropertyTextValue.Add("gen.masaname.format", new string[] {
                getString(218),
                getString(219),
                getString(220),
                getString(221)
            });

            mapPropertyTextValue.Add("gen.sankranti.name.format", new string[] {
                getString(213),
                getString(214),
                getString(215),
                getString(216)
            });
            
            mapPropertyTextValue.Add("gen.timeformat", new string[] {
                getString(233),
                getString(234)
            });

            mapPropertyTextValue.Add("core.sorttype", new string[] {
                "Sort by Event Type",
                "Sort by Time"
            });
        }

        public string getStringValue(string property, int value)
        {
            if (mapPropertyTextValue == null)
                initMapProperty();

            if (mapPropertyTextValue.ContainsKey(property))
            {
                string[] vals = mapPropertyTextValue[property];
                if (value >= 0 && value < vals.Length)
                    return vals[value];
            }

            return string.Empty;
        }

    }
}
