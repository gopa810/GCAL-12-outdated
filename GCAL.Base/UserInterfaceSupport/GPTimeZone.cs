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
                return (startYear >= nYear && endYear <= nYear);
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

        public double BiasHoursForDate(GPGregorianTime vc)
        {
            Rule rule = FindActiveRule(vc.getLocalTime());
            if (rule != null)
                return Convert.ToDouble(rule.OffsetSeconds) / 3600.0;

            Transition trans = FindActiveTransition(vc.getLocalTime());
            if (trans != null)
                return Convert.ToDouble(trans.OffsetInSeconds - OffsetSeconds) / 3600.0;


            return 0.0;
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
    }
}
