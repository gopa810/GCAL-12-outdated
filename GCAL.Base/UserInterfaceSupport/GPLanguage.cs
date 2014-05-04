using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCAL.Base
{
    public class GPLanguage
    {
        public string LanguageName = string.Empty;
        public string LanguageFile = string.Empty;
        public bool modified_this = false;
        private GPStrings _strs = null;


        public bool getModified()
        {
            return modified_this || getStrings().Modified;
        }

        public void setModified(bool value)
        {
            modified_this = value;
            getStrings().Modified = value;
        }

        public GPStrings getStrings()
        {
            if (_strs == null)
            {
                _strs = new GPStrings();
                if (LanguageFile.Length == 0)
                {
                    _strs.InitializeFromResources();
                }
                else
                {
                    using (StreamReader reader = File.OpenText(LanguageFile))
                    {
                        _strs.ReadStream(reader);
                    }
                    LanguageName = _strs.Language;
                }
            }
            return _strs;
        }

        public void setStrings(GPStrings value)
        {
            _strs = new GPStrings(value);
            setModified(true);
        }

        public bool loadFile(string fileName)
        {
            LanguageName = string.Empty;
            LanguageFile = fileName;
            using (StreamReader reader = File.OpenText(LanguageFile))
            {
                _strs = new GPStrings();
                _strs.ReadStream(reader);
                LanguageName = _strs.Language;
            }
            return (LanguageName.Length > 0);
        }

        public void saveFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("lang\t{0}", LanguageName);
                for (int i = 0; i < getStrings().gstr.Count; i++)
                {
                    if (getStrings().gstr[i].Length > 0)
                        sw.WriteLine("{0}\t{1}\t{2}", i, getStrings().gstr[i], getStrings().keys[i]);
                }
            }
        }

        ~GPLanguage()
        {
            if (getModified() && LanguageFile.Length > 0)
            {
                saveFile(LanguageFile);
            }
        }

        public GPLanguage()
        {
        }

        public GPLanguage(string s, string n)
        {
            LanguageName = s;
            LanguageFile = n;
        }

        public override string ToString()
        {
            return LanguageName;
        }
    }
}
