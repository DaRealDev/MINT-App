using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Statistics;

/// <summary>
/// This helps to visualize statistics. X-values can only be numbers or DateTime. (Time will be shown in hours.)
/// </summary>
public class Visualizer : MonoBehaviour
{
    // the overall panel object, which the visualization is shown up in
    [SerializeField] private GameObject panel;
    // the distance between max value and the frame of the panel
    [SerializeField] private float yDistance = 20;

    // the line, which indicates the average y value
    [SerializeField] private Image averageLine;

    // the statistics this visualizer is using to show
    [SerializeField] private Statistics statistics;

    // the statistics viewer that helps the user to visualize only a part of the data in the entire graph view
    [SerializeField] private StatisticsViewer statisticsViewer;

    // indicates whether the visualization is drawn / shown or not
    private bool shown;

    // list containing all lines connecting the points
    private List<Image> lines;
    // list containing all circles representing points
    private List<RectTransform> circles;

    // the thickness of the lines
    [SerializeField] private float thickness = 2;
    // the radius of the circles representing points
    [SerializeField] private float circleRadius = 15f;

    // the game object which is a parent to all lines and circles, so when this is moved, all lines and circles are moved as well simultaneously
    private GameObject graph;
    // the parents of the line and circle game objects in the hierarchy
    private GameObject graphLines, graphCircles;
    // allows the user to scroll (autoscrolls if on the very right of graph)
    private GameObject scrollView;
    // the gameobject that is moved to scroll
    private GameObject scrollContent;

    // the color of the lines
    [SerializeField] private Color lineColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    // the color of the circles
    [SerializeField] private Color circleColor = new Color(1f, 0.5f, 0f);
    // the color of the x and y axis
    [SerializeField] private Color ledgerLineColor = new Color(1f, 1f, 1f, 0.1f);
    // the color of the x and y axis
    [SerializeField] private Color axisColor = new Color(1f, 1f, 1f, 0.25f);
    // the color of the average line
    [SerializeField] private Color averageLineColor = new Color(1f, 1f, 1f, 0.4f);

    // the dynamic and minimum scale of the x axis
    private float widthPerUnit, minWidthPerUnit;

    // the x and y axis panel
    private GameObject xAxisPanel, yAxisPanel, xLedgerPanel;

    // the amount of ledger lines on the y axis
    [SerializeField] private int yLedgerLines = 5;
    private List<Text> yTexts;
    // the amount of ledger lines on the x axis that can be seen on the screen at once
    [SerializeField] private int xLedgerLines = 5;
    private List<Text> xTexts;
    private List<RectTransform> xLines;
    // the height of each ledger line on the x axis
    [SerializeField] private float xLedgerLineHeight = 15;

    // the amount of decimal places to round to
    [Range(1, 4)] [SerializeField] private int roundDecimalPlaces = 1;

    // the size of the info text on the x and y axis
    [SerializeField] private int fontSize = 200;

    // the maximum unit value shown on the x axis (is neglected if xAxisIsLimited == false)
    [SerializeField] private int xMax;

    // indicates whether the user is able to scroll or not, as the graph is greater than it is shown on the screen
    private bool maxXReached;

    // indicates whether the graph should fill the content panel or can escape from it to the right
    private bool keepEntireGraph;

    // the unit of the x values (needed to know how to treat the x axis ledger line info)
    [SerializeField] private xUnit.Units unit = xUnit.Units.HOURS;

    // is true if the the graph is bigger than the ui and the first x info texts has been updated for the last time, otherwise it is set to false
    private bool maxReachedFirstUpdated;

    // the currently shown information panel
    private PointInformationPanel currentPointInformationPanel;

    /// <summary>
    /// This methods is used to set all important values that the visualizer needs to show the statistics
    /// </summary>
    public void SetValues(Statistics statistics)
    {
        // assign the statistics to the corresponding instance variable
        this.statistics = statistics;

        // set the visualizer of the statistics to this one
        statistics.SetVisualizer(this);

        // instantiate empty lists
        lines = new List<Image>();
        circles = new List<RectTransform>();

        // assign the scroll view
        scrollView = panel.transform.Find("Scroll View").gameObject;

        // assign the content panel
        scrollContent = scrollView.transform.Find("Viewport").transform.Find("Content").gameObject;

        // assign the empty graph game object
        graph = scrollContent.transform.Find("Graph").gameObject;

        // assign the line parent
        graphLines = graph.transform.Find("Lines").gameObject;
        // assign the circle parent
        graphCircles = graph.transform.Find("Circles").gameObject;

        // assign the axis panels
        xAxisPanel = panel.transform.Find("x-Axis Panel").gameObject;
        yAxisPanel = panel.transform.Find("y-Axis Panel").gameObject;

        // assign the x axis info panel
        xLedgerPanel = graph.transform.Find("xLedgerLines").gameObject;

        // set the distance that corresponds to one unit of x and the ui coordinate system
        minWidthPerUnit = scrollContent.GetComponent<RectTransform>().sizeDelta.x / xMax;
        // set the width scale on the x axis to the width at beginning (is changing over the time)
        widthPerUnit = scrollContent.GetComponent<RectTransform>().sizeDelta.x;

        // update the width per unit on the x axis
        UpdateWidthPerUnit(false);
    }

    /// <summary>
    /// This method make the visualizer draw all lines and circles (dynamically).
    /// </summary>
    public void Show()
    {
        // If the visualizer is already showing the graph, do not show the graph again.
        if (shown) return;

        shown = true;

        // update the width per unit on the x axis
        UpdateWidthPerUnit(false);

        // show the x and y axis panel and draw its ui
        DrawInfoUI();

        // draw all connecting lines
        for (int i = 0; i < statistics.GetPoints().Count - 1; i++)
        {
            // draw a line between this and the next point
            DrawLine(statistics.GetPoints()[i], statistics.GetPoints()[i + 1]);
        }

        // draw all circles
        for (int i = 0; i < statistics.GetPoints().Count; i++)
        {
            // a circle at the point
            DrawCircle(statistics.GetPoints()[i]);
        }
    }

    /// <summary>
    /// This method draws the legder lines and shows its values.
    /// </summary>
    private void DrawInfoUI()
    {
        // draw the y axis
        DrawYAxis();

        // the size of the panel where the graph can be seen in
        Vector2 viewSize = scrollView.GetComponent<RectTransform>().sizeDelta;

        // the size of the y panel
        Vector2 yPanelSize = yAxisPanel.GetComponent<RectTransform>().sizeDelta;
        Vector2 xPanelSize = xAxisPanel.GetComponent<RectTransform>().sizeDelta;

        // instantiate empty lists
        yTexts = new List<Text>();
        xTexts = new List<Text>();
        xLines = new List<RectTransform>();

        // the distance between each y ledger line
        float yPart = (viewSize.y - yDistance) / (float)(yLedgerLines - 1);
        // create all ledger lines and texts on the y axis
        for (int i = 0; i < yLedgerLines; i++)
        {
            // create a new ledger line object
            GameObject ledgerObj = Object.Instantiate(UIManager.GetInstance().imagePrefab, yAxisPanel.transform);
            // rename it
            ledgerObj.name = "LedgerLine" + yAxisPanel.transform.childCount;

            // calculate its position
            Vector2 pos = new Vector2(viewSize.x / 2 + yPanelSize.x / 2, yPart * i - yPanelSize.y / 2);

            // set its position and size
            RectTransform rt = ledgerObj.GetComponent<RectTransform>();
            rt.localPosition = pos;
            rt.sizeDelta = new Vector2(viewSize.x, thickness);

            // set its color
            Image img = ledgerObj.GetComponent<Image>();
            img.color = ledgerLineColor;

            // create text
            GameObject textObj = Object.Instantiate(UIManager.GetInstance().textPrefab, yAxisPanel.transform);
            // rename the text object
            textObj.name = "Info" + yAxisPanel.transform.childCount;

            // set the position
            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.localPosition = new Vector2(0, pos.y);

            // get and set the text, font size and color
            Text text = textObj.GetComponent<Text>();
            text.text = "";
            text.fontSize = fontSize;
            text.color = axisColor;

            // add the text to the list
            yTexts.Add(text);
        }

        float xStep = viewSize.x / (float)(xLedgerLines + 1);

        for (int i = 0; i < xLedgerLines; i++)
        {
            // calculate the position of the text info and the ledger line
            Vector2 pos = new Vector2(xStep * (i + 1) - xPanelSize.x / 2, 0);
            // create the info text and ledger line
            AddXInfo(pos.x);
        }

        // set the y and x info texts properly
        UpdateYInfoText();
        UpdateXInfoText();
    }

    /// <summary>
    /// This method creates a info text and ledger line on the x axis at the particular x position. At the end both object are added to the lists 'xLists' and 'xTexts'.
    /// </summary>
    private void AddXInfo(float xPos, object xObj = null)
    {
        // convert the x object to a readable string
        string infoText = "";
        if (xObj != null)
        {
            infoText = GetNameFromXObject(xObj);
        }

        RectTransform scrollRt = scrollContent.GetComponent<RectTransform>();

        // create a new ledger line object
        GameObject ledgerObj = Object.Instantiate(UIManager.GetInstance().imagePrefab, xLedgerPanel.transform);
        // rename the object
        ledgerObj.name = "LedgerLine" + xLedgerPanel.transform.childCount;

        Vector2 pos = new Vector2(xPos, -scrollRt.sizeDelta.y / 2 + xLedgerLineHeight / 2 + thickness / 2);

        // set its position and size
        RectTransform rt = ledgerObj.GetComponent<RectTransform>();
        rt.localPosition = pos;
        rt.sizeDelta = new Vector2(thickness, xLedgerLineHeight);

        // set its color
        Image img = ledgerObj.GetComponent<Image>();
        img.color = ledgerLineColor;

        // add the ledger line to the list
        xLines.Add(rt);

        // create text
        GameObject textObj = Instantiate(UIManager.GetInstance().textPrefab, xLedgerPanel.transform);
        // rename the object
        textObj.name = "Info" + xLedgerPanel.transform.childCount;

        // set the position
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.localPosition = new Vector2(pos.x, -scrollRt.sizeDelta.y / 2 - fontSize / 10);

        // get and set the text, font size and color
        Text text = textObj.GetComponent<Text>();
        text.text = infoText;
        text.fontSize = fontSize;
        text.color = axisColor;

        // add the text to the list
        xTexts.Add(text);

        // reparent it to the x info object
        textObj.transform.SetParent(xAxisPanel.transform);

        // make the x info text follow the x ledger line which is still a child of the content
        ObjectFollower objFollower = textObj.AddComponent<ObjectFollower>();
        objFollower.Follow(ledgerObj);
    }

    /// <summary>
    /// This method draws the y axis.
    /// </summary>
    private void DrawYAxis()
    {
        // If the x and y axis line already exist, do not draw them again.
        if (yAxisPanel.transform.Find("Line")) return;

        //****************** y axis ******************\\
        GameObject yLine = Object.Instantiate(UIManager.GetInstance().imagePrefab, yAxisPanel.transform);
        // rename the instantiated object
        yLine.name = "Line";

        RectTransform yRt = yLine.GetComponent<RectTransform>();
        // resize the line
        yRt.sizeDelta = new Vector2(thickness, yAxisPanel.GetComponent<RectTransform>().sizeDelta.y);
        // position the line on the right side
        yRt.localPosition = new Vector2(yAxisPanel.GetComponent<RectTransform>().sizeDelta.x / 2 - yRt.sizeDelta.x / 2, -thickness / 2);

        // set the color
        Image yImg = yLine.GetComponent<Image>();
        yImg.color = ledgerLineColor;
    }

    /// <summary>
    /// This method deletes and stops drawing all lines and circles.
    /// </summary>
    public void Close()
    {
        // If the visualizer is not showing the graph any more, do not delete the graph again.
        if (shown == false) return;

        shown = false;

        // delete all circles and lines
        for (int i = 0; i < graphLines.transform.childCount; i++)
        {
            Object.Destroy(graphLines.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < graphCircles.transform.childCount; i++)
        {
            Object.Destroy(graphCircles.transform.GetChild(i).gameObject);
        }
        circles.Clear();
        lines.Clear();
    }

    /// <summary>
    /// This method translates the statistics data to ui coordinates.
    /// </summary>
    private Vector2 GetPositionFromPoint(Point point)
    {
        // get the minimum and the maximum value of the statistics
        float yMin = statistics.GetYMin();
        float yMax = statistics.GetYMax();

        // If the statistics has neither yMin nor yMax calculated, ...
        if (statistics.GetPoints().Count < 2)
        {
            // ... return this default position.
            return new Vector2(0, 0);
        }

        // get the x object of the point
        object xObj = point.GetX();

        // the x value that is computed next
        float x = GetXValue(xObj);

        // get the y value of the point 
        float y = point.GetY();

        // get the size of the panel
        RectTransform rt = scrollView.GetComponent<RectTransform>();
        float height = rt.sizeDelta.y;
        float width = rt.sizeDelta.x;

        // compute the ui coordinate and return it
        float percentageOfHeight = (y - yMin) / (yMax - yMin);

        float uiX = x * widthPerUnit - width / 2;
        float uiY = -height / 2 + percentageOfHeight * (height - yDistance);

        return new Vector2(uiX, uiY);
    }

    /// <summary>
    /// This method calculates the Point from the particular ui position.
    /// </summary>
    private Point GetPointFromPosition(Vector2 pos)
    {
        // get the size of the content view
        Vector2 contentSize = scrollContent.GetComponent<RectTransform>().sizeDelta;

        // calculate what percentage the x and y pos are of the width and height
        float percX = (pos.x + scrollView.GetComponent<RectTransform>().sizeDelta.x / 2) / contentSize.x;
        float percY = pos.y / (contentSize.y - yDistance);

        // now multiply these percentages width the spans in the 'Point coordinate system'
        float y = (statistics.GetYMax() - statistics.GetYMin()) * percY + statistics.GetYMin();
        object x;

        // If the current unit is a time unit, ...
        if (unit != xUnit.Units.NUMBER)
        {
            // ... calculate the x value, as if it is time unit.
            x = ((System.DateTime)statistics.GetPoints()[0].GetX()).AddHours(GetXDifference() * percX);
        }
        // Otherwise ...
        else
        {
            // ... calculate the x value, as if it is a common number.
            x = (float)System.Convert.ToDouble(statistics.GetLastPoint().GetX()) + GetXDifference() * percX;
        }

        return new Point(x, y);
    }

    /// <summary>
    /// This method converts an x object to a float.
    /// </summary>
    private float GetXValue(object xObj)
    {
        // If x is type of 'DateTime', ...
        if (xObj.GetType() == System.DateTime.Now.GetType())
        {
            // ... convert this DateTime object to an amount of hours.
            return DateToHours((System.DateTime)xObj);
        }
        // Otherwise ...
        else
        {
            // ... convert x to a float.
            return (float)System.Convert.ToDouble(xObj);
        }
    }

    /// <summary>
    /// This method calculates the differnce between the very first and last x value.
    /// </summary>
    private float GetXDifference()
    {
        if (statistics.GetPoints().Count <= 1) return 1;
        return GetXValue(statistics.GetPoints()[statistics.GetPoints().Count - 1].GetX()) - GetXValue(statistics.GetPoints()[0].GetX());
    }

    /// <summary>
    /// This method converts DateTime to a float, which represents the hours since data collection started.
    /// </summary>
    public float DateToHours(System.DateTime dateTime)
    {
        // subtract the very first DateTime from the the given DateTime
        System.TimeSpan timeSpan = dateTime.Subtract((System.DateTime)statistics.GetPoints()[0].GetX());
        // round to three decimal places and return it
        return Mathf.Round((float)(timeSpan.TotalHours * 1000f)) / 1000f;
    }

    /// <summary>
    /// This method draws a circle at the particular point.
    /// </summary>
    private void DrawCircle(Point point)
    {
        // create a circle
        GameObject pointImage = Object.Instantiate(UIManager.GetInstance().pointPrefab, graphCircles.transform);
        // rename this object
        pointImage.name = "Circle_" + graphCircles.transform.childCount;

        // set the information for the point
        PointInfo pointInfo = pointImage.GetComponent<PointInfo>();
        pointInfo.SetVisualizer(this);
        pointInfo.SetIndex(point.GetIndexOfList(statistics.GetPoints()));

        RectTransform rt = pointImage.GetComponent<RectTransform>();
        // position it right there, where the point is located in the visualization
        rt.localPosition = GetPositionFromPoint(point);
        // resize the circle
        rt.sizeDelta = new Vector2(circleRadius, circleRadius);

        Image img = pointImage.transform.GetChild(0).GetComponent<Image>();
        // set the shape to a circular shape
        img.sprite = UIManager.GetInstance().circleSprite;
        // set the color
        img.color = circleColor;

        // add the RectTransform component to the circle list
        circles.Add(rt);
    }
    
    /// <summary>
    /// This method draws a line between two particular points. The line shows up in the visualization immediately.
    /// </summary>
    private void DrawLine(Point point1, Point point2)
    {
        // create image object
        GameObject lineObject = Object.Instantiate(UIManager.GetInstance().imagePrefab, graphLines.transform);
        // rename this object
        lineObject.name = "Line_" + graphLines.transform.childCount;

        // get and store its image component
        Image img = lineObject.GetComponent<Image>();
        lines.Add(img);

        // set the color of the image
        img.color = lineColor;

        // set the rotation, position and size of the image correctly
        SetLineTransform(img.GetComponent<RectTransform>(), point1, point2);
    }

    /// <summary>
    /// This sets the rotation, position and length of the line between two particular points.
    /// </summary>
    private void SetLineTransform(RectTransform rt, Point point1, Point point2)
    {
        // set the thickness of the image
        rt.sizeDelta = new Vector2(thickness, thickness);

        // translate the statistics points to points that meet the ui coordinate system
        Vector2 pos1 = GetPositionFromPoint(point1);
        Vector2 pos2 = GetPositionFromPoint(point2);

        // set its position (right between point1 and point2)
        Vector2 middlePos = (pos1 + pos2) / 2;
        rt.localPosition = middlePos;

        // set the thickness and length of the line
        rt.sizeDelta = new Vector2(Vector2.Distance(pos1, pos2), thickness);

        // rotate the line, so point1 and point2 are connected
        float angle = Mathf.Atan((pos2.y - pos1.y) / (pos2.x - pos1.x)) * 57.2958f;
        rt.localEulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// This method is called, when a new point is added to the statistics.
    /// </summary>
    /// <param name="changed"> is true if yMin or yMax changed </param>
    public void OnPointAdded(bool changed)
    {
        // draw and update all lines and circles only if the visualizer is supposed to
        if (shown == false) return;

        // update the width per unit on the x axis if necessary
        UpdateWidthPerUnit(changed);

        // If the statistics has more than 1 point, ...
        if (statistics.GetPoints().Count > 1)
        {
            // ... draw a line between the last two points.
            DrawLine(statistics.GetPoints()[statistics.GetPoints().Count - 2], statistics.GetLastPoint());
        }

        // draw a circle at the ui positition of the latest added point
        DrawCircle(statistics.GetLastPoint());

        // update the size and the position of the scroll view
        UpdateScrollView();

        // update the x info texts
        UpdateXInfoText();

        // update the y position of the average line
        UpdateAverageLine();
    }

    /// <summary>
    /// This method is called when more than one point has been added to the statistics.
    /// </summary>
    /// <param name="amount"> the amount of new points </param>
    public void OnPointsAdded(int amount)
    {
        // draw and update all lines and circles only if the visualizer is supposed to
        if (shown == false) return;

        // update the width per unit on the x axis if necessary
        UpdateWidthPerUnit(true);

        // draw all circles and lines, until the second last point of the list has been reached
        // then just call the 'OnPointAdded' method, which finishes it up by drawing the last point
        for (int i = statistics.GetPoints().Count - amount; i < statistics.GetPoints().Count - 1; i++)
        {
            // If the current point is not the very first one, ...
            if (i > 0)
            {
                // ... draw a line between the last two points.
                DrawLine(statistics.GetPoints()[i - 1], statistics.GetPoints()[i]);
            }

            // draw a circle at the ui positition of the latest added point
            DrawCircle(statistics.GetPoints()[i]);
        }

        OnPointAdded(true);
    }

    /// <summary>
    /// This method resizes and repositions the scroll view, so the user is able to scroll to the right and
    /// to the left to see the entire graph.
    /// </summary>
    private void UpdateScrollView()
    {
        // If the graph can be larger than the width of the content panel, ...
        if (keepEntireGraph == false)
        {
            // ... and if the maximum x value is reached, ...
            if (maxXReached)
            {
                // ... resize the scroll view, so scrolling can be done.
                RectTransform rt = scrollContent.GetComponent<RectTransform>();

                float scrollViewWidth = scrollView.GetComponent<RectTransform>().sizeDelta.x;
                float x = Mathf.Abs(GetPositionFromPoint(statistics.GetPoints()[0]).x
                                            - GetPositionFromPoint(statistics.GetLastPoint()).x);

                float onTheVeryRightX = -1 * rt.sizeDelta.x + scrollViewWidth / 2;

                // If the scroll view is not on the very right, ...
                if ((int)onTheVeryRightX >= (int)rt.localPosition.x - 10)
                {
                    // ... reposition the scroll view to the very right.
                    rt.localPosition = new Vector2(-(1.5f * x - scrollViewWidth), rt.localPosition.y);
                }

                // adapt the width of the content panel, so that the previous added point cann be seen
                // on the very right
                rt.sizeDelta = new Vector2(x, rt.sizeDelta.y);
            }
        }
    }

    /// <summary>
    /// This method sets the bool 'keepEntireGraph' to true or false. This bool indicates whether the whole graph should be seen in the content panel or not and scrolling is allowed.
    /// </summary>
    public void KeepEntireGraph(bool keep)
    {
        keepEntireGraph = keep;
        UpdateWidthPerUnit(false);
    }

    /// <summary>
    /// This method recalculates the width per unit on the x axis if necessary and updates the whole
    /// visualization afterwards.
    /// </summary>
    private void UpdateWidthPerUnit(bool changed)
    {
        // If the minimum width scale has not been reached yet or the graph should be kept in the content panel, ...
        if (xMaxIsReached() == false || keepEntireGraph)
        {
            // ... calculate this scale.
            float calculatedScale = scrollView.GetComponent<RectTransform>().sizeDelta.x / GetXDifference();

            // If the calculated scale is less than the minimum width, ...
            if (calculatedScale <= minWidthPerUnit && keepEntireGraph == false)
            {
                // ... alter the calculated scale value then.
                calculatedScale = minWidthPerUnit;
                maxXReached = true;
            }

            // assign the new scale
            widthPerUnit = calculatedScale;

            // update the entire graph, as the width per unit on the x axis has changed
            UpdateGraph();
            return;
        }
        // If yMin or yMax changed, update the graph.
        if (changed) UpdateGraph();
    }

    /// <summary>
    /// This method checks whether the graph can be seen in the scroll view entirely or not.
    /// </summary>
    private bool xMaxIsReached()
    {
        return widthPerUnit <= minWidthPerUnit;
    }

    /// <summary>
    /// This method updates position, rotation and thickness of all lines and circles. It is called, when yMin or yMax of the statistics changed.
    /// </summary>
    public void UpdateGraph()
    {
        // If the statistics has more than one point, ...
        if (statistics.GetPoints().Count <= 1) return;
        // ... go on and update the graph.

        // If the visualization does not have any circles or lines, an update is not necesarry.
        if (circles == null || lines == null) return;

        // Iterate over all circles
        for (int i = 0; i < circles.Count; i++)
        {
            // update the position of the current circle
            RectTransform rt = circles[i];
            rt.localPosition = GetPositionFromPoint(statistics.GetPoints()[i]);

            // If i is 1 or greater, ...
            if (i > 0)
            {
                // ... update the RectTransform component of the line connecting this and the previous point.
                SetLineTransform(lines[i - 1].GetComponent<RectTransform>(), statistics.GetPoints()[i - 1], statistics.GetPoints()[i]);
            }
        }

        // update the info values on the y axis
        UpdateYInfoText();
    }

    /// <summary>
    /// This method updates the info texts on the y axis.
    /// </summary>
    private void UpdateYInfoText()
    {
        // the y value steps between each ledger line
        float steps = (statistics.GetYMax() - statistics.GetYMin()) / (float)(yTexts.Count - 1);

        for (int i = 0; i < yTexts.Count; i++)
        {
            // update the values
            yTexts[i].text = Round(statistics.GetYMin() + steps * i) + "";
        }
    }

    /// <summary>
    /// This method updates the info texts on the x axis, when xMax has not been reached yet.
    /// </summary>
    private void UpdateXInfoText()
    {
        if (statistics == null) return;
        // If the statistics does not have any points, stop.
        if (statistics.GetPoints() == null) return;
        if (statistics.GetPoints().Count == 0) return;
        // If the width per unit is adjusting, ...
        if (xMaxIsReached() == false)
        {
            // update the x info texts
            UpdateXInfoTextFullShown();
        }
        // Otherwise ...
        else
        {
            // If the graph is about to become bigger than the ui, update the x info texts for the last time.
            if (maxReachedFirstUpdated == false)
            {
                // update the first x info texts
                UpdateXInfoTextFullShown(xMax / (float)(xLedgerLines + 1));

                // do not update the first x info texts not any more
                maxReachedFirstUpdated = true;
            }

            // ... add a ledger line and info text to the x axis, if the distance between the last
            // added ledger line and the right edge of the content view is greater than the distance between each
            // ledger line on the x axis:

            // the width of the scroll view
            float scrollViewWidth = scrollView.GetComponent<RectTransform>().sizeDelta.x;
            // the x pos of the last added ledger line
            float lastXInfoPosX = xLines[xLines.Count - 1].GetComponent<RectTransform>().localPosition.x;
            // the distance between the this ledger line and the right edge
            float diff = scrollContent.GetComponent<RectTransform>().sizeDelta.x - lastXInfoPosX - scrollViewWidth / 2;

            // the distance between each ledger line
            float xStep = scrollViewWidth / (float)(xLedgerLines + 1);

            // fill the right gap with ledger lines
            for (int i = 0; i < (int)(diff / xStep); i++)
            {
                // the (x) pos of the info text and ledger line
                Vector2 pos = new Vector2(lastXInfoPosX + xStep * (i + 1), 0);

                // create the info text and ledger line
                AddXInfo(pos.x, GetPointFromPosition(pos).GetX());
            }
        }
    }

    /// <summary>
    /// This method updates the x info texts. This should only be called, if the graph is entirely shown.
    /// </summary>
    private void UpdateXInfoTextFullShown()
    {
        UpdateXInfoTextFullShown(GetXDifference() / (float)(xLedgerLines + 1));
    }

    /// <summary>
    /// This method updates the x info texts. This should only be called, if the graph is entirely shown.
    /// </summary>
    private void UpdateXInfoTextFullShown(float xStep)
    {
        // ... and if the unit is a time unit, ...
        if (unit != xUnit.Units.NUMBER)
        {
            // ... set the x info texts.
            System.DateTime startDt = (System.DateTime)statistics.GetPoints()[0].GetX();

            // ... update the x info text.
            for (int i = 0; i < xTexts.Count; i++)
            {
                xTexts[i].text = GetNameFromXObject(startDt.AddHours(xStep * (i + 1)));
            }
        }
        // Otherwise ...
        else
        {
            float startValue = (float)System.Convert.ToDouble(statistics.GetPoints()[0].GetX());

            // ... just update the x info text with common numbers.
            for (int i = 0; i < xTexts.Count; i++)
            {
                xTexts[i].text = Round(startValue + xStep * (i + 1)) + "";
            }
        }
    }

    /// <summary>
    /// This method gives the name of an x object corresponding to the current unit on the x axis back.
    /// </summary>
    private string GetNameFromXObject(object xObj)
    {
        // If the unit of the x object is a time unit, ...
        if (unit != xUnit.Units.NUMBER)
        {
            // ... get the name of the date time corresponding to the current unit.
            return xUnit.GetName(unit, (System.DateTime)xObj);
        }

        // Otherwise, return the object as a string.
        return Round((float)System.Convert.ToDouble(xObj)).ToString();
    }

    /// <summary>
    /// This method updates the average line position.
    /// </summary>
    private void UpdateAverageLine()
    {
        // If the average line image is not assigned, do not calculate anything and stop right here.
        if (averageLine == null) return;
        // If this visualizer does not have a statistics assigned, stop.
        if (statistics == null) return;
        // If the statistics does not have any points, stop.
        if (statistics.GetPoints() == null) return;
        if (statistics.GetPoints().Count == 0) return;

        // get the rect transform component of the average line image
        RectTransform rt = averageLine.GetComponent<RectTransform>();

        // If the average line is still hidden, ...
        if (averageLine.gameObject.activeSelf == false)
        {
            // ... show it, ...
            averageLine.gameObject.SetActive(true);
            // ... set its thickness ...
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, thickness);
            // ... and set its color.
            averageLine.color = averageLineColor;
        }

        // the y pos of the average line corresponding to the average of the y values of the statistics
        float yPos = GetPositionFromPoint(new Point(0, GetAverageYValue())).y;

        // update the position
        averageLine.transform.localPosition = new Vector2(averageLine.transform.localPosition.x, yPos);
    }

    /// <summary>
    /// This method calculates the average y value.
    /// </summary>
    private float GetAverageYValue()
    {
        // the sum of all y values
        float sum = 0;

        // add all y values to the sum variable
        for (int i = 0; i < statistics.GetPoints().Count; i++)
        {
            sum += statistics.GetPoints()[i].GetY();
        }

        // divide the sum with the amount of points and return it
        return sum / (float)statistics.GetPoints().Count;
    }

    /// <summary>
    /// This method repaints the entire graph.
    /// </summary>
    public void Repaint()
    {
        HideCurrentPointInformationPanel();

        // delete all circles and lines
        for (int i = 0; i < graphLines.transform.childCount; i++)
        {
            Object.Destroy(graphLines.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < graphCircles.transform.childCount; i++)
        {
            Object.Destroy(graphCircles.transform.GetChild(i).gameObject);
        }
        lines = new List<Image>();
        circles = new List<RectTransform>();

        RectTransform rt = scrollContent.GetComponent<RectTransform>();
        rt.sizeDelta = scrollView.GetComponent<RectTransform>().sizeDelta;

        // reset the distance that corresponds to one unit of x and the ui coordinate system
        minWidthPerUnit = scrollContent.GetComponent<RectTransform>().sizeDelta.x / xMax;
        // reset the width scale on the x axis to the width at beginning (is changing over the time)
        widthPerUnit = scrollContent.GetComponent<RectTransform>().sizeDelta.x;

        // update the width per unit on the x axis
        UpdateWidthPerUnit(false);

        // delete all x info texts and legder lines
        for (int i = 0; i < xLedgerPanel.transform.childCount; i++)
        {
            Destroy(xLedgerPanel.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < xAxisPanel.transform.childCount; i++)
        {
            Destroy(xAxisPanel.transform.GetChild(i).gameObject);
        }

        xTexts = new List<Text>();
        xLines = new List<RectTransform>();

        // create all the x ledger line and info texts again
        float xStep = scrollView.GetComponent<RectTransform>().sizeDelta.x / (float)(xLedgerLines + 1);
        for (int i = 0; i < xLedgerLines; i++)
        {
            // calculate the position of the text info and the ledger line
            Vector2 pos = new Vector2(xStep * (i + 1) - xAxisPanel.GetComponent<RectTransform>().sizeDelta.x / 2, 0);
            // create the info text and ledger line
            AddXInfo(pos.x);
        }

        // reset the y info texts
        for (int i = 0; i < yTexts.Count; i++)
        {
            yTexts[i].GetComponent<Text>().text = "";
        }

        // reset the average line
        averageLine.gameObject.SetActive(false);

        // get the points of the statistics
        Point[] points = new Point[statistics.GetPoints().Count];
        statistics.GetPoints().CopyTo(points);

        // make the statistics not to store the points, as it already has them
        statistics.SetRecoveringState(true);

        // delete all the points from the statistics
        statistics.Clear();

        // add the points to the statistics again, so OnPointAdded is called again and the graph is repainted
        statistics.AddPoints(new List<Point>(points));

        // make the graph store all the data again
        statistics.SetRecoveringState(false);

        // reposition the first circle, as this circle is positioned wrong at first
        circles[0].localPosition = GetPositionFromPoint(statistics.GetPoints()[0]);

        UpdateYInfoText();
        // update the first x info texts
        UpdateXInfoTextFullShown(xMax / (float)(xLedgerLines + 1));

        // reposition the scroll content to the very right
        float scrollViewWidth = scrollView.GetComponent<RectTransform>().sizeDelta.x;
        float x = circles[circles.Count - 1].localPosition.x + scrollViewWidth / 2 + circleRadius / 2;
        LeanTween.moveLocalX(scrollContent.gameObject, -(1.5f * x - scrollViewWidth), 0.1f);

        // stop the content view if it is moving
        scrollView.GetComponent<ScrollRect>().velocity = Vector2.zero;
    }

    /// <summary>
    /// This method is called, when the user presses a finger onto the scroll view.
    /// </summary>
    public void OnFingerDown()
    {
        HideCurrentPointInformationPanel();
    }

    /// <summary>
    /// This method shows the information of the point that has been clicked on by the user.
    /// </summary>
    /// <param name="pointObject"> the game object that has been clicked on </param>
    public void ShowInfoOfPoint(GameObject pointObject, int index)
    {
        HideCurrentPointInformationPanel();

        // create the information prefab at the position of the point
        GameObject info = Instantiate(UIManager.GetInstance().pointInfoPrefab, transform); //pointObject.transform.parent);

        // set the position
        RectTransform rt = info.GetComponent<RectTransform>();
        // rt.localPosition = pointObject.GetComponent<RectTransform>().localPosition;
        rt.position = pointObject.GetComponent<RectTransform>().position;

        // set its size to 0
        info.transform.localScale = Vector3.zero;

        // set the x and y texts
        PointInformationPanel pointInformationPanel = info.GetComponent<PointInformationPanel>();
        pointInformationPanel.SetXText(xUnit.GetDetailedString(unit, statistics.GetPoints()[index].GetX()));
        pointInformationPanel.SetYText(statistics.GetPoints()[index].GetY().ToString());

        currentPointInformationPanel = pointInformationPanel;

        // make the info pop up
        LeanTween.scale(info, new Vector3(1, 1, 1), 0.5f).setEaseOutElastic();
    }

    /// <summary>
    /// This method makes the current point information panel disappear.
    /// </summary>
    private void HideCurrentPointInformationPanel()
    {
        // If an information panel is already visible, ...
        if (currentPointInformationPanel != null)
        {
            // ... hide it.
            currentPointInformationPanel.Hide();
        }
    }

    /// <summary>
    /// This method rounds on the number of decimal places defined  in the inspector.
    /// </summary>
    private float Round(float value)
    {
        return Mathf.Round(value * Mathf.Pow(10, roundDecimalPlaces)) / Mathf.Pow(10, roundDecimalPlaces);
    }

    /*********************************\
     * all getter methods
    \*********************************/
    public StatisticsViewer GetStatisticsViewer()
    {
        return statisticsViewer;
    }
    public int GetXLedgerLineAmount()
    {
        return xLedgerLines;
    }
    public xUnit.Units GetUnit()
    {
        return unit;
    }
    public int GetXMax()
    {
        return xMax;
    }
    /*********************************\
    \*********************************/

    /*********************************\
     * all setter methods
    \*********************************/
    public void SetXLedgerLineAmount(int amount)
    {
        if (amount >= 2) xLedgerLines = amount;
    }
    public void SetUnit(xUnit.Units unit)
    {
        this.unit = unit;
    }
    public void SetXMax(int xMax)
    {
        this.xMax = xMax;
    }
    /*********************************\
    \*********************************/
}
