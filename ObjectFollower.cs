using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    private GameObject target;

    private Vector3 defaultScale;

    private bool hideConditionAdded;
    private float minX;

    private void Start()
    {
        defaultScale = transform.localScale;
    }

    /// <summary>
    /// This method makes this game object follow the other game object.
    /// </summary>
    /// <param name="obj"> the game object this game object is supposed to follow </param>
    public void Follow(GameObject obj)
    {
        target = obj;
    }

    /// <summary>
    /// This method makes this game object disappear, when its x position is lower than the given parametre.
    /// </summary>
    public void AddHideCondition(float minX)
    {
        hideConditionAdded = true;
        this.minX = minX;
    }

    void Update()
    {
        // If this game object is supposed to follow an object, ...
        if (target != null)
        {
            Vector3 currentPos = transform.position;

            // ... but if this game object is outside of the user's view, ...
            if (hideConditionAdded && currentPos.x <= minX)
            {
                // ... hide it.
                transform.localScale = Vector3.zero;
            }
            // If it can be seen, ...
            else
            {
                // ... adapt the position to the position of the target.
                currentPos.x = target.transform.position.x;

                transform.position = currentPos;

                transform.localScale = defaultScale;
            }
        }
    }
}
