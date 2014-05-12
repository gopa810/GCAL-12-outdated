using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPConstants
    {
        public const int DW_SUNDAY = 0;
        public const int DW_MONDAY = 1;
        public const int DW_TUESDAY = 2;
        public const int DW_WEDNESDAY = 3;
        public const int DW_THURSDAY = 4;
        public const int DW_FRIDAY = 5;
        public const int DW_SATURDAY = 6;

        public const int CCE_SUN = 0x0001;
        public const int CCE_TIT = 0x0002;
        public const int CCE_NAK = 0x0004;
        public const int CCE_SNK = 0x0008;
        public const int CCE_CNJ = 0x0010;
        public const int CCE_SORT = 0x0100;
        public const int CCE_ALL = 0x0fff;

        public const int CCTYPE_DATE = 1;
        public const int CCTYPE_S_ARUN = 10;
        public const int CCTYPE_S_RISE = 11;
        public const int CCTYPE_S_NOON = 12;
        public const int CCTYPE_S_SET = 13;
        public const int CCTYPE_TITHI = 20;
        public const int CCTYPE_NAKS = 21;
        public const int CCTYPE_SANK = 22;
        public const int CCTYPE_CONJ = 23;
        public const int CoreEventMoonRise = 30;
        public const int CoreEventMoonSet = 31;
        public const int CCTYPE_SUNECLIPSE_PARTIAL_START = 40;
        public const int CCTYPE_SUNECLIPSE_PARTIAL_END = 41;
        public const int CCTYPE_SUNECLIPSE_FULL_START = 43;
        public const int CCTYPE_SUNECLIPSE_FULL_END = 44;
        public const int CCTYPE_SUNECLIPSE_CENTER = 45;
        public const int CCTYPE_MOONECLIPSE_PENUM_PART_START = 50;
        public const int CCTYPE_MOONECLIPSE_PENUM_PART_END = 51;
        public const int CCTYPE_MOONECLIPSE_PENUM_FULL_START = 52;
        public const int CCTYPE_MOONECLIPSE_PENUM_FULL_END = 53;
        public const int CCTYPE_MOONECLIPSE_MAIN_PART_START = 54;
        public const int CCTYPE_MOONECLIPSE_MAIN_PART_END = 55;
        public const int CCTYPE_MOONECLIPSE_MAIN_FULL_START = 56;
        public const int CCTYPE_MOONECLIPSE_MAIN_FULL_END = 57;
        public const int CCTYPE_MOONECLIPSE_CENTER = 58;
        public const int CCTYPE_TRAVELLING_START = 70;
        public const int CCTYPE_TRAVELLING_END = 72;

        public static int[] SUNECLIPSE_CONSTS = new int[] {
            CCTYPE_SUNECLIPSE_PARTIAL_START,
            CCTYPE_SUNECLIPSE_FULL_START,
            CCTYPE_SUNECLIPSE_CENTER,
            CCTYPE_SUNECLIPSE_FULL_END,
            CCTYPE_SUNECLIPSE_PARTIAL_END
        };

        public static int[] MOONECLIPSE_CONSTS = new int[] {
            CCTYPE_MOONECLIPSE_PENUM_PART_START,
            0,
            CCTYPE_MOONECLIPSE_MAIN_PART_START,
            CCTYPE_MOONECLIPSE_MAIN_FULL_START,
            CCTYPE_MOONECLIPSE_CENTER,
            CCTYPE_MOONECLIPSE_MAIN_FULL_END,
            CCTYPE_MOONECLIPSE_MAIN_PART_END,
            0,
            CCTYPE_MOONECLIPSE_PENUM_PART_END
        };

        public const int EV_NULL = 0x100;
        public const int EV_SUDDHA = 0x101;
        public const int EV_UNMILANI = 0x102;
        public const int EV_VYANJULI = 0x103;
        public const int EV_TRISPRSA = 0x104;
        public const int EV_UNMILANI_TRISPRSA = 0x105;
        public const int EV_PAKSAVARDHINI = 0x106;
        public const int EV_JAYA = 0x107;
        public const int EV_JAYANTI = 0x108;
        public const int EV_PAPA_NASINI = 0x109;
        public const int EV_VIJAYA = 0x110;

        public const int FAST_NULL = 0x0;
        public const int FAST_NOON = 0x201;
        public const int FAST_SUNSET = 0x202;
        public const int FAST_MOONRISE = 0x203;
        public const int FAST_DUSK = 0x204;
        public const int FAST_MIDNIGHT = 0x205;
        public const int FAST_EKADASI = 0x206;
        public const int FAST_DAY = 0x207;

        public const int FEAST_NULL = 0;
        public const int FEAST_TODAY_FAST_YESTERDAY = 1;
        public const int FEAST_TOMMOROW_FAST_TODAY = 2;

        public const int SPEC_RETURNRATHA = 3;
        public const int SPEC_HERAPANCAMI = 4;
        public const int SPEC_GUNDICAMARJANA = 5;
        public const int SPEC_GOVARDHANPUJA = 6;
        public const int SPEC_VAMANADVADASI = 7;
        public const int SPEC_VARAHADVADASI = 8;
        public const int SPEC_RAMANAVAMI = 9;
        public const int SPEC_JANMASTAMI = 10;
        public const int SPEC_RATHAYATRA = 11;
        public const int SPEC_GAURAPURNIMA = 12;
        public const int SPEC_NANDAUTSAVA = 13;
        public const int SPEC_MISRAFESTIVAL = 14;
        public const int SPEC_PRABHAPP = 15;
        public const int SPEC_RADHASTAMI = 16;

        public const int EP_TYPE_NULL = 0;
        public const int EP_TYPE_3DAY = 1;
        public const int EP_TYPE_4TITHI = 2;
        public const int EP_TYPE_NAKEND = 3;
        public const int EP_TYPE_SUNRISE = 4;
        public const int EP_TYPE_TEND = 5;


        public const double AU = 149597869.0;


        public const int DCEX_NAKSATRA_MIDNIGHT = 3;
        public const int DCEX_MOONRISE = 4;
        public const int DCEX_SUNRISE = 5;
    }
}
