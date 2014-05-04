using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public enum GPCalculationOperation
    {
        None,
        Calendar,
        CalendarPlusCore,
        CalendarForTwoLocations,
        CoreEvents,
        MasaList,
        AppearanceDay,
        Today
    }


    public enum GPCalculationParameters
    {
        LocationProvider,
        LocationA,
        LocationB,
        StartWesternDate,
        EndWesternDate,
        StartVedicDate,
        EndVedicDate,
        StartYear,
        CountYear,
        WesternDateTime
    }
}
