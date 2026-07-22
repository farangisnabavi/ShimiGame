using UnityEngine;
using System.Collections.Generic;

// Attach this to an empty GameObject that oversees one molecule puzzle
// (similar setup to MoleculeCompletionChecker, but for a different purpose).
//
// This script does NOT validate recipes, snap objects, or know about UI/LevelManager.
// Its only job is: look at a list of SnapSystem3D objects, and for the ones that
// are currently snapped, collect their assigned ElementData into a simple list.
public class PlacedElementsCollector : MonoBehaviour
{
    [Header("Objects to Watch")]
    [Tooltip("Every SnapSystem3D that belongs to this molecule puzzle.")]
    public List<SnapSystem3D> snapObjects = new List<SnapSystem3D>();

    // Public method other scripts can call at any time to get the current
    // list of elements that are correctly snapped into place.
    public List<ElementData> GetPlacedElements()
    {
        List<ElementData> placedElements = new List<ElementData>();

        // Go through every SnapSystem3D we're watching
        foreach (SnapSystem3D snapObject in snapObjects)
        {
            // Skip if the reference is missing entirely
            if (snapObject == null)
                continue;

            // Skip any object that hasn't snapped yet - only placed elements count
            if (!snapObject.IsSnapped)
                continue;

            // Try to find the ElementReference component on the same GameObject
            ElementReference elementRef = snapObject.GetComponent<ElementReference>();

            // Skip objects that don't have an ElementReference attached
            if (elementRef == null)
                continue;

            // Skip if the ElementReference exists but has no ElementData assigned
            if (elementRef.Element == null)
                continue;

            // Passed all checks - add this element to the result list
            placedElements.Add(elementRef.Element);
        }

        return placedElements;
    }
}