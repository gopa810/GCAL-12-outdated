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
                foreach (XmlElement item in elem.ChildNodes)
                {
                    if (item.Name == "timezone")
                    {
                        GPTimeZone tzone = new GPTimeZone();
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
                                {
                                    trans.setDateString(subs.GetAttribute("date"));
                                }
                                if (subs.HasAttribute("offset"))
                                {
                                    int.TryParse(subs.GetAttribute("offset"), out trans.OffsetInSeconds);
                                }
                                if (subs.HasAttribute("abbr"))
                                {
                                    trans.Abbreviation = subs.GetAttribute("abbr");
                                }
                                if (subs.HasAttribute("dst"))
                                {
                                    int dst = 0;
                                    int.TryParse(subs.GetAttribute("dst"), out dst);
                                    trans.Dst = ((dst == 1) ? true : false);
                                    if (trans.Dst == false && tzone.OffsetSeconds == 0)
                                    {
                                        tzone.OffsetSeconds = trans.OffsetInSeconds;
                                    }
                                }
                                tzone.Transitions.Add(trans);
                            }
                        }
                        tzone.RefreshEnds();
                        if (tzone.Name.Length > 0)
                            tzonesList.Add(tzone);

                    }
                }

            }


            return tzonesList;
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

                foreach (GPTimeZone.Transition trans in tzone.Transitions)
                {
                    tzoneTransNode = doc.CreateElement("transition");
                    tzoneNode.AppendChild(tzoneTransNode);

                    tzoneTransNode.SetAttribute("date", trans.getDateString());
                    tzoneTransNode.SetAttribute("offset", (trans.OffsetInSeconds / 60).ToString());
                    tzoneTransNode.SetAttribute("abbr", trans.Abbreviation);
                    tzoneTransNode.SetAttribute("dst", (trans.Dst ? "1" : "0"));
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
    }
}
