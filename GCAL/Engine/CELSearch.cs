using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using GCAL.Base;
using GCAL.Controls;

namespace GCAL.Engine
{
    public class CELSearch : CELBase
    {
        public class Results
        {
            public CELSearch Parent = null; 
            public string Title = string.Empty;
            public string Type = string.Empty;
            public string SearchText = string.Empty;
            public string ActionScript = string.Empty;
            public List<ResultsLine> Lines = new List<ResultsLine>();
            public GPCalculationOperation Operation = GPCalculationOperation.None;
            public Dictionary<GPCalculationParameters, object> Parameters = new Dictionary<GPCalculationParameters,object>();

            public Results(CELSearch par, string st)
            {
                Parent = par;
                SearchText = st;
            }

            public void ScanText(string term, StringBuilder sb)
            {
                int found;
                int termLength = term.Length;
                string s = sb.ToString();
                using (StringReader sr = new StringReader(s))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        if (Parent.ShouldCancel)
                        {
                            return;
                        } 
                        found = line.IndexOf(term, 0, StringComparison.CurrentCultureIgnoreCase);
                        if (found >= 0)
                        {
                            ResultsLine rline = new ResultsLine();
                            int start = Math.Max(found - 30, 0);
                            rline.Prefix = line.Substring(start, found - start);
                            rline.Term = line.Substring(found, termLength);
                            rline.Postfix = line.Substring(found + termLength, Math.Min(line.Length - termLength - found, found + termLength + 20));
                            Lines.Add(rline);
                        }
                        line = sr.ReadLine();
                    }
                }
            }

            internal string getDataString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Title);
                sb.Append("<part>");
                sb.Append(Type);
                sb.Append("<part>");
                foreach (ResultsLine rl in Lines)
                {
                    sb.AppendFormat("{0}<r>{1}<r>{2}<tr>", rl.Prefix, rl.Term, rl.Postfix);
                }
                sb.Append("<part>");
                sb.Append(ActionScript);

                return sb.ToString();
            }
        }

        public class ResultsLine
        {
            public string Prefix = string.Empty;
            public string Term = string.Empty;
            public string Postfix = string.Empty;
        }

        public GPLocationProvider Location = null;
        public bool ShouldCancel = false;
        public SearchResultsList listControl = null;
        private string p_text = string.Empty;
        public List<Results> ResultsList = new List<Results>();
        public bool finished = false;


        public CELSearch(string txt)
        {
            p_text = txt;
        }

        public string Text
        {
            get
            {
                return p_text;
            }
        }

        protected override void Execute()
        {
            ResultsList.Clear();
            //ClearControlList();
            int limit;
            if (Location == null || p_text == null || p_text.Length == 0)
            {
                return;
            }
            // find in texts

            StringBuilder sb = new StringBuilder();
            Results res = new Results(this, p_text);
            GPGregorianTime vc = new GPGregorianTime(Location);
            vc.Today();

            //
            // today results (this day, tomorrow and day after
            //
            #region today screen results
            limit = GPUserDefaults.IntForKey("search.today.days", 3);
            for (int i = 0; i < limit; i++)
            {
                sb.Remove(0, sb.Length);
                FormaterPlain.AvcGetTodayInfo(vc, Location, sb);
                res.Title = GPAppHelper.getDateText(vc);
                res.Type = GPStrings.getString(174);
                res.ScanText(p_text, sb);
                if (res.Lines.Count > 0)
                {
                    res.Operation = GPCalculationOperation.Today;
                    res.Parameters.Add(GPCalculationParameters.LocationProvider, Location);
                    res.Parameters.Add(GPCalculationParameters.StartWesternDate, vc.Copy());
                    res.ActionScript += string.Format("scriptObject.setMyDate({0},{1},{2});", vc.getYear(), vc.getMonth(), vc.getDay());
                    res.ActionScript += "window.location.href='today.html'";
                    ResultsList.Add(res);
                    res = new Results(this, p_text);
                }
                vc.NextDay();
            }

            // move results to control
            if (!ShouldCancel)
                FlushResultsToControl();

            #endregion

            #region calendar results
            limit = GPUserDefaults.IntForKey("search.calendar.months", 12);
            GPCalendarResults rcal = new GPCalendarResults();

            vc.Today();
            vc.AddDays(1 - vc.getDay());
            for (int i = 0; i < limit; i++)
            {
                rcal.CalculateCalendar(vc, GPGregorianTime.GetMonthMaxDays(vc.getYear(), vc.getMonth()));
                FormaterPlain.FormatCalendarOld(rcal, sb);
                res.Title = string.Format("{0} {1}", GPStrings.getString(759 + vc.getMonth()), vc.getYear());
                res.Type = GPStrings.getString(44);
                res.ScanText(p_text, sb);
                if (res.Lines.Count > 0)
                {
                    res.Operation = GPCalculationOperation.Calendar;
                    res.Parameters.Add(GPCalculationParameters.LocationProvider, Location);
                    res.Parameters.Add(GPCalculationParameters.StartWesternDate, vc.Copy());
                    GPGregorianTime vc2 = vc.Copy();
                    vc2.AddMonths(1);
                    res.Parameters.Add(GPCalculationParameters.EndWesternDate, vc2);
                    res.ActionScript += "saveString('locationtype', 'mylocation');";
                    res.ActionScript += "saveString('startyear', '" + vc.getYear() + "');";
                    res.ActionScript += "saveString('startmonth', '" + vc.getMonth() + "');";
                    res.ActionScript += "saveString('startday', '" + vc.getDay() + "');";
                    res.ActionScript += "saveString('endperiodtype', '3');";
                    res.ActionScript += "saveString('endperiodlength', '1');";
                    res.ActionScript += "window.location.href='calendar.html'";
                    ResultsList.Add(res);
                    res = new Results(this, p_text);
                }
                vc.AddMonths(1);
            }

            // move results to control
            if (!ShouldCancel)
                FlushResultsToControl();

            #endregion

            #region core events results
            limit = GPUserDefaults.IntForKey("search.coreevents.months", 1);
            GPCoreEventResults reve = new GPCoreEventResults();
            vc.Today();
            vc.AddDays(1 - vc.getDay());
            for (int i = 0; i < limit; i++)
            {
                sb.Remove(0, sb.Length);
                GPGregorianTime vcEnd = vc.Copy();
                vcEnd.AddDays(31);
                reve.CalculateEvents(Location, vc, vcEnd);
                FormaterPlain.FormatEventsText(reve, sb);
                res.Title = string.Format("{0} {1}", GPStrings.getString(759 + vc.getMonth()), vc.getYear());
                res.Type = GPStrings.getString(46);
                res.ScanText(p_text, sb);
                if (res.Lines.Count > 0)
                {
                    res.Operation = GPCalculationOperation.CoreEvents;
                    res.Parameters.Add(GPCalculationParameters.LocationProvider, Location);
                    res.Parameters.Add(GPCalculationParameters.StartWesternDate, vc.Copy());
                    res.Parameters.Add(GPCalculationParameters.EndWesternDate, vcEnd);
                    res.ActionScript += "saveString('locationtype', 'mylocation');";
                    res.ActionScript += "saveString('startyear', '" + vc.getYear() + "');";
                    res.ActionScript += "saveString('startmonth', '" + vc.getMonth() + "');";
                    res.ActionScript += "saveString('startday', '" + vc.getDay() + "');";
                    res.ActionScript += "saveString('endperiodtype', '3');";
                    res.ActionScript += "saveString('endperiodlength', '1');";
                    res.ActionScript += "window.location.href='coreevents.html'";
                    ResultsList.Add(res);
                    res = new Results(this, p_text);
                }
                vc.AddMonths(1);
            }

            // move results to control
            if (!ShouldCancel)
                FlushResultsToControl();

            #endregion

            #region masa list
            limit = GPUserDefaults.IntForKey("search.masalist.years", 3);
            GPMasaListResults rmas = new GPMasaListResults();
            vc.Today();
            for (int i = 0; i < limit; i++)
            {
                sb.Remove(0, sb.Length);
                rmas.CalcMasaList(Location, vc.getYear(), 1);
                FormaterPlain.FormatMasaListText(rmas, sb);
                res.Title = string.Format("{0}", vc.getYear());
                res.Type = GPStrings.getString(48);
                res.ScanText(p_text, sb);
                if (res.Lines.Count > 0)
                {
                    res.Operation = GPCalculationOperation.MasaList;
                    res.Parameters.Add(GPCalculationParameters.LocationProvider, Location);
                    res.Parameters.Add(GPCalculationParameters.StartYear, vc.getYear());
                    res.Parameters.Add(GPCalculationParameters.CountYear, 1);
                    res.ActionScript += "saveString('locationtype', 'mylocation');";
                    res.ActionScript += "saveString('startyear', '" + vc.getYear() + "');";
                    res.ActionScript += "saveString('yearcount', '1');";
                    res.ActionScript += "window.location.href='masalist.html'";
                    ResultsList.Add(res);
                    res = new Results(this, p_text);
                }
                vc.AddYears(1);
            }

            // move results to control
            if (!ShouldCancel)
                FlushResultsToControl();

            #endregion

            finished = true;
        }

        /// <summary>
        /// thread safe call
        /// </summary>
        public void FlushResultsToControl()
        {
            //listControl.Invoke(new CELAsyncResultReceiverTaskMethod(FlushResultsToControlSync), this);
        }

        /// <summary>
        /// thread safe call
        /// </summary>
        public void ClearControlList()
        {
            //listControl.Invoke(new CELAsyncResultReceiverTaskMethod(ClearControlListSync), this);
        }

        /// <summary>
        /// needs to be executed on thread of Control
        /// </summary>
        /// <param name="task"></param>
        public void ClearControlListSync(CELBase task)
        {
            //listControl.Items.Clear();
        }

        /// <summary>
        /// needs to be executed on thread of Control
        /// </summary>
        /// <param name="task"></param>
        public void FlushResultsToControlSync(CELBase task)
        {
            /*listControl.BeginUpdate();

            foreach (Results res in ResultsList)
            {
                listControl.Items.Add(res);
            }
            ResultsList.Clear();
            listControl.EndUpdate();*/
        }

    }
}
