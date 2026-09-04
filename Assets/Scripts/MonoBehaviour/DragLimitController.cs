using UnityEngine;

public class DragLimitController : MonoBehaviour
{
    [Header("Drag Limit")]
    [SerializeField] private int maxDrags = 5;

    private int currentDrags = 0;

    public bool CanDrag()
    {
        return currentDrags < maxDrags;
    }

    public void RegisterDrag()
    {
        if (currentDrags < maxDrags)
        {
            currentDrags++;

            Debug.Log("Drag used: " + currentDrags + " / " + maxDrags);
        }
    }

    public int GetRemainingDrags()
    {
        return maxDrags - currentDrags;
    }

    public void ResetDrags()
    {
        currentDrags = 0;
    }
}