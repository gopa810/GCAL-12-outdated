using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPTimeZone
    {
        /// <summary>
        /// Rule specification for one day (start or end day)
        /// of period with Daylight Saving Time
        /// </summary>
        public class RuleSpec
        {
            public int Month;
            public int WeekOfMonth;
            public int DayOfWeek;
            public int Hour;

            public static int IntegerFromWeekday(System.DayOfWeek dow)
            {
                if (dow == System.DayOfWeek.Friday) return 5;
                if (dow == System.DayOfWeek.Monday) return 1;
                if (dow == System.DayOfWeek.Saturday) return 6;
                if (dow == System.DayOfWeek.Sunday) return 0;
                if (dow == System.DayOfWeek.Thursday) return 4;
                if (dow == System.DayOfWeek.Tuesday) return 2;
                return 3;
            }

            public void getDate(int year, out DateTime startDate)
            {
                DateTime tmp;
                startDate = new DateTime(year, Month, 1);
                int dow = IntegerFromWeekday(startDate.DayOfWeek);
                startDate = startDate.AddDays((7 - dow + DayOfWeek) % 7);
                if (WeekOfMonth == 5)
                {
                    while (true)
                    {
                        tmp = startDate.AddDays(7);
                        if (tmp.Month != startDate.Month)
                            break;
                        startDate = tmp;
                    }
                }
                else
                {
                    for (int i = 1; i < WeekOfMonth; i++)
                    {
                        startDate = startDate.AddDays(7);
                    }
                }
            }
        }

        /// <summary>
        /// Complete rule for DST periods valid for range of years.
        /// </summary>
        public class Rule
        {
            public int startYear;
            public int endYear;
            public int OffsetSeconds;

            public int OffsetInMinutes
            {
                get { return OffsetSeconds / 60; }
                set { OffsetSeconds = value * 60; }
            }


            public RuleSpec startDay = new RuleSpec();
            public RuleSpec endDay = new RuleSpec();

            public static RuleSpec recognizeDaySpec(string p)
            {
                RuleSpec rs = null;
                string[] sp = p.Split('-');
                if (sp.Length == 4)
                {
                    rs = new RuleSpec();
                    int.TryParse(sp[0], out rs.Month);
                    int.TryParse(sp[1], out rs.WeekOfMonth);
                    int.TryParse(sp[2], out rs.DayOfWeek);
                    int.TryParse(sp[3], out rs.Hour);
                }
                return rs;
            }

            public void getStartEndDates(int year, out DateTime startDate, out DateTime endDate)
            {
                startDay.getDate(year, out startDate);
                endDay.getDate(year, out endDate);
            }

            public bool Contains(int nYear)
            {
                return (startYear <= nYear && endYear >= nYear);
            }

            public bool Contains(DateTime ut)
            {
                if (!Contains(ut.Year))
                    return false;
                DateTime s, e;
                startDay.getDate(ut.Year, out s);
                endDay.getDate(ut.Year, out e);

                if (startDay.Month < endDay.Month)
                {
                    // dst during summer
                    return (s <= ut && ut <= e);
                }
                else
                {
                    // dst during winter
                    return (s >= ut || ut >= e); 
                }
            }
        }

        /// <summary>
        /// Explicit transition for DST period based on exact dates.
        /// </summary>
        public class Transition
        {
            // starting date and time
            public DateTime startDate;

            // ending date and time
            public DateTime endDate;

            // new timezone offset relative to UTC in seconds
            public int OffsetInSeconds = 0;

            public int OffsetInMinutes
            {
                get { return OffsetInSeconds / 60; }
                set { OffsetInSeconds = value * 60; }
            }

            public bool Contains(DateTime time)
            {
                return startDate <= time && endDate > time;
            }

            // textual representation of timezone base offset
            public string getOffsetString()
            {
                return GPTimeZone.SecondsToString(OffsetInSeconds, 2);
            }

            public static DateTime recognizeDateTime(string str)
            {
                string[] sp = str.Split('-');
                if (sp.Length == 6)
                {
                    return new DateTime(int.Parse(sp[0]), int.Parse(sp[1]), int.Parse(sp[2]), int.Parse(sp[3]),
                        int.Parse(sp[4]), int.Parse(sp[5]));
                }
                return DateTime.Now;
            }
        }

        public int Id = 0;
        public string Name = string.Empty;
        public int OffsetSeconds = 0;
        public string NormalAbbr = "";
        public string DstAbbr = "";
        public bool DstUsed = false;

        public List<Transition> Transitions = new List<Transition>();
        public List<Rule> Rules = new List<Rule>();
        private Transition lastActiveTransition = null;
        private Rule lastActiveRule = null;


        public int OffsetInMinutes
        {
            get { return OffsetSeconds / 60; }
            set { OffsetSeconds = value * 60; }
        }

        public double getOffsetHours()
        {
            return OffsetSeconds / 3600.0;
        }

        public override string ToString()
        {
            return Name;
        }

        // textual representation of timezone base offset
        public string getOffsetString()
        {
            return GPTimeZone.SecondsToString(OffsetSeconds, 2);
        }

        public int getMaximumOffsetSeconds()
        {
            int max = OffsetSeconds;
            foreach (Transition tr in Transitions)
            {
                max = Math.Max(max, tr.OffsetInSeconds);
            }
            return max + OffsetSeconds;
        }

        public bool hasDstInYear(int year)
        {
            foreach (Transition tr in Transitions)
            {
                if (tr.startDate.Year >= year && tr.endDate.Year <= year)
                    return true;
            }

            foreach (Rule rule in Rules)
            {
                if (rule.startYear >= year && rule.endYear <= year)
                    return true;
            }

            return false;
        }

        public DateTime StartDateInYear(int year)
        {
            foreach (Rule rule in Rules)
            {
                if (rule.Contains(year))
                {
                    DateTime startDate;
                    rule.startDay.getDate(year, out startDate);
                    return startDate;
                }
            }

            foreach (Transition tr in Transitions)
            {
                if (tr.startDate.Year == year)
                    return tr.startDate;
            }
            return new DateTime();
        }

        public DateTime EndDateInYear(int year)
        {
            foreach (Rule rule in Rules)
            {
                if (rule.Contains(year))
                {
                    DateTime endDate;
                    rule.endDay.getDate(year, out endDate);
                    return endDate;
                }
            }
            foreach (Transition tr in Transitions)
            {
                if (tr.endDate.Year == year)
                    return tr.endDate;
            }
            return new DateTime();
        }

        public static string SecondsToString(long secs, int parts)
        {
            char sign;
            if (secs < 0)
            {
                secs = -secs;
                sign = '-';
            }
            else
            {
                sign = '+';
            }
            long hours = secs / 3600;
            secs = secs - hours * 3600;
            long mins = secs / 60;
            secs = secs - mins * 60;
            if (parts == 1)
            {
                return string.Format("{1}{0:00}", hours, sign);
            }
            else if (parts == 2)
            {
                return string.Format("{1}{0:00}:{2:00}", hours, sign, mins);
            }
            else if (parts == 3)
            {
                return string.Format("{1}{0:00}:{2:00}:{3:00}", hours, sign, mins, secs);
            }

            return string.Empty;
        }

        public string getFullName()
        {
            return string.Format("{0} {1}", GPAppHelper.GetTextTimeZone(OffsetSeconds), Name);
        }

        public void AddTransition(Transition trans)
        {
            Transitions.Add(trans);
        }

        public bool isSupportDaylightSaving()
        {
            return DstUsed;
        }

        public int BiasSecondsForDate(GPGregorianTime vc)
        {
            Rule rule = FindActiveRule(vc.getLocalTimeRaw());
            if (rule != null)
                return rule.OffsetSeconds - OffsetSeconds;

            Transition trans = FindActiveTransition(vc.getLocalTimeRaw());
            if (trans != null)
                return trans.OffsetInSeconds - OffsetSeconds;

            return 0;
        }

        public bool DaylightSavingInEFfect(GPGregorianTime vc)
        {
            Rule rule = FindActiveRule(vc.getLocalTime());
            if (rule != null)
                return true;

            Transition trans = FindActiveTransition(vc.getLocalTime());
            if (trans != null)
                return true;

            return false;
        }

        public Rule FindActiveRule(DateTime ut)
        {
            if (lastActiveRule != null && lastActiveRule.Contains(ut))
                return lastActiveRule;

            foreach (Rule rule in Rules)
            {
                if (rule.Contains(ut))
                {
                    lastActiveRule = rule;
                    return rule;
                }
            }

            return null;
        }

        public Transition FindActiveTransition(DateTime ut)
        {
            if (lastActiveTransition != null && lastActiveTransition.Contains(ut))
                return lastActiveTransition;

            foreach (Transition tr in Transitions)
            {
                if (tr.Contains(ut))
                {
                    lastActiveTransition = tr;
                    return tr;
                }
            }

            return null;
        }

        // return values
        // 0 - DST is off, yesterday was off
        // 1 - DST is on, yesterday was off
        // 2 - DST is on, yesterday was on
        // 3 - DST is off, yesterday was on
        public int GetDaylightChangeType(GPGregorianTime vcDay)
        {
            GPGregorianTime vcPrevDay = new GPGregorianTime(vcDay);
            vcPrevDay.PreviousDay();

            bool t1 = DaylightSavingInEFfect(vcDay);
            bool t2 = DaylightSavingInEFfect(vcPrevDay);

            return (t1 ? (t2 ? 2 : 1) : (t2 ? 3 : 0));
        }

        public DateTime dateFromParts(string a, string b, string c, string d)
        {
            try
            {
                return new DateTime(int.Parse(a), int.Parse(b), int.Parse(c), int.Parse(d), 0, 0);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Reading data of time zone into this object
        /// </summary>
        /// <param name="tzdata"></param>
        public void initWithData(string tzdata)
        {
            string[] sepSep = new string[] { "<sep>" };
            string[] sepTr = new string[] { "<tr>" };
            string[] sepR = new string[] { "<r>" };
            string[] data = tzdata.Split(sepSep, StringSplitOptions.None);

            if (data.Length > 4)
            {
                Name = data[0];
                int.TryParse(data[1], out OffsetSeconds);
                NormalAbbr = data[2];
                DstAbbr = data[3];
                DstUsed = data[4].Equals("1");
            }

            if (DstUsed)
            {
                if (data.Length > 5)
                {
                    Transitions.Clear();
                    string[] transListStr = data[5].Split(sepTr, StringSplitOptions.None);
                    foreach (string line in transListStr)
                    {
                        string[] p = line.Split(sepR, StringSplitOptions.None);
                        if (p.Length == 10)
                        {
                            Transition trans = new Transition();
                            trans.startDate = dateFromParts(p[1], p[2], p[3], p[4]);
                            trans.endDate = dateFromParts(p[5], p[6], p[7], p[8]);
                            trans.OffsetInSeconds = 0;
                            int.TryParse(p[9], out trans.OffsetInSeconds);
                            trans.OffsetInSeconds *= 60; // because input data has offset in minutes
                            trans.OffsetInSeconds += OffsetSeconds;
                            Transitions.Add(trans);
                        }
                    }
                }

                if (data.Length > 6)
                {
                    Rules.Clear();
                    string[] rulesListStr = data[6].Split(sepTr, StringSplitOptions.None);
                    foreach (string line in rulesListStr)
                    {
                        string[] p = line.Split(sepR, StringSplitOptions.None);
                        if (p.Length == 12)
                        {
                            Rule rule = new Rule();
                            rule.startYear = int.Parse(p[1]);
                            rule.startDay.Month = int.Parse(p[2]);
                            rule.startDay.WeekOfMonth = int.Parse(p[3]);
                            rule.startDay.DayOfWeek = int.Parse(p[4]);
                            rule.startDay.Hour = int.Parse(p[5]);
                            rule.endYear = int.Parse(p[6]);
                            rule.endDay.Month = int.Parse(p[7]);
                            rule.endDay.WeekOfMonth = int.Parse(p[8]);
                            rule.endDay.DayOfWeek = int.Parse(p[9]);
                            rule.endDay.Hour = int.Parse(p[10]);
                            rule.OffsetSeconds = int.Parse(p[11]) * 60 + OffsetSeconds;
                            Rules.Add(rule);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Joining list of strings with given separator string.
        /// </summary>
        /// <param name="list">List of strings, can be empty or null.</param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public string joinList(List<string> list, string sep)
        {
            StringBuilder sb = new StringBuilder();

            if (list != null)
            {
                if (list.Count > 0)
                    sb.Append(list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    sb.Append(sep);
                    sb.Append(list[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates string representation of time zone data for easy transition between Javascript and this object.
        /// </summary>
        /// <returns></returns>
        public string getStringData()
        {
            int ids = 1;
            List<string> data = new List<string>();
            List<string> tr = new List<string>();
            List<string> rul = new List<string>();
            List<string> tmp = new List<string>();


            foreach (Transition trans in Transitions)
            {
                tmp.Clear();
                tmp.Add(ids.ToString());
                ids++;
                tmp.Add(trans.startDate.Year.ToString());
                tmp.Add(trans.startDate.Month.ToString());
                tmp.Add(trans.startDate.Day.ToString());
                tmp.Add(trans.startDate.Hour.ToString());
                tmp.Add(trans.endDate.Year.ToString());
                tmp.Add(trans.endDate.Month.ToString());
                tmp.Add(trans.endDate.Day.ToString());
                tmp.Add(trans.endDate.Hour.ToString());
                // BIAS for this transition in minutes
                tmp.Add((trans.OffsetInMinutes - OffsetInMinutes).ToString());
                tr.Add(joinList(tmp, "<r>"));
            }

            foreach (Rule rule in Rules)
            {
                tmp.Clear();
                tmp.Add(ids.ToString());
                ids++;
                tmp.Add(rule.startYear.ToString());
                tmp.Add(rule.startDay.Month.ToString());
                tmp.Add(rule.startDay.WeekOfMonth.ToString());
                tmp.Add(rule.startDay.DayOfWeek.ToString());
                tmp.Add(rule.startDay.Hour.ToString());
                tmp.Add(rule.endYear.ToString());
                tmp.Add(rule.endDay.Month.ToString());
                tmp.Add(rule.endDay.WeekOfMonth.ToString());
                tmp.Add(rule.endDay.DayOfWeek.ToString());
                tmp.Add(rule.endDay.Hour.ToString());
                // BIAS for this rule in minutes
                tmp.Add((rule.OffsetInMinutes - OffsetInMinutes).ToString());
                rul.Add(joinList(tmp, "<r>"));
            }

            data.Add(Name);
            data.Add(OffsetSeconds.ToString());
            data.Add(NormalAbbr);
            data.Add(DstAbbr);
            data.Add(DstUsed ? "1" : "0");
            data.Add(joinList(tr, "<tr>"));
            data.Add(joinList(rul, "<tr>"));

            return joinList(data, "<sep>");
        }

        public static GPTimeZone UTC0 
        {
            get
            {
                GPTimeZone tz = new GPTimeZone();
                tz.Name = "UTC+0";
                tz.NormalAbbr = "utc";
                tz.OffsetSeconds = 0;
                return tz;
            }
        }
    }
}
