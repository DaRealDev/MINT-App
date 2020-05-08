using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    // the last x position of the finger / mouse
    private float _lastX;

    // the rotation speed
    [SerializeField] private float speed = 0.5f; 

    /// <summary>
    /// This method is called when the user puts his / her finger onto the screen.
    /// </summary>
    public void FingerDown()
    {
        // get and store the x position of the first touch / mouse click
        _lastX = Input.mousePosition.x;
    }

    /// <summary>
    /// This method is called when the user drags his / her finger over the screen.
    /// </summary>
    public void FingerDragged()
    {
        // get the current finger / mouse position
        float x = Input.mousePosition.x;

        // rotate the camera around the turbine corresponding to the finger / mouse movement
        transform.Rotate(0, (x - _lastX) * speed, 0);

        // override the last x position of the finger / the mouse
        _lastX = x;
    }

    /// <summary>
    /// This method resets the camera rotation.
    /// </summary>
    public void Reset()
    {
        AnimationController.GetInstance().ResetRotation(gameObject);
    }

}
