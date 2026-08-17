using UnityEngine;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.Spawning;

namespace PeriodicTableSystem.UI
{
    /// <summary>
    /// Represents a single cell in the new, isolated Periodic Table UI.
    /// Holds a reference to one PeriodicElementData and, when clicked,
    /// asks the PeriodicElementSpawner to spawn a new instance of it.
    ///
    /// This component does not interact with any existing gameplay,
    /// UI, or drag-and-drop scripts. The spawner reference is assigned
    /// via the Inspector (no FindObjectOfType).
    /// </summary>
    public class PeriodicTableElementItem : MonoBehaviour
    {
        [Header("Element Data")]
        [Tooltip("The data asset this UI cell represents, e.g. Sodium.asset.")]
        [SerializeField] private PeriodicElementData elementData;

        [Header("Spawner Reference")]
        [Tooltip("The spawner responsible for instantiating this element's 3D prefab. " +
                 "Assign the PeriodicElementSpawner instance in the scene here.")]
        [SerializeField] private PeriodicElementSpawner spawner;

        public PeriodicElementData ElementData => elementData;

        /// <summary>
        /// Wire this method to a UI Button's OnClick() event.
        /// Triggers spawning of this cell's element.
        /// </summary>
        public void OnClick()
        {
            if (spawner == null)
            {
                Debug.LogWarning($"[PeriodicTableElementItem] No spawner assigned on '{gameObject.name}'.");
                return;
            }

            spawner.SpawnElement(elementData);
        }
    }
}
