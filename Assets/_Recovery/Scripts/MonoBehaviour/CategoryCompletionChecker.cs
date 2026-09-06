using UnityEngine;
using System;
using System.Collections.Generic;

// CategoryCompletionChecker has a single responsibility:
// "Has the player correctly categorized every required compound?"
//
// It does NOT validate individual placements - that's CategoryChecker's job.
// It only LISTENS to CategoryChecker's results and tracks overall progress.
// It knows nothing about drag & drop, snapping, zones, or UI.
public class CategoryCompletionChecker : MonoBehaviour
{
    [Header("Source Of Placement Events")]
    [Tooltip("The CategoryChecker that validates individual placements. This script only listens to it.")]
    public CategoryChecker categoryChecker;

    [Header("Required Compounds")]
    [Tooltip("Every compound that must be correctly categorized for this puzzle to be complete.")]
    public List<CompoundReference> requiredCompounds = new List<CompoundReference>();
    
    [SerializeField] private GameObject completionPanel;

    // Internal set of compounds that have been correctly placed so far.
    // A HashSet is used instead of a List because it automatically prevents
    // duplicate entries and gives fast, simple "already contains this?" checks -
    // exactly what we need to satisfy requirement 6 and 10 (ignore duplicate correct events).
    private HashSet<CompoundReference> correctlyPlaced = new HashSet<CompoundReference>();

    // Public read-only state - other scripts can check this, but only this script can set it.
    public bool IsCompleted { get; private set; } = false;

    // Fired exactly once, the moment every required compound has been correctly placed.
    public event Action OnCategoryCompleted;

    // Subscribe to CategoryChecker's event when this component becomes active.
    // OnEnable/OnDisable is used (instead of Start/OnDestroy) so subscriptions
    // are correctly re-established if this object is ever disabled and re-enabled.
    void OnEnable()
    {
        if (categoryChecker != null)
        {
            categoryChecker.OnCorrectPlacement += HandleCorrectPlacement;
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": CategoryChecker reference is missing.");
        }
    }

    // Always unsubscribe to avoid memory leaks or calls into a destroyed object.
    void OnDisable()
    {
        if (categoryChecker != null)
        {
            categoryChecker.OnCorrectPlacement -= HandleCorrectPlacement;
        }
    }

    // Called automatically every time CategoryChecker reports a CORRECT placement.
    // Wrong placements never reach this method, because we only subscribed to OnCorrectPlacement -
    // this automatically satisfies requirement 9 (ignore wrong placements).
    private void HandleCorrectPlacement(CompoundReference compound, CategoryZone zone)
    {
        // Don't do anything further if the puzzle is already completed.
        if (IsCompleted)
        {
            return;
        }

        // Only count this compound if it's actually one of the required compounds for this puzzle.
        // This prevents unrelated compounds (from a different puzzle instance) from affecting this checker.
        if (!requiredCompounds.Contains(compound))
        {
            return;
        }

        // HashSet.Add() returns false if the item is already present, which naturally
        // handles requirement 10 (ignore duplicate correct events) with no extra checks needed.
        correctlyPlaced.Add(compound);

        CheckForCompletion();
    }

    // Compares how many required compounds have been correctly placed
    // against how many are required in total.
    private void CheckForCompletion()
    {
        if (correctlyPlaced.Count >= requiredCompounds.Count)
        {
            IsCompleted = true;
            Debug.Log("Category puzzle completed!");
            OnCategoryCompleted?.Invoke();
            completionPanel.SetActive(true);
        }
    }
}