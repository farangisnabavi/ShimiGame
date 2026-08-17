using UnityEngine;
using PeriodicTableSystem.Data;

namespace PeriodicTableSystem.Spawning
{
    /// <summary>
    /// Isolated spawner for the Periodic Table system.
    ///
    /// Sole responsibility: given a PeriodicElementData, instantiate a new,
    /// independent GameObject from its 3D prefab. No limits, no counting,
    /// no chemistry logic, no dependency on any existing gameplay script.
    /// </summary>
    public class PeriodicElementSpawner : MonoBehaviour
    {
        [Header("Optional Parent")]
        [Tooltip("Optional parent transform for spawned elements (e.g. PeriodicElementContainer). " +
                 "If left empty, spawned elements are placed at the scene root.")]
        [SerializeField] private Transform spawnContainer;

        /// <summary>
        /// Creates a brand new, independent instance of the given element's 3D prefab.
        /// Safe to call repeatedly with the same PeriodicElementData — every call
        /// produces a separate GameObject.
        /// </summary>
        /// <param name="elementData">The data describing which element to spawn.</param>
        /// <returns>The newly instantiated GameObject, or null if spawning was not possible.</returns>
        public GameObject SpawnElement(PeriodicElementData elementData)
        {
            if (elementData == null)
            {
                Debug.LogWarning("[PeriodicElementSpawner] SpawnElement called with a null PeriodicElementData.");
                return null;
            }

            if (elementData.Prefab3D == null)
            {
                Debug.LogWarning($"[PeriodicElementSpawner] '{elementData.ElementName}' has no 3D Prefab assigned.");
                return null;
            }

            GameObject spawnedInstance = spawnContainer != null
                ? Instantiate(elementData.Prefab3D, spawnContainer)
                : Instantiate(elementData.Prefab3D);

            return spawnedInstance;
        }
    }
}
