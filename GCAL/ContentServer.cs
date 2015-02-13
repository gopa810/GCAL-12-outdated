using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Drawing;

using GCAL.Base;
using GCAL.Engine;
using GCAL.Dialogs;

namespace GCAL
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public class ContentServer
    {
        public class ButtonCommandTag
        {
            public string Command { get; set; }
            public int StringIndex { get; set; }
        }

        public class PageHistoryEntry
        {
            public string Page { get; set; }
        }

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

        public class FlowPage
        {
            public string Name;
            public string Source;
            public List<FlowCommand> Commands = new List<FlowCommand>();
            public Dictionary<string, FlowAction> Actions = new Dictionary<string, FlowAction>();
        }

        public class FlowAction
        {
            public string Name;
            public List<FlowCommand> Commands = new List<FlowCommand>();
        }

        public class FlowCommand
        {
            public string Command;
            public List<FlowCommand> Commands = new List<FlowCommand>();
            public string[] Args;
            public FlowCommand()
            {
            }

            public FlowCommand(string[] strs)
            {
                if (strs.Length > 0)
                {
                    Command = strs[0];
                    Args = new string[strs.Length - 1];
                    Array.Copy(strs, 1, Args, 0, strs.Length - 1);
                }
            }
            public string getArg(int i)
            {
                if (i < 0 || i >= Args.Length)
                    return string.Empty;
                return Args[i];
            }
            public int getArgInt(int i)
            {
                int value = -1;

                if (int.TryParse(getArg(i), out value))
                {
                    return value;
                }

                return -1;
            }

            public string getArgSubst(int i, Dictionary<string, string> dict, bool bPlain)
            {
                string sa = getArg(i);
                if (sa.IndexOf("$") >= 0)
                {
                    string[] sas = sa.Split(' ');
                    string[] nas = new string[sas.Length];
                    for(int im = 0; im < sas.Length; im++)
                    {
                        string s = sas[im];
                        if (s.StartsWith("$"))
                        {
                            int ik = 0;
                            string sq = s.Substring(1);
                            if (dict.ContainsKey(sq))
                            {
                                nas[im] = dict[sq];
                            }
                            else if (int.TryParse(sq, out ik))
                            {
                                if (bPlain)
                                    nas[im] = GPStrings.getPlainString(ik);
                                else
                                    nas[im] = GPStrings.getString(ik);
                            }
                            else
                            {
                                nas[im] = s;
                            }
                        }
                        else
                        {
                            nas[im] = s;
                        }

                    }

                    sa = String.Join(" ", nas);
                }

                return sa;
            }
            public override bool Equals(object obj)
            {
                if (obj is string)
                {
                    return Command.Equals(obj as string);
                }
                return base.Equals(obj);
            }
        }

        public FlowPage CurrentPage { get; set; }
        public WebBrowser WebBrowser { get; set; }
        public string ContentDir { get; set; }
        public string CurrentContents;
        private List<PageHistoryEntry> pageHistory = new List<PageHistoryEntry>();
        private int pageHistoryIndex = 0;

        private List<HistoryEntry> history = new List<HistoryEntry>();
        private List<int> lastIndices = new List<int>();
        private int historyIndex = 0;
        private string currentFile = string.Empty;
        private Dictionary<string, string> dictStrings = new Dictionary<string, string>();
        private GPGregorianTime myDate = null;
        private List<GPLocation> locationsList = new List<GPLocation>();
        private int locationsEnumerator = 0;
        private List<GPString> stringsList = new List<GPString>();
        private int stringsEnumerator = 0;
        private CELSearch searchTask = null;
        private CELUpdateLanguageList updateLanguageListTask = null;
        private CELSendMyLanguageFile sendMyLanguageFileTask = null;
        public object CurrentCalculatedObject = null;
        public Dictionary<string, FlowPage> Pages = new Dictionary<string, FlowPage>();

        // user interface controls
        public Form MainForm { get; set; }
        public Control TopBar { get; set; }
        public Control BottomBar { get; set; }
        public List<Button> TopButtons = new List<Button>();
        public List<Button> BottomButtons = new List<Button>();

        public static int currentEditButtonStringIndex = -1;

        public ContentServer()
        {
            dictStrings.Add("leftArrow", "&nbsp;&#9001;&#9001;&nbsp;");
            dictStrings.Add("rightArrow", "&nbsp;&#9002;&#9002;&nbsp;");
            dictStrings.Add("stringsFile", GPStrings.getSharedStrings().getCustomFilePath());
            dictStrings.Add("transemail", "translations@gcal.home.sk");
            dictStrings.Add("durationhour", "6");
            dictStrings.Add("durationmin", "0");
            dictStrings.Add("strpart", "0");
        }

        public Dictionary<string, string> getProperties()
        {
            return dictStrings;
        }

        public string GetFilePath(string file)
        {
            return Path.Combine(ContentDir, file);
        }

        public void LoadFlows()
        {
            string fileName = GetFilePath("flows.g");
            List<object> stack = new List<object>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                object target = this;
                string[] line = null;
                stack.Add(target);

                while ((line = ReadFlowLine(sr)) != null)
                {
                    if (target is ContentServer)
                    {
                        if (line.Length > 1 && line[0].Equals("page"))
                        {
                            FlowPage fp = new FlowPage();
                            fp.Name = line[1];
                            fp.Source = line[1];
                            stack.Add(target);
                            target = fp;
                        }
                    }
                    else if (target is FlowCommand)
                    {
                        FlowCommand flowCmd = target as FlowCommand;
                        if (line.Length > 1 && line[0].Equals("if"))
                        {
                            FlowCommand fc = new FlowCommand(line);
                            flowCmd.Commands.Add(fc);
                            stack.Add(fc);
                            target = fc;
                        }
                        else if (line.Length > 0 && line[0].Equals("end"))
                        {
                            target = stack[stack.Count - 1];
                            stack.RemoveAt(stack.Count - 1);
                        }
                        else
                        {
                            FlowCommand fc = new FlowCommand(line);
                            flowCmd.Commands.Add(fc);
                        }
                    }
                    else if (target is FlowPage)
                    {
                        FlowPage fp = target as FlowPage;
                        if (line.Length > 1 && line[0].Equals("source"))
                        {
                            fp.Source = line[1];
                        }
                        else if (line.Length > 1 && line[0].Equals("set"))
                        {
                            FlowCommand fc = new FlowCommand(line);
                            fp.Commands.Add(fc);
                        }
                        else if (line.Length > 1 && line[0].Equals("exec"))
                        {
                            fp.Commands.Add(new FlowCommand(line));
                        }
                        else if (line.Length > 1 && line[0].Equals("if"))
                        {
                            FlowCommand fc = new FlowCommand(line);
                            stack.Add(target);
                            target = fc;
                            fp.Commands.Add(fc);
                        }
                        else if (line.Length > 1 && line[0].Equals("action"))
                        {
                            FlowAction fa = new FlowAction();
                            fa.Name = line[1];
                            stack.Add(target);
                            target = fa;
                        }
                        else if (line.Length > 0 && line[0].Equals("end"))
                        {
                            target = stack[stack.Count - 1];
                            stack.RemoveAt(stack.Count - 1);
                            if (target is ContentServer)
                            {
                                Pages.Add(fp.Name, fp);
                            }
                        }
                        else
                        {
                            FlowCommand fc = new FlowCommand(line);
                            fp.Commands.Add(fc);
                        }
                    }
                    else if (target is FlowAction)
                    {
                        FlowAction fa = target as FlowAction;
                        if (line.Length > 0 && line[0].Equals("end"))
                        {
                            target = stack[stack.Count - 1];
                            stack.RemoveAt(stack.Count - 1);
                            if (target is FlowPage)
                            {
                                FlowPage fp = target as FlowPage;
                                fp.Actions.Add(fa.Name, fa);
                            }
                        }
                        else if (line.Length > 1 && line[0].Equals("if"))
                        {
                            FlowCommand fc = new FlowCommand(line);
                            stack.Add(fc);
                            target = fc;
                            fa.Commands.Add(fc);
                        }
                        else
                        {
                            FlowCommand cmd = new FlowCommand(line);
                            fa.Commands.Add(cmd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads array of strings as line from given stream
        /// It is similar for splitting string by spaces, but this
        /// functions is evaluating also ' and " characters, so
        /// characters enclosed in pair of ' or pair of " are considered
        /// as one string.
        /// </summary>
        /// <param name="sr">Input stream</param>
        /// <returns>Array of strings</returns>
        public string[] ReadFlowLine(StreamReader sr)
        {
            List<StringBuilder> sbs = new List<StringBuilder>();
            StringBuilder curr = new StringBuilder();
            sbs.Add(curr);
            int mode = 0;
            string line = sr.ReadLine();
            
            if (line == null)
                return null;

            foreach (char c in line)
            {
                if (mode == 0)
                {
                    if (c == '\'')
                    {
                        mode = 2;
                    }
                    else if (c == '\"')
                    {
                        mode = 4;
                    }
                    else if (c == '#')
                    {
                        mode = 7;
                    }
                    else if (!char.IsWhiteSpace(c))
                    {
                        curr.Append(c);
                        mode = 1;
                    }
                }
                else if (mode == 1)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        curr = new StringBuilder();
                        sbs.Add(curr);
                        mode = 0;
                    }
                    else
                    {
                        curr.Append(c);
                    }
                }
                else if (mode == 2)
                {
                    if (c == '\\')
                    {
                        mode = 3;
                    }
                    else if (c == '\'')
                    {
                        curr = new StringBuilder();
                        sbs.Add(curr);
                        mode = 0;
                    }
                    else
                    {
                        curr.Append(c);
                    }
                }
                else if (mode == 3)
                {
                    if (c == 'n')
                    {
                        curr.Append('\n');
                    }
                    else if (c == 't')
                    {
                        curr.Append('\t');
                    }
                    else if (c == 'r')
                    {
                        curr.Append('\r');
                    }
                    else
                    {
                        curr.Append(c);
                    }
                    mode = 2;
                }
                else if (mode == 4)
                {
                    if (c == '\\')
                    {
                        mode = 5;
                    }
                    else if (c == '\"')
                    {
                        curr = new StringBuilder();
                        sbs.Add(curr);
                        mode = 0;
                    }
                    else
                    {
                        curr.Append(c);
                    }
                }
                else if (mode == 5)
                {
                    if (c == 'n')
                    {
                        curr.Append('\n');
                    }
                    else if (c == 't')
                    {
                        curr.Append('\t');
                    }
                    else if (c == 'r')
                    {
                        curr.Append('\r');
                    }
                    else
                    {
                        curr.Append(c);
                    }
                    mode = 4;
                }
                else if (mode == 7)
                {
                }
            }

            if (sbs.Count > 0 && curr.Length == 0)
            {
                sbs.RemoveAt(sbs.Count - 1);
            }

            string [] array = new string[sbs.Count];
            for(int i = 0; i < sbs.Count; i++)
            {
                array[i] = sbs[i].ToString();
            }

            return array;
        }

        public void LoadStartPage()
        {
            if (getCurrentLanguageId() < 0)
            {
                LoadPage("languages", true);
            }
            else
            {
                int startPage = GPUserDefaults.IntForKey("gen.startpage", 0);
                if (startPage == 1)
                {
                    LoadPage("nextfest", true);
                }
                else if (startPage == 2)
                {
                    LoadPage("today", true);
                }
                else
                {
                    LoadPage("mainmenu", true);
                }
            }
        }

        public void EditString(int i)
        {
            currentEditButtonStringIndex = i;

            (MainForm as StartForm).showEditTranslationMenu();
        }

        public void EditStringDialog(int i)
        {
            DialogEditString des = new DialogEditString(i);

            if (des.ShowDialog() == DialogResult.OK)
            {
                GPStrings.getSharedStrings().setString(i, des.getNewText());
                GPStrings.getSharedStrings().Modified = true;
                LoadPage(CurrentPage.Name, false);
            }
        }

        /// <summary>
        /// Loading definition of page. Here we can decide if we will build
        /// some html page based on definition in file, or we if we will
        /// show some special control for this type of page.
        /// </summary>
        /// <param name="pageFile">Name of page. There should be filel on disk with
        /// name either {name}.p or {name}.html
        /// </param>
        public void LoadPage(string pageId, bool bInsertHistory)
        {
            if (bInsertHistory)
            {
                while (pageHistoryIndex + 1 >= 0 && pageHistoryIndex + 1 < pageHistory.Count)
                {
                    pageHistory.RemoveAt(pageHistoryIndex + 1);
                }
                PageHistoryEntry pageEntry = new PageHistoryEntry();
                pageEntry.Page = pageId;
                pageHistory.Add(pageEntry);
                pageHistoryIndex = pageHistory.Count - 1;
            }

            clearTopButtons();
            GPStrings.pushRich(false);
            if (pageHistoryIndex > 0)
            {
                addTopButton("< " + GPStrings.getPlainString(238), "goBack", 238);
            }
            if (!pageId.Equals("mainmenu"))
            {
                addTopButton(GPStrings.getPlainString(1054), "mainmenu", 1054);
            }
            GPStrings.popRich();

            recalculateLayout();

            if (!Pages.ContainsKey(pageId))
            {
                CurrentPage = null;
                return;
            }

            string fileName;
            List<FlowCommand> fileInstructions = new List<FlowCommand>();
            CurrentPage = Pages[pageId];
            Debugger.Log(0,"","$$$ CurrentPage is now: " + CurrentPage.Name + "\n");
            string pageFile = CurrentPage.Source;

            // executing page commands
            foreach (FlowCommand cmd in CurrentPage.Commands)
            {
                executeFlowCommand(cmd);
            }

            recalculateLayout();

            // loading page from source file
            fileName = GetFilePath(string.Format("{0}.p", pageFile));
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string[] lp = ReadFlowLine(sr);
                    while (lp != null)
                    {
                        FlowCommand cmd = new FlowCommand(lp);
                        if (cmd.Command != null && cmd.Command.Length > 0)
                        {
                            fileInstructions.Add(cmd);
                        }
                        lp = ReadFlowLine(sr);
                    }
                }
            }
            else
            {
                LoadFile(string.Format("{0}.html", pageFile));
            }

            if (fileInstructions.Count == 0)
                return;

            bool handled = false;

            // here we should do something for showing new page
            // either build a new HTML document
            // or show some special control class
            if (pageFile.Equals("special_case_view"))
            {
                // add some code here for initialization of new controll class
                // which is specific to this page

                // mark as handled
                handled = true;
            }

            if (handled)
                return;

            // here check for class
            // class should be mentioned somewhere in the content of file
            string classOfFile = "";

            if (classOfFile.Equals("choice_case_view"))
            {
                // add some code here for initialization of new controll class
                // which is specific to this page

                // mark as handled
                handled = true;
            }

            if (handled)
                return;

            // here do some general stuff
            // converting page intructions into HTML code
            // and load HTML text into webView
            HtmlBuildInstructions builder = new HtmlBuildInstructions();
            builder.contentServer = this;
            builder.Build(fileInstructions);

            currentFile = fileName;
            ModifyFileVariables(builder.Builder);
            WebBrowser.DocumentText = builder.getHtmlText();
        }

        public string BuildHtmlFromInstructions(List<FlowCommand> pageInstructions)
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();
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

            Debugger.Log(0, "", "----------------------------------------\n");
            Debugger.Log(0, "", sb.ToString());
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
                if (parts[0] == "version")
                    return GPAppHelper.getShortVersionText();
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
                    sb.AppendFormat("{0}<r>{1}", ev.nSpec, ev.getText());
                }
            }
            return sb.ToString();
        }

        public string getTithiName(int i)
        {
            return GPTithi.getFullName(i);
        }

        public string getNaksatraName(int i)
        {
            return GPNaksatra.getName(i);
        }

        public string getYogaName(int i)
        {
            return GPYoga.getName(i);
        }

        public string getSankrantiName(int i)
        {
            return GPSankranti.getName(i);
        }

        public string getMasaName(int i)
        {
            return GPMasa.GetName(i);
        }

        public string getMasaAbr(int i)
        {
            return GPAppHelper.getMonthAbr(i);
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
                    //return string.Format("<span style='color:red;font-size:9pt;'>STR-{0}</span> {1}", idx, GPStrings.getSharedStrings().getString(idx));
                    return GPStrings.getString(idx);
                }
                return string.Empty;
            }
            else if (p1.Equals("calculate"))
            {
                if (p2 == "calendar")
                {
                    CELGenerateCalendar gc = new CELGenerateCalendar(this);
                    CurrentCalculatedObject = gc.CalculatedObject;
                    return gc.HtmlText;
                }
                else if (p2 == "coreevents")
                {
                    CELGenerateCoreEvents ge = new CELGenerateCoreEvents(this);
                    CurrentCalculatedObject = ge.CalculatedObject;
                    return ge.HtmlText;
                }
                else if (p2 == "appday")
                {
                    CELGenerateAppearanceDay ga = new CELGenerateAppearanceDay(this);
                    CurrentCalculatedObject = ga.CalculatedObject;
                    return ga.HtmlText;
                }
                else if (p2 == "masalist")
                {
                    CELGenerateMasaList gm = new CELGenerateMasaList(this);
                    CurrentCalculatedObject = gm.CalculatedObject;
                    return gm.HtmlText;
                }
                else if (p2 == "calcore")
                {
                    CELGenerateCalendarPlusCore gcc = new CELGenerateCalendarPlusCore(this);
                    CurrentCalculatedObject = gcc.CalculatedObject;
                    return gcc.HtmlText;
                }
                else if (p2 == "cal2locs")
                {
                    CELGenerateCalendarTwoLocs gcc = new CELGenerateCalendarTwoLocs(this);
                    CurrentCalculatedObject = gcc.CalculatedObject;
                    return gcc.HtmlText;
                }
                else if (p2 == "travel")
                {
                    CELGenerateCalendarTravelling gcc = new CELGenerateCalendarTravelling(this);
                    CurrentCalculatedObject = gcc.CalculatedObject;
                    return gcc.HtmlText;
                }
                else if (p2 == "today")
                {
                    GPLocationProvider loc = GPAppHelper.getMyLocation();

                    if (myDate == null)
                        resetToday();

                    StringBuilder sb = new StringBuilder();
                    FormaterHtml.WriteTodayInfoHTML(myDate, loc, sb, 10, ContentDir);
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

        public string evaluate3params(string p1, string p2, string p3)
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
            else if (p1.Equals("dstr"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                GPVedicTime va;
                GPEngine.VCTIMEtoVATIME(gt, out va, gt.getLocationProvider());
                int num = 0;
                int.TryParse(p3, out num);
                if (p2.Equals("month"))
                {
                    gt.AddMonths(num);
                    return gt.getMonthYearString();
                }
                else if (p2.Equals("year"))
                {
                    gt.AddYears(num);
                    return gt.getYear().ToString();
                }
                else if (p2.Equals("masa"))
                {
                    va.masa = (va.masa + num)%12;
                    return String.Format("{0} {1}", GPMasa.GetName(va.masa), GPStrings.getString(22));
                }
                else if (p2.Equals("gyear"))
                {
                    va.gyear += num;
                    return String.Format("{0} {1}", GPStrings.getString(23), va.gyear);
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

        public void goPage(string pageId)
        {
            LoadPage(pageId, true);
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

        private void loadEventFromParameters()
        {
            GPEvent ev = GPEventList.getShared().find(getInt("eventid"));
            if (ev != null)
            {
                if (ev is GPEventTithi)
                {
                    saveInt("eventtithi", ((GPEventTithi)ev).nTithi);
                    saveInt("eventmasa", ((GPEventTithi)ev).nMasa);
                    saveInt("eventtype", 0);
                }
                else if (ev is GPEventSankranti)
                {
                    saveInt("eventsankranti", ((GPEventSankranti)ev).nSankranti);
                    saveInt("eventtype", 1);
                }
                else if (ev is GPEventRelative)
                {
                    saveInt("eventeventref", ((GPEventRelative)ev).nSpecRef);
                    saveInt("eventtype", 2);
                }
                else if (ev is GPEventNaksatra)
                {
                    saveInt("eventnaksatra", ((GPEventNaksatra)ev).nNaksatra);
                    saveInt("eventtype", 3);
                }
                else if (ev is GPEventAstro)
                {
                    saveInt("eventastro1", ((GPEventAstro)ev).nAstroType);
                    saveInt("eventdata1", ((GPEventAstro)ev).nData);
                    saveInt("eventtype", 5);
                }
                else if (ev is GPEventYoga)
                {
                    saveInt("eventyoga", ((GPEventYoga)ev).nYoga);
                    saveInt("eventtype", 4);
                }
                saveInt("eventclass", ev.nClass);
                saveInt("eventoffset1", ev.nOffset);
                saveInt("eventoffset2", ev.nOffset);

                GPStrings.pushRich(false);
                saveString("eventtitle", GPString.htmlToPlain(ev.getText()));
                saveString("eventfastsubject", GPString.htmlToPlain(ev.getFastSubject()));
                GPStrings.popRich();

                saveString("eventtitleraw", ev.getRawText());
                saveString("eventfastsubjectraw", ev.getRawFastSubject());
                saveInt("eventtitleid", ev.textStringId);
                saveInt("eventfastsubjectid", ev.fastSubjectStringId);
                saveInt("eventsinceyear", ev.nStartYear);
                saveInt("eventvisibility", ev.nVisible);
                saveInt("eventfasttype", ev.getRawFastType());
                saveInt("eventspec", ev.nSpec);
                saveInt("eventused", ev.nUsed);
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
            else if (evType == 3)
            {
                GPEventNaksatra evn = new GPEventNaksatra();
                evn.nNaksatra = getInt("eventnaksatra");
                ev = evn;
            }
            else if (evType == 4)
            {
                GPEventYoga evy = new GPEventYoga();
                evy.nYoga = getInt("eventyoga");
                ev = evy;
            }
            else if (evType == 5)
            {
                GPEventAstro eva = new GPEventAstro();
                eva.nAstroType = getInt("eventastro1");
                eva.nData = getInt("eventdata1");
                ev = eva;
            }
            ev.fastSubjectStringId = getInt("eventfastsubjectid");
            ev.textStringId = getInt("eventtitleid");
            ev.setText(GPString.plainToHtml(getString("eventtitle")));
            ev.setFastSubject(GPString.plainToHtml(getString("eventfastsubject")));
            ev.setRawText(getString("eventtitleraw"));
            ev.setRawFastSubject(getString("eventfastsubjectraw"));
            ev.nClass = getInt("eventclass");
            ev.nStartYear = getInt("eventsinceyear");
            ev.nUsed = 1;
            ev.nVisible = getInt("eventvisibility");
            ev.setRawFastType(getInt("eventfasttype"));
            ev.eventId = GPEventList.getNextID();

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
            else if (cmd.Equals("saveRecentLocation"))
            {
                string ppx = String.Empty;
                if (dictStrings.ContainsKey("ppx"))
                {
                    ppx = dictStrings["ppx"];
                }
                GPLocationProvider locProv = getLocationWithPostfix(ppx);
                GPLocationProvider.putRecent(locProv);
            }
            else if (cmd.Equals("clearlocationdata"))
            {
                saveString("locationname", null);
                saveString("locationcountrycode", null);
                saveString("locationcountry", null);
                saveString("locationlatitude", null);
                saveString("locationlongitude", null);
                saveString("locationtimezone", null);
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
                if (ev != null && ev.nUsed > 0)
                {
                    GPEventList.getShared().RemoveEvent(ev);
                }
            }
            else if (cmd.Equals("loadeventid"))
            {
                loadEventFromParameters();
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
                saveInt("eventfasttype", 0);
                saveString("eventtitle", "New Event");
                saveString("eventtitleraw", "New Event");
                saveString("eventfastsubject", "");
                saveString("eventfastsubjectraw", "");
                saveInt("eventtitleid", -1);
                saveInt("eventfastsubjectid", -1);
                saveInt("eventsinceyear", -10000);
                saveInt("eventnaksatra", 0);
                saveInt("eventastro1", 0);
                saveInt("eventvisibility", 1);
            }
            else if (cmd.Equals("savetzone"))
            {
                string tzdata = getString("tzdata");
                GPTimeZone ntz = new GPTimeZone();
                ntz.Id = GPTimeZoneList.sharedTimeZones().getNextId();
                ntz.initWithData(tzdata);
                saveInt("timezoneid", ntz.Id);
                GPTimeZoneList.sharedTimeZones().addTimezone(ntz);
            }
            else if (cmd.Equals("loadtzone"))
            {
                GPTimeZone tz = GPTimeZoneList.sharedTimeZones().GetTimezoneById(getInt("timezoneid"));
                if (tz != null)
                {
                    saveString("tzdata", tz.getStringData());
                    saveString("timezonename", tz.Name);
                    saveString("timezoneoffset", tz.getOffsetString());
                    saveInt("tzusedcount", GPLocationList.getShared().GetLocationCountForTimezone(tz.Name));
                }
            }
            else if (cmd.Equals("updatetzone"))
            {
                GPTimeZone tz = GPTimeZoneList.sharedTimeZones().GetTimezoneById(getInt("timezoneid"));
                if (tz != null)
                {
                    tz.initWithData(getString("tzdata"));
                }
            }
            else if (cmd.Equals("deltzone"))
            {
                GPTimeZoneList.sharedTimeZones().DeleteTimezone(getInt("timezoneid"));
            }
            else if (cmd.Equals("mainmenu"))
            {
                goPage(cmd);
            }
            else if (cmd.Equals("goBack"))
            {
                if (pageHistoryIndex > 0)
                {
                    runAction("onBack");
                    pageHistoryIndex--;
                    PageHistoryEntry phe = pageHistory[pageHistoryIndex];
                    LoadPage(phe.Page, false);
                }
            }
            else if (cmd.StartsWith("action:"))
            {
                runAction(cmd.Substring(7));
            }
            else if (cmd.Equals("saveContent"))
            {
                saveContent();
            }
            else if (cmd.Equals("printContent"))
            {
                ((StartForm)MainForm).showPrintMenu();
                //WebBrowser.ShowPrintDialog();
            }
            else if (cmd.Equals("settings"))
            {
            }
            else if (cmd.Equals("setCurrentDate"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                dictStrings["startday"] = gt.getDay().ToString();
                dictStrings["startmonth"] = gt.getMonth().ToString();
                dictStrings["startyear"] = gt.getYear().ToString();
            }
            else if (cmd.Equals("gotoNextMonth"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                gt.setDate(getInt("startyear", gt.getYear()), getInt("startmonth", gt.getMonth()), getInt("startday", gt.getDay()));
                gt.AddMonths(1);
                dictStrings["startday"] = gt.getDay().ToString();
                dictStrings["startmonth"] = gt.getMonth().ToString();
                dictStrings["startyear"] = gt.getYear().ToString();
            }
            else if (cmd.Equals("gotoNextYear"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                gt.setDate(getInt("startyear", gt.getYear()), getInt("startmonth", gt.getMonth()), getInt("startday", gt.getDay()));
                gt.AddYears(1);
                dictStrings["startday"] = gt.getDay().ToString();
                dictStrings["startmonth"] = gt.getMonth().ToString();
                dictStrings["startyear"] = gt.getYear().ToString();
            }
            else if (cmd.Equals("setCurrentVedicDate"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                GPVedicTime va;
                GPEngine.VCTIMEtoVATIME(gt, out va, gt.getLocationProvider());
                dictStrings["starttithi"] = va.tithi.ToString();
                dictStrings["startmasa"] = va.masa.ToString();
                dictStrings["startgaurabda"] = va.gyear.ToString();
            }
            else if (cmd.Equals("vedicDateToGregorian"))
            {
                GPLocationProvider currentLocation = getLocationWithPostfix(dictStrings["ppx"]);
                GPVedicTime va = new GPVedicTime();
                va.tithi = getInt("starttithi", 0);
                va.masa = getInt("startmasa", 11);
                va.gyear = getInt("startgaurabda", 530);
                GPGregorianTime gt = new GPGregorianTime(currentLocation);
                GPEngine.VATIMEtoVCTIME(va, out gt, gt.getLocationProvider());
                dictStrings["startday"] = gt.getDay().ToString();
                dictStrings["startmonth"] = gt.getMonth().ToString();
                dictStrings["startyear"] = gt.getYear().ToString();
            }
            else if (cmd.Equals("moveOneMasa"))
            {
                GPVedicTime va = new GPVedicTime();
                va.tithi = getInt("starttithi", 0);
                va.masa = getInt("startmasa", 11);
                va.gyear = getInt("startgaurabda", 530);
                if (va.masa == 10)
                {
                    va.masa = 11;
                    va.gyear++;
                }
                else
                {
                    va.masa = (va.masa + 1) % 12;
                }
                dictStrings["startmasa"] = va.masa.ToString();
                dictStrings["startgaurabda"] = va.gyear.ToString();
            }
            else if (cmd.Equals("moveOneGaurabda"))
            {
                GPVedicTime va = new GPVedicTime();
                va.tithi = getInt("starttithi", 0);
                va.masa = getInt("startmasa", 11);
                va.gyear = getInt("startgaurabda", 530);
                va.gyear++;
                dictStrings["startgaurabda"] = va.gyear.ToString();
            }
            else if (cmd.StartsWith("today:"))
            {
                string specific = cmd.Substring(6);
                if (specific.Equals("prev"))
                {
                    todayGoPrev();
                }
                else if (specific.Equals("today"))
                {
                    todayToday();
                }
                else if (specific.Equals("next"))
                {
                    todayGoNext();
                }
            }
        }



        public int getTimezoneUsage(int tzoneid)
        {
            int count = 0;
            GPTimeZone tz = GPTimeZoneList.sharedTimeZones().GetTimezoneById(tzoneid);
            if (tz != null)
            {
                foreach (GPLocation loc in GPLocationList.getShared().locations)
                {
                    if (loc.getTimeZoneName().Equals(tz.Name))
                        count++;
                }
            }

            return count;
        }

        public string getEventsByName(string name)
        {
            List<GPEvent> events = new List<GPEvent>();
            GPEventList list = GPEventList.getShared();
            if (name != null && name.Length > 0)
            {
                foreach (GPEventTithi et in list.tithiEvents)
                {
                    if (et.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(et);
                    }
                }
                foreach (GPEventSankranti es in list.sankrantiEvents)
                {
                    if (es.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(es);
                    }
                }
                foreach (GPEventRelative er in list.relativeEvents)
                {
                    if (er.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(er);
                    }
                }
                foreach (GPEventNaksatra er in list.naksatraEvents)
                {
                    if (er.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(er);
                    }
                }
                foreach (GPEventAstro er in list.astroEvents)
                {
                    if (er.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        events.Add(er);
                    }
                }
                foreach (GPEventYoga er in list.yogaEvents)
                {
                    if (er.getText().IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
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
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.getText(), ev.getShortDesc());
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
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.getText(), ev.getShortDesc());
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
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.getText(), ev.getShortDesc());
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
            foreach (GPEventNaksatra er in list.naksatraEvents)
            {
                if (er.nClass == classId)
                {
                    events.Add(er);
                }
            }
            foreach (GPEventAstro er in list.astroEvents)
            {
                if (er.nClass == classId)
                {
                    events.Add(er);
                }
            }
            foreach (GPEventYoga er in list.yogaEvents)
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
                sb.AppendFormat("{0}<r>{1}<r>{2}", ev.eventId, ev.getText(), ev.getShortDesc());
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

        public string gstr(int i)
        {
            return GPStrings.getString(i);
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

        public int getInt(string key, int defaultValue)
        {
            int i = defaultValue;
            if (dictStrings.ContainsKey(key))
            {
                if (!int.TryParse(dictStrings[key], out i))
                    i = defaultValue;
            }
            return i;
        }
        public void saveString(string key, string value)
        {
            if (dictStrings.ContainsKey(key))
            {
                if (value == null)
                    dictStrings.Remove(key);
                else
                    dictStrings[key] = value;
            }
            else
            {
                if (value != null)
                    dictStrings.Add(key, value);
            }
            Debugger.Log(0,"","saveString(" + key + "," + value + ")\n");
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

        public GPLocation getLocation(int i)
        {
            if (i >= 0 && i < locationsList.Count)
            {
                return locationsList[i];
            }
            return null;
        }

        public void findStrings(string s)
        {
            stringsList = new List<GPString>();
            if (s == null)
                s = string.Empty;
            GPStrings gstr = GPStrings.getSharedStrings();
            int startIndex = 0;
            int.TryParse(s, out startIndex);
            bool indexDefined = (startIndex > 0 || s.Equals("0"));

            for (int i = startIndex; i < gstr.gstr.Count; i++ )
            {
                String loc = gstr.getStringValue(i);
                if (indexDefined || s.Length == 0 || loc.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    GPString gs = new GPString();
                    gs.index = i;
                    gs.rawHtml = loc;
                    stringsList.Add(gs);
                    if (stringsList.Count > 100)
                        break;
                }
            }
            stringsEnumerator = 0;
        }
        
        public string getNextString()
        {
            if (stringsEnumerator >= 0 && stringsEnumerator < stringsList.Count)
            {
                GPString loc = stringsList[stringsEnumerator];
                stringsEnumerator++;
                return string.Format("{0}<br>{1}", loc.index, loc.rawHtml);
            }

            return string.Empty;
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

        public string getRecentLocationsCount()
        {
            return GPLocationProvider.getRecent().Count.ToString();
        }

        public string getRecentLocation(int i)
        {
            GPLocationProvider lp = GPLocationProvider.getRecent()[i];
            return String.Format("{0}<tr>{1}<tr>{2}<tr>{3}", lp.getType(), lp.getName(), lp.getLocationDescription(), i);
        }

        public string getTimezonesByName(string partName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (GPTimeZone tz in GPTimeZoneList.sharedTimeZones().getTimeZones())
            {
                if (partName != null && tz.Name.IndexOf(partName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    if (sb.Length > 0)
                        sb.Append("<line>");
                    sb.AppendFormat("{0}<r>{1}<r>{2}", tz.Id, tz.Name, tz.getFullName());
                }
            }
            return sb.ToString();
        }

        public string getTimezonesByOffset(string off)
        {
            int i;
            int.TryParse(off, out i);
            StringBuilder sb = new StringBuilder();
            foreach (GPTimeZone tz in GPTimeZoneList.sharedTimeZones().getTimeZones())
            {
                if (tz.OffsetSeconds == i)
                {
                    if (sb.Length > 0)
                        sb.Append("<line>");
                    sb.AppendFormat("{0}<r>{1}<r>{2}", tz.Id, tz.Name, tz.getFullName());
                }
            }
            return sb.ToString();
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
            GPLocationProvider locProv = null;
            string type = getString("locationtype" + postfix);
            if (type == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(getInt("locationid" + postfix));
                if (loc != null)
                {
                    locProv = new GPLocationProvider(loc);
                    locProv.setType(GPLocationProvider.TYPE_SELECTED);
                }
            }

            if (type == "entered")
            {
                GPLocation loc = new GPLocation();
                loc.setCity(getString("locationname" + postfix));
                loc.setLongitudeString(getString("locationlongitude" + postfix));
                loc.setLatitudeString(getString("locationlatitude" + postfix));
                loc.setTimeZoneName(getString("locationtimezone" + postfix));
                locProv = new GPLocationProvider(loc);
                locProv.setType(GPLocationProvider.TYPE_FULLENTER);
            }

            if (type == "mylocation")
            {
                locProv = GPAppHelper.getMyLocation();
                locProv.setType(GPLocationProvider.TYPE_MYLOCATION);
            }

            if (type == "recent")
            {
                string idxs = getString("recentIndex" + postfix);
                int idx;
                if (int.TryParse(idxs, out idx))
                {
                    if (idx >= 0 && idx < GPLocationProvider.getRecent().Count)
                    {
                        locProv = GPLocationProvider.getRecent()[idx];
                    }
                }
            }

            return locProv;
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
            GPLocationList.getShared().Modified = true;
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
            GPCountryList.getShared().Modified = true;
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
                    GPCountryList.getShared().Modified = true;
                }
            }
        }


        public void specialCommand(string str)
        {
            if (str.Equals("#calcm;"))
            {
                TestForm form = new TestForm();
                form.content = this;
                form.Show();
            }
        }

        public void updateLanguageList()
        {
            updateLanguageListTask = new CELUpdateLanguageList();
            updateLanguageListTask.Invoke();
        }

        public string getUpdateLanguageListStatus()
        {
            if (updateLanguageListTask != null)
            {
                return updateLanguageListTask.getMessage();
            }
            return String.Empty;
        }

        public void sendMyLanguageFile()
        {
            sendMyLanguageFileTask = new CELSendMyLanguageFile();
            sendMyLanguageFileTask.Invoke();
        }

        public string getSendMyLanguageFileStatus()
        {
            if (sendMyLanguageFileTask != null)
            {
                return sendMyLanguageFileTask.getMessage();
            }
            return string.Empty;
        }

        public void searchResultString(string str)
        {
            if (str == null)
                return;
            if (str.StartsWith("#"))
            {
                if (str.EndsWith(";"))
                {
                    specialCommand(str);
                }
                return;
            }

            if (searchTask != null)
                searchTask.ShouldCancel = true;

            searchTask = new CELSearch(str);
            searchTask.Location = GPAppHelper.getMyLocation();
            searchTask.Invoke();
        }

        public int isSearchFinished()
        {
            return (searchTask != null && searchTask.finished) ? 1 : 0;
        }

        public int isSearching()
        {
            return searchTask != null ? 1 : 0;
        }

        public int searchResultsCount()
        {
            if (searchTask != null)
                return searchTask.ResultsList.Count;
            return 0;
        }

        public string getSearchResult(int i)
        {
            if (searchTask != null && searchTask.ResultsList.Count > i)
                return searchTask.ResultsList[i].getDataString();
            return "";
        }

        public void setMyDate(int y, int m, int d)
        {
            if (myDate == null)
                resetToday();
            myDate.setDate(y, m, d);
        }

        public string nextTip()
        {
            string s = GPAppHelper.NextStartupTip();
            return s != null ? s : "<i>" + GPStrings.getString(1195) + "</i>";
        }

        public int getLanguagesCount()
        {
            return GPLanguageList.getShared().languages.Count;
        }

        public string getLanguageName(int i)
        {
            List<GPLanguage> ll = GPLanguageList.getShared().languages;
            return ll[i].LanguageId.ToString() + "<r>" + ll[i].LanguageName;
        }

        public int getCurrentLanguageId()
        {
            return GPLanguageList.getShared().currentLanguageId;
        }

        public void setCurrentLanguageId(int id)
        {
            GPLanguageList.getShared().setCurrentLanguageId(id);
        }


        public void runAction(string s)
        {
            if (CurrentPage == null)
                return;

            FlowAction action = null;

            if (CurrentPage.Actions.ContainsKey(s))
            {
                action = CurrentPage.Actions[s];
            }

            if (action == null)
                return;

            foreach (FlowCommand command in action.Commands)
            {
                // executing command
                executeFlowCommand(command);
            }
        }

        public void executeFlowCommand(FlowCommand cmd)
        {
            if (cmd.Command.Equals("goto"))
            {
                LoadPage(cmd.getArgSubst(0, dictStrings, true), true);
                return;
            }
            else if (cmd.Command.Equals("set"))
            {
                string var = cmd.getArg(0);
                if (var.StartsWith("$"))
                    var = var.Substring(1);
                dictStrings[var] = cmd.getArgSubst(1, dictStrings, true);
            }
            else if (cmd.Equals("button"))
            {
                if (cmd.getArg(0).Equals("top"))
                {
                    addTopButton(cmd.getArgSubst(1, dictStrings, true), cmd.getArgSubst(2, dictStrings, true), cmd.getArgInt(3));
                }
                else if (cmd.getArg(0).Equals("bottom"))
                {
                    addBottomButton(cmd.getArgSubst(1, dictStrings, true), cmd.getArgSubst(2, dictStrings, true), cmd.getArgInt(3));
                }
            }
            else if (cmd.Command.Equals("exec"))
            {
                ExecuteCommand(cmd.getArgSubst(0, dictStrings, true));
            }
            else if (cmd.Command.Equals("script"))
            {
                WebBrowser.Document.InvokeScript(cmd.getArg(0));
            }
            else if (cmd.Command.Equals("if"))
            {
                if (evaluateConditionExpression(cmd.getArgSubst(0, dictStrings, true), cmd.getArgSubst(1, dictStrings, true), cmd.getArgSubst(2, dictStrings, true)))
                {
                    foreach(FlowCommand fc in cmd.Commands)
                    {
                        executeFlowCommand(fc);
                    }
                }
            }
        }

        public bool evaluateConditionExpression(string arg1, string oper, string arg2)
        {
            switch(oper)
            {
                case "==":
                    return safeInt(arg1) == safeInt(arg2);
                case "!=":
                    return safeInt(arg1) != safeInt(arg2);
                case ">":
                    return safeInt(arg1) > safeInt(arg2);
                case "<":
                    return safeInt(arg1) < safeInt(arg2);
                case ">=":
                    return safeInt(arg1) >= safeInt(arg2);
                case "<=":
                    return safeInt(arg1) <= safeInt(arg2);
                case "eq":
                    return arg1.Equals(arg2);
                case "ne":
                    return !arg1.Equals(arg2);
                default:
                    break;
            }

            return false;
        }

        public int safeInt(string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return 0;
        }

        public void saveContent()
        {
            if (CurrentCalculatedObject is GPCalendarResults)
            {
                OnSaveCalendar();
            }
            else if (CurrentCalculatedObject is GPCoreEventResults)
            {
                OnSaveEvents();
            }
            else if (CurrentCalculatedObject is GPAppDayResults)
            {
                OnSaveAppday();
            }
            else if (CurrentCalculatedObject is GPMasaListResults)
            {
                OnSaveMasaList();
            }
            else if (CurrentCalculatedObject is GPCalendarPlusEventsResults)
            {
                OnSaveCalendarPlusEvents();
            }
            else if (CurrentCalculatedObject is GPCalendarTwoLocResults)
            {
                OnSaveTwoCalendars();
            }
        }



        private void OnSaveCalendar()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText,
                FileFormatType.HtmlText, FileFormatType.HtmlTable, FileFormatType.Csv,
                FileFormatType.Ical, FileFormatType.Vcal, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatCalendarPlain((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatCalendarRtf((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteCalendarHTML((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterHtml.WriteCalendarHtmlTable((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 5)
                    {
                        FormaterCSV.FormatCalendarCSV((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 6)
                    {
                        FormaterICAL.FormatCalendarICAL((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 7)
                    {
                        FormaterVCAL.FormatCalendarVCAL((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 8)
                    {
                        FormaterXml.WriteCalendarXml((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }

        private void OnSaveCalendarPlusEvents()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarPlusEventsResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatCalendarPlusCorePlain((CurrentCalculatedObject as GPCalendarPlusEventsResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterHtml.WriteCalendarPlusCoreHTML((CurrentCalculatedObject as GPCalendarPlusEventsResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }
        private void OnSaveEvents()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText,
                FileFormatType.HtmlText, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCoreEventResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatEventsText((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatEventsRtf((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteEventsHTML((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterXml.FormatEventsXML((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }
        private void OnSaveTwoCalendars()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarTwoLocResults)
                {
                    if (sfd.FilterIndex == 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        FormaterHtml.WriteCompareCalendarHTML((CurrentCalculatedObject as GPCalendarTwoLocResults), sb);
                        File.WriteAllText(sfd.FileName, sb.ToString());
                    }
                }
            }
        }
        private void OnSaveMasaList()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPMasaListResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatMasaListText((CurrentCalculatedObject as GPMasaListResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatMasaListRtf((CurrentCalculatedObject as GPMasaListResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteMasaListHTML((CurrentCalculatedObject as GPMasaListResults), sb);
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }

        private void OnSaveAppday()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, FileFormatType.HtmlText, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPAppDayResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatAppDayText((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatAppDayRtf((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteAppDayHTML((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterXml.FormatAppDayXML((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }

        public void clearTopButtons()
        {
            foreach (Button btn in TopButtons)
            {
                btn.Visible = false;
                btn.Tag = null;
            }

            foreach (Button btm in BottomButtons)
            {
                btm.Visible = false;
                btm.Tag = null;
            }
        }

        public int getTopButtonsCount()
        {
            int index = 0;
            for (int i = 0; i < TopButtons.Count; i++)
            {
                if (TopButtons[i].Tag != null)
                {
                    index++;
                }
            }
            return index;
        }

        public int getBottomButtonsCount()
        {
            int index = 0;
            foreach (Button b in BottomButtons)
            {
                if (b.Tag != null)
                {
                    index++;
                }
            }
            return index;
        }

        public void addTopButton(string buttonTitle, string buttonTag, int buttonStringIndex)
        {
            int index = -1;
            for (int i = 0; i < TopButtons.Count; i++)
            {
                if (TopButtons[i].Tag == null)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                Button btn = TopButtons[index];
                btn.Text = buttonTitle;
                ButtonCommandTag bct = new ButtonCommandTag();
                bct.Command = buttonTag;
                bct.StringIndex = buttonStringIndex;
                btn.Tag = bct;
                btn.Visible = true;
            }
        }
        
        public void addBottomButton(string buttonTitle, string buttonTag, int buttonStringIndex)
        {
            int index = -1;
            for (int i = 0; i < BottomButtons.Count; i++)
            {
                if (BottomButtons[i].Tag == null)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                Button btn = BottomButtons[index];
                btn.Text = buttonTitle;
                ButtonCommandTag bct = new ButtonCommandTag();
                bct.Command = buttonTag;
                bct.StringIndex = buttonStringIndex;
                btn.Tag = bct;
                btn.Visible = true;
            }
        }
        public void recalculateLayout()
        {
            int top = getTopButtonsCount();
            int bottom = getBottomButtonsCount();

            Rectangle rect = MainForm.ClientRectangle;
            Point origin = new Point(rect.X, rect.Y);
            Size size = new Size(rect.Width, rect.Height);

            if (TopBar != null)
                TopBar.Visible = (top > 0);
            if (BottomBar != null)
                BottomBar.Visible = (bottom > 0);

            if (top > 0)
            {
                origin.Y += 34;
                size.Height -= 34;
            }

            if (bottom > 0)
            {
                size.Height -= 34;
            }

            WebBrowser.Location = origin;
            WebBrowser.Size = size;

        }
    }
}
