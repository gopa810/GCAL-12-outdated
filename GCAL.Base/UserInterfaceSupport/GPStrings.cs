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


    }
}
