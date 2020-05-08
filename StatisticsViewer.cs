using UnityEngine;

public class StatisticsViewer : MonoBehaviour
{
    [SerializeField] private Visualizer visualizer;
    [SerializeField] private GameObject selectedViewImage;

    // the amount of x ledger lines of the original visualizer
    private int originalXLedgerLines;
    // the unit of the orignal visualizer
    private xUnit.Units originalUnit;
    // xMax of the orignal visualizer
    private int originalXMax;

    private void Start()
    {
        originalXLedgerLines = visualizer.GetXLedgerLineAmount();
        originalUnit = visualizer.GetUnit();
        originalXMax = visualizer.GetXMax();
    }

    // the original statstics, so when the visualizer is assigned a new statistics, the orignal does not get lost
    private Statistics originalStats;

    /// <summary>
    /// This method adjusts the ui view, so that only the values, that correspond to the particular view, can be seen
    /// </summary>
    public bool SetView(StatisticsView.View view)
    {
        if (originalStats == null) return false;

        float viewHours = StatisticsView.GetHours(view);

        // the date time of the last point of the original statstics converted to hours
        float lastTotalHours = visualizer.DateToHours(System.Convert.ToDateTime(originalStats.GetLastPoint().GetX()));
        // the date time of the first point of the original statstics converted to hours
        float firstTotalHours = visualizer.DateToHours(System.Convert.ToDateTime(originalStats.GetPoints()[0].GetX()));

        // If the current view includes points that the statistics cannot provide, ...
        if (lastTotalHours - viewHours < firstTotalHours && viewHours != -1)
        {
            // ... do not change the view.
            return false;
        }

        // set xMax that correspond to the view
        int xMax = (int)viewHours;
        if (xMax < 0)
        {
            xMax = originalXMax;
        }
        visualizer.SetXMax(xMax);

        // set the amount of x ledger lines that correspond to the view
        int xLedgerLineAmount = StatisticsView.GetXLedgerLineAmount(view);
        if (xLedgerLineAmount < 0)
        {
            xLedgerLineAmount = originalXLedgerLines;
        }
        visualizer.SetXLedgerLineAmount(xLedgerLineAmount);

        // set the x unit that correspond to the view
        xUnit.Units unit = StatisticsView.GetUnit(view);
        if (unit == xUnit.Units.NUMBER)
        {
            unit = originalUnit;
        }
        visualizer.SetUnit(unit);

        // draw all circles and lines that correspond to the new statistics
        visualizer.Repaint();

        return true;
    }

    /// <summary>
    /// This method updates the position of the selected view image.
    /// </summary>
    private void UpdateSelectedViewImagePosition(RectTransform rt)
    {
        LeanTween.moveLocalX(selectedViewImage, rt.localPosition.x, 0.4f).setEaseOutElastic();
    }

    /*********************************\
     * all setter methods
    \*********************************/
    public void SetOriginalStatistics(Statistics stats)
    {
        originalStats = stats;
    }
    public void LAST_24_HOURS(RectTransform sender)
    {
        if (SetView(StatisticsView.View.LAST_24_HOURS))
            UpdateSelectedViewImagePosition(sender);
    }
    public void LAST_7_DAYS(RectTransform sender)
    {
        if (SetView(StatisticsView.View.LAST_7_DAYS))
            UpdateSelectedViewImagePosition(sender);
    }
    public void LAST_28_DAYS(RectTransform sender)
    {
        if (SetView(StatisticsView.View.LAST_28_DAYS))
            UpdateSelectedViewImagePosition(sender);
    }
    public void LAST_3_MONTHS(RectTransform sender)
    {
        if (SetView(StatisticsView.View.LAST_3_MONTHS))
            UpdateSelectedViewImagePosition(sender);
    }
    public void LAST_6_MONTHS(RectTransform sender)
    {
        if (SetView(StatisticsView.View.LAST_6_MONTHS))
            UpdateSelectedViewImagePosition(sender);
    }
    public void DEFAULT_VIEW(RectTransform sender)
    {
        if (SetView(StatisticsView.View.DEFAULT))
            UpdateSelectedViewImagePosition(sender);
    }
    /*********************************\
    \*********************************/

}
