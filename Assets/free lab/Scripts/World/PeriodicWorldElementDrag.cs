using UnityEngine;

namespace PeriodicTableSystem.World
{
    public class PeriodicWorldElementDrag : MonoBehaviour
    {
        [SerializeField] private float dragHeight = 1f;
        [SerializeField] private LayerMask groundLayer;
        
        private Camera mainCamera;
        private bool isDragging = false;
        private Rigidbody rb;
        private Vector3 offset;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            rb = GetComponent<Rigidbody>();
        }

        void OnMouseDown()
        {
            isDragging = true;
            if (rb != null) rb.isKinematic = true;
    
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                offset = transform.position - hit.point;
                offset.y = 0; 
            }
        }


        void OnMouseDrag()
        {
            if (!isDragging) return;
    
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 newPos = hit.point + offset;
                newPos.y = dragHeight;  
                transform.position = newPos;
            }
        }


        void OnMouseUp()
        {
            isDragging = false;
            if (rb != null) rb.isKinematic = false;
        }
    }
}