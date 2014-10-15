using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPDisplays
    {
        public class Keys
        {
            public const string CalendarSankranti = "cal.sankranti.info";
            public const string CalendarKsaya = "cal.ksaya";
            public const string CalendarVriddhi = "cal.vriddhi";
            public const string CaturmasyaPurnima = "gen.caturmasya.purnima";
            public const string CaturmasyaPratipat = "gen.caturmasya.pratipat";
            public const string CaturmasyaEkadasi = "gen.caturmasya.ekadasi";
            public const string CalArunodayaTithi = "cal.arunodaya.tithi";
            public const string CalArunodayaTime = "cal.arunodaya.time";
            public const string CalSunriseTime = "cal.sunrise.time";
            public const string CalNoonTime = "cal.noon.time";
            public const string CalSunsetTime = "cal.sunset.time";
            public const string CalMoonriseTime = "cal.moonrise.time";
            public const string CalMoonsetTime = "cal.moonset.time";
            public const string CalSunLong = "cal.sunrise.sun.longitude";
            public const string CalMoonLong = "cal.sunrise.moon.longitude";
            public const string CalAyanamsa = "cal.sunrise.ayanamsa";
            public const string CalJulian = "cal.sunrise.julianday";
            public const string CalDstChange = "cal.dst.info";
            public const string CalMasaChange = "cal.masastart.info";

            public static string FestivalClass(int i)
            {
                return "cal.festivals." + i.ToString();
            }
        }

        public class AppDay
        {
            //48
            public static bool childNameSuggestions()
            {
                return GPUserDefaults.BoolForKey("appday.childn", true);
            }
            public static void childNameSuggestions(bool value)
            {
                GPUserDefaults.SetBoolForKey("appday.childn", value);
            }
            //52
            public static int celebrationCount()
            {
                return GPUserDefaults.IntForKey("appday.celebs", 3);
            }
            public static void celebrationCount(int value)
            {
                GPUserDefaults.SetIntForKey("appday.celebs", value);
            }

        }

        public class Today
        {
            // 29
            public static bool SunriseVisible() { return GPUserDefaults.BoolForKey("today.sunrise", true); }
            public static void SunriseVisible(bool value) { GPUserDefaults.SetBoolForKey("today.sunrise", value); }

            // 30

            public static bool NoonVisible() { return GPUserDefaults.BoolForKey("today.noon", true); }
            public static void NoonVisible(bool value) { GPUserDefaults.SetBoolForKey("today.noon", value); }

            // 31

            public static bool SunsetVisible() { return GPUserDefaults.BoolForKey("today.sunset", true); }
            public static void SunsetVisible(bool value) { GPUserDefaults.SetBoolForKey("today.sunset", value); }

            // 32
            public static bool SandhyaTimesVisible() { return GPUserDefaults.BoolForKey("today.sandhyas", false); }
            public static void SandhyaTimesVisible(bool value) { GPUserDefaults.SetBoolForKey("today.sandhyas", value); }
            // 33
            public static bool SunriseInfo() { return GPUserDefaults.BoolForKey("today.sunrise.info", true); }
            public static void SunriseInfo(bool value) { GPUserDefaults.SetBoolForKey("today.sunrise.info", value); }

            // 45
            public static bool BrahmaMuhurtaVisible() { return GPUserDefaults.BoolForKey("today.muhurta.brahma", true); }
            public static void BrahmaMuhurtaVisible(bool value) { GPUserDefaults.SetBoolForKey("today.muhurta.brahma", value); }

            // 46
            public static bool RasiOfMoonVisible() { return GPUserDefaults.BoolForKey("today.rasi.moon", true); }
            public static void RasiOfMoonVisible(bool value) { GPUserDefaults.SetBoolForKey("today.rasi.moon", value); }

            // 47
            public static bool NaksatraPadaVisible() { return GPUserDefaults.BoolForKey("today.naksatra.pada", false); }
            public static void NaksatraPadaVisible(bool value) { GPUserDefaults.SetBoolForKey("today.naksatra.pada", value); }

            public static bool TithiList() { return GPUserDefaults.BoolForKey("today.tithi.list", false); }
            public static void TithiList(bool value) { GPUserDefaults.SetBoolForKey("today.tithi.list", value); }

            public static bool NaksatraList() { return GPUserDefaults.BoolForKey("today.naksatra.list", false); }
            public static void NaksatraList(bool value) { GPUserDefaults.SetBoolForKey("today.naksatra.list", value); }

            public static bool WindowAutosize() { return GPUserDefaults.BoolForKey("todayframe.autosize", false); }
            public static void WindowAutosize(bool value) { GPUserDefaults.SetBoolForKey("todayframe.autosize", value); }

            public static bool VisibleAtLaunch() { return GPUserDefaults.BoolForKey("todayform.visiblelaunch", true); }
            public static void VisibleAtLaunch(bool value) { GPUserDefaults.SetBoolForKey("todayform.visiblelaunch", value); }

            public static bool TopmostWindow() { return GPUserDefaults.BoolForKey("todayform.topmost", false); }
            public static void TopmostWindow(bool value) { GPUserDefaults.SetBoolForKey("todayform.topmost", value); }

        }

        public class CoreEvents
        {
            public static bool Sunrise() { return GPUserDefaults.BoolForKey("core.sun", true); }
            public static void Sunrise(bool value) { GPUserDefaults.SetBoolForKey("core.sun", value); }
            public static bool Moonrise() { return GPUserDefaults.BoolForKey("core.moon", true); }
            public static void Moonrise(bool value) { GPUserDefaults.SetBoolForKey("core.moon", value); }
            public static bool Tithi() { return GPUserDefaults.BoolForKey("core.tithi", true); }
            public static void Tithi(bool value) { GPUserDefaults.SetBoolForKey("core.tithi", value); }
            public static bool Naksatra() { return GPUserDefaults.BoolForKey("core.naksatra", true); }
            public static void Naksatra(bool value) { GPUserDefaults.SetBoolForKey("core.naksatra", value); }
            public static bool Sankranti() { return GPUserDefaults.BoolForKey("core.sankranti", true); }
            public static void Sankranti(bool value) { GPUserDefaults.SetBoolForKey("core.sankranti", value); }
            public static bool Conjunction() { return GPUserDefaults.BoolForKey("core.conjunction", true); }
            public static void Conjunction(bool value) { GPUserDefaults.SetBoolForKey("core.conjunction", value); }
            public static bool SunEclipse() { return GPUserDefaults.BoolForKey("core.suneclipse", true); }
            public static void SunEclipse(bool value) { GPUserDefaults.SetBoolForKey("core.suneclipse", value); }
            public static bool MoonEclipse() { return GPUserDefaults.BoolForKey("core.mooneclipse", true); }
            public static void MoonEclipse(bool value) { GPUserDefaults.SetBoolForKey("core.mooneclipse", value); }
            public static bool Sort() { return SortType() == 1; }
            public static void Sort(bool value) { SortType(value ? 1 : 0); }
            public static int SortType() { return GPUserDefaults.IntForKey("core.sorttype", 1); }
            public static void SortType(int value) { GPUserDefaults.SetIntForKey("core.sorttype", value); }
        }

        public class Calendar
        {
            public static bool TithiArunodayaVisible() { return GPUserDefaults.BoolForKey(Keys.CalArunodayaTithi, false); }
            public static void TithiArunodayaVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalArunodayaTithi, value); }
            public static bool TimeArunodayaVisible() { return GPUserDefaults.BoolForKey(Keys.CalArunodayaTime, false); }
            public static void TimeArunodayaVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalArunodayaTime, value); }
            public static bool TimeSunriseVisible() { return GPUserDefaults.BoolForKey(Keys.CalSunriseTime, false); }
            public static void TimeSunriseVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalSunriseTime, value); }
            public static bool TimeSunsetVisible() { return GPUserDefaults.BoolForKey(Keys.CalSunsetTime, false); }
            public static void TimeSunsetVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalSunsetTime, value); }
            public static bool TimeMoonriseVisible() { return GPUserDefaults.BoolForKey("cal.moonrise.time", false); }
            public static void TimeMoonriseVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.moonrise.time", value); }
            public static bool TimeMoonsetVisible() { return GPUserDefaults.BoolForKey("cal.moonset.time", false); }
            public static void TimeMoonsetVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.moonset.time", value); }
            public static bool FestivalsVisible() { return GPUserDefaults.BoolForKey("cal.festivals", true); }
            public static void FestivalsVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals", value); }
            public static bool MonthHeader() { return GPUserDefaults.IntForKey("cal.headertype", 1) == 1; }
            public static bool MasaHeader() { return GPUserDefaults.IntForKey("cal.headertype", 1) == 2; }
            public static bool KsayaTithiInfoVisible() { return GPUserDefaults.BoolForKey(Keys.CalendarKsaya, false); }
            public static void KsayaTithiInfoVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalendarKsaya, value); }
            public static bool VriddhiTithiInfoVisible() { return GPUserDefaults.BoolForKey(Keys.CalendarVriddhi, false); }
            public static void VriddhiTithiInfoVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalendarVriddhi, value); }

            public static bool SunLongitudeVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.sun.longitude", false); }
            public static void SunLongitudeVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.sun.longitude", value); }
            public static bool MoonLongitudeVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.moon.longitude", false); }
            public static void MoonLongitudeVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.moon.longitude", value); }
            public static bool AyanamsaValueVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.ayanamsa", false); }
            public static void AyanamsaValueVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.ayanamsa", value); }
            public static bool JulianDayVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.julianday", false); }
            public static void JulianDayVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.julianday", value); }
            public static bool SankrantiInfoVisible() { return GPUserDefaults.BoolForKey(Keys.CalendarSankranti, true); }
            public static void SankrantiInfoVisible(bool value) { GPUserDefaults.SetBoolForKey(Keys.CalendarSankranti, value); }
            public static bool EkadasiInfoVisible() { return GPUserDefaults.BoolForKey("cal.fastingekadasi.info", true); }
            public static void EkadasiInfoVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.fastingekadasi.info", value); }
            public static bool HideEmptyDays() { return GPUserDefaults.BoolForKey("cal.hide.empty", true); }
            public static void HideEmptyDays(bool value) { GPUserDefaults.SetBoolForKey("cal.hide.empty", value); }
            public static bool StartMasaVisible() { return GPUserDefaults.BoolForKey("cal.masastart.info", true); }
            public static void StartMasaVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.masastart.info", value); }
            public static bool FestivalClass0() { return GPUserDefaults.BoolForKey("cal.festivals.0", true); }
            public static void FestivalClass0(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.0", value); }
            public static bool FestivalClass1() { return GPUserDefaults.BoolForKey("cal.festivals.1", true); }
            public static void FestivalClass1(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.1", value); }
            public static bool FestivalClass2() { return GPUserDefaults.BoolForKey("cal.festivals.2", true); }
            public static void FestivalClass2(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.2", value); }
            public static bool FestivalClass3() { return GPUserDefaults.BoolForKey("cal.festivals.3", true); }
            public static void FestivalClass3(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.3", value); }
            public static bool FestivalClass4() { return GPUserDefaults.BoolForKey("cal.festivals.4", true); }
            public static void FestivalClass4(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.4", value); }
            public static bool FestivalClass5() { return GPUserDefaults.BoolForKey("cal.festivals.5", true); }
            public static void FestivalClass5(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.5", value); }
            public static bool FestivalClass6() { return GPUserDefaults.BoolForKey("cal.festivals.6", true); }
            public static void FestivalClass6(bool value) { GPUserDefaults.SetBoolForKey("cal.festivals.6", value); }
            public static bool NoonTime() { return GPUserDefaults.BoolForKey("cal.noon.time", false); }
            public static void NoonTime(bool value) { GPUserDefaults.SetBoolForKey("cal.noon.time", value); }
            public static bool DSTNotice() { return GPUserDefaults.BoolForKey("cal.dst.info", true); }
            public static void DSTNotice(bool value) { GPUserDefaults.SetBoolForKey("cal.dst.info", value); }
            public static bool NaksatraVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.naksatra", true); }
            public static void NaksatraVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.naksatra", value); }
            public static bool YogaVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.yoga", true); }
            public static void YogaVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.yoga", value); }
            public static bool FastingFlagVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.fastingflag", true); }
            public static void FastingFlagVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.fastingflag", value); }
            public static bool PaksaInfoVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.paksa", true); }
            public static void PaksaInfoVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.paksa", value); }
            public static bool RasiVisible() { return GPUserDefaults.BoolForKey("cal.sunrise.moon.rasi", true); }
            public static void RasiVisible(bool value) { GPUserDefaults.SetBoolForKey("cal.sunrise.moon.rasi", value); }
            public static bool EkadasiParanaDetails() { return GPUserDefaults.BoolForKey("cal.ekadasi.parana.details", true); }
            public static void EkadasiParanaDetails(bool value) { GPUserDefaults.SetBoolForKey("cal.ekadasi.parana.details", value); }
            public static int AnniversaryType() { return GPUserDefaults.IntForKey("cal.anniversary", 0); }
            public static void AnniversaryType(int value) { GPUserDefaults.SetIntForKey("cal.anniversary", value); }
            public static int HeaderType() { return GPUserDefaults.IntForKey("cal.headertype", 0); }
            public static void HeaderType(int value) { GPUserDefaults.SetIntForKey("cal.headertype", value); }


        }

        public class MasaList
        {
            public static bool MasaVisible(int masa)
            {
                return GPUserDefaults.BoolForKey("masa.visible." + masa.ToString(), true);
            }

            public static void SetMasaVisible(int masa, bool value)
            {
                GPUserDefaults.SetBoolForKey("masa.visible." + masa.ToString(), value);
            }
        }

        public class General
        {
            public static int CaturmasyaSystem() { return GPUserDefaults.IntForKey("gen.caturmasya", 1); }
            public static void CaturmasyaSystem(int value) { GPUserDefaults.SetIntForKey("gen.caturmasya", value); }
            public static bool CaturmasyaPurnima() { return CaturmasyaSystem() == 1; }
            public static bool CaturmasyaPratipat() { return CaturmasyaSystem() == 2; }
            public static bool CaturmasyaEkadasi() { return CaturmasyaSystem() == 3; }
            public static int FirstDayOfWeek() { return GPUserDefaults.IntForKey("gen.week.firstday", 0); }
            public static void FirstDayOfWeek(int value) { GPUserDefaults.SetIntForKey("gen.week.firstday", value); }
            public static bool DefaultEventsEditable() { return GPUserDefaults.BoolForKey("gen.defevents.editable", false); }
            public static void DefaultEventsEditable(bool value) { GPUserDefaults.SetBoolForKey("gen.defevents.editable", value); }
            public static bool HighlightEvenLines() { return GPUserDefaults.BoolForKey("gen.highlight.oddlines", true); }
            public static void HighlightEvenLines(bool value) { GPUserDefaults.SetBoolForKey("gen.highlight.oddlines", value); }
            public static bool TimeFormat24() { 
                GPGregorianTime.timeFormat24 = (GPUserDefaults.IntForKey("gen.timeformat", 1) == 1);
                return GPGregorianTime.timeFormat24; 
            }	//42
            public static void TimeFormat24(bool value) { 
                GPGregorianTime.timeFormat24 = value;
                GPUserDefaults.SetIntForKey("gen.timeformat", (value ? 1 : 0));
            }	//42
            public static int TimeFormat()
            {
                return GPUserDefaults.IntForKey("gen.timeformat", 1);
            }	//42
            public static void TimeFormat(int value)
            {
                GPUserDefaults.SetIntForKey("gen.timeformat", value);
            }	//42
            public static bool OldStyleFasting() { return FastingNotation() == 1; }
            public static int FastingNotation() { return GPUserDefaults.IntForKey("gen.fastingnotation", 0); }
            public static void FastingNotation(int value)
            {
                GPUserDefaults.SetIntForKey("gen.fastingnotation", value);
            }
            public static int NameMasaFormat() { return GPUserDefaults.IntForKey("gen.masaname.format", 0); }
            public static void NameMasaFormat(int value) { GPUserDefaults.SetIntForKey("gen.masaname.format", value); }
            public static int SankrantiNameFormat() { return GPUserDefaults.IntForKey("gen.sankranti.name.format", 0); }
            public static void SankrantiNameFormat(int value) { GPUserDefaults.SetIntForKey("gen.sankranti.name.format", value); }
        }
    }
}
