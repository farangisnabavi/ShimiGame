using UnityEngine;

namespace PeriodicTableSystem.Data
{
    /// <summary>
    /// Isolated data definition for a single periodic table element.
    /// This is a pure data container (ScriptableObject) — it does NOT
    /// represent a spawned instance, and it is never modified at runtime.
    ///
    /// This class is intentionally separate from any existing "ElementData"
    /// class in the project. It has no dependency on, and no relationship
    /// to, existing gameplay scripts.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewPeriodicElement",
        menuName = "PeriodicTableSystem/Periodic Element Data")]
    public class PeriodicElementData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Full name of the element, e.g. 'Sodium'.")]
        [SerializeField] private string elementName;

        [Tooltip("Chemical symbol, e.g. 'Na'.")]
        [SerializeField] private string symbol;

        [Tooltip("Atomic number, e.g. 11 for Sodium.")]
        [SerializeField] private int atomicNumber;

        [Header("Visual Representation")]
        [Tooltip("The 3D prefab that will be instantiated when this element is spawned.")]
        [SerializeField] private GameObject prefab3D;

        public string ElementName => elementName;
        public string Symbol => symbol;
        public int AtomicNumber => atomicNumber;
        public GameObject Prefab3D => prefab3D;
    }
}
