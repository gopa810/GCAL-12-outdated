using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GCAL.Base
{
    public class GPLanguageList
    {
        public List<GPLanguage> languages = new List<GPLanguage>();
        public int currentLanguageId = -1;

        private static GPLanguage currLang = null;
        private static GPLanguage defLang = null;
        private static GPLanguageList _sharedList = null;

        public static GPLanguageList getShared()
        {
            if (_sharedList == null)
            {
                _sharedList = new GPLanguageList();
                _sharedList.initialize();
            }
            return _sharedList;
        }

        public static GPLanguage getCurrentLanguage()
        {
            if (currLang == null)
            {
                if (GPLanguageList.getShared().languages.Count == 0)
                    return new GPLanguage("<default>", "");
                currLang = GPLanguageList.getShared().languages[0];
            }
            return currLang;
        }

        public static void setCurrentLanguage(GPLanguage value)
        {
            GPStrings.setSharedStrings(value.getStrings());
            currLang = value;
        }

        public void setCurrentLanguageId(int id)
        {
            GPUserDefaults.SetIntForKey("gcal.current.language", id);
            currentLanguageId = id;
            GPLanguage lan = findLanguageWithId(id);
            if (lan != null)
            {
                lan.loadFile(lan.LanguageFile);
                GPStrings.setSharedStrings(lan.getStrings());
            }
        }

        public static GPLanguage getDefaultLanguage()
        {
            if (defLang == null)
            {
                defLang = GPLanguageList.getShared().languages[0];
            }
            return defLang;
        }


        public GPLanguage findLanguageWithId(int wid)
        {
            foreach (GPLanguage lan in languages)
            {
                if (lan.LanguageId == wid)
                    return lan;
            }
            return null;
        }

        private void initialize()
        {
            currentLanguageId = GPUserDefaults.IntForKey("gcal.current.language", -1);
            string langFileStart = "lang\t";
            string[] files = GPFileHelper.EnumerateLanguageFiles();
            foreach (string s in files)
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    string line = reader.ReadLine();
                    if (line != null && line.StartsWith(langFileStart))
                    {
                        GPLanguage nlang = new GPLanguage();
                        nlang.LanguageName = line.Substring(langFileStart.Length);
                        nlang.LanguageFile = s;
                        line = reader.ReadLine();
                        if (line != null && line.StartsWith("langid\t"))
                        {
                            if (int.TryParse(line.Substring(7), out nlang.LanguageId))
                            {
                                languages.Add(nlang);
                            }
                        }

                    }
                }
            }
        }

    }
}
