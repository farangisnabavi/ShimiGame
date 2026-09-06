using UnityEngine;

// Attach this script to any atom GameObject that needs to be identified
// as a specific chemical element.
//
// This script has NO gameplay logic. It's purely a lightweight bridge that
// links a GameObject in the scene to its corresponding ElementData asset,
// so other scripts (matching, checking, bonding, etc.) can read "what element is this?"
public class ElementReference : MonoBehaviour
{
    [Header("Element Assignment")]
    [Tooltip("The ElementData asset that identifies which element this GameObject represents.")]
    [SerializeField]
    private ElementData elementData;

    // Public read-only property other scripts use to access the assigned element.
    // Using a property (instead of exposing the field directly) keeps this
    // reference read-only from outside scripts, while still editable in the Inspector
    // via the serialized private field above.
    public ElementData Element => elementData;
}