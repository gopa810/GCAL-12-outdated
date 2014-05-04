using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GCAL.Base
{
    public class GPEventList: GPObjectListBase
    {
        public List<GPEventTithi> tithiEvents = new List<GPEventTithi>();
        public List<GPEventSankranti> sankrantiEvents = new List<GPEventSankranti>();
        public List<GPEventRelative> relativeEvents = new List<GPEventRelative>();

        private static GPEventList _sharedList = null;
        private static GPEvent eventNotFound = new GPEvent("Not found");
        
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
        }

        public override string GetDefaultResourceName()
        {
            return GCAL.Base.Properties.Resources.Events;
        }

        public override string GetDefaultFileName()
        {
            return "Events.txt";
        }
        public override void InsertNewObjectFromStrings(string[] parts)
        {
            if (parts.Length < 1)
                return;

            Debugger.Log(0, "", parts[0] + "," + parts.Length.ToString() + "\n");
            if (parts[0] == "T" && parts.Length >= 11)
            {
                GPEventTithi eve = new GPEventTithi();
                eve.nClass = int.Parse(parts[1]);
                eve.nMasa = int.Parse(parts[2]);
                eve.nTithi = int.Parse(parts[3]);
                eve.nVisible = int.Parse(parts[4]);
                eve.strFastSubject = parts[5];
                eve.strText = parts[6];
                eve.setRawFastType(int.Parse(parts[7]));
                eve.nUsed = int.Parse(parts[8]);
                eve.nStartYear = int.Parse(parts[9]);
                int.TryParse(parts[10], out eve.nSpec);
                tithiEvents.Add(eve);
            }
            else if (parts[0] == "R" && parts.Length >= 14)
            {
                GPEventRelative eve = new GPEventRelative();
                eve.nClass = int.Parse(parts[1]);
                eve.nVisible = int.Parse(parts[4]);
                eve.strFastSubject = parts[5];
                eve.strText = parts[6];
                eve.setRawFastType(int.Parse(parts[7]));
                eve.nUsed = int.Parse(parts[8]);
                eve.nStartYear = int.Parse(parts[9]);
                int.TryParse(parts[10], out eve.nSpec);
                eve.nSpecRef = int.Parse(parts[11]);
                eve.nOffsetFromEvent = int.Parse(parts[13]);
                relativeEvents.Add(eve);
            }
            else if (parts[0] == "S" && parts.Length >= 13)
            {
                GPEventSankranti eve = new GPEventSankranti();
                eve.nClass = int.Parse(parts[1]);
                eve.nVisible = int.Parse(parts[4]);
                eve.strFastSubject = parts[5];
                eve.strText = parts[6];
                eve.setRawFastType(int.Parse(parts[7]));
                eve.nUsed = int.Parse(parts[8]);
                eve.nStartYear = int.Parse(parts[9]);
                int.TryParse(parts[10], out eve.nSpec);
                eve.nOffsetFromSankranti = int.Parse(parts[12]);
                sankrantiEvents.Add(eve);
            }
        }

        public override void SaveData(StreamWriter writer)
        {
            foreach (GPEventTithi eve in tithiEvents)
            {
                writer.WriteLine("T\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                    eve.nClass, eve.nMasa, eve.nTithi, eve.nVisible, eve.strFastSubject, eve.strText,
                    eve.getRawFastType(), eve.nUsed, eve.nStartYear, eve.nSpec);
            }
            foreach (GPEventRelative eve in relativeEvents)
            {
                writer.WriteLine("R\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                    eve.nClass, "", "", eve.nVisible, eve.strFastSubject, eve.strText, eve.getRawFastType(),
                    eve.nUsed, eve.nStartYear, eve.nSpec, eve.nSpecRef, "", eve.nOffsetFromEvent);
            }
            foreach (GPEventSankranti eve in sankrantiEvents)
            {
                writer.WriteLine("S\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
                    eve.nClass, "", "", eve.nVisible, eve.strFastSubject, eve.strText, eve.getRawFastType(),
                    eve.nUsed, eve.nStartYear, eve.nSpec, "", eve.nOffsetFromSankranti);
            }
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
        }
    }
}
