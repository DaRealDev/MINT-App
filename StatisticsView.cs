public static class StatisticsView
{
    /// <summary>
    /// The different views that can be selected and seen in the visualizer afterwards
    /// </summary>
    public enum View
    {
        LAST_24_HOURS, LAST_7_DAYS, LAST_28_DAYS, LAST_3_MONTHS, LAST_6_MONTHS, DEFAULT
    }

    /// <summary>
    /// This method converts the amount of hours that correspond to the specific time span
    /// </summary>
    public static float GetHours(View view)
    {
        switch (view)
        {
            case View.LAST_24_HOURS: return 24;
            case View.LAST_7_DAYS: return 24 * 7;
            case View.LAST_28_DAYS: return 24 * 7 * 4;
            case View.LAST_3_MONTHS: return 24 * 7 * 4 * 3;
            case View.LAST_6_MONTHS: return 24 * 7 * 4 * 6;
            default: return -1;
        }
    }

    /// <summary>
    /// This method returns the most appropiate amount of x ledger lines as regards the particular view.
    /// </summary>
    public static int GetXLedgerLineAmount(View view)
    {
        switch (view)
        {
            case View.LAST_24_HOURS: return 6 - 1;
            case View.LAST_7_DAYS: return 7 - 1;
            case View.LAST_28_DAYS: return 4 - 1;
            case View.LAST_3_MONTHS: return 3 - 1;
            case View.LAST_6_MONTHS: return 6 - 1;
            default: return -1;
        }
    }

    /// <summary>
    /// This method gives back the time unit which is the most appropiate one for the particular view.
    /// </summary>
    public static xUnit.Units GetUnit(View view)
    {
        switch (view)
        {
            case View.LAST_24_HOURS: return xUnit.Units.HOURS;
            case View.LAST_7_DAYS: return xUnit.Units.DAYS;
            case View.LAST_28_DAYS: return xUnit.Units.DAYS;
            case View.LAST_3_MONTHS: return xUnit.Units.MONTHS;
            case View.LAST_6_MONTHS: return xUnit.Units.MONTHS;
            default: return xUnit.Units.NUMBER;
        }
    }
}
