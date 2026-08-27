using UnityEngine;

namespace PeriodicTableSystem.UI
{
    public class PeriodicElementDropArea : MonoBehaviour
    {
        [SerializeField] private Collider dropCollider;
        
        private void Awake()
        {
            if (dropCollider == null) dropCollider = GetComponent<Collider>();
        }

        public bool IsPointInside(Vector3 point)
        {
            if (dropCollider == null) return true;
            return dropCollider.bounds.Contains(point);
        }
    }
}