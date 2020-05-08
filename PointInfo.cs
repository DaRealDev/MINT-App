using UnityEngine;

public class PointInfo : MonoBehaviour
{
    private Visualizer visualizer;
    private int index = -1;

    /// <summary>
    /// This method calls the "ShowInfoOfPoint" method of its corresponding visualizer component.
    /// </summary>
    public void ShowInfo()
    {
        if (visualizer != null && index >= 0) visualizer.ShowInfoOfPoint(gameObject, index);
    }

    /*********************************\
    * all setter methods
    \*********************************/
    public void SetVisualizer(Visualizer visualizer)
    {
        this.visualizer = visualizer;
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }
    /*********************************\
    \*********************************/
}
