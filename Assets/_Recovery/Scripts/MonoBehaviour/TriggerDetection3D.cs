using UnityEngine;

// Attach this script to the GameObject that acts as the Trigger Area.
// That GameObject needs a Collider component with "Is Trigger" checked.
//
// This script works independently from the Drag & Drop script.
// It simply detects when ANY object with a Collider enters, stays in,
// or exits this trigger zone - including objects being dragged.
public class TriggerDetection3D : MonoBehaviour
{
    // Public property that other scripts can read to check
    // whether an object is currently inside this trigger.
    // (Read-only from outside, only this script can change it)
    public bool IsObjectInside { get; private set; } = false;

    // Optional: stores a reference to the object currently inside the trigger.
    // Useful if other scripts need to know WHICH object triggered it.
    public GameObject CurrentObjectInside { get; private set; } = null;

    void Start()
    {
        // Safety check: warn the developer if the collider isn't set as a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning(gameObject.name + ": Collider is not set to 'Is Trigger'. Please enable it in the Inspector.");
        }
    }

    // Called automatically by Unity when another collider ENTERS this trigger
    private void OnTriggerEnter(Collider other)
    {
        IsObjectInside = true;
        CurrentObjectInside = other.gameObject;

        Debug.Log(other.gameObject.name + " entered the trigger area.");
    }

    // Called automatically by Unity every frame WHILE another collider STAYS inside this trigger
    private void OnTriggerStay(Collider other)
    {
        // This confirms the object is still inside.
        // Kept minimal since no extra logic (like validation) is required here.
        IsObjectInside = true;
    }

    // Called automatically by Unity when another collider EXITS this trigger
    private void OnTriggerExit(Collider other)
    {
        // Only reset the state if the object leaving is the one we were tracking
        if (other.gameObject == CurrentObjectInside)
        {
            IsObjectInside = false;
            CurrentObjectInside = null;

            Debug.Log(other.gameObject.name + " exited the trigger area.");
        }
    }
}