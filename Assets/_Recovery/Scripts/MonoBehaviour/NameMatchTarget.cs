using UnityEngine;
using System;

// Attach this script to a target area (e.g., a name label or symbol card)
// that has a Collider set to "Is Trigger".
//
// It detects when a draggable element is dropped on top of it, then compares
// the dropped object's ElementData against the expected one for this target.
//
// This script works independently from the Drag & Drop script - it only
// listens for the mouse button being released while an object overlaps it.
public class NameMatchTarget : MonoBehaviour
{
    [Header("Target Setup")]
    [Tooltip("The Element ScriptableObject this target expects to receive.")]
    public ElementData expectedElement;

    // Tracks which draggable object (if any) is currently overlapping this target
    private GameObject currentObjectInside = null;

    // Public property so other scripts can check the last match result
    public bool? LastMatchWasCorrect { get; private set; } = null;

    // Public event fired every time a drop is checked against this target.
    // Passes the dropped GameObject and whether the match was correct.
    public event Action<GameObject, bool> OnMatchChecked;

    void Start()
    {
        // Safety check: warn if the collider isn't set up as a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning(gameObject.name + ": Collider should be set to 'Is Trigger' for name matching to work.");
        }
    }

    void Update()
    {
        // Only check for a drop if something is currently overlapping this target
        // and the mouse button was just released (the "drop" moment)
        if (currentObjectInside != null && Input.GetMouseButtonUp(0))
        {
            CheckMatch(currentObjectInside);
        }
    }

    // Called automatically by Unity when a collider enters this trigger area
    private void OnTriggerEnter(Collider other)
    {
        // Only track objects that actually have a DraggableElement component
        if (other.GetComponent<DraggableElement>() != null)
        {
            currentObjectInside = other.gameObject;
        }
    }

    // Called automatically by Unity when a collider exits this trigger area
    private void OnTriggerExit(Collider other)
    {
        // Stop tracking if the object that left is the one we were watching
        if (other.gameObject == currentObjectInside)
        {
            currentObjectInside = null;
        }
    }

    // Compares the dropped object's assigned element against the expected one
    void CheckMatch(GameObject droppedObject)
    {
        DraggableElement draggable = droppedObject.GetComponent<DraggableElement>();

        Debug.Log(
        "CHECK MATCH | " +
        "Object: " + droppedObject.name +
        " | Draggable: " + (draggable != null) +
        " | Assigned Element: " +
        (draggable != null && draggable.assignedElement != null) +
        " | Expected Element: " +
        (expectedElement != null));

        // Safety check: make sure both elements are actually assigned
        if (draggable == null || draggable.assignedElement == null || expectedElement == null)
            return;

        // The actual comparison: are these the same Element ScriptableObject?
        bool isCorrect = draggable.assignedElement == expectedElement;

        LastMatchWasCorrect = isCorrect;

        // Notify any listening scripts of the result (e.g., a future feedback or scoring system)
        OnMatchChecked?.Invoke(droppedObject, isCorrect);

        Debug.Log(droppedObject.name + " dropped on " + gameObject.name + " - Correct: " + isCorrect);
    }

    // Optional public method other scripts can call to manually re-check the current overlap
    public bool? CheckCurrentOverlap()
    {
        if (currentObjectInside != null)
        {
            CheckMatch(currentObjectInside);
        }
        return LastMatchWasCorrect;
    }
}