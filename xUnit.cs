using System;

public static class xUnit
{
    /// <summary>
    /// The different units that can be possibly chosen for the x axis
    /// </summary>
    public enum Units
    {
        HOURS, DAYS, MONTHS, YEARS, NUMBER
    }

    /// <summary>
    /// This metehod converts the particular unit to its amount of hours.
    /// </summary>
    public static int ToHours(this Units unit)
    {
        switch (unit)
        {
            case Units.HOURS:   return 1;
            case Units.DAYS:    return 24;
            case Units.MONTHS:  return 24 * 28;
            case Units.YEARS:   return 24 * 365;
            default:            return 1;
        }
    }

    /// <summary>
    /// This method converts a date time to its string corresponding to the chosen unit.
    /// </summary>
    public static string GetName(this Units unit, DateTime dateTime)
    {
        switch (unit)
        {
            case Units.HOURS:   return dateTime.ToShortTimeString();
            case Units.DAYS:    return dateTime.Day.ToString() + "." + dateTime.Month + " (" + GetDayName(dateTime) + ")";
            case Units.MONTHS:  return GetMonthName(dateTime.Month) + " " + dateTime.Year;
            case Units.YEARS:   return dateTime.Year.ToString();
            default:            return null;
        }
    }

    /// <summary>
    /// This method returns the German abbreviation of the day of the week of the particular date time.
    /// </summary>
    private static string GetDayName(DateTime dateTime)
    {
        switch (dateTime.DayOfWeek)
        {
            case DayOfWeek.Monday:      return "Mo";
            case DayOfWeek.Tuesday:     return "Di";
            case DayOfWeek.Wednesday:   return "Mi";
            case DayOfWeek.Thursday:    return "Do";
            case DayOfWeek.Friday:      return "Fr";
            case DayOfWeek.Saturday:    return "Sa";
            case DayOfWeek.Sunday:      return "So";
            default: return "?";
        }
    }

    /// <summary>
    /// This method converts an the index of a month to its German abbreviation.
    /// </summary>
    private static string GetMonthName(int monthIndex)
    {
        switch (monthIndex)
        {
            case 1:  return "Jan.";
            case 2:  return "Feb.";
            case 3:  return "März";
            case 4:  return "Apr.";
            case 5:  return "Mai";
            case 6:  return "Jun.";
            case 7:  return "Jul.";
            case 8:  return "Aug.";
            case 9:  return "Sept.";
            case 10: return "Okt.";
            case 11: return "Nov.";
            case 12: return "Dez.";
            default: return monthIndex.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetDetailedString(this Units unit, object xObj)
    {
        if (unit == Units.NUMBER)
        {
            return xObj.ToString();
        }
        DateTime dt = (DateTime)xObj;

        return dt.ToShortDateString() + "  " + dt.ToShortTimeString();
    }
}
