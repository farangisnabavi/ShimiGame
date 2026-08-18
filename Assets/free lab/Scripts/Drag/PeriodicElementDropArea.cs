using UnityEngine;

namespace PeriodicTableSystem.UI
{
    [RequireComponent(typeof(BoxCollider))]
    public class PeriodicElementDropArea : MonoBehaviour
    {
        [Header("Drop Area")]
        [SerializeField] private BoxCollider dropCollider;

        private void Reset()
        {
            dropCollider = GetComponent<BoxCollider>();
        }

        private void Awake()
        {
            if (dropCollider == null)
            {
                dropCollider = GetComponent<BoxCollider>();
            }
        }

        public bool IsWorldPointInside(Vector3 worldPoint)
        {
            if (dropCollider == null)
            {
                Debug.LogWarning("[PeriodicElementDropArea] BoxCollider is missing!");
                return false;
            }

            // Convert the world position into the collider's local space.
            Vector3 localPoint =
                dropCollider.transform.InverseTransformPoint(worldPoint);

            Vector3 center = dropCollider.center;
            Vector3 halfSize = dropCollider.size * 0.5f;

            bool insideX =
                localPoint.x >= center.x - halfSize.x &&
                localPoint.x <= center.x + halfSize.x;

            bool insideZ =
                localPoint.z >= center.z - halfSize.z &&
                localPoint.z <= center.z + halfSize.z;

            bool inside = insideX && insideZ;

            Debug.Log(
                $"[PeriodicElementDropArea]\n" +
                $"World Point: {worldPoint}\n" +
                $"Local Point: {localPoint}\n" +
                $"Collider Center: {center}\n" +
                $"Collider Size: {dropCollider.size}\n" +
                $"Inside X: {insideX}\n" +
                $"Inside Z: {insideZ}\n" +
                $"FINAL RESULT: {inside}"
            );

            return inside;
        }
    }
}