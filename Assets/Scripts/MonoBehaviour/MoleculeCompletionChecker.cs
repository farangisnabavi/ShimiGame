using UnityEngine;
using System;
using System.Collections.Generic;

// Attach this to an empty GameObject that oversees ONE molecule puzzle
// (e.g., "MoleculeChecker_NaCl", "MoleculeChecker_Water").
//
// This script coordinates two independent systems - PlacedElementsCollector
// and MoleculeRecipeChecker - to decide when a molecule has been correctly
// assembled. It does NOT contain any drag, trigger, or snap logic itself,
// and it does not modify any of those other scripts.
public class MoleculeCompletionChecker : MonoBehaviour
{
    [Header("Required Snapped Objects")]
    [Tooltip("Every SnapSystem3D that must be snapped for this molecule to be considered placed.")]
    public List<SnapSystem3D> requiredSnapObjects = new List<SnapSystem3D>();

    [Header("Recipe Validation References")]
    [Tooltip("The collector that gathers ElementData from all currently snapped objects.")]
    public PlacedElementsCollector placedElementsCollector;

    [Tooltip("The checker that holds all valid molecule recipes and compares against them.")]
    public MoleculeRecipeChecker moleculeRecipeChecker;

    [Tooltip("The exact name of the recipe this checker should validate against (must match a recipe name in MoleculeRecipeChecker).")]
    public string moleculeName;

    // Public property so other scripts can check completion state at any time
    public bool IsCompleted { get; private set; } = false;

    // Public event fired once, exactly when the molecule is correctly completed
    public event Action OnMoleculeCompleted;

    void Update()
    {
        // Only keep checking if the puzzle isn't already marked as completed
        if (!IsCompleted)
        {
            CheckCompletion();
        }
    }

    // Main entry point: checks snapping first, then recipe validity
    void CheckCompletion()
    {
        // Step 1 & 2: make sure every required object is snapped
        if (!AreAllObjectsSnapped())
        {
            return; // Not all pieces are in place yet - nothing more to check
        }

        // Step 3: gather the currently placed elements
        List<ElementData> placedElements = GetPlacedElementsSafely();
        if (placedElements == null)
        {
            return; // Missing reference or collection failed - already logged inside the method
        }

        // Step 4 & 5: send the list to the recipe checker and read the result
        bool isRecipeCorrect = CheckRecipeSafely(placedElements);

        // Step 6 & 7: act on the result
        if (isRecipeCorrect)
        {
            IsCompleted = true;
            Debug.Log(moleculeName + " completed successfully! All elements match the recipe.");
            OnMoleculeCompleted?.Invoke();
        }
        else
        {
            Debug.Log(moleculeName + " is not yet correct. All pieces are snapped, but the element combination does not match the recipe.");
        }
    }

    // Checks whether every SnapSystem3D in the list is currently snapped.
    // Returns false immediately if the list is empty or any reference is missing.
    bool AreAllObjectsSnapped()
    {
        if (requiredSnapObjects.Count == 0)
        {
            Debug.LogWarning(gameObject.name + ": No required snap objects assigned.");
            return false;
        }

        foreach (SnapSystem3D snapObject in requiredSnapObjects)
        {
            if (snapObject == null)
            {
                Debug.LogWarning(gameObject.name + ": A required snap object reference is missing (null).");
                return false;
            }

            if (!snapObject.IsSnapped)
            {
                // Not an error - this simply means the puzzle isn't finished yet
                return false;
            }
        }

        foreach (SnapSystem3D snapObject in requiredSnapObjects)
        {
            if (snapObject == null)
            {
                Debug.LogWarning(gameObject.name + ": A required snap object reference is missing (null).");
                return false;
            }

            Debug.Log("IsSnapped: " + snapObject.IsSnapped);

            if (!snapObject.IsSnapped)
            {
                return false;
            }
        }

        return true;
    }

    // Safely retrieves the placed elements list, with null checks and logging.
    // Returns null if the collector reference is missing so the caller can bail out cleanly.
    List<ElementData> GetPlacedElementsSafely()
    {
        if (placedElementsCollector == null)
        {
            Debug.LogWarning(gameObject.name + ": Placed Elements Collector reference is missing.");
            return null;
        }

        List<ElementData> placedElements = placedElementsCollector.GetPlacedElements();

        if (placedElements == null || placedElements.Count == 0)
        {
            Debug.Log(gameObject.name + ": No elements were collected yet.");
        }

        return placedElements;
    }

    // Safely calls the recipe checker, with null checks and logging.
    // Returns false if the recipe checker reference is missing or the name is empty.
    bool CheckRecipeSafely(List<ElementData> placedElements)
    {
        if (moleculeRecipeChecker == null)
        {
            Debug.LogWarning(gameObject.name + ": Molecule Recipe Checker reference is missing.");
            return false;
        }

        if (string.IsNullOrEmpty(moleculeName))
        {
            Debug.LogWarning(gameObject.name + ": Molecule Name is empty - cannot look up a recipe.");
            return false;
        }

        return moleculeRecipeChecker.CheckMolecule(moleculeName, placedElements);
    }

    // Optional public method other scripts can call to manually force a re-check
    public bool CheckNow()
    {
        CheckCompletion();
        return IsCompleted;
    }
}