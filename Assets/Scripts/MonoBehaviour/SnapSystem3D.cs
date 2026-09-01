using UnityEngine;
using System;

// Attach this script to the SAME draggable object that already has the Drag & Drop script.
// It works alongside that script and the Trigger Detection script without modifying either.
//
// This script listens for the mouse being released, checks if the object is
// currently inside a designated trigger area, and if so, smoothly snaps it
// to a predefined position. Optionally, it can then lock the object so it
// can no longer be dragged.
public class SnapSystem3D : MonoBehaviour
{
    [Header("References (set in Inspector)")]
    [Tooltip("The Trigger Detection script on the target trigger zone.")]
    public TriggerDetection3D targetTrigger;

    [Tooltip("The Drag & Drop script on THIS object (used to lock dragging after snap).")]
    public MonoBehaviour dragAndDropScript;

    [Header("Snap Settings")]
    [Tooltip("The position this object will snap to.")]
    public Transform snapPosition;

    [Tooltip("How fast the object moves toward the snap position.")]
    public float snapSpeed = 5f;

    [Tooltip("If true, dragging is disabled after this object snaps.")]
    public bool lockAfterSnap = true;

    // Public property so other scripts can check if this object has already snapped
    public bool IsSnapped { get; private set; } = false;

    // Optional event other scripts can subscribe to (e.g., for future systems)
    // It fires once, right when the object successfully snaps.
    public event Action OnSnapped;

    private bool isMovingToSnap = false; // Tracks whether the smooth snap movement is in progress

    void Update()
    {
        // Only check for a snap opportunity if we haven't already snapped
        if (!IsSnapped)
        {
            // Detect the moment the mouse button is released (drop event)
            if (Input.GetMouseButtonUp(0))
            {
                TryStartSnap();
            }
        }

        // If we're currently moving toward the snap position, keep moving smoothly
        if (isMovingToSnap)
        {
            MoveTowardsSnapPosition();
        }
    }

    void TryStartSnap()
    {
        // Safety check: make sure required references are assigned
        if (targetTrigger == null || snapPosition == null)
            return;

        // Check if THIS object is the one currently inside the trigger area
        bool isThisObjectInside = targetTrigger.IsObjectInside &&
                                   targetTrigger.CurrentObjectInside == this.gameObject;

        if (isThisObjectInside)
        {
            // Begin smoothly moving this object to the snap position
            isMovingToSnap = true;
        }
        Debug.Log(isThisObjectInside);
    }

    void MoveTowardsSnapPosition()
    {
        // Smoothly interpolate the object's position toward the snap position
        transform.position = Vector3.Lerp(transform.position, snapPosition.position, Time.deltaTime * snapSpeed);

        // Check if the object has arrived close enough to the target position
        if (Vector3.Distance(transform.position, snapPosition.position) < 0.01f)
        {
            // Snap exactly into place to avoid floating-point drift
            transform.position = snapPosition.position;
            isMovingToSnap = false;
            IsSnapped = true;

            // Lock the object from further dragging, if enabled
            if (lockAfterSnap && dragAndDropScript != null)
            {
                dragAndDropScript.enabled = false;
            }

            // Notify any listening scripts that snapping has completed
            OnSnapped?.Invoke();
            Debug.Log("SNAP COMPLETED - EVENT FIRED");
        }
    }
}