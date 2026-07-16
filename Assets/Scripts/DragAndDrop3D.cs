using UnityEngine;

// Attach this script directly to any 3D object you want to drag and drop.
// Make sure the object has a Collider component (e.g., BoxCollider) so it can be clicked.
// This version locks the X axis so the object only moves along Y and Z while dragging.
public class DragAndDrop3D : MonoBehaviour
{
    private Camera mainCamera;       // Reference to the main camera
    private bool isDragging = false; // Tracks whether the object is currently being dragged
    private float objectDistance;    // Distance from the camera to the object (kept constant while dragging)
    private float lockedX;           // Stores the object's X position at the moment dragging starts

    void Start()
    {
        // Cache the main camera so we don't call Camera.main every frame (better performance)
        mainCamera = Camera.main;
    }

    void Update()
    {
        // When the left mouse button is pressed, check if we clicked this object
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // While holding the mouse button and dragging, move the object with the mouse
        if (isDragging && Input.GetMouseButton(0))
        {
            DragObject();
        }

        // When the mouse button is released, stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void TryStartDrag()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits this specific object's collider
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                isDragging = true;

                // Store the distance between the camera and the object
                // so the object stays at the same depth while dragging
                objectDistance = Vector3.Distance(mainCamera.transform.position, transform.position);

                // Remember the object's current X position so we can lock it
                // back in place every frame during this drag
                lockedX = transform.position.x;
            }
        }
    }

    void DragObject()
    {
        // Create a ray from the camera through the current mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Get a point along that ray at the same distance as the object
        // This gives us a new position that follows the mouse in 3D space
        Vector3 newPosition = ray.GetPoint(objectDistance);

        // Overwrite the X value with the locked X so the object never moves
        // sideways along that axis - only Y and Z are allowed to change
        newPosition.x = lockedX;

        // Apply the final position to the object
        transform.position = newPosition;
    }
}