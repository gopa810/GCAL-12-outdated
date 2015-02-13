using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace GCAL.Base
{
    public class GPEventList: GPObjectXmlListBase
    {
        public List<GPEventTithi> tithiEvents = new List<GPEventTithi>();
        public List<GPEventSankranti> sankrantiEvents = new List<GPEventSankranti>();
        public List<GPEventRelative> relativeEvents = new List<GPEventRelative>();
        public List<GPEventNaksatra> naksatraEvents = new List<GPEventNaksatra>();
        public List<GPEventYoga> yogaEvents = new List<GPEventYoga>();
        public List<GPEventAstro> astroEvents = new List<GPEventAstro>();

        public static int nextId = 1;
        private static GPEventList _sharedList = null;
        private static GPEvent eventNotFound = new GPEvent("Not found");

        /*private int currSid = 1300;
        private StringBuilder sidSb = new StringBuilder();*/

        public static GPEventList getShared()
        {
            if (_sharedList == null)
                _sharedList = new GPEventList();
            return _sharedList;
        }

        public GPEventList()
        {
            InitializeFromResources();

            foreach (GPEventRelative rel in relativeEvents)
            {
                GPEvent ev = GetSpecialEvent(rel.nSpecRef);
                if (ev != eventNotFound && ev != rel)
                {
                    ev.addChildrenItem(rel);
                    rel.addChildrenItem(ev);
                }
            }

            Modified = false;
        }

        public override string GetDefaultResourceForKey(FileKey fk)
        {
            return GCAL.Base.Properties.Resources.Events;
        }

        public override string GetDefaultFileNameForKey(FileKey fk)
        {
            return "Events.xml";
        }
        public override void AcceptXml(XmlDocument doc)
        {
            foreach (XmlNode elem in doc.ChildNodes)
            {
                if (elem.NodeType == XmlNodeType.Element && elem.Name.Equals("events"))
                {
                    XmlElement root = elem as XmlElement;
                    nextId = GetAttributeIntValue(root, "nextid", 1000);
                    foreach (XmlElement ev in root.ChildNodes)
                    {
                        InsertEvent(ev);
                    }
                }
            }

            Modified = false;
            /*if (currSid > 1300)
            {
                Modified = true;
                File.WriteAllText("c:\\Users\\Gopa702\\AppData\\Roaming\\GCAL\\newstrings.txt", sidSb.ToString());
            }*/
        }

        public void InsertEvent(XmlElement elem)
        {
            string evType = elem.GetAttribute("type");
            GPEvent e = null;
            if (evType.Equals("TITHI"))
            {
                GPEventTithi et = new GPEventTithi();
                et.nTithi = GetSubelementIntValue(elem, "tithi", 0);
                et.nMasa = GetSubelementIntValue(elem, "masa", 0);
                e = et;
            }
            else if (evType.Equals("REL"))
            {
                GPEventRelative er = new GPEventRelative();
                er.nSpecRef = GetSubelementIntValue(elem, "specref", 0);
                e = er;
            }
            else if (evType.Equals("SAN"))
            {
                GPEventSankranti es = new GPEventSankranti();
                es.nSankranti = GetSubelementIntValue(elem, "sankranti", 0);
                e = es;
            }
            else if (evType.Equals("NAK"))
            {
                GPEventNaksatra en = new GPEventNaksatra();
                en.nNaksatra = GetSubelementIntValue(elem, "naksatra", 0);
                e = en;
            }
            else if (evType.Equals("YOG"))
            {
                GPEventYoga en = new GPEventYoga();
                en.nYoga = GetSubelementIntValue(elem, "naksatra", 0);
                e = en;
            }
            else if (evType.Equals("ASTRO"))
            {
                GPEventAstro ea = new GPEventAstro();
                ea.nAstroType = GetSubelementIntValue(elem, "astrotype", 1);
                ea.nData = GetSubelementIntValue(elem, "data", 0);
                e = ea;
            }

            e.nClass = GetAttributeIntValue(elem, "class", 6);
            e.nVisible = GetAttributeIntValue(elem, "visible", 1);
            e.nUsed = GetAttributeIntValue(elem, "used", 0);

            e.nStartYear = GetSubelementIntValue(elem, "startyear", -10000);
            e.nSpec = GetSubelementIntValue(elem, "specid", -1);
            e.nOffset = GetSubelementIntValue(elem, "offset", 0);
            e.nDeleted = GetSubelementIntValue(elem, "deleted", 0);
            e.setRawFastType(GetSubelementIntValue(elem, "fasttype", 0));
            e.setRawFastSubject(GetSubelementValue(elem, "fastsubject", "").Trim());
            e.setRawText(GetSubelementValue(elem, "text", "").Trim());
            e.textStringId = GetSubelementIntValue(elem, "textstringid", -1);
            e.fastSubjectStringId = GetSubelementIntValue(elem, "fastsubjectstringid", -1);
            e.eventId = GetSubelementIntValue(elem, "eventid", 0);

            /*if (e.nClass < 6)
            {
                if (e.strText.Length > 0 && e.textStringId < 0)
                {
                    sidSb.AppendFormat("{0}\t{1}\n", currSid, e.strText);
                    e.textStringId = currSid;
                    currSid++;
                }

                if (e.strFastSubject.Length > 0 && e.fastSubjectStringId < 0)
                {
                    sidSb.AppendFormat("{0}\t{1}\n", currSid, e.strFastSubject);
                    e.fastSubjectStringId = currSid;
                    currSid++;
                }
            }*/

            add(e);
        }

        public static string GetSubelementValue(XmlElement elem, String subElemName, String defaultValue)
        {
            string value = defaultValue;
            foreach (XmlElement item in elem.GetElementsByTagName(subElemName))
            {
                value = item.InnerText;
                break;
            }
            return value;
        }

        public static int GetSubelementIntValue(XmlElement elem, String subElemName, int defaultValue)
        {
            String s = GetSubelementValue(elem, subElemName, defaultValue.ToString());
            int value = 0;
            if (int.TryParse(s, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static int GetAttributeIntValue(XmlElement elem, String attributeName, int defaultValue)
        {
            String s = elem.GetAttribute(attributeName);
            int value = 0;
            if (int.TryParse(s, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public override void Save()
        {
            if (Modified)
            {
                string fileName = getFullPathForFile(GetDefaultFileNameForKey(FileKey.Primary));
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    SaveData(sw, FileKey.Primary);
                }
            }
        }

        public override void SaveData(StreamWriter writer, FileKey fk)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("events");
            XmlElement elem;
            doc.AppendChild(root);
            root.SetAttribute("nextid", nextId.ToString());

            foreach (GPEventTithi eve in tithiEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "TITHI");
                SaveEventElement(doc, eve, elem);
                SaveElementChild(elem, "tithi", eve.nTithi.ToString());
                SaveElementChild(elem, "masa", eve.nMasa.ToString());
                root.AppendChild(elem);
            }
            foreach (GPEventRelative eve in relativeEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "REL");
                SaveEventElement(doc, eve, elem);
                SaveElementChild(elem, "specref", eve.nSpecRef.ToString());
                root.AppendChild(elem);
            }
            foreach (GPEventSankranti eve in sankrantiEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "SAN");
                SaveEventElement(doc, eve, elem);
                SaveElementChild(elem, "sankranti", eve.nSankranti.ToString());
                root.AppendChild(elem);
            }
            foreach (GPEventNaksatra eve in naksatraEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "NAK");
                SaveEventElement(doc, eve, elem);
                SaveElementChild(elem, "naksatra", eve.nNaksatra.ToString());
                root.AppendChild(elem);
            }
            foreach (GPEventYoga evy in yogaEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "YOG");
                SaveEventElement(doc, evy, elem);
                SaveElementChild(elem, "yoga", evy.nYoga.ToString());
                root.AppendChild(elem);
            }
            foreach (GPEventAstro eva in astroEvents)
            {
                elem = doc.CreateElement("event");
                elem.SetAttribute("type", "ASTRO");
                SaveEventElement(doc, eva, elem);
                SaveElementChild(elem, "astrotype", eva.nAstroType.ToString());
                SaveElementChild(elem, "data", eva.nData.ToString());
                root.AppendChild(elem);
            }

            doc.Save(writer);
        }

        private static XmlElement SaveEventElement(XmlDocument doc, GPEvent eve, XmlElement elem)
        {
            // ---
            elem.SetAttribute("class", eve.nClass.ToString());
            elem.SetAttribute("visible", eve.nVisible.ToString());
            elem.SetAttribute("used", eve.nUsed.ToString());
            // ---
            // save fast subject
            SaveElementChild(elem, "fastsubjectstringid", eve.fastSubjectStringId.ToString());
            SaveElementChild(elem, "fastsubject", eve.getRawFastSubject());

            // save text
            SaveElementChild(elem, "textstringid", eve.textStringId.ToString());
            SaveElementChild(elem, "text", eve.getRawText());

            // save others
            SaveElementChild(elem, "startyear", eve.nStartYear.ToString());
            SaveElementChild(elem, "specid", eve.nSpec.ToString());
            SaveElementChild(elem, "offset", eve.nOffset.ToString());
            SaveElementChild(elem, "deleted", eve.nDeleted.ToString());
            SaveElementChild(elem, "eventid", eve.eventId.ToString());
            SaveElementChild(elem, "fasttype", eve.getRawFastType().ToString());

            return elem;
        }

        private static XmlElement SaveElementChild(XmlElement elem, string property, string value)
        {
            XmlElement prop = elem.OwnerDocument.CreateElement(property);
            prop.InnerText = value;
            elem.AppendChild(prop);
            return prop;
        }

        public GPEvent GetSpecialEvent(int i)
        {
            foreach (GPEvent eve in tithiEvents)
            {
                if (eve.nSpec == i)
                    return eve;
            }
            foreach (GPEvent eve in relativeEvents)
            {
                if (eve.nSpec == i)
                    return eve;
            }
            foreach (GPEvent eve in sankrantiEvents)
            {
                if (eve.nSpec == i)
                    return eve;
            }
            foreach (GPEvent eve in naksatraEvents)
            {
                if (eve.nSpec == i)
                    return eve;
            }

            return eventNotFound;
        }

        public void RemoveEvent(object eve)
        {
            if (eve is GPEventRelative)
            {
                relativeEvents.Remove(eve as GPEventRelative);
            }
            else if (eve is GPEventSankranti)
            {
                sankrantiEvents.Remove(eve as GPEventSankranti);
            }
            else if (eve is GPEventTithi)
            {
                tithiEvents.Remove(eve as GPEventTithi);
            }
            else if (eve is GPEventNaksatra)
            {
                naksatraEvents.Remove(eve as GPEventNaksatra);
            }
            else if (eve is GPEventAstro)
            {
                astroEvents.Remove(eve as GPEventAstro);
            }
            else if (eve is GPEventYoga)
            {
                yogaEvents.Remove(eve as GPEventYoga);
            }

            Modified = true;
        }

        public void add(object eve)
        {
            if (eve is GPEventRelative)
            {
                relativeEvents.Add(eve as GPEventRelative);
            }
            else if (eve is GPEventSankranti)
            {
                sankrantiEvents.Add(eve as GPEventSankranti);
            }
            else if (eve is GPEventTithi)
            {
                tithiEvents.Add(eve as GPEventTithi);
            }
            else if (eve is GPEventNaksatra)
            {
                naksatraEvents.Add(eve as GPEventNaksatra);
            }
            else if (eve is GPEventAstro)
            {
                astroEvents.Add(eve as GPEventAstro);
            }
            else if (eve is GPEventYoga)
            {
                yogaEvents.Add(eve as GPEventYoga);
            }

            Modified = true;
        }

        public GPEvent find(int id)
        {
            foreach (GPEventTithi ev1 in tithiEvents)
            {
                if (ev1.eventId == id)
                    return ev1;
            }

            foreach (GPEventSankranti ev2 in sankrantiEvents)
            {
                if (ev2.eventId == id)
                    return ev2;
            }

            foreach (GPEventRelative ev3 in relativeEvents)
            {
                if (ev3.eventId == id)
                    return ev3;
            }

            foreach (GPEventNaksatra ev4 in naksatraEvents)
            {
                if (ev4.eventId == id)
                    return ev4;
            }

            foreach (GPEventAstro ev4 in astroEvents)
            {
                if (ev4.eventId == id)
                    return ev4;
            }

            foreach (GPEventYoga ev4 in yogaEvents)
            {
                if (ev4.eventId == id)
                    return ev4;
            }

            return null;
        }

        public static int getNextID()
        {
            int nid = nextId;
            nextId++;
            return nid;
        }
    }
}
