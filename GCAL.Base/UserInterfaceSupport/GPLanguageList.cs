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

        public static GPLanguage getDefaultLanguage()
        {
            if (defLang == null)
            {
                defLang = GPLanguageList.getShared().languages[0];
            }
            return defLang;
        }

        private void initialize()
        {
            languages.Add(new GPLanguage("<default>", ""));

            string langFileStart = "lang\t";
            string[] files = GPFileHelper.EnumerateLanguageFiles();
            foreach (string s in files)
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    string line = reader.ReadLine();
                    if (line != null && line.StartsWith(langFileStart))
                    {
                        languages.Add(new GPLanguage(line.Substring(langFileStart.Length), s));
                    }
                }
            }
        }

    }
}
