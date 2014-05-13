using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GCAL.Base
{
    public class GPLocationChange
    {
        public GPLocation LocationA { get; set; }
        public GPLocation LocationB { get; set; }

        public bool TimezoneStart { get; set; }

        public double julianStart { get; set; }
        public double julianEnd { get; set; }

        public String humanStart 
        {
            get
            {
                GPGregorianTime gt = new GPGregorianTime(LocationA);
                gt.setJulianGreenwichTime(new GPJulianTime(julianStart, 0));
                return string.Format("{0} {1} {2}", gt.getShortDateString(), gt.getLongTimeString(), LocationA.getTimeZoneName());
            }
        }
        public String humanEnd
        {
            get
            {
                GPGregorianTime gt = new GPGregorianTime(LocationB);
                gt.setJulianGreenwichTime(new GPJulianTime(julianEnd, 0));
                return string.Format("{0} {1} {2}", gt.getShortDateString(), gt.getLongTimeString(), LocationB.getTimeZoneName());
            }
        }


        public void writeToXmlNode(XmlElement elem, XmlDocument doc)
        {
            XmlElement e1;

            e1 = doc.CreateElement("LocationA");
            elem.AppendChild(e1);
            LocationA.writeToXmlNode(e1, doc);

            e1 = doc.CreateElement("LocationB");
            elem.AppendChild(e1);
            LocationB.writeToXmlNode(e1, doc);

            e1 = doc.CreateElement("Time");
            elem.AppendChild(e1);
            e1.SetAttribute("TzStart", TimezoneStart.ToString());
            e1.SetAttribute("JuStart", julianStart.ToString());
            e1.SetAttribute("JuEnd", julianEnd.ToString());
        }

        public void loadFromXmlNode(XmlElement elem)
        {
            foreach (XmlElement e1 in elem.ChildNodes)
            {
                if (e1.Name.Equals("LocationA"))
                {
                    LocationA = new GPLocation();
                    LocationA.loadFromXmlNode(e1);
                }
                else if (e1.Name.Equals("LocationB"))
                {
                    LocationB = new GPLocation();
                    LocationB.loadFromXmlNode(e1);
                }
                else if (e1.Name.Equals("Time"))
                {
                    bool b;
                    double d;
                    if (e1.HasAttribute("TzStart"))
                    {
                        b = true;
                        bool.TryParse(e1.GetAttribute("TzStart"), out b);
                        TimezoneStart = b;
                    }
                    if (e1.HasAttribute("JuStart"))
                    {
                        double.TryParse(e1.GetAttribute("JuStart"), out d);
                        julianStart = d;
                    }
                    if (e1.HasAttribute("JuEnd"))
                    {
                        double.TryParse(e1.GetAttribute("JuEnd"), out d);
                        julianEnd = d;
                    }
                }
            }
        }

        public String humanLength
        {
            get
            {
                int len = Convert.ToInt32((julianEnd - julianStart)*1440);
                if (len < 0)
                    len = 30;
                int hr = len / 60;
                len -= hr * 60;
                return String.Format("{0} hr {1:00} min", hr, len);
            }
        }

        private GPLocation temp = new GPLocation();

        public GPLocation getTravellingLocation(double ratio)
        {
            double lonA, lonB;
            double latA, latB;

            lonA = LocationA.GetLongitudeEastPositive();
            lonB = LocationB.GetLongitudeEastPositive();
            latA = LocationA.GetLatitudeNorthPositive();
            latB = LocationB.GetLatitudeNorthPositive();

            GPLocation newLoc = new GPLocation();
            newLoc.setTimeZone(TimezoneStart ? LocationA.getTimeZone() : LocationB.getTimeZone());
            newLoc.setLongitudeEastPositive(lonA + (lonB - lonA) * ratio);
            newLoc.setLatitudeNorthPositive(latA + (latB - latA) * ratio);
            newLoc.SetAltitude(0);

            return temp;
        }

        public GPLocation getLocation(double jdate)
        {
            if (jdate < julianStart)
                return temp;
            if (jdate > julianEnd)
                return temp;

            double lonA, lonB;
            double latA, latB;

            lonA = LocationA.GetLongitudeEastPositive();
            lonB = LocationB.GetLongitudeEastPositive();
            latA = LocationA.GetLatitudeNorthPositive();
            latB = LocationB.GetLatitudeNorthPositive();

            temp.setTimeZone(TimezoneStart ? LocationA.getTimeZone() : LocationB.getTimeZone());
            temp.setLongitudeEastPositive(lonA + (lonB - lonA) * (jdate - julianStart) / (julianEnd - julianStart));
            temp.setLatitudeNorthPositive(latA + (latB - latA) * (jdate - julianStart) / (julianEnd - julianStart));
            temp.SetAltitude(0);

            return temp;
        }
    }
}
