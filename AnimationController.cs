using UnityEngine;

/// <summary>
/// This class cares about all ui animations which are started when an event is triggered and the particular method is called.
/// </summary>
public class AnimationController : MonoBehaviour
{
    // the time an animation needs to finish
    [SerializeField] private float duration = 0.2f;

    // the instance to create a singleton of this class
    private static AnimationController instance;
    private void Start() { instance = this; }
    public static AnimationController GetInstance() { return instance; }

    /// <summary>
    /// This method moves the particular game object to the right (the width of th screen).
    /// </summary>
    public void ToRight(GameObject gameObj)
    {
        // If the game object is referenced, ...
        if (gameObj != null)
        {
            // ... animate this game object.
            LeanTween.moveLocalX(gameObj, UIManager.GetInstance().ui.GetComponent<RectTransform>().sizeDelta.x, duration).setEaseInOutCubic();
        }
    }

    /// <summary>
    /// This method moves the particular game object to the left (the width of th screen).
    /// </summary>
    public void ToLeft(GameObject gameObj)
    {
        // If the game object is referenced, ...
        if (gameObj != null)
        {
            // ... animate this game object.
            LeanTween.moveLocalX(gameObj, -UIManager.GetInstance().ui.GetComponent<RectTransform>().sizeDelta.x, duration).setEaseInOutCubic();
        }
    }

    /// <summary>
    /// This method moves the particular game object into the centre of the screen.
    /// </summary>
    public void Centre(GameObject gameObj)
    {
        // If the game object is referenced, ...
        if (gameObj != null)
        {
            // ... animate this game object.
            LeanTween.moveLocalX(gameObj, 0, duration).setEaseInOutCubic();
        }
    }

    /// <summary>
    /// This method resets the rotation of the object to new Vector(0, 0, 0).
    /// </summary>
    public void ResetRotation(GameObject gameObj)
    {
        // If the game object is referenced, ...
        if (gameObj != null)
        {
            // ... animate this game object.
            LeanTween.rotateLocal(gameObj, Vector3.zero, duration).setEaseInCubic();
        }
    }
}
