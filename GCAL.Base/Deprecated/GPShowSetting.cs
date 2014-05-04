using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GCAL.Base
{
    public class GPShowSetting
    {

        private static Dictionary<int, int> gss = new Dictionary<int, int>();
        private static bool is_modified = false;

        public static int getValue(int index)
        {
            if (gss.ContainsKey(index))
                return gss[index];
            return 0;
        }

        public static void setValue(int index, int value)
        {
            gss[index] = value;
            is_modified = true;
        }

        public static void setModified(bool b)
        {
            is_modified = b;
        }

        public static bool getModified()
        {
            return is_modified;
        }

        public static void LoadFile(string fileName)
        {
            gss.Clear();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    int index, value;
                    string[] parts = line.Split(' ');
                    if (parts.Length == 3 && parts[0] == "i")
                    {
                        if (int.TryParse(parts[1], out index) && int.TryParse(parts[2], out value))
                        {
                            gss[index] = value;
                        }
                    }
                    line = sr.ReadLine();
                }
            }
        }

        public static void SaveFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (int key in gss.Keys)
                {
                    sw.WriteLine("i {0} {1}", key, gss[key]);
                }
            }
        }

    }
}

/*CShowSetting gss[] =
{
	{0, 0, "ARTI", "Tithi at arunodaya"},//0
	{0, 0, "ARTM", "Arunodaya Time"},//1
	{0, 0, "SRTM", "Sunrise Time"},//2
	{0, 0, "SSTM", "Sunset Time"},//3
	{0, 0, "MRTM", "Moonrise Time"},//4
	{0, 0, "MSTM", "Moonset Time"},//5
	{1, 1, "FEST", "Festivals"},//6
	{0, 0, "KSAY", "Info about ksaya tithi"},//7
	{0, 0, "VRDH", "Info about vriddhi tithi"},//8
	{0, 0, "SLON", "Sun Longitude"},//9
	{0, 0, "MLON", "Moon Longitude"},//10
	{0, 0, "AYAN", "Ayanamsha value"},//11
	{0, 0, "JDAY", "Julian Day"},//12
	{1, 1, "CPUR", "Caturmasya Purnima System"}, //13
	{0, 0, "CPRA", "Caturmasya Pratipat System"}, //14
	{0, 0, "CEKA", "Caturmasya Ekadasi System"}, //15
	{1, 1, "SANI", "Sankranti Info"}, //16
	{1, 1, "EKAI", "Ekadasi Info"}, //17
	{1, 1, "VHDR", "Masa Header Info"}, //18
	{0, 0, "PHDR", "Month Header Info"}, //19
	{0, 0, "EDNS", "Do not show empty days"}, //20
	{0, 0, "SBEM", "Show begining of masa"}, //21
	{1, 1, "F000", "Appearance days of the Lord"},//22
	{1, 1, "F001", "Events in the pastimes of the Lord"},//23
	{1, 1, "F002", "App, Disapp of Recent Acaryas"},//24
	{1, 1, "F003", "App, Disapp of Mahaprabhu's Associates and Other Acaryas"},//25
	{1, 1, "F004", "ISKCON's Historical Events"},//26
	{1, 1, "F005", "Bengal-specific Holidays"},//27
	{1, 1, "F006", "My Personal Events"}, //28
	{1, 1, "TSSR", "Todat Sunrise"},  //29 Today sunrise
	{1, 1, "TSSN", "Today Noon"},  //30 today noon
	{1, 1, "TSSS", "Today Sunset"},  //31 today sunset
	{0, 0, "TSAN", "Sandhya Times"},  //32 today + sandhya times
	{1, 1, "TSIN", "Sunrise Info"},  //33 today sunrise info
	{0, 0, "ASIN", "Noon Time"},  //34 astro - noon time
	{1, 1, "NDST", "Notice about DST"}, //35 notice about the change of the DST
	{1, 1, "DNAK", "Naksatra"}, // 36 naksatra info for each day
	{1, 1, "DYOG", "Yoga"}, //37 yoga info for each day
	{1, 1, "FFLG", "Fasting Flag"},//38
	{1, 1, "DPAK", "Paksa Info"},//39 paksa info
	{0, 0, "FDIW", "First Day in Week"},//40 first day in week
	{0, 0, "DRAS", "Rasi"}, //41 moon rasi for each calendar day
	{0, 0, "OSFA", "Old Style Fasting text"}, //42 old style fasting text
	{0, 0, "MLNT", "Name of month - type"}, //43 month type name 0-vaisnava,1-bengal,2-hindu,3-vedic
	{0, 0, "EDBL", "Editable Default Events"}, //44 editable default events
	{0, 0, "TSBM", "Today Brahma Muhurta"},     //45 brahma muhurta in today screen
	{0, 0, "TROM", "Today Rasi of the Moon"}, // 46 rasi of the moon in today screen
	{0, 0, "TNPD", "Today Naksatra Pada details"}, // 47 naksatra pada details in today screen
	{0, 0, "ADCS", "Child Names Suggestions"}, // 48 child name suggestions in Appearance Day screen
	{0, 0, "MNFO", "Masa Name Format"}, // 49 format of masa name
	{0, 0, "EPDR", "Ekadasi Parana details"}, // 50 ekadasi parana details
	{0, 0, "ANIV", "Aniversary show format"} // 51 format of aniversary info
    {3, 3, "", ""} //52 number of results in appday
 *  {1, 1, "", ""} //53 sun info in core events
 *  {1, 1, "", ""} //54 tithi info core events
 *  {1, 1, "", ""} //55 naksatra info core events
 *  {1, 1, "", ""} //56 sankranti info core events
 *  {1, 1, "", ""} //57 conjunction info core events
 *  {1, 1, "", ""} //58 eclipses info core event
 *  {1, 1, "", ""} //62 sort core events

  

 * 
 * * };
*/
