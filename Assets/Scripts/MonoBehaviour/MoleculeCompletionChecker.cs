using UnityEngine;
using System;
using System.Collections.Generic;

// Attach this script to an empty GameObject that represents the "Molecule Manager"
// for a specific puzzle (e.g., "Molecule_H2O_Checker").
//
// This script does NOT control snapping, dragging, or triggers.
// It only WATCHES the public "IsSnapped" property on each required SnapSystem3D
// and reports when all of them are true.
public class MoleculeCompletionChecker : MonoBehaviour
{
    [Header("Required Objects")]
    [Tooltip("List every SnapSystem3D component that must be snapped for this puzzle to be complete.")]
    public List<SnapSystem3D> requiredSnapObjects = new List<SnapSystem3D>();

    // Public property so other scripts can check completion state at any time
    public bool IsCompleted { get; private set; } = false;

    // Public event that fires once, exactly when the molecule becomes complete.
    // Other scripts (UI, level manager, etc.) can subscribe to this without
    // this script needing to know they exist.
    public event Action OnMoleculeCompleted;

    void Update()
    {
        // Only keep checking if the puzzle isn't already marked as completed
        if (!IsCompleted)
        {
            CheckCompletion();
        }
    }

    // Checks whether every required object has snapped into place
    void CheckCompletion()
    {
        // Safety check: don't evaluate if no objects were assigned
        if (requiredSnapObjects.Count == 0)
            return;

        // Loop through every required snap object
        foreach (SnapSystem3D snapObject in requiredSnapObjects)
        {
            // If any reference is missing or not yet snapped, the molecule isn't complete
            if (snapObject == null || !snapObject.IsSnapped)
            {
                return; // Exit early - not all objects are placed yet
            }
        }

        // If we reach this point, every object in the list is snapped correctly
        IsCompleted = true;

        // Notify any listening scripts that the molecule is now complete
        OnMoleculeCompleted?.Invoke();
    }

    // Optional public method other scripts can call to manually re-check completion
    // (useful if objects can be reset or unsnapped in the future)
    public bool CheckIfCompletedNow()
    {
        CheckCompletion();
        return IsCompleted;
    }
}