using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPTimeZone
    {
        public class Transition
        {
            // this is UTC time in seconds converted to long integer by 
            // using formulas:
            // 1 hour = 60 min
            // 1 day  = 24 hours
            // 1 month = 32 days
            // 1 year = 12 months
            public long Timestamp = 0;
            public long TimestampEnd = 0;


            // new timezone offset relative to UTC in seconds
            public int OffsetInSeconds = 0;

            // abbreviation of timezone
            public string Abbreviation = string.Empty;

            // is DST in effect?
            public bool Dst = false;

            public bool Contains(long time)
            {
                return Timestamp <= time && TimestampEnd > time;
            }

            // textual representation of timezone base offset
            public string getOffsetString()
            {
                return GPTimeZone.SecondsToString(OffsetInSeconds, 2);
            }

            public string getDateString()
            {
                //1971-04-25 23:00:00 +0000
                GPTimestamp ts = new GPTimestamp(Timestamp);
                return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00} +0000", ts.year, ts.month, ts.day, ts.hour, 0, 0);
            }
            public void setDateString(string value)
            {
                    long y, m, d, h, mn, sc, offm, offs;
                    long.TryParse(value.Substring(0, 4), out y);
                    long.TryParse(value.Substring(5, 2), out m);
                    long.TryParse(value.Substring(8, 2), out d);
                    long.TryParse(value.Substring(11, 2), out h);
                    long.TryParse(value.Substring(14, 2), out mn);
                    long.TryParse(value.Substring(17, 2), out sc);
                    long.TryParse(value.Substring(21, 2), out offm);
                    long.TryParse(value.Substring(23, 2), out offs);
                    if (value[20] == '-')
                        offm = -offm;

                    long result = sc + 60 * (mn + 60 * (h + 24 * (d + 32 * (m + 12 * y))));
                    result += ((offm * 60) + offs) * 60;
                    Timestamp = result;
            }

            public DateTime getDateTime()
            {
                GPTimestamp ts = new GPTimestamp(Timestamp);
                return new DateTime(ts.year, ts.month, ts.day, ts.hour,
                    ts.minute, ts.second);
            }

        }

        public string Name = string.Empty;
        public long OffsetSeconds = 0;
        public List<Transition> Transitions = new List<Transition>();
        private Transition lastActiveTransition = null;
        private int lastActiveTransitionIndex = -1;


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

        public long getMaximumOffsetSeconds()
        {
            long max = OffsetSeconds;
            foreach (Transition tr in Transitions)
            {
                max = Math.Max(max, tr.OffsetInSeconds);
            }
            return max;
        }

        public bool hasDstInYear(int year)
        {
            foreach (Transition tr in Transitions)
            {
                if (GPTimestamp.Year(tr.Timestamp) == year)
                {
                    return true;
                }
            }

            return false;
        }

        public DateTime StartDateInYear(int year)
        {
            Transition selected = null;
            foreach (Transition tr in Transitions)
            {
                if (GPTimestamp.Year(tr.Timestamp) == year && tr.Dst == true)
                {
                    selected = tr;
                    break;
                }
            }
            if (selected == null)
                return new DateTime();
            return (new GPTimestamp(selected.Timestamp)).getDateTime();
        }

        public DateTime EndDateInYear(int year)
        {
            Transition selected = null;
            foreach (Transition tr in Transitions)
            {
                if (GPTimestamp.Year(tr.Timestamp) == year && tr.Dst == false)
                {
                    selected = tr;
                    break;
                }
            }
            if (selected == null)
                return new DateTime();
            return (new GPTimestamp(selected.Timestamp)).getDateTime();
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

        public void RefreshEnds()
        {
            for (int i = 1; i < Transitions.Count; i++)
            {
                Transitions[i - 1].TimestampEnd = Transitions[i].Timestamp;
            }
        }

        public void AddTransition(Transition trans)
        {
            Transition t = null;

            for (int i = 0; i < Transitions.Count; i++ )
            {
                t = Transitions[i];
                if (t.Timestamp > trans.Timestamp)
                {
                    Transitions.Insert(i, trans);
                    return;
                }
            }

            if (Transitions.Count > 0)
            {
                Transition tr = Transitions[Transitions.Count - 1];
                tr.TimestampEnd = tr.Timestamp + (86400L * 32L * 12L);
            }

            Transitions.Add(trans);
        }

        public bool isSupportDaylightSaving()
        {
            return Transitions.Count > 1;
        }

        public double BiasHoursForDate(GPGregorianTime vc)
        {
            Transition trans = FindActiveTransition(vc.getTimestamp());

            if (trans != null)
                return Convert.ToDouble(trans.OffsetInSeconds - OffsetSeconds) / 3600.0;
            return 0.0;
        }

        public Transition FindActiveTransition(long ut)
        {
            if (lastActiveTransition != null && lastActiveTransition.Timestamp < ut && lastActiveTransition.TimestampEnd > ut)
                return lastActiveTransition;

            if (Transitions.Count == 0)
                return null;

            if (Transitions.Count == 1)
                return Transitions[0];

            int a, b, c;
            bool found = true;
            a = 0;
            b = Transitions.Count - 1;
            c = (a + b) / 2;
            if (Transitions[b].Contains(ut))
            {
                lastActiveTransition = Transitions[b];
                lastActiveTransitionIndex = b;
                return lastActiveTransition;
            }
            while (!Transitions[c].Contains(ut))
            {
                if (a == c)
                {
                    found = false;
                    break;
                }
                if (Transitions[c].Timestamp > ut)
                {
                    b = c;
                }
                else
                {
                    a = c;
                }
                c = (a + b) / 2;
            }
            if (found)
            {
                lastActiveTransition = Transitions[c];
                lastActiveTransitionIndex = c;
            }
            return lastActiveTransition;
        }

        public Transition GetNextTransition(GPTimestamp ts)
        {
            foreach (Transition trans in Transitions)
            {
                if (trans.Timestamp > ts.getValue())
                    return trans;
            }
            return null;
        }

        // return values
        // 0 - DST is off, yesterday was off
        // 1 - DST is on, yesterday was off
        // 2 - DST is on, yesterday was on
        // 3 - DST is off, yesterday was on
        public int GetDaylightChangeType(GPGregorianTime vc2)
        {
            long uToday = vc2.getTimestamp();
            long uYesterday = uToday - 86400;

            Transition tzToday = FindActiveTransition(uToday);
            Transition tzYesterday = FindActiveTransition(uYesterday);

            bool t1 = tzToday != null ? tzToday.Dst : false;
            bool t2 = tzYesterday != null ? tzYesterday.Dst : false;

            if (t1)
            {
                if (t2)
                    return 2;
                else
                    return 1;
            }
            else if (t2)
            {
                return 3;
            }
            else
                return 0;
        }
    }
}
