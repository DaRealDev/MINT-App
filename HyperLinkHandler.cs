using UnityEngine;

public class HyperLinkHandler : MonoBehaviour
{
    /// <summary>
    /// This method makes the mobile device open the browser with the particular given url.
    /// </summary>
    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}
