using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace GCAL.Base
{
    public class GPTimeZoneList
    {
        public bool Modified = false;
        private XmlDocument tzdoc = null;
        private XmlNode mainNode = null;
        private List<GPTimeZone> tzonesList = null;
        private int LastId = 1;

        public XmlDocument getTimezonesXml()
        {
            if (tzdoc == null)
            {
                Assembly assem = this.GetType().Assembly;

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(GCAL.Base.Properties.Resources.Timezones);

                tzdoc = doc;
            }
            return tzdoc;
        }

        private static GPTimeZoneList tzlist = null;

        public static GPTimeZoneList sharedTimeZones()
        {
            if (tzlist == null)
            {
                tzlist = new GPTimeZoneList();
            }
            return tzlist;
        }

        public XmlNode getMainNode()
        {
            if (mainNode == null)
            {
                XmlDocument doc = getTimezonesXml();
                XmlNodeList elems = doc.ChildNodes;
                foreach (XmlNode xnode in elems)
                {
                    if (xnode.Name == "timezone_list")
                    {
                        mainNode = xnode;
                        break;
                    }
                }
            }
            return mainNode;
        }

        public GPTimeZone GetTimezoneByName(string name)
        {
            foreach (GPTimeZone tz in getTimeZones())
            {
                if (tz.Name == name)
                    return tz;
            }
            return null;
        }

        public List<GPTimeZone> getTimeZones()
        {
            if (tzonesList == null)
            {
                tzonesList = new List<GPTimeZone>();
                XmlNode node = getMainNode();
                XmlElement elem = node as XmlElement;
                string s1;
                foreach (XmlElement item in elem.ChildNodes)
                {
                    if (item.Name == "timezone")
                    {
                        GPTimeZone tzone = new GPTimeZone();

                        if (item.HasAttribute("normalAbbr"))
                            tzone.NormalAbbr = item.GetAttribute("normalAbbr");
                        if (item.HasAttribute("dstAbbr"))
                            tzone.DstAbbr = item.GetAttribute("dstAbbr");
                        if (item.HasAttribute("offset"))
                        {
                            s1 = item.GetAttribute("offset");
                            tzone.OffsetSeconds = 0;
                            int.TryParse(s1, out tzone.OffsetSeconds);
                            tzone.OffsetSeconds *= 60;
                        }
                        if (item.HasAttribute("dst"))
                            tzone.DstUsed = item.GetAttribute("dst").Equals("True");

                        tzone.Id = getNextId();
                        foreach (XmlElement subs in item.ChildNodes)
                        {
                            if (subs.Name == "name")
                            {
                                tzone.Name = subs.InnerText;
                            }
                            else if (subs.Name == "transition")
                            {
                                GPTimeZone.Transition trans = new GPTimeZone.Transition();
                                if (subs.HasAttribute("date"))
                                    trans.startDate = GPTimeZone.Transition.recognizeDateTime(subs.GetAttribute("date"));
                                if (subs.HasAttribute("datend"))
                                    trans.endDate = GPTimeZone.Transition.recognizeDateTime(subs.GetAttribute("datend"));
                                if (subs.HasAttribute("offset"))
                                {
                                    int.TryParse(subs.GetAttribute("offset"), out trans.OffsetInSeconds);
                                    trans.OffsetInSeconds *= 60;
                                }
                                tzone.Transitions.Add(trans);
                            }
                            else if (subs.Name.Equals("rule"))
                            {
                                GPTimeZone.Rule rule = new GPTimeZone.Rule();
                                if (subs.HasAttribute("ruleStart"))
                                    rule.startDay = GPTimeZone.Rule.recognizeDaySpec(subs.GetAttribute("ruleStart"));
                                if (subs.HasAttribute("ruleEnd"))
                                    rule.endDay = GPTimeZone.Rule.recognizeDaySpec(subs.GetAttribute("ruleEnd"));
                                if (subs.HasAttribute("yearStart"))
                                    int.TryParse(subs.GetAttribute("yearStart"), out rule.startYear);
                                if (subs.HasAttribute("yearEnd"))
                                    int.TryParse(subs.GetAttribute("yearEnd"), out rule.endYear);
                                if (subs.HasAttribute("offset"))
                                {
                                    int.TryParse(subs.GetAttribute("offset"), out rule.OffsetSeconds);
                                    rule.OffsetSeconds *= 60;
                                }
                                tzone.Rules.Add(rule);
                            }
                        }
//                        tzone.RefreshEnds();
                        if (tzone.Name.Length > 0)
                            tzonesList.Add(tzone);

                    }
                }

            }


            return tzonesList;
        }

        public int getNextId()
        {
            return LastId++;
        }


        /// <summary>
        /// Create XML document as representation of complete timezone data
        /// </summary>
        /// <returns>XMLDocument</returns>
        public XmlDocument generateXmlDocument()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("timezone_list");
            doc.AppendChild(root);

            XmlElement tzoneNode = null;
            XmlElement tzoneTransNode = null;
            XmlElement child = null;

            foreach (GPTimeZone tzone in getTimeZones())
            {
                tzoneNode = doc.CreateElement("timezone");
                root.AppendChild(tzoneNode);

                child = doc.CreateElement("name");
                tzoneNode.AppendChild(child);
                child.InnerText = tzone.Name;
                child.SetAttribute("normalAbbr", tzone.NormalAbbr);
                child.SetAttribute("dstAbbr", tzone.DstAbbr);
                child.SetAttribute("offset", (tzone.OffsetSeconds / 60).ToString());
                child.SetAttribute("dst", tzone.DstUsed ? "True" : "False");

                foreach (GPTimeZone.Transition trans in tzone.Transitions)
                {
                    tzoneTransNode = doc.CreateElement("transition");
                    tzoneNode.AppendChild(tzoneTransNode);

                    tzoneTransNode.SetAttribute("date", trans.startDate.ToString("yyyy-MM-dd-HH-mm-ss"));
                    tzoneTransNode.SetAttribute("datend", trans.endDate.ToString("yyyy-MM-dd-HH-mm-ss"));
                    tzoneTransNode.SetAttribute("offset", trans.OffsetInMinutes.ToString());
                }

                foreach (GPTimeZone.Rule rule in tzone.Rules)
                {
                    XmlElement elem = doc.CreateElement("rule");
                    tzoneNode.AppendChild(elem);

                    elem.SetAttribute("ruleStart", String.Format("{0}-{1}-{2}-{3}", rule.startDay.Month, rule.startDay.WeekOfMonth, rule.startDay.DayOfWeek, rule.startDay.Hour));
                    elem.SetAttribute("ruleEnd", String.Format("{0}-{1}-{2}-{3}", rule.endDay.Month, rule.endDay.WeekOfMonth, rule.endDay.DayOfWeek, rule.endDay.Hour));
                    elem.SetAttribute("yearStart", rule.startYear.ToString());
                    elem.SetAttribute("yearEnd", rule.endYear.ToString());
                    elem.SetAttribute("offset", rule.OffsetInMinutes.ToString());
                }
            }

            return doc;
        }

        public void saveXml(string fileName)
        {
            XmlDocument doc = generateXmlDocument();

            doc.Save(fileName);
        }

        ~GPTimeZoneList()
        {
            if (Modified)
            {
                string fileName = "Timezones.xml";
                string dir = GPFileHelper.getAppDataDirectory();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fileName = Path.Combine(dir, fileName);
                saveXml(fileName);
            }
        }

        /*private class rec 
        {
            public int sec;
            public string text;
        }*/

        public string getTimezonesOffsetListDesc()
        {
            GPSortedIntStringList sl = new GPSortedIntStringList();
            foreach (GPTimeZone tz in getTimeZones())
            {
                sl.push(Convert.ToInt32(tz.OffsetSeconds), "UTC " + tz.getOffsetString());
            }

            return sl.ToString();
        }

        public GPTimeZone GetTimezoneById(int id)
        {
            foreach (GPTimeZone tz in getTimeZones())
            {
                if (tz.Id == id)
                    return tz;
            }
            return null;
        }

        public void DeleteTimezone(int timezoneId)
        {
            List<GPTimeZone> tzones = getTimeZones();
            for (int i = 0; i < tzones.Count; i++)
            {
                if (tzones[i].Id == timezoneId)
                {
                    tzones.RemoveAt(i);
                    i--;
                }
            }
        }

        public void addTimezone(GPTimeZone ntz)
        {
            getTimeZones().Add(ntz);
            Modified = true;
        }

        public string GetDefaultFileName()
        {
            return "Timezones.xml";
        }

        public string getFullPathForFile(string fileName)
        {
            string dir = GPFileHelper.getAppDataDirectory();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, fileName);
        }

        public void Save()
        {
            if (Modified)
            {
                string fileName = getFullPathForFile(GetDefaultFileName());
                saveXml(fileName);
            }
        }
    }
}
