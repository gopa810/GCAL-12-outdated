using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.InteropServices;

using GCAL.Base;
using GCAL.Engine;

namespace GCAL
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public class ContentServer
    {
        public class HistoryEntry
        {
            public string File { get; set; }
            public string Title { get; set; }
            public HistoryEntry()
            {
                File = string.Empty;
                Title = string.Empty;
            }

            public HistoryEntry(string f)
            {
                File = f;
                Title = string.Empty;
            }

            public HistoryEntry(string f, string t)
            {
                File = f;
                Title = t;
            }
        }

        public WebBrowser WebBrowser { get; set; }
        public string ContentDir { get; set; }
        public string CurrentContents;
        private List<HistoryEntry> history = new List<HistoryEntry>();
        private int historyIndex = 0;
        private string currentFile = string.Empty;
        private Dictionary<string, string> dictStrings = new Dictionary<string, string>();

        public string GetFilePath(string file)
        {
            return Path.Combine(ContentDir, file);
        }

        public void LoadStartPage()
        {
            LoadFile("mainmenu.html");
        }

        /// <summary>
        /// Loading file from local resources and modifying content
        /// by replacing variables with their values
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFile(string fileName)
        {
            Debugger.Log(0, "", "Navigation File Name: " + fileName + "\n");
            StringBuilder sb = new StringBuilder();

            string filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
                sb.Append(File.ReadAllText(filePath));
            else
            {
                return;
            }

            ModifyFileVariables(sb);

            currentFile = fileName;
            WebBrowser.DocumentText = sb.ToString();
        }

        /// <summary>
        /// Modification of file
        /// 
        /// Replacing variable names with their values
        /// </summary>
        /// <param name="sb"></param>
        protected void ModifyFileVariables(StringBuilder sb)
        {
            sb.Replace("{%homedir}", ContentDir);
            sb.Replace("{%current_theme}", "theme0.css");

            int ni, ei, len;
            string val;
            char [] temp = new char[] {};
            ni = findIndexOf(sb, "{%");
            while (ni >= 0)
            {
                ei = findIndexOf(sb, "}", ni);
                len = ei - ni - 2;
                temp = new char[len];
                sb.CopyTo(ni + 2, temp, 0, len);
                val = evaluateVariable(new string(temp));
                sb.Remove(ni, len + 3);
                sb.Insert(ni, val);
                ni = findIndexOf(sb, "{%", ni);
            }
        }

        protected string evaluateVariable(string s)
        {
            string[] parts = s.Split(' ');
            if (parts.Length == 1)
            {
            }
            else if (parts.Length == 2)
            {
                if (parts[0] == "string")
                {
                    return getString(parts[1]);
                }
                else if (parts[0] == "title")
                {
                    int idx = 0;
                    if (int.TryParse(parts[1], out idx))
                    {
                        if (isValidHistoryIndex(historyIndex + idx))
                            return  history[historyIndex + idx].Title;
                    }
                    return string.Empty;
                }
                else if (parts[0] == "calculate")
                {
                    if (parts[1] == "calendar")
                    {
                        CELGenerateCalendar gc = new CELGenerateCalendar(this);
                        return gc.HtmlText;
                    }
                    else if (parts[1] == "coreevents")
                    {
                        CELGenerateCoreEvents ge = new CELGenerateCoreEvents(this);
                        return ge.HtmlText;
                    }
                    else if (parts[1] == "appday")
                    {
                        CELGenerateAppearanceDay ga = new CELGenerateAppearanceDay(this);
                        return ga.HtmlText;
                    }
                    else if (parts[1] == "masalist")
                    {
                        CELGenerateMasaList gm = new CELGenerateMasaList(this);
                        return gm.HtmlText;
                    }
                    else if (parts[1] == "calcore")
                    {
                        CELGenerateCalendarPlusCore gcc = new CELGenerateCalendarPlusCore(this);
                        return gcc.HtmlText;
                    }
                    else if (parts[1] == "cal2locs")
                    {
                        CELGenerateCalendarTwoLocs gcc = new CELGenerateCalendarTwoLocs(this);
                        return gcc.HtmlText;
                    }
                    else if (parts[1] == "today")
                    {
                        GPLocationProvider loc = GPAppHelper.getMyLocation();

                        GPGregorianTime myDate = new GPGregorianTime(loc);
                        myDate.Today();

                        StringBuilder sb = new StringBuilder();
                        FormaterHtml.WriteTodayInfoHTML(myDate, loc, sb, 10);
                        return sb.ToString();
                    }
                }
            }
            else if (parts.Length == 3)
            {
                if (parts[0] == "find")
                {
                    if (parts[1] == "locations")
                    {
                        StringBuilder sb = new StringBuilder();
                        List<GPLocation> locs = FindCity(dictStrings[parts[2]]);
                        foreach (GPLocation loc in locs)
                        {
                            if (sb.Length > 0)
                                sb.Append(",\n");
                            sb.AppendFormat("[\"{0}\", \"{1}\", \"{2}\"]", loc.getCity(), loc.getFullName(), loc.getId());
                        }
                        return sb.ToString();
                    }
                }
            }
            return String.Empty;
        }

        protected int findIndexOf(StringBuilder sb, string s)
        {
            return findIndexOf(sb, s, 0);
        }

        protected int findIndexOf(StringBuilder sb, string s, int startIndex)
        {
            int f = 0;
            for (int i = startIndex; i < sb.Length; i++)
            {
                if (sb[i] == s[f])
                {
                    f++;
                    if (f >= s.Length)
                        return i - s.Length + 1;
                }
                else
                {
                    f = 0;
                }
            }

            return -1;
        }

        protected bool isValidHistoryIndex(int i)
        {
            return i >= 0 && i < history.Count;
        }

        /// <summary>
        /// Functions used in javascript on pages
        /// </summary>
        #region functions for scripting

        public string getDir()
        {
            return ContentDir;
        }

        public string getUri(string fname)
        {
            var uri = new System.Uri(GetFilePath(fname));
            var converted = uri.AbsoluteUri;
            return converted;
        }

        public void clearHistory()
        {
            historyIndex = -1;
            history.Clear();
        }

        public void addFutureFile(string file)
        {
            history.Add(new HistoryEntry(file));
        }

        public void addFutureFile2(string file, string title)
        {
            history.Add(new HistoryEntry(file, title));
        }

        public void goBack()
        {
            if (isValidHistoryIndex(historyIndex - 1))
            {
                historyIndex--;
                LoadFile(history[historyIndex].File);
            }
        }

        public void goNext()
        {
            //Debugger.Log(0, "", WebBrowser.DocumentText);
            if (isValidHistoryIndex(historyIndex + 1))
            {
                historyIndex++;
                LoadFile(history[historyIndex].File);
            }
        }

        public void setCurrTitle(string t)
        {
            if (isValidHistoryIndex(historyIndex))
            {
                history[historyIndex].Title = t;
            }
            else
            {
                addFutureFile2(currentFile, t);
                historyIndex = history.Count - 1;
            }
        }

        public string getString(string key)
        {
            if (dictStrings.ContainsKey(key))
                return dictStrings[key];
            return String.Empty;
        }

        public int getInt(string key)
        {
            int i = 0;
            if (dictStrings.ContainsKey(key))
            {
                int.TryParse(dictStrings[key], out i);
            }
            return i;
        }

        public void saveString(string key, string value)
        {
            if (dictStrings.ContainsKey(key))
                dictStrings[key] = value;
            else
                dictStrings.Add(key, value);
        }

        public void saveInt(string key, int value)
        {
            if (dictStrings.ContainsKey(key))
                dictStrings[key] = value.ToString();
            else
                dictStrings.Add(key, value.ToString());
        }

        #endregion


        public List<GPLocation> FindCity(string s)
        {
            List<GPLocation> locs = new List<GPLocation>();
            if (s == null)
                s = string.Empty;

            foreach (GPLocation loc in GPLocationList.getShared().locations)
            {
                if (s.Length == 0 || loc.getCity().IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    locs.Add(loc);
                    if (locs.Count > 100)
                        break;
                }
            }

            return locs;
        }
    }
}
