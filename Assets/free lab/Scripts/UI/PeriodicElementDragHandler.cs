using UnityEngine;
using UnityEngine.EventSystems;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.Spawning;

namespace PeriodicTableSystem.UI
{
    [RequireComponent(typeof(PeriodicTableElementItem))]
    public class PeriodicElementDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private PeriodicElementSpawner spawner;
        [SerializeField] private PeriodicElementDropArea dropArea;
        [SerializeField] private Camera worldCamera;

        private PeriodicTableElementItem item;
        private GameObject preview;
        [SerializeField] private GameObject panel;
        
        private void Awake()
        {
            item = GetComponent<PeriodicTableElementItem>();
            if (worldCamera == null) worldCamera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item.elementData?.prefab3D == null || worldCamera == null) return;

            preview = Instantiate(item.elementData.prefab3D);
            preview.name = $"Preview_{item.elementData.symbol}";
            SetPreviewMode(preview, true);
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (preview == null) return;
            Ray ray = worldCamera.ScreenPointToRay(eventData.position);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
                preview.transform.position = ray.GetPoint(distance);

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (preview == null) return;
            Vector3 position = preview.transform.position;
            bool valid = dropArea == null || dropArea.IsPointInside(position);
            Destroy(preview);
            preview = null;

            if (valid && spawner != null)
                spawner.SpawnElement(item.elementData, position);
           
        }

        private void SetPreviewMode(GameObject obj, bool isPreview)
        {
            foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = isPreview;
                rb.detectCollisions = !isPreview;
            }
            foreach (var c in obj.GetComponentsInChildren<Collider>())
                c.enabled = !isPreview;
           
        }
    }
}
