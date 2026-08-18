using UnityEngine;
using UnityEngine.EventSystems;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.Spawning;

namespace PeriodicTableSystem.UI
{
    /// <summary>
    /// Isolated drag & drop handler for the Periodic Table system.
    ///
    /// Implements Unity's standard UI event interfaces (IBeginDragHandler,
    /// IDragHandler, IEndDragHandler) directly — this is a brand new
    /// implementation and does not use, inherit from, or copy any existing
    /// drag-and-drop system in the project.
    ///
    /// Flow:
    ///   Begin drag  -> create a temporary, non-colliding preview
    ///   Drag        -> move the preview to follow the cursor
    ///   End drag    -> if released over a valid PeriodicElementDropArea,
    ///                  destroy the preview and spawn a real, independent
    ///                  element instance via PeriodicElementSpawner.
    ///                  Otherwise, just destroy the preview.
    ///
    /// This script only depends on the new Periodic Table system
    /// (PeriodicElementData, PeriodicElementSpawner, PeriodicTableElementItem,
    /// PeriodicElementDropArea). It does not reference any existing gameplay
    /// script, and it does not modify PeriodicTableElementItem in any way —
    /// it simply reads its existing public ElementData property.
    /// </summary>
    [RequireComponent(typeof(PeriodicTableElementItem))]
    public class PeriodicElementDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Dependencies (new system only)")]
        [Tooltip("The Periodic Table UI cell this handler drags. Auto-filled from this GameObject if left empty.")]
        [SerializeField] private PeriodicTableElementItem sourceItem;

        [Tooltip("Spawner used to create the real element instance on a valid drop.")]
        [SerializeField] private PeriodicElementSpawner spawner;

        [Tooltip("The area a dragged element must be released over to be spawned.")]
        [SerializeField] private PeriodicElementDropArea dropArea;

        [Header("Camera")]
        [Tooltip("Camera used to convert the screen-space mouse position into a world-space position. " +
                 "Assign explicitly — do not rely on Camera.main, since Canvas render mode/camera setup varies.")]
        [SerializeField] private Camera worldCamera;

        [Header("Drag Plane")]
        [Tooltip("Optional transform defining the drag plane (its position = plane height, its up axis = plane normal). " +
                 "If left empty, 'Fixed Plane Height' below is used with a world-up normal instead.")]
        [SerializeField] private Transform dragPlaneTransform;

        [Tooltip("World-space Y height of the drag plane, used only if 'Drag Plane Transform' is not assigned. " +
                 "Set this to the height of the intended gameplay surface.")]
        [SerializeField] private float fixedPlaneHeight = 0f;

        private GameObject activePreview;
        private Vector3 lastKnownDragPoint;

        private void Reset()
        {
            sourceItem = GetComponent<PeriodicTableElementItem>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            PeriodicElementData elementData = sourceItem != null ? sourceItem.ElementData : null;

            if (elementData == null || elementData.Prefab3D == null || worldCamera == null)
            {
                Debug.LogWarning("[PeriodicElementDragHandler] Cannot begin drag: missing element data, prefab, or world camera.");
                activePreview = null;
                return;
            }

            activePreview = Instantiate(elementData.Prefab3D);
            activePreview.name = elementData.ElementName + " (Drag Preview)";
            PreparePreview(activePreview);

            Debug.Log($"[PeriodicElementDragHandler] Begin Drag: {elementData.ElementName}");

            UpdatePreviewPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (activePreview == null)
            {
                return;
            }

            // Intentionally no per-frame logging here — only state transitions are logged.
            UpdatePreviewPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (activePreview == null)
            {
                return;
            }

            PeriodicElementData elementData = sourceItem != null ? sourceItem.ElementData : null;

            // Use the same fixed drag plane the preview was following, so the
            // drop position matches exactly where the preview appeared to be.
            Vector3 dropPoint = GetPointOnDragPlane(eventData, lastKnownDragPoint);
            bool droppedInValidArea = dropArea != null && dropArea.IsWorldPointInside(dropPoint);

            Debug.Log($"[PeriodicElementDragHandler] End Drag Position: {dropPoint}");
            Debug.Log($"[PeriodicElementDragHandler] Inside Drop Area: {droppedInValidArea}");

            // The preview is always temporary and is destroyed here, regardless of outcome.
            Destroy(activePreview);
            activePreview = null;

            if (droppedInValidArea && elementData != null && spawner != null)
            {
                GameObject spawnedElement = spawner.SpawnElement(elementData);
                if (spawnedElement != null)
                {
                    spawnedElement.transform.position = dropPoint;
                    Debug.Log($"[PeriodicElementDragHandler] Spawned: {elementData.ElementName}");
                }
            }
        }

        /// <summary>
        /// Moves the active preview along the fixed drag plane so it never
        /// drifts forward/backward in depth as the mouse moves.
        /// </summary>
        private void UpdatePreviewPosition(PointerEventData eventData)
        {
            Vector3 targetPoint = GetPointOnDragPlane(eventData, activePreview.transform.position);
            activePreview.transform.position = targetPoint;
            lastKnownDragPoint = targetPoint;
        }

        /// <summary>
        /// Converts the current pointer position into a world-space point on the
        /// configured fixed drag plane. This is the single source of truth for both
        /// preview movement and final drop-position/validity checks, so the two
        /// always agree.
        /// </summary>
        private Vector3 GetPointOnDragPlane(PointerEventData eventData, Vector3 fallback)
        {
            if (worldCamera == null)
            {
                return fallback;
            }

            Ray ray = worldCamera.ScreenPointToRay(eventData.position);
            Plane dragPlane = GetDragPlane();

            if (dragPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            // Ray parallel to the plane (extremely unlikely with a typical camera setup) — hold position.
            return fallback;
        }

        /// <summary>
        /// Builds the fixed drag plane. If a Drag Plane Transform is assigned, its
        /// position and up-axis define the plane (supports angled/custom surfaces).
        /// Otherwise, a horizontal plane at Fixed Plane Height is used.
        /// </summary>
        private Plane GetDragPlane()
        {
            if (dragPlaneTransform != null)
            {
                return new Plane(dragPlaneTransform.up, dragPlaneTransform.position);
            }

            return new Plane(Vector3.up, new Vector3(0f, fixedPlaneHeight, 0f));
        }

        /// <summary>
        /// Ensures the temporary preview cannot physically interfere with existing
        /// gameplay objects (no collisions, no physics forces) while it is being dragged.
        /// </summary>
        private void PreparePreview(GameObject preview)
        {
            foreach (Rigidbody rb in preview.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }

            foreach (Collider col in preview.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        private void OnDestroy()
        {
            // Safety net: never leave a temporary preview behind if this handler is destroyed mid-drag.
            if (activePreview != null)
            {
                Destroy(activePreview);
            }
        }
    }
}
