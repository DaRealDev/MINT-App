using System.Collections.Generic;
using UnityEngine;

public class Statistics
{
    // the maximum and minimum value
    private float yMax, yMin;
    // These bools are set to true if a value has been assigned to yMax or yMin each.
    private bool yMinAssigned, yMaxAssigned;

    // the id is needed to store and read data
    private string id;

    // all points this statistics has
    private List<Point> points;

    // the visualizer showing the current statistics (can still be null)
    private Visualizer visualizer;

    // indicates whether data is recovered or not, so data that is recovered is not stored again
    private bool recoveringData;

    /// <summary>
    /// This stores data (points) in a list.
    /// </summary>
    public Statistics(string id, Visualizer visualizer = null)
    {
        this.id = id.Replace(" ", "");
        this.points = new List<Point>();
        this.visualizer = visualizer;
    }

    /// <summary>
    /// This method adds a point to this statistics.
    /// </summary>
    public void AddPoint(Point point, bool updateVisualizer = true)
    {
        AddPoint(point.GetX(), point.GetY(), updateVisualizer);
    }

    /// <summary>
    /// This method adds a point to this statistics.
    /// </summary>
    public void AddPoint(object x, float y, bool updateVisualizer = true)
    {
        // This bool shows if yMin or yMax has been changed.
        bool changed = false;

        // If no value has been assigned to yMin yet or y is less than yMin, ...
        if (yMinAssigned == false || yMin > y)
        {
            // ... override yMin, ...
            yMin = y;
            // ... store that yMin has been changed ...
            changed = true;
            // ... and store that a value has been assigned to yMin.
            yMinAssigned = true;
        }
        // If no value has not been assigned to yMax yet or y is greater than yMax, ...
        if (yMaxAssigned == false || yMax < y)
        {
            // ... override yMax, ...
            yMax = y;
            // ... store that yMax has been changed ...
            changed = true;
            // ... and store that a value has been assigned to yMax.
            yMaxAssigned = true;
        }

        // create the point
        Point point = new Point(x, y);

        // add it to the point list
        points.Add(point);

        // store the point only if the this point has not been stored yet
        if (recoveringData == false) StorePoint(point, points.Count - 1);

        // If a visualizer is assigned to this instance ...
        if (visualizer != null && updateVisualizer)
        {
            // ... inform the visualizer that a point was added.
            visualizer.OnPointAdded(changed);
        }
    }

    /// <summary>
    /// This method adds all particular points to the points list and makes the
    /// visualizer (if it exists) draw them to the ui afterwards.
    /// </summary>
    public void AddPoints(List<Point> newPoints)
    {
        // If points are wanted to be added, although the given point list is empty, do not continue.
        if (newPoints.Count <= 0) return;

        // Add each point to the points list, but make the visualizer not update.
        foreach (Point p in newPoints)
        {
            AddPoint(p, false);
        }

        // update when the transfer has finished
        visualizer.OnPointsAdded(newPoints.Count);
    }

    /*********************************\
     * storing and reading data
    \*********************************/

    /// <summary>
    /// This method stores a particular point, which can be recovered when the application is reopened.
    /// </summary>
    private void StorePoint(Point point, int index)
    {
        PlayerPrefs.SetString(id + "_" + index, point.GetString());
    }

    /// <summary>
    /// This method gets all the points that have been stored before and adds them to the points list.
    /// </summary>
    public void RecoverData()
    {
        recoveringData = true;

        int currentIndex = 0;

        List<Point> recoveredPoints = new List<Point>();

        // loop through the player prefs, until it has found all points
        while (true)
        {
            // get the point as a string, which is linked to this id and index
            string point = PlayerPrefs.GetString(id + "_" + currentIndex);

            // If the string is null or just empty, ...
            if (point == null || point.Replace(" ", "") == "")
            {
                // ... the end has been reached, so this statistics does not have no more points stored.
                // -> stop search
                break;
            }

            // create point from point string
            Point p = StringToPoint(point);
            // add recovered point to the local list 'recoveredPoints'
            recoveredPoints.Add(p);

            // increase the index to continue searching for all points
            currentIndex++;
        }

        // add all points to the points list
        AddPoints(recoveredPoints);

        recoveringData = false;
    }

    /// <summary>
    /// This method clears all points that are stored.
    /// </summary>
    public void ClearStorage()
    {
        int currentIndex = 0;

        // loop through the player prefs, until it has found all points
        while (true)
        {
            // get the point as a string, which is linked to this id and index
            string point = PlayerPrefs.GetString(id + "_" + currentIndex);

            // If the string is null or just empty, ...
            if (point == null || point.Replace(" ", "") == "")
            {
                // ... the end has been reached, so this statistics does not have no more points stored.
                // -> stop search
                break;
            }

            // delete the point
            PlayerPrefs.DeleteKey(id + "_" + currentIndex);

            // increase the index to continue searching for all points
            currentIndex++;
        }
    }

    private Point StringToPoint(string pointString)
    {
        // split the string at the semicolon, so the first path is the x object as a string and the other part is the y part is a string
        string[] xAndY = pointString.Split(';');

        // convert the y string to a float value
        float y = (float)System.Convert.ToDouble(xAndY[1]);

        object x = null;

        // If the x string is a date time object, ...
        if (xAndY[0].Contains(":"))
        {
            // ... convert the x string to a date time object
            x = System.Convert.ToDateTime(xAndY[0]);
        }
        // Otherwise, ...
        else
        {
            // ... convert the x string to a float value
            x = (float)System.Convert.ToDouble(xAndY[0]);
        }

        return new Point(x, y);
    }

    /*********************************\
    \*********************************/

    /// <summary>
    /// The Point class stores an x- and a y-value.
    /// </summary>
    public class Point
    {
        private object x;
        private float  y;

        public Point(object x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// This method converts the point to a readable string.
        /// </summary>
        public string GetString()
        {
            // If x is a type of date time, ...
            if (x.GetType() == System.DateTime.Now.GetType())
            {
                // ... convert the date time to a readable string.
                return System.Convert.ToDateTime(x) + ";" + y;
            }
            // Otherwise, treat the x object as a number:
            return System.Convert.ToDouble(x) + ";" + y;
        }

        /// <summary>
		/// This method evaluates what index this point is at. If it is not part of the list, -1 is returned.
		/// </summary>
		/// <param name="points"> the list in which the point is supposed to be in </param>
        public int GetIndexOfList(List<Point> points)
		{
            for (int i = 0; i < points.Count; i++)
			{
                if (points[i].GetX() == GetX() && points[i].GetY() == GetY())
				{
					return i;
				}
		    }
			return -1;
		}

        /*********************************\
         * all getter methods
        \*********************************/
        public object GetX()
        {
            return x;
        }
        public float GetY()
        {
            return y;
        }
        /*********************************\
        \*********************************/
    }

    /*********************************\
     * all getter methods
    \*********************************/
    public List<Point> GetPoints()
    {
        return points;
    }
    public float GetYMax()
    {
        return yMax;
    }
    public float GetYMin()
    {
        return yMin;
    }
    public Point GetLastPoint()
    {
        return points[points.Count - 1];
    }
    /*********************************\
    \*********************************/


    /*********************************\
     * all setter methods
    \*********************************/
    public void SetVisualizer(Visualizer visualizer)
    {
        this.visualizer = visualizer;
    }
    public void Clear()
    {
        this.points.Clear();
    }
    public void SetRecoveringState(bool recoveringData)
    {
        this.recoveringData = recoveringData;
    }
    /*********************************\
    \*********************************/
}
