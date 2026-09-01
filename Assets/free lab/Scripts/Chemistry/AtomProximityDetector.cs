using UnityEngine;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Chemistry
{
    public class AtomProximityDetector : MonoBehaviour
    {
        [SerializeField] private float proximityRadius = 2f;

        private PeriodicElementInstance ownerInstance;
        private SphereCollider proximityTrigger;

        public PeriodicElementInstance GetOwnerInstance()
        {
            return ownerInstance;
        }

        private void Awake()
        {
            ownerInstance = GetComponent<PeriodicElementInstance>();

            if (ownerInstance == null)
            {
                Debug.LogError(
                    $"[Detector] PeriodicElementInstance not found on {gameObject.name}",
                    this
                );
                enabled = false;
                return;
            }

            SetupTrigger();

            Debug.Log(
                $"[Detector] {gameObject.name} initialized successfully.",
                this
            );
        }

        private void SetupTrigger()
        {
            Collider[] colliders = GetComponents<Collider>();

            foreach (Collider col in colliders)
            {
                if (col.isTrigger && col is SphereCollider sphere)
                {
                    proximityTrigger = sphere;
                    break;
                }
            }

            if (proximityTrigger == null)
            {
                proximityTrigger = gameObject.AddComponent<SphereCollider>();
                proximityTrigger.isTrigger = true;
            }

            proximityTrigger.radius = proximityRadius;
        }

        private void OnValidate()
        {
            if (proximityTrigger != null)
            {
                proximityTrigger.radius = proximityRadius;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ownerInstance == null)
                return;

            AtomProximityDetector otherDetector =
                other.GetComponent<AtomProximityDetector>();

            if (otherDetector == null)
                return;

            PeriodicElementInstance otherInstance =
                otherDetector.GetOwnerInstance();

            if (otherInstance == null)
                return;

            if (otherInstance == ownerInstance)
                return;

            // BondManager را مستقیماً از Scene پیدا می‌کنیم
            BondManager manager =
                FindFirstObjectByType<BondManager>();

            if (manager == null)
            {
                Debug.LogError(
                    "[Detector] NO BondManager EXISTS IN THE SCENE!",
                    this
                );
                return;
            }

            Debug.Log(
                $"[Detector] {ownerInstance.ElementData.symbol} detected " +
                $"{otherInstance.ElementData.symbol}. Sending request to BondManager.",
                this
            );

            manager.RequestBond(
                ownerInstance,
                otherInstance,
                ownerInstance
            );
        }
    }
}