using UnityEngine;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Chemistry
{
    public class BondVisual : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private PeriodicElementInstance atomA;
        private PeriodicElementInstance atomB;

        public void Initialize(PeriodicElementInstance a, PeriodicElementInstance b, BondType bondType)
        {
            atomA = a;
            atomB = b;

            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }

            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.useWorldSpace = true;

            switch (bondType)
            {
                case BondType.Covalent:
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                    break;
                case BondType.Ionic:
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.blue;
                    break;
                case BondType.Metallic:
                    lineRenderer.startColor = Color.yellow;
                    lineRenderer.endColor = Color.yellow;
                    break;
                default:
                    lineRenderer.startColor = Color.gray;
                    lineRenderer.endColor = Color.gray;
                    break;
            }

            UpdateVisual();
        }

        private void LateUpdate()
        {
            if (atomA == null || atomB == null)
            {
                Destroy(gameObject);
                return;
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (lineRenderer == null || atomA == null || atomB == null) return;
            lineRenderer.SetPosition(0, atomA.transform.position);
            lineRenderer.SetPosition(1, atomB.transform.position);
        }
    }
}