using UnityEngine;
using UnityEngine.UI;

public class PointInformationPanel : MonoBehaviour
{
    [SerializeField] private Text xText, yText;

    /// <summary>
    /// This method makes the information panel disappear.
    /// </summary>
    public void Hide()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInBack().setDestroyOnComplete(true);
    }

    /*********************************\
     * all setter methods
    \*********************************/
    public void SetXText(string text)
    {
        xText.text = text;
    }
    public void SetYText(string text)
    {
        yText.text = text;
    }
    /*********************************\
    \*********************************/
}
