using UnityEngine;

public class RotateTurbine : MonoBehaviour
{
    private static RotateTurbine _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static RotateTurbine GetInstance()
    {
        return _instance;
    }

    /// <summary>
    /// This method rotates the turbine corresponding to the frequency.
    /// </summary>
    /// <param name="frequency"></param>
    public void Rotate(float frequency)
    {
        transform.Rotate(0, 360f * frequency * Time.deltaTime, 0);
    }
}
