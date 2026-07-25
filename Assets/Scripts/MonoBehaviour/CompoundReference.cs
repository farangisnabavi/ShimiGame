using UnityEngine;

// Attach this to any draggable compound GameObject.
// Its ONLY job is to hold a reference to a CompoundData asset.
// It contains no drag logic, no drop logic - just a data link.
public class CompoundReference : MonoBehaviour
{
    [Tooltip("Assign the CompoundData asset that represents this specific object (e.g. NaCl, H2O).")]
    [SerializeField] private CompoundData compoundData;

    // Read-only public access - other scripts can read this, but never modify it directly.
    public CompoundData Compound => compoundData;
}