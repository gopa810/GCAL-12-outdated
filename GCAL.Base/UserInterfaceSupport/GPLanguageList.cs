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
                int currLangId = GPUserDefaults.IntForKey("gcal.current.language", -1);
                List<GPLanguage> languages = GPLanguageList.getShared().languages; 
                foreach (GPLanguage lang in languages)
                {
                    if (lang.LanguageId == currLangId)
                    {
                        currLang = lang;
                    }
                }

                if (currLang == null)
                    return new GPLanguage("<default>", "");
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
            refreshLanguageList();
        }

        public void refreshLanguageList()
        {
            languages = new List<GPLanguage>();
            string[] files = GPFileHelper.EnumerateLanguageFiles();
            foreach (string s in files)
            {
                GPLanguage nlang = new GPLanguage();
                if (nlang.LoadHeader(s))
                {
                    languages.Add(nlang);
                }
            }
        }


        public bool IsNewVersion(GPLanguage lang)
        {
            if (lang.LanguageId <= 0 || lang.LanguageVersion <= 0)
                return false;

            foreach (GPLanguage lan in languages)
            {
                if (lan.LanguageId == lang.LanguageId)
                {
                    return lang.LanguageVersion > lan.LanguageVersion;
                }
            }

            return true;
        }
    }
}
