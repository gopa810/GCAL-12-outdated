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
        private List<int> lastIndices = new List<int>();
        private int historyIndex = 0;
        private string currentFile = string.Empty;
        private Dictionary<string, string> dictStrings = new Dictionary<string, string>();
        private GPGregorianTime myDate = null;
        private List<GPLocation> locationsList = new List<GPLocation>();
        private int locationsEnumerator = 0;

        public ContentServer()
        {
            dictStrings.Add("leftArrow", "<span style='font-size:200%'>&#8592;</span><br>");
            dictStrings.Add("rightArrow", "<span style='font-size:200%'>&#8594;</span><br>");
        }

        public string GetFilePath(string file)
        {
            return Path.Combine(ContentDir, file);
        }

        public void LoadStartPage()
        {
            //LoadFile("mainmenu.html");
            LoadFile("dlg-editevent.html?preaction=initnewevent");
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

            if (fileName.IndexOf('?') > 0)
            {
                string [] fp1 = fileName.Split('?');
                fileName = fp1[0];
                string[] fp2 = fp1[1].Split('&');
                foreach (string par in fp2)
                {
                    string[] pardef = par.Split('=');
                    if (pardef.Length == 2)
                    {
                        if (pardef[0] == "preaction")
                            ExecuteCommand(pardef[1]);
                        else
                            saveString(pardef[0], pardef[1]);
                    }
                    else
                        saveString(par, string.Empty);
                }
            }

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
                return evaluate2params(parts[0], parts[1]);
            }
            else if (parts.Length == 3)
            {
                return evaluate3params(parts[0], parts[1], parts[2]);
            }
            return String.Empty;
        }

        public string getSpecEventsList()
        {
            StringBuilder sb = new StringBuilder();
            foreach(GPEventTithi ev in GPEventList.getShared().tithiEvents)
            {
                if (ev.nSpec > 0)
                {
                    if (sb.Length > 0)
                        sb.Append("<line>");
                    sb.AppendFormat("{0}<r>{1}", ev.nSpec, ev.strText);
                }
            }
            return sb.ToString();
        }

        public string getTithiName(int i)
        {
            return GPTithi.getFullName(i);
        }

        public string getSankrantiName(int i)
        {
            return GPSankranti.getName(i);
        }

        public string getMasaName(int i)
        {
            return GPMasa.GetName(i);
        }

        public string getFastName(int i)
        {
            return GPFastType.getName(i);
        }

        public string getEventClassName(int i)
        {
            return GPEventClass.getName(i);
        }

        public string evaluate2params(string p1, string p2)
        {
            if (p1 == "string")
            {
                return getString(p2);
            }
            else if (p1 == "title")
            {
                int idx = 0;
                if (int.TryParse(p2, out idx))
                {
                    if (isValidHistoryIndex(historyIndex + idx))
                        return history[historyIndex + idx].Title;
                }
                return string.Empty;
            }
            else if (p1.Equals("gstr"))
            {
                int idx = 0;
                if (int.TryParse(p2, out idx))
                {
                    return GPStrings.getSharedStrings().getString(idx);
                }
                return string.Empty;
            }
            else if (p1.Equals("calculate"))
            {
                if (p2 == "calendar")
                {
                    CELGenerateCalendar gc = new CELGenerateCalendar(this);
                    return gc.HtmlText;
                }
                else if (p2 == "coreevents")
                {
                    CELGenerateCoreEvents ge = new CELGenerateCoreEvents(this);
                    return ge.HtmlText;
                }
                else if (p2 == "appday")
                {
                    CELGenerateAppearanceDay ga = new CELGenerateAppearanceDay(this);
                    return ga.HtmlText;
                }
                else if (p2 == "masalist")
                {
                    CELGenerateMasaList gm = new CELGenerateMasaList(this);
                    return gm.HtmlText;
                }
                else if (p2 == "calcore")
                {
                    CELGenerateCalendarPlusCore gcc = new CELGenerateCalendarPlusCore(this);
                    return gcc.HtmlText;
                }
                else if (p2 == "cal2locs")
                {
                    CELGenerateCalendarTwoLocs gcc = new CELGenerateCalendarTwoLocs(this);
                    return gcc.HtmlText;
                }
                else if (p2 == "today")
                {
                    GPLocationProvider loc = GPAppHelper.getMyLocation();

                    if (myDate == null)
                        resetToday();

                    StringBuilder sb = new StringBuilder();
                    FormaterHtml.WriteTodayInfoHTML(myDate, loc, sb, 10);
                    return sb.ToString();
                }
                else if (p2 == "nextfest")
                {
                    CELCheckNextWeeksCalendar nwc = new CELCheckNextWeeksCalendar();
                    nwc.SyncExecute();
                    return nwc.getNextFestDaysString();
                }
            }
            else if (p1.Equals("getCurrentSelectionText"))
            {
                int idx = GPUserDefaults.IntForKey(p2, 0);
                return GPStrings.getSharedStrings().getStringValue(p2, idx);
            }
            else if (p1.Equals("proc"))
            {
                if (p2.Equals("getDateText"))
                {
                    if (myDate != null)
                        return GPAppHelper.getDateText(myDate);
                }
                else if (p2.Equals("locationFullName"))
                {
                    if (myDate != null)
                        return myDate.getLocation().getFullName();
                }
            }

            return string.Empty;
        }

        protected string evaluate3params(string p1, string p2, string p3)
        {
            if (p1 == "find")
            {
                if (p2 == "locations")
                {
                    if (p3.StartsWith("$"))
                        p3 = getString(p3.Substring(1));
                    StringBuilder sb = new StringBuilder();
                    List<GPLocation> locs = FindCity(dictStrings[p3]);
                    foreach (GPLocation loc in locs)
                    {
                        if (sb.Length > 0)
                            sb.Append(",\n");
                        sb.AppendFormat("[\"{0}\", \"{1}\", \"{2}\"]", loc.getCity(), loc.getFullName(), loc.getId());
                    }
                    return sb.ToString();
                }
            }
            else if (p1.Equals("getSelectionText"))
            {
                int idx = 0;
                if (int.TryParse(p3, out idx))
                {
                    return GPStrings.getSharedStrings().getStringValue(p2, idx);
                }
            }
            else if (p1.Equals("isSelectedSetting"))
            {
                int idx = 0;
                if (int.TryParse(p3, out idx))
                {
                    int idx2 = GPUserDefaults.IntForKey(p2, 0);
                    return idx == idx2 ? "1" : "0";
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

        public void resetToday()
        {
            GPLocationProvider lp = GPAppHelper.getMyLocation();
            myDate = new GPGregorianTime(lp);
            myDate.Today();
        }

        public void todayToday()
        {
            resetToday();
            LoadFile("today.html");
        }

        public void todayGoPrev()
        {
            if (myDate != null)
                myDate.PreviousDay();
            LoadFile("today.html");
        }

        public void todayGoNext()
        {
            if (myDate != null)
                myDate.NextDay();
            LoadFile("today.html");
        }

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

        public void insertFutureFile2(string file, string title)
        {
            history.Insert(historyIndex + 1, new HistoryEntry(file, title));
        }

        public void delCurrFile()
        {
            if (isValidHistoryIndex(historyIndex))
            {
                history.RemoveAt(historyIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void goBack()
        {
            int top = lastIndices.Count - 1;
            int count = 0;

            // reading count of pages to remove
            // count of pages was writen in some goNextExt command
            if (top >= 0)
                count = lastIndices[top];
            while (count > 0)
            {
                if (isValidHistoryIndex(historyIndex))
                    history.RemoveAt(historyIndex);
                count--;
            }

            // remove #pages from stack
            if (top >= 0)
                lastIndices.RemoveAt(top);

            // load previous page
            while (isValidHistoryIndex(historyIndex - 1))
            {
                historyIndex--;
                if (!history[historyIndex].File.StartsWith("#"))
                {
                    LoadFile(history[historyIndex].File);
                    break;
                }
            }
        }

        public void goNext()
        {
            goNextExt(0);
        }

        /// <summary>
        /// Load next page
        /// </summary>
        /// <param name="b">Count of pages - this is count of pages to be removed, when 
        /// BACK button is pressed on next page. Why is this needed? Because we can insert
        /// a few new pages before calling goNext. When pressing BACK button on that new page,
        /// we need to remove all those pages from history, because if not, it will
        /// break succession of pages in history</param>
        public void goNextExt(int b)
        {
            // inserting #pages into stack
            lastIndices.Add(b);

            // loading next page
            if (isValidHistoryIndex(historyIndex + 1))
            {
                historyIndex++;
                if (history[historyIndex].File.StartsWith("#"))
                    ExecuteCommand(history[historyIndex].File);
                else
                    LoadFile(history[historyIndex].File);
            }
        }

        public GPEvent saveEventFromParameters()
        {
            GPEvent ev = null;
            int evType = getInt("eventtype");
            if (evType == 0)
            {
                GPEventTithi evx = new GPEventTithi();
                evx.nMasa = getInt("eventmasa");
                evx.nTithi = getInt("eventtithi");
                ev = evx;
            }
            else if (evType == 1)
            {
                GPEventSankranti evx = new GPEventSankranti();
                evx.nSankranti = getInt("eventsankranti");
                evx.nOffset = getInt("eventoffset1");
                ev = evx;
            }
            else if (evType == 2)
            {
                GPEventRelative evx = new GPEventRelative();
                evx.nSpecRef = getInt("eventeventref");
                evx.nOffset = getInt("eventoffset2");
                ev = evx;
            }
            ev.strText = getString("eventtitle");
            ev.strFastSubject = getString("eventfastsubject");
            ev.nClass = getInt("eventclass");
            ev.nStartYear = getInt("eventsinceyear");
            ev.nUsed = 1;
            ev.nVisible = getInt("eventvisibility");
            ev.setRawFastType(getInt("eventfasttype"));

            GPEventList.getShared().add(ev);

            return ev;
        }

        public void ExecuteCommand(string cmd)
        {
            if (cmd.StartsWith("#"))
                cmd = cmd.Substring(1);
            Dictionary<string, string> args = new Dictionary<string, string>();

            if (cmd.IndexOf('?') > 0)
            {
                string[] fp1 = cmd.Split('?');
                cmd = fp1[0];
                string[] fp2 = fp1[1].Split('&');
                foreach (string par in fp2)
                {
                    string[] pardef = par.Split('=');
                    if (pardef.Length == 2)
                        args.Add(pardef[0], pardef[1]);
                    else
                        args.Add(par, string.Empty);
                }
            }

            if (cmd.Equals("setmylocation"))
            {
                GPLocationProvider lp = getLocationWithPostfix("");
                GPAppHelper.setMyLocation(lp);
                GPAppHelper.saveMyLocation();
            }
            else if (cmd.Equals("loadlocationid"))
            {
                int locId = 0;
                string sLocId = getString("locationid");
                if (int.TryParse(sLocId, out locId))
                {
                    GPLocation loc = GPLocationList.getShared().findLocationById(locId);
                    if (loc != null)
                    {
                        saveString("locationname", loc.getCity());
                        saveString("locationcountrycode", loc.getCountryCode());
                        saveString("locationcountry", loc.getCountryName());
                        saveString("locationlatitude", loc.getLatitudeString());
                        saveString("locationlongitude", loc.getLongitudeString());
                        saveString("locationtimezone", loc.getTimeZoneName());
                    }
                }
            }
            else if (cmd.Equals("savetzforcountry"))
            {
                GPCountry country = GPCountryList.getShared().GetCountryByName(getString("locationcountry"));
                if (country != null)
                {
                    country.Timezones.Add(getString("locationtimezone"));
                }
            }
            else if (cmd.Equals("newevent"))
            {
                GPEvent ev = saveEventFromParameters();
                saveInt("eventid", ev.eventId);
            }
            else if (cmd.Equals("savechangedevent"))
            {
                ExecuteCommand("removeeventid");
                GPEvent ev = saveEventFromParameters();
                ev.eventId = getInt("eventid");
            }
            else if (cmd.Equals("removeeventid"))
            {
                GPEvent ev = GPEventList.getShared().find(getInt("eventid"));
                if (ev != null)
                {
                    GPEventList.getShared().RemoveEvent(ev);
                }
            }
            else if (cmd.Equals("loadeventid"))
            {
                GPEvent ev = GPEventList.getShared().find(getInt("eventid"));
                if (ev != null)
                {
                    if (ev is GPEventTithi)
                    {
                        saveInt("eventtithi", ((GPEventTithi)ev).nTithi);
                        saveInt("eventmasa", ((GPEventTithi)ev).nMasa);
                    }
                    else if (ev is GPEventSankranti)
                    {
                        saveInt("eventsankranti", ((GPEventSankranti)ev).nSankranti);
                    }
                    else if (ev is GPEventRelative)
                    {
                        saveInt("eventeventref", ((GPEventRelative)ev).nSpecRef);
                    }
                    saveInt("eventclass", ev.nClass);
                    saveInt("eventoffset1", ev.nOffset);
                    saveInt("eventoffset2", ev.nOffset);
                    saveString("eventtitle", ev.strText);
                    saveString("eventfastsubject", ev.strFastSubject);
                    saveInt("eventsinceyear", ev.nStartYear);
                    saveInt("eventvisibility", ev.nVisible);
                    saveInt("eventfasttype", ev.getRawFastType());
                }
            }
            else if (cmd.Equals("initnewevent"))
            {
                saveInt("eventtithi", 0);
                saveInt("eventmasa", 0);
                saveInt("eventsankranti", 0);
                saveInt("eventeventref", 0);
                saveInt("eventclass", 6);
                saveInt("eventoffset1", 0);
                saveInt("eventoffset2", 0);
                saveString("eventtitle", "New Event");
                saveInt("eventfasttype", 0);
                saveString("eventfastsubject", "");
                saveInt("eventsinceyear", -10000);
                saveInt("eventvisibility", 1);
            }
        }

        public string getEventsByName(string name)
        {
            List<GPEvent> events = new List<GPEvent>();
            GPEventList list = GPEventList.getShared();
            if (name != null && name.Length > 0)
            {
                foreach (GPEventTithi et in list.tithiEvents)
                {
                    if (et.strText.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(et);
                    }
                }
                foreach (GPEventSankranti es in list.sankrantiEvents)
                {
                    if (es.strText.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(es);
                    }
                }
                foreach (GPEventRelative er in list.relativeEvents)
                {
                    if (er.strText.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(er);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (GPEvent ev in events)
            {
                if (sb.Length > 0)
                    sb.Append("<line>");
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.strText, ev.getShortDesc());
            }

            return sb.ToString();
        }

        public string getEventsByTithiMasa(int tithi, int masa)
        {
            List<GPEvent> events = new List<GPEvent>();
            GPEventList list = GPEventList.getShared();
            foreach (GPEventTithi et in list.tithiEvents)
            {
                if ((et.nTithi == tithi || tithi < 0) && (et.nMasa == masa || masa < 0))
                {
                    events.Add(et);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (GPEvent ev in events)
            {
                if (sb.Length > 0)
                    sb.Append("<line>");
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.strText, ev.getShortDesc());
            }

            return sb.ToString();
        }

        public string getEventsBySankranti(int sankranti)
        {
            List<GPEvent> events = new List<GPEvent>();
            GPEventList list = GPEventList.getShared();
            foreach (GPEventSankranti es in list.sankrantiEvents)
            {
                if (es.nSankranti == sankranti || sankranti < 0)
                {
                    events.Add(es);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (GPEvent ev in events)
            {
                if (sb.Length > 0)
                    sb.Append("<line>");
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.strText, ev.getShortDesc());
            }

            return sb.ToString();
        }

        public string getEventsByEventClass(int classId)
        {
            List<GPEvent> events = new List<GPEvent>();
            GPEventList list = GPEventList.getShared();
            foreach (GPEventTithi et in list.tithiEvents)
            {
                if (et.nClass == classId)
                {
                    events.Add(et);
                }
            }
            foreach (GPEventSankranti es in list.sankrantiEvents)
            {
                if (es.nClass == classId)
                {
                    events.Add(es);
                }
            }
            foreach (GPEventRelative er in list.relativeEvents)
            {
                if (er.nClass == classId)
                {
                    events.Add(er);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (GPEvent ev in events)
            {
                if (sb.Length > 0)
                    sb.Append("<line>");
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.strText, ev.getShortDesc());
            }

            return sb.ToString();
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

        public void setUserDefaultsInt(string key, string value)
        {
            int i = 0;
            int.TryParse(value, out i);
            GPUserDefaults.SetIntForKey(key, i);
        }

        public string getUserDefaultsInt(string key)
        {
            return GPUserDefaults.IntForKey(key, 0).ToString();
        }

        public void setUserDefaultsBool(string key, string value)
        {
            int i = 0;
            int.TryParse(value, out i);
            GPUserDefaults.SetBoolForKey(key, i != 0);
        }

        public string getUserDefaultsBool(string key)
        {
            return (GPUserDefaults.BoolForKey(key, false) ? 1 : 0).ToString();
        }


        public void findLocations(string s)
        {
            locationsList = new List<GPLocation>();
            if (s == null)
                s = string.Empty;

            foreach (GPLocation loc in GPLocationList.getShared().locations)
            {
                if (s.Length == 0 || loc.getCity().IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    locationsList.Add(loc);
                    if (locationsList.Count > 100)
                        break;
                }
            }
            locationsEnumerator = 0;

        }

        public string getNextLocation()
        {
            if (locationsEnumerator >= 0 && locationsEnumerator < locationsList.Count)
            {
                GPLocation loc = locationsList[locationsEnumerator];
                locationsEnumerator++;
                return string.Format("{0}<br>{1}<br>{2}", loc.getCity(), loc.getFullName(), loc.getId());
            }

            return string.Empty;
        }

        public string getTimezoneOffsets()
        {
            return GPTimeZoneList.sharedTimeZones().getTimezonesOffsetListDesc();
        }

        public string getTimezonesByOffset(string off)
        {
            int i;
            int.TryParse(off, out i);
            GPSortedIntStringList so = new GPSortedIntStringList();
            so.Flag = true;
            foreach (GPTimeZone tz in GPTimeZoneList.sharedTimeZones().getTimeZones())
            {
                if (tz.OffsetSeconds == i)
                {
                    so.push((int)tz.OffsetSeconds, tz.Name);
                }
            }
            return so.ToString();
        }

        public string getTimezonesByCountry(string off)
        {
            StringBuilder sb = new StringBuilder();
            foreach (GPCountry country in GPCountryList.getShared().countries)
            {
                if (country.getCode().Equals(off))
                {
                    foreach (string s in country.Timezones)
                    {
                        if (sb.Length > 0)
                            sb.Append("<line>");
                        GPTimeZone tz = GPTimeZoneList.sharedTimeZones().GetTimezoneByName(s);
                        if (tz != null)
                        {
                            sb.AppendFormat("{0}<br>{1}<br>{2}", tz.Id, tz.Name, tz.getOffsetString());
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public string getTimezoneNameById(int id)
        {
            GPTimeZone tz = GPTimeZoneList.sharedTimeZones().GetTimezoneById(id);
            if (tz != null)
                return tz.Name;
            return "";
        }

        public string getCountriesByText(string s)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sc = new StringBuilder();
            foreach (GPCountry country in GPCountryList.getShared().countries)
            {
                if (country.getName().StartsWith(s, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (sb.Length > 0)
                        sb.Append("<line>");
                    sb.AppendFormat("{0}<br>{1}", country.getCode(), country.getName());
                }
                else if (country.getName().IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    if (sc.Length > 0)
                        sc.Append("<line>");
                    sc.AppendFormat("{0}<br>{1}", country.getCode(), country.getName());
                }
            }
            if (sb.Length > 0)
                sb.Append("<line>");
            sb.Append(sc);
            return sb.ToString();
        }

        public string getCountryName(string s)
        {
            GPCountry country = GPCountryList.getShared().GetCountryByCode(s);
            if (country != null)
                return country.getName();
            return "";
        }

        public void removeLocation(int locId)
        {
            GPLocationList.getShared().DeleteLocationWithId(locId);
        }

        public void removeString(string key)
        {
            if (dictStrings.ContainsKey(key))
                dictStrings.Remove(key);
        }

        public void renameCountryByCode(string countryCode, string newCountryName)
        {
            GPCountry country = GPCountryList.getShared().GetCountryByCode(countryCode);
            if (country != null)
            {
                country.setName(newCountryName);
                GPCountryList.getShared().Modified = true;
            }
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

        public GPLocationProvider getLocationWithPostfix(string postfix)
        {
            string type = getString("locationtype" + postfix);
            if (type == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(getInt("locationid" + postfix));
                if (loc != null)
                    return new GPLocationProvider(loc);
            }

            if (type == "entered")
            {
                GPLocation loc = new GPLocation();
                loc.setCity(getString("locationname" + postfix));
                loc.setLongitudeString(getString("locationlongitude" + postfix));
                loc.setLatitudeString(getString("locationlatitude" + postfix));
                loc.setTimeZoneName(getString("locationtimezone" + postfix));
                return new GPLocationProvider(loc);
            }

            if (type == "mylocation")
                return GPAppHelper.getMyLocation();

            return null;
        }

        public void saveNewLocation()
        {
            GPLocation loc = new GPLocation();

            loc.setCity(getString("locationname"));
            loc.setCountryCode(getString("locationcountrycode"));
            loc.setLongitudeString(getString("locationlongitude"));
            loc.setLatitudeString(getString("locationlatitude"));
            loc.setTimeZoneName(getString("locationtimezone"));

            GPLocationList.getShared().locations.Add(loc);
        }

        public void saveEditedLocation()
        {
            int i;
            GPLocation loc;

            if (int.TryParse(getString("locationid"), out i))
            {
                loc = GPLocationList.getShared().findLocationById(i);
                if (loc != null)
                {
                    loc.setCity(getString("locationname"));
                    loc.setCountryCode(getString("locationcountrycode"));
                    loc.setLongitudeString(getString("locationlongitude"));
                    loc.setLatitudeString(getString("locationlatitude"));
                    loc.setTimeZoneName(getString("locationtimezone"));

                    GPLocationList.getShared().Modified = true;
                }
            }
        }

        public void log(string s)
        {
            Debugger.Log(0, "", "Log from javascript: " + s + "\n");
        }

        public void createCountry(string ccode, string cname)
        {
            GPCountry nc = new GPCountry();
            nc.setCode(ccode);
            nc.setName(cname);
            GPCountryList.getShared().countries.Add(nc);
        }

        public string existCountry(string ccode, string cname)
        {
            StringBuilder sb = new StringBuilder();
            if (ccode == null)
                sb.Append("err1;");
            else if (ccode.Length == 0)
                sb.Append("err1;");
            if (cname == null)
                sb.Append("err2;");
            else if (cname.Length == 0)
                sb.Append("err2;");
            if (ccode != null && GPCountryList.getShared().GetCountryByCode(ccode) != null)
                sb.Append("err3;");
            if (cname != null && GPCountryList.getShared().GetCountryByName(cname) != null)
                sb.Append("err4;");

            return sb.ToString().Trim(';');
        }

        public void deleteTimezoneForCountry(string countryName, string timezoneName)
        {
            GPCountry country = GPCountryList.getShared().GetCountryByName(countryName);
            if (country != null)
            {
                int i = country.Timezones.IndexOf(timezoneName);
                if (i >= 0)
                {
                    country.Timezones.RemoveAt(i);
                }
            }
        }
    }
}
