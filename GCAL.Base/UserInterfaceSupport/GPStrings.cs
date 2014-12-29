using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCAL.Base
{
    public class GPStrings: GPObjectListBase
    {
        public string Language = "English";
        public int LanguageId = 2;
        public int LanguageVersion = 1;

        public List<string> gstr = new List<string>();
        public List<string> keys = new List<string>();
        public Dictionary<string, int> map = new Dictionary<string, int>();

        public static bool showNumberOfString = true;
        private static GPStrings _sharedStrings = null;
        private static GPStrings _originalStrings = null;

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

        public override string ToString()
        {
            StringBuilder writer = new StringBuilder();
            writer.AppendFormat("lang\t{0}\n", Language);
            writer.AppendFormat("langid\t{0}\n", LanguageId);
            writer.AppendFormat("modified\t{0}\n", Modified.ToString());
            writer.AppendFormat("version\t{0}\n", LanguageVersion);
            for (int i = 0; i < gstr.Count; i++)
            {
                writer.AppendFormat("{0}\t{1}\n", i, gstr[i]);
            }
            return writer.ToString();
        }

        public string getCustomFilePath()
        {
            string fileName = string.Empty;
            
            fileName = getFullPathForFile(GetDefaultFileNameForKey(FileKey.Primary));

            return fileName;
        }

        public static GPStrings getSharedStrings()
        {
            if (_sharedStrings == null)
            {
                _sharedStrings = new GPStrings();
                string fileName = _sharedStrings.getCustomFilePath();
                if (File.Exists(fileName))
                {
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        _sharedStrings.ReadStream(sr, FileKey.Primary);
                    }
                }
                else
                {
                    _sharedStrings = GPLanguageList.getCurrentLanguage().getStrings();
                    _sharedStrings.Modified = false;
                    _sharedStrings.Save(true);
                }
            }
            return _sharedStrings;
        }

        public static GPStrings getOriginalStrings()
        {
            if (_originalStrings == null)
            {
                _originalStrings = new GPStrings();
                _originalStrings.InitializeFromDefaultResources();
            }
            return _originalStrings;
        }

        public static void setSharedStrings(GPStrings value)
        {
            _sharedStrings = value;
            _sharedStrings.Save(true);
        }

        public override string GetDefaultResourceForKey(FileKey fk)
        {
            return GCAL.Base.Properties.Resources.Strings;
        }

        public override string GetDefaultFileNameForKey(FileKey fk)
        {
            return "Strings.txt";
        }

        public override void InsertNewObjectFromStrings(string[] parts, FileKey fk)
        {
            if (parts.Length >= 2)
            {
                int index;
                if (parts[0] == "lang")
                {
                    Language = parts[1];
                }
                else if (parts[0].Equals("langid"))
                {
                    int.TryParse(parts[1], out LanguageId);
                }
                else if (parts[0].Equals("modified"))
                {
                    bool.TryParse(parts[1], out Modified);
                }
                else if (parts[0].Equals("version"))
                {
                    int.TryParse(parts[1], out LanguageVersion);
                }
                else if (int.TryParse(parts[0], out index))
                {
                    setString(index, parts[1]);
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

        public static string getString(int index)
        {
            return getSharedStrings().getStringValue(index, false);
        }

        public static string getUpperString(int index)
        {
            return getSharedStrings().getStringValue(index, true);
        }

        public static string getPlainString(int index)
        {
            return getSharedStrings().getPlainStringValue(index);
        }

        public static string getOriginalString(int index)
        {
            return getOriginalStrings().getPlainStringValue(index);
        }

        public string getStringValue(int index)
        {
            return getStringValue(index, false);
        }

        public string getStringValue(int index, bool bUpper)
        {
            if (index < 0 || index >= gstr.Count)
                return string.Empty;
            if (showNumberOfString)
            {
                return string.Format("<span class=highred oncontextmenu='javascript:scriptObject.EditString({0});return false;'>{1}</span>", index, (bUpper ? gstr[index].ToUpper() : gstr[index]));
            }
            else
            {
                if (bUpper)
                    return gstr[index].ToUpper();
                return gstr[index];
            }
        }

        public string getPlainStringValue(int index)
        {
            if (index < 0 || index >= gstr.Count)
                return string.Empty;
            return gstr[index];
        }

        public string getString(string key)
        {
            if (map.ContainsKey(key))
                return getStringValue(map[key]);
            return string.Empty;
        }

        public static int getCount()
        {
            return getSharedStrings().gstr.Count;
        }

        public override void SaveData(System.IO.StreamWriter writer, FileKey fk)
        {
            writer.WriteLine("lang\t{0}", Language);
            writer.WriteLine("langid\t{0}", LanguageId);
            writer.WriteLine("modified\t{0}", Modified.ToString());
            writer.WriteLine("version\t{0}", LanguageVersion);
            for (int i = 0; i < gstr.Count; i++)
            {
                writer.WriteLine("{0}\t{1}", i, gstr[i]);
            }
        }

        private Dictionary<string, string[]> mapPropertyTextValue = null;

        private void initMapProperty()
        {
            mapPropertyTextValue = new Dictionary<string, string[]>();

            mapPropertyTextValue.Add("cal.anniversary", new string[] {
                getStringValue(202),
                getStringValue(203),
                getStringValue(204)
            });

            mapPropertyTextValue.Add("cal.headertype", new string[] {
                getStringValue(198),
                getStringValue(199),
                getStringValue(200)
            });

            mapPropertyTextValue.Add("gen.caturmasya", new string[] {
                getStringValue(228),
                getStringValue(229),
                getStringValue(230),
                getStringValue(231)
            });

            mapPropertyTextValue.Add("gen.week.firstday", new string[] {
                getStringValue(0),
                getStringValue(1),
                getStringValue(2),
                getStringValue(3),
                getStringValue(4),
                getStringValue(5),
                getStringValue(6)
            });

            mapPropertyTextValue.Add("gen.fastingnotation", new string[] {
                getStringValue(223),
                getStringValue(224)
            });

            mapPropertyTextValue.Add("gen.masaname.format", new string[] {
                getStringValue(218),
                getStringValue(219),
                getStringValue(220),
                getStringValue(221)
            });

            mapPropertyTextValue.Add("gen.sankranti.name.format", new string[] {
                getStringValue(213),
                getStringValue(214),
                getStringValue(215),
                getStringValue(216)
            });
            
            mapPropertyTextValue.Add("gen.timeformat", new string[] {
                getStringValue(233),
                getStringValue(234)
            });

            mapPropertyTextValue.Add("core.sorttype", new string[] {
                getStringValue(1263),
                getStringValue(1264)
            });

            mapPropertyTextValue.Add("gen.startpage", new string[] {
                getStringValue(1054),
                getStringValue(452),
                getStringValue(174)
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
