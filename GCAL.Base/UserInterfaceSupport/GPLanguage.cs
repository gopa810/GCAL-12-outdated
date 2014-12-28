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
        public int LanguageVersion = 1;
        public int LanguageId = 1;
        public bool LanguageModified = false;
        private GPStrings _strs = null;

        public static readonly string LANGFILETAG_NAME = "lang\t";
        public static readonly string LANGFILETAG_ID = "langid\t";
        public static readonly string LANGFILETAG_VERSION = "version\t";
        public static readonly string LANGFILETAG_MODIFIED = "modified\t";

        public void setModified(bool value)
        {
            LanguageModified = value;
            getStrings().Modified = value;
        }

        public GPStrings getStrings()
        {
            if (_strs == null)
            {
                _strs = new GPStrings();
                if (LanguageFile.Length == 0)
                {
                    _strs.InitializeFromDefaultResources();
                }
                else
                {
                    using (StreamReader reader = File.OpenText(LanguageFile))
                    {
                        _strs.ReadStream(reader, GPObjectListBase.FileKey.Primary);
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

        /// <summary>
        /// Loading information about language strings contained in given file
        /// </summary>
        /// <param name="filename">Full path of file with language strings</param>
        public bool LoadHeader(string filename)
        {
            this.LanguageFile = filename;
            int maxLines = 20;

            using (StreamReader reader = new StreamReader(filename))
            {
                string line = reader.ReadLine();
                while (line != null && maxLines > 0)
                {
                    if (line.StartsWith(LANGFILETAG_ID))
                    {
                        int n = -1;
                        if (int.TryParse(line.Substring(LANGFILETAG_ID.Length), out n))
                        {
                            this.LanguageId = n;
                        }
                    }
                    else if (line.StartsWith(LANGFILETAG_NAME))
                    {
                        this.LanguageName = line.Substring(LANGFILETAG_NAME.Length);
                    }
                    line = reader.ReadLine();
                    maxLines--;
                }
            }

            return this.LanguageId > 0;
        }

        public bool loadFile(string fileName)
        {
            LanguageName = string.Empty;
            LanguageFile = fileName;
            using (StreamReader reader = File.OpenText(LanguageFile))
            {
                _strs = new GPStrings();
                _strs.ReadStream(reader, GPObjectListBase.FileKey.Primary);
                LanguageName = _strs.Language;
            }
            return (LanguageName.Length > 0);
        }

        public void saveFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("lang\t{0}", LanguageName);
                sw.WriteLine("langid\t{1}", LanguageId);
                for (int i = 0; i < getStrings().gstr.Count; i++)
                {
                    if (getStrings().gstr[i].Length > 0)
                        sw.WriteLine("{0}\t{1}\t{2}", i, getStrings().gstr[i], getStrings().keys[i]);
                }
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
