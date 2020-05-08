using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // the UIManager variable that is needed to access the objects of this class
    private static UIManager _instance;

    // the different visualizer for the data that is collected by the 'weather station'
    [SerializeField] private Visualizer voltageVis, temperatureVis, humidityVis;
    private Statistics voltageStats, temperatureStats, humidityStats;

    [SerializeField] private InputField houseHeightInput;

    /// <summary>
    /// This method is called when the application has been started.
    /// </summary>
    private void Awake()
    {
        _instance = this;
    }
     /*
    /// <summary>
    /// This method is called when the application starts and prepares the data.
    /// </summary>
    private void Start()
    {
        voltageStats = new Statistics("Voltage");
        SetUpStatistics(voltageVis, voltageStats);

        temperatureStats = new Statistics("Temperature");
        SetUpStatistics(temperatureVis, temperatureStats);

        humidityStats = new Statistics("Humidity");
        SetUpStatistics(humidityVis, humidityStats);

        // -- delete from here --

        if (voltageStats.GetPoints().Count == 0) tempDT = DateTime.Now;
        else tempDT = ((DateTime)voltageStats.GetLastPoint().GetX());
        
        for (int i = 0; i < 1; i++)
        {
            DateTime dt = tempDT.Add(new TimeSpan(0, i, UnityEngine.Random.Range(0, 59), 0));
            voltageStats.AddPoint(dt, UnityEngine.Random.Range(0, 100));

            /*
            DateTime dt = tempDT.Add(new TimeSpan(0, i, 30, 0));
            stats.AddPoint(dt, Mathf.Sin(i * 0.5f));
            -
        }

        -
        tempDT = DateTime.Now;
        if (voltageStats.GetPoints().Count != 0) tempDT = ((DateTime)voltageStats.GetLastPoint().GetX());
        StartCoroutine(Temp(100, voltageStats));
        -
        // -- end delete --
    }*/

    /// <summary>
    /// This method is called when the application starts and prepares the data.
    /// </summary>
    private void Start()
    {
        voltageStats = new Statistics("Voltage");
        SetUpStatistics(voltageVis, voltageStats);

        temperatureStats = new Statistics("Temperature");
        SetUpStatistics(temperatureVis, temperatureStats);

        humidityStats = new Statistics("Humidity");
        SetUpStatistics(humidityVis, humidityStats);
    }

    /// <summary>
    /// This method collects the stored data of the statistics, initializes the
    /// corresponding visualizer and shows the data.
    /// </summary>
    private void SetUpStatistics(Visualizer visualizer, Statistics statistics)
    {
        visualizer.SetValues(statistics);
        visualizer.Show();

        visualizer.GetStatisticsViewer().SetOriginalStatistics(statistics);

        statistics.ClearStorage();
        statistics.RecoverData();
    }

    /// <summary>
    /// This method is called when the house height input has been changed.
    /// </summary>
    public void OnHouseHeightChanged()
    {
        
    }

    private DateTime tempDT; // temp

    // temp
    private System.Collections.IEnumerator Temp(int max, Statistics stats, int i = 0)
    {
        yield return new WaitForSeconds(0f);

        DateTime dt = tempDT.Add(new TimeSpan(0, i, UnityEngine.Random.Range(0, 59), 0));
        stats.AddPoint(dt, UnityEngine.Random.Range(0, 100));

        /*
        DateTime dt = tempDT.Add(new TimeSpan(0, i, 30, 0));
        stats.AddPoint(dt, Mathf.Sin(i * 0.5f));
        //*/

        i++;

        if (max > i)
        {
            StartCoroutine(Temp(max, stats, i));
        }
        else
        {
            voltageVis.Repaint();
        }
    }

    /**********************************************\
     * all ui objects that are needed externally
    \**********************************************/

    // the overall user interface
    public Canvas ui;

    // the prefab with an Image component on it
    public GameObject imagePrefab;
    // the prefab for points of a visualizer
    public GameObject pointPrefab;
    // the prefab with an Text component on it
    public GameObject textPrefab;
    // a sprite shaped as a circle
    public Sprite circleSprite;

    // the input of the height of the house
    public InputField inputHeightOfHouse;

    // the prefab for showing the information about a point
    public GameObject pointInfoPrefab;

    /**********************************************\
    \**********************************************/

    /*********************************\
     * all getter methods
    \*********************************/
    public static UIManager GetInstance()
    {
        return _instance;
    }
    public Statistics GetVoltageStatistics()
    {
        return voltageStats;
    }
    public Statistics GetTemperatureStatistics()
    {
        return temperatureStats;
    }
    public Statistics GetHumidityStatistics()
    {
        return humidityStats;
    }
    /*********************************\
    \*********************************/
}
