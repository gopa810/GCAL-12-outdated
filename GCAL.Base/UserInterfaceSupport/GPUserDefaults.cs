using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCAL.Base
{
    public class GPUserDefaults
    {
        #region non-static members

        public Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        public Dictionary<string, int> intValues = new Dictionary<string, int>();
        public Dictionary<string, string> stringValues = new Dictionary<string, string>();

        public static bool BoolForKey(string key, bool defaultValue)
        {
            if (sharedUserDefaults().boolValues.ContainsKey(key))
                return sharedUserDefaults().boolValues[key];
            return defaultValue;
        }

        public static void SetBoolForKey(string key, bool value)
        {
            sharedUserDefaults().boolValues[key] = value;
        }
        public static int IntForKey(string key, int defaultValue)
        {
            if (sharedUserDefaults().intValues.ContainsKey(key))
                return sharedUserDefaults().intValues[key];
            return defaultValue;
        }

        public static void SetIntForKey(string key, int value)
        {
            sharedUserDefaults().intValues[key] = value;
        }
        public static string StringForKey(string key, string defaultValue)
        {
            if (sharedUserDefaults().stringValues.ContainsKey(key))
                return sharedUserDefaults().stringValues[key];
            return defaultValue;
        }

        public static void SetStringForKey(string key, string value)
        {
            sharedUserDefaults().stringValues[key] = value;
        }

        public void SaveFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (string key in boolValues.Keys)
                {
                    sw.WriteLine("B\t{0}\t{1}", key, boolValues[key]);
                }
                foreach (string key in intValues.Keys)
                {
                    sw.WriteLine("I\t{0}\t{1}", key, intValues[key]);
                }
                foreach (string key in stringValues.Keys)
                {
                    sw.WriteLine("S\t{0}\t{1}", key, stringValues[key]);
                }
            }
        }

        public void LoadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                string[] s = File.ReadAllLines(fileName);
                boolValues.Clear();
                intValues.Clear();
                stringValues.Clear();
                foreach (string line in s)
                {
                    string[] ps = line.Split('\t');
                    if (ps.Length == 3)
                    {
                        if (ps[0] == "B")
                        {
                            SetBoolForKey(ps[1], bool.Parse(ps[2]));
                        }
                        else if (ps[0] == "I")
                        {
                            SetIntForKey(ps[1], int.Parse(ps[2]));
                        }
                        else if (ps[0] == "S")
                        {
                            SetStringForKey(ps[1], ps[2]);
                        }
                    }
                }
            }
            else
            {
                initDefault();
            }
        }

        ~GPUserDefaults()
        {
            SaveFile(getFileName());
        }

        #endregion

        #region static members

        private static GPUserDefaults inst = null;

        private static GPUserDefaults sharedUserDefaults()
        {
            if (inst == null)
            {
                inst = new GPUserDefaults();
                inst.LoadFile(getFileName());
            }
            return inst;
        }

        public static string getFileName()
        {
            string dir = GPFileHelper.getAppDataDirectory();
            return Path.Combine(dir, "UserDefaults.txt");
        }

        #endregion

        public void initDefault()
        {
            SetIntForKey("gen.caturmasya", 2);
            SetIntForKey("gen.week.firstday", 0);
            SetBoolForKey("gen.defevents.editable", false);
            SetBoolForKey("gen.highlight.oddlines", true);
            SetIntForKey("gen.timeformat", 1);
            SetIntForKey("gen.fastingnotation", 0);
            SetIntForKey("gen.masaname.format", 0);
            SetIntForKey("gen.sankranti.name.format", 0);
            SetBoolForKey("masa.visible.0", true);
            SetBoolForKey("masa.visible.1", true);
            SetBoolForKey("masa.visible.2", true);
            SetBoolForKey("masa.visible.3", true);
            SetBoolForKey("masa.visible.4", true);
            SetBoolForKey("masa.visible.5", true);
            SetBoolForKey("masa.visible.6", true);
            SetBoolForKey("masa.visible.7", true);
            SetBoolForKey("masa.visible.8", true);
            SetBoolForKey("masa.visible.9", true);
            SetBoolForKey("masa.visible.10", true);
            SetBoolForKey("masa.visible.11", true);
            SetBoolForKey("masa.visible.12", true);
            SetIntForKey("cal.headertype", 0);
            SetIntForKey("cal.anniversary", 0);
            SetBoolForKey("cal.ekadasi.parana.details", true);
            SetBoolForKey("cal.sunrise.moon.rasi", true);
            SetBoolForKey("cal.sunrise.paksa", true);
            SetBoolForKey("cal.sunrise.fastingflag", true);
            SetBoolForKey("cal.sunrise.yoga", true);
            SetBoolForKey("cal.sunrise.naksatra", true);
            SetBoolForKey("cal.dst.info", true);
            SetBoolForKey("cal.noon.time", false);
            SetBoolForKey("cal.festivals.0", true);
            SetBoolForKey("cal.festivals.1", true);
            SetBoolForKey("cal.festivals.2", true);
            SetBoolForKey("cal.festivals.3", true);
            SetBoolForKey("cal.festivals.4", true);
            SetBoolForKey("cal.festivals.5", true);
            SetBoolForKey("cal.festivals.6", true);
            SetBoolForKey("cal.masastart.info", true);
            SetBoolForKey("cal.hide.empty", false);
            SetBoolForKey("cal.fastingekadasi.info", true);
            SetBoolForKey("cal.sankranti.info", true);
            SetBoolForKey("cal.sunrise.julianday", false);
            SetBoolForKey("cal.sunrise.ayanamsa", false);
            SetBoolForKey("cal.sunrise.moon.longitude", false);
            SetBoolForKey("cal.sunrise.sun.longitude", false);
            SetBoolForKey("cal.festivals", true);
            SetBoolForKey("cal.moonset.time", false);
            SetBoolForKey("cal.moonrise.time", false);
            SetBoolForKey(GPDisplays.Keys.CalSunsetTime, false);
            SetBoolForKey(GPDisplays.Keys.CalSunriseTime, false);
            SetBoolForKey(GPDisplays.Keys.CalArunodayaTime, false);
            SetBoolForKey(GPDisplays.Keys.CalArunodayaTithi, false);
            SetBoolForKey(GPDisplays.Keys.CalendarKsaya, false);
            SetBoolForKey(GPDisplays.Keys.CalendarVriddhi, false);
            SetIntForKey("core.sorttype", 1);
            SetBoolForKey("core.mooneclipse", true);
            SetBoolForKey("core.suneclipse", true);
            SetBoolForKey("core.conjunction", true);
            SetBoolForKey("core.sankranti", true);
            SetBoolForKey("core.naksatra", true);
            SetBoolForKey("core.tithi", true);
            SetBoolForKey("core.moon", true);
            SetBoolForKey("core.sun", true);
            SetBoolForKey("todayform.topmost", false);
            SetBoolForKey("todayform.visiblelaunch", true);
            SetBoolForKey("todayframe.autosize", false);
            SetBoolForKey("today.naksatra.list", false);
            SetBoolForKey("today.tithi.list", false);
            SetBoolForKey("today.naksatra.pada", false);
            SetBoolForKey("today.rasi.moon", true);
            SetBoolForKey("today.muhurta.brahma", true);
            SetBoolForKey("today.sunrise.info", true);
            SetBoolForKey("today.sandhyas", false);
            SetBoolForKey("today.sunset", true);
            SetBoolForKey("today.noon", true);
            SetBoolForKey("today.sunrise", true);
            SetIntForKey("appday.celebs", 3);
            SetBoolForKey("appday.childn", true);
            SetIntForKey("nextfest.days", 16);
            SetBoolForKey("nextfest.onlyfast", false);
        }

    }
}
