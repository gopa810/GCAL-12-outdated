using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPTimestamp
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;

        public GPTimestamp()
        {
        }

        public GPTimestamp(long tstamp)
        {
            setValue(tstamp);
        }

        public GPTimestamp(DateTime dt)
        {
            setDateTime(dt);
        }

        public DateTime getDateTime()
        {
            return new DateTime(year, month, day, hour, minute, second);
        }

        public void setDateTime(DateTime value)
        {
            year = value.Year;
            month = value.Month;
            day = value.Day;
            hour = value.Hour;
            minute = value.Minute;
            second = value.Second;
        }

        public DateTime getDate()
        {
            return new DateTime(year, month, day);
        }

        public void setDate(DateTime value)
        {
            year = value.Year;
            month = value.Month;
            day = value.Day;
        }

        public DateTime getTime()
        {
            return new DateTime(year, month, day, hour, minute, second);
        }

        public void setTime(DateTime value)
        {
            hour = value.Hour;
            minute = value.Minute;
            second = value.Second;
        }

        public static int Year(long timestamp)
        {
            return Convert.ToInt32(timestamp / (12 * 32 * 24 * 3600));
        }

        public void setValue(long value)
        {
            long v = value;
            second = Convert.ToInt32(v % 60);
            v = (v - second) / 60;
            minute = Convert.ToInt32(v % 60);
            v = (v - minute) / 60;
            hour = Convert.ToInt32(v % 24);
            v = (v - hour) / 24;
            day = Convert.ToInt32(v % 32);
            v = (v - day) / 32;
            month = Convert.ToInt32(v % 12);
            v = (v - month) / 12;
            year = Convert.ToInt32(v);
        }

        public long getValue()
        {
            return second + 60 * (minute + 60 * (hour + 24 * (day + 32 * (month + 12 * Convert.ToInt64(year)))));
        }

    }
}
