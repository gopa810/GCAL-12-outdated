using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GCAL.Base
{
    public class GPLocationProvider
    {
        private List<GPLocationChange> changes = new List<GPLocationChange>();
        private GPLocation defaultLocation = null;
        private double currStart = -1;
        private double currEnd = -1;
        private GPLocationChange currentChange = null;
        private GPLocation currentLocation = null;


        public GPLocationProvider()
        {
        }

        public GPLocationProvider(GPLocation loc)
        {
            defaultLocation = loc;
        }

        public void setDefaultLocation(GPLocation def)
        {
            defaultLocation = def;
        }

        public bool hasTravelling(double julianDay)
        {
            if (changes.Count < 1)
                return false;

            double dist = 1000.0;

            if (julianDay >= currStart && julianDay <= currEnd)
            {
                dist = Math.Min(dist, Math.Abs(currStart - julianDay));
                dist = Math.Min(dist, Math.Abs(currEnd - julianDay));
                if (dist < 0.7)
                    return true;
            }

            foreach (GPLocationChange lch in changes)
            {
                dist = Math.Min(dist, Math.Abs(lch.julianStart - julianDay));
                dist = Math.Min(dist, Math.Abs(lch.julianEnd - julianDay));
                if (dist < 0.7)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets location based on given julian day
        /// </summary>
        /// <param name="julianDay"></param>
        /// <returns></returns>
        public GPLocation getLocation(double julianDay)
        {
            if (julianDay >= currStart && julianDay <= currEnd)
            {
                if (currentLocation != null)
                {
                    return currentLocation;
                }
                if (currentChange != null)
                {
                    return currentChange.getLocation(julianDay);
                }
            }

            if (changes.Count < 1)
                return defaultLocation;
            double lastStart = 0;

            foreach(GPLocationChange lch in changes)
            {
                if (lch.julianStart > julianDay)
                {
                    currStart = lastStart;
                    currEnd = lch.julianStart;
                    currentLocation = lch.LocationA;
                    currentChange = null;
                    return currentLocation;
                }

                if (lch.julianStart <= julianDay && lch.julianEnd >= julianDay)
                {
                    currStart = lch.julianStart;
                    currEnd = lch.julianEnd;
                    currentLocation = null;
                    currentChange = lch;
                    return currentChange.getLocation(julianDay);
                }

                lastStart = lch.julianEnd;
            }

            int last = changes.Count - 1;
            if (changes[last].julianEnd < julianDay)
            {
                currStart = changes[last].julianEnd;
                currEnd = currStart + 2000000;
                currentLocation = changes[last].LocationB;
                return currentLocation;
            }

            return defaultLocation;
        }

        public string getFullName()
        {
            return getLocation(0).getFullName();
        }

        public double GetLongitudeEastPositive()
        {
            return getLocation(0).GetLongitudeEastPositive();
        }

        public double GetLatitudeNorthPositive()
        {
            return getLocation(0).GetLatitudeNorthPositive();
        }

        public string getCity()
        {
            return getLocation(0).getCity();
        }

        public double getTimeZoneOffsetHours()
        {
            return getLocation(0).getTimeZoneOffsetHours();
        }

        public GPTimeZone getTimeZone()
        {
            return getLocation(0).getTimeZone();
        }

        public string getName()
        {
            return getLocation(0).getName();
        }

        public int getLocationsCount()
        {
            return changes.Count + 1;
        }

        public GPLocation getLocationAtIndex(int i)
        {
            if (changes.Count == 0)
                return defaultLocation;
            if (i > 0)
                return changes[i-1].LocationB;
            return changes[i].LocationA;
        }

        public void setLocationAtIndex(int i, GPLocation loc)
        {
            if (changes.Count == 0)
                setDefaultLocation(loc);
            if (i < changes.Count)
                changes[i].LocationA = loc;
            if (i > 0)
                changes[i-1].LocationB = loc;
        }

        public GPLocationChange getChangeAtIndex(int i)
        {
            if (i < 0 || i >= changes.Count)
                return null;
            return changes[i];
        }

        public int getChangeCount()
        {
            return changes.Count;
        }

        public void removeChangeAtIndex(int i)
        {
            if (i < 0 || i >= changes.Count)
                return;
            changes.RemoveAt(i);
        }

        public void addChange(GPLocationChange newChange)
        {
            // remove changes with intersection
            removeChangesInRange(newChange.julianStart, newChange.julianEnd);

            if (changes.Count == 0)
            {
                changes.Add(newChange);
            }
            else
            {
                // retrieve index for this new change
                int newIndex = getIndexForNewChange(newChange.julianEnd);

                // insert cange at given index
                if (newIndex < 0)
                {
                    changes.Add(newChange);
                    newIndex = changes.Count - 1;
                }
                else
                {
                    changes.Insert(newIndex, newChange);
                }

                // validate next and prev location
                if (newIndex > 0)
                {
                    changes[newIndex - 1].LocationB = newChange.LocationA;
                }
                if (newIndex < (changes.Count - 1))
                {
                    changes[newIndex + 1].LocationA = newChange.LocationB;
                }
            }
        }

        public void removeChangesInRange(double jstart, double jend)
        {
            int i = 0;
            while (i < changes.Count)
            {
                GPLocationChange c = changes[i];
                if (c.julianStart > jend || c.julianEnd < jstart)
                {
                    i++;
                }
                else
                {
                    changes.RemoveAt(i);
                }
            }
        }

        public int getIndexForNewChange(double jend)
        {
            int i = 0;
            while (i < changes.Count)
            {
                GPLocationChange c = changes[i];
                if (c.julianStart > jend)
                    return i;
                i++;
            }

            return -1;
        }

        public void Clear()
        {
            changes.Clear();
        }

        public void removeChangesAfterIndex(int changeIndex)
        {
            while (changes.Count > changeIndex)
                changes.RemoveAt(changeIndex);
        }

        public void removeChangesBeforeIndex(int changeIndex)
        {
            for (int i = 0; i < changeIndex; i++)
                changes.RemoveAt(0);
        }

        public void insertChangeAtIndex(int p, GPLocationChange newChange)
        {
            changes.Insert(p, newChange);
        }

        /// <summary>
        /// Loads complete data for location provider from Xml file
        /// </summary>
        /// <param name="file">file name (complete path)</param>
        public void loadXml(string file)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(file);

            if (doc.ChildNodes.Count != 1)
                return;
            XmlElement elem = null;
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Equals("MyLocation"))
                {
                    elem = node as XmlElement;
                }
            }

            if (elem == null)
                return;

            changes = new List<GPLocationChange>();

            foreach (XmlElement e1 in elem.ChildNodes)
            {
                if (e1.Name.Equals("Change"))
                {
                    GPLocationChange chng = new GPLocationChange();
                    chng.loadFromXmlNode(e1);
                    changes.Add(chng);
                }
                else if (e1.Name.Equals("DefaultLocation"))
                {
                    defaultLocation = new GPLocation();
                    defaultLocation.loadFromXmlNode(e1);
                }
            }
        }

        /// <summary>
        /// Save complete data about this location provider to XML file
        /// </summary>
        /// <param name="file">complete file name (with path)</param>
        public void saveXml(string file)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elem1;
            XmlElement elem2;

            elem1 = doc.CreateElement("MyLocation");
            doc.AppendChild(elem1);

            foreach (GPLocationChange chng in changes)
            {
                elem2 = doc.CreateElement("Change");
                elem1.AppendChild(elem2);

                chng.writeToXmlNode(elem2, doc);
            }

            elem2 = doc.CreateElement("DefaultLocation");
            elem1.AppendChild(elem2);

            defaultLocation.writeToXmlNode(elem2, doc);

            doc.Save(file);

        }
    }
}
