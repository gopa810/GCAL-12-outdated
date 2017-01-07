using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GCAL.Base
{
    public class GCALBase
    {
        /// <summary>
        /// Creates GPLocationProvider object from given data.
        /// </summary>
        /// <param name="city">Any string</param>
        /// <param name="longitude">Unit is degrees. Positive values are to the east, negative values are to the west.</param>
        /// <param name="latitude">Unit is degrees. Positive values are for the north hemisphere, negative values are for south hemisphere.</param>
        /// <param name="timeZoneName">Name of timezone. This is one of the values returned by function GPTimeZoneList.sharedTimeZones().getTimeZones()</param>
        /// <returns></returns>
        public static GPLocation EncapsulateLocation(string city, double longitude, double latitude, string timeZoneName)
        {
            GPLocation loca = new GPLocation();
            loca.setCity(city);
            loca.setLongitudeEastPositive(longitude);
            loca.setLatitudeNorthPositive(latitude);
            loca.setTimeZoneName(timeZoneName);

            return loca;
        }

        /// <summary>
        /// Calculation of calendar
        /// </summary>
        /// <param name="city"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="timezoneName"></param>
        /// <param name="startDate"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static XmlDocument CalculateCalendar(GPLocation locationProvider, DateTime startDate, int days)
        {
            GPCalendarResults cr = new GPCalendarResults();


            GPGregorianTime sd = new GPGregorianTime(locationProvider);

            sd.setDate(startDate.Year, startDate.Month, startDate.Day);


            cr.CurrentLocation = locationProvider;
            cr.CalculateCalendar(sd, days);

            return FormaterXml.GetCalendarXmlDocument(cr);
        }

        public static XmlDocument CalculateAppearanceDay(GPLocation loc, DateTime startDateTime)
        {
            GPAppDayResults gap = new GPAppDayResults();
            gap.location = loc;
            GPGregorianTime ad = new GPGregorianTime(gap.location);
            ad.setDateTime(startDateTime);

            gap.calculateAppearanceDayData(gap.location, ad);

            return FormaterXml.GetAppDayXml(gap);
        }

        public static XmlDocument CalculateSankrantis(GPLocation loc, DateTime startDate, DateTime endDate)
        {
            GPGregorianTime vcStart = new GPGregorianTime(loc);
            vcStart.setDateTime(startDate);

            GPGregorianTime vcEnd = new GPGregorianTime(loc);
            vcEnd.setDateTime(endDate);

            return FormaterXml.GetSankrantiXml(loc, vcStart, vcEnd);
        }

        /// <summary>
        /// Returns first day of gaurabda vaisnava calendar for given year
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(GPLocation loc, int year)
        {
            GPGregorianTime vcStart = GPGaurabdaYear.getFirstDayOfYear(loc, year);

            DateTime dt = new DateTime(vcStart.getYear(), vcStart.getMonth(), vcStart.getDay());

            return dt;
        }

    }
}
