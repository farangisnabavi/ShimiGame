using UnityEngine;

public class DragAndDrop3D : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float objectDistance;
    private float lockedX;

    [Header("Drag Limit")]
    [SerializeField] private DragLimitController dragLimitController;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            DragObject();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void TryStartDrag()
    {
        // اول بررسی می‌کنیم که هنوز اجازه Drag داریم یا نه
        if (dragLimitController != null && !dragLimitController.CanDrag())
        {
            Debug.Log("No more drags allowed!");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                isDragging = true;

                // فقط وقتی Drag واقعاً شروع شد، یک حرکت مصرف می‌شود
                if (dragLimitController != null)
                {
                    dragLimitController.RegisterDrag();
                }

                objectDistance = Vector3.Distance(
                    mainCamera.transform.position,
                    transform.position
                );

                lockedX = transform.position.x;
            }
        }
    }

    void DragObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 newPosition = ray.GetPoint(objectDistance);

        newPosition.x = lockedX;

        transform.position = newPosition;
    }
}