using UnityEngine;

namespace PeriodicTableSystem.World
{
    public class PeriodicWorldElementDrag : MonoBehaviour
    {
        [SerializeField] private float dragHeight = 1f;
        [SerializeField] private LayerMask groundLayer;

        private Camera mainCamera;
        private bool isDragging;
        private Rigidbody rb;
        private Vector3 offset;

        private void Awake()
        {
            mainCamera = Camera.main;
            rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.constraints =
                    RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezeRotation;
            }
        }

        private void OnMouseDown()
        {
            Debug.Log("ELEMENT CLICKED!");

            isDragging = true;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                offset = transform.position - hit.point;
                offset.y = 0;
            }
        }

        private void OnMouseDrag()
        {
            if (!isDragging)
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 newPosition = hit.point + offset;
                newPosition.y = dragHeight;

                transform.position = newPosition;
            }
        }

        private void OnMouseUp()
        {
            isDragging = false;
        }
    }
}