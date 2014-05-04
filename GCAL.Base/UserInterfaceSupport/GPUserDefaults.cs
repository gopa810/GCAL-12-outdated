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
                string [] s = File.ReadAllLines(fileName);
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

    }
}
