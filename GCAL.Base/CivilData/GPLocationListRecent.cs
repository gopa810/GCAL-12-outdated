using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GCAL.Base
{
    public class GPLocationListRecent
    {
        private static List<GPLocation> recent = null;

        public GPLocationListRecent()
        {
        }


        public static void initRecent()
        {
            recent = new List<GPLocation>();
            LoadRecent();
        }

        public static List<GPLocation> getRecent()
        {
            if (recent == null)
            {
                initRecent();
            }
            return recent;
        }

        public static void putRecent(GPLocation lp)
        {
            int idx = findRecent(lp);
            if (idx > 0)
            {
                recent.RemoveAt(idx);
            }
            recent.Insert(0, lp);
            while (recent.Count > 10)
            {
                recent.RemoveAt(10);
            }
        }

        public static int findRecent(GPLocation lp)
        {
            if (recent == null)
            {
                initRecent();
            }

            for (int i = 0; i < recent.Count; i++)
            {
                if (recent[i].Equals(lp))
                    return i;
            }
            return -1;
        }



        public static string GetRecentFileName()
        {
            return Path.Combine(GPFileHelper.getAppDataDirectory(), "recentloc.xml");
        }

        public static void SaveRecent()
        {
            if (recent != null)
            {
                XmlDocument doc = new System.Xml.XmlDocument();

                XmlElement root = doc.CreateElement("recent");
                doc.AppendChild(root);
                foreach (GPLocation lp in recent)
                {
                    XmlElement elem = doc.CreateElement("location");
                    elem.SetAttribute("encoded", lp.encodeLocation());
                    root.AppendChild(elem);
                }

                doc.Save(GetRecentFileName());
            }
        }

        public static void LoadRecent()
        {
            XmlDocument doc = new System.Xml.XmlDocument();
            String fileName = GetRecentFileName();
            if (File.Exists(fileName))
            {
                if (recent == null)
                    recent = new List<GPLocation>();
                else
                    recent.Clear();
                doc.Load(fileName);
                foreach (XmlElement elem in doc.ChildNodes)
                {
                    if (elem.Name.Equals("recent"))
                    {
                        foreach (XmlElement child in elem.ChildNodes)
                        {
                            if (child.HasAttribute("encoded"))
                            {
                                GPLocation lp = new GPLocation();
                                lp.decodeLocation(child.GetAttribute("encoded"));
                                recent.Add(lp);
                            }
                        }
                    }
                }
            }
        }

    }
}
