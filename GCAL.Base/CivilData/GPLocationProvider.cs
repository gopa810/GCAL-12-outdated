using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
