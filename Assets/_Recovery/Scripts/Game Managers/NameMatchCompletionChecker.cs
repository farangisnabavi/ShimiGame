using UnityEngine;
using System;
using System.Collections.Generic;

// Attach this to an empty GameObject that oversees a set of name-matching targets
// for one puzzle (e.g., "NameMatchChecker_Level1").
//
// It does NOT modify NameMatchTarget - it only reads the public
// "LastMatchWasCorrect" property that NameMatchTarget already exposes.
public class NameMatchCompletionChecker : MonoBehaviour
{
    [Header("Targets to Watch")]
    [Tooltip("Every NameMatchTarget that must be correctly matched for this puzzle.")]
    public List<NameMatchTarget> requiredTargets = new List<NameMatchTarget>();
    [SerializeField] private GameObject completionPanel;

    // Public property so other scripts can check the current state
    public bool AllMatchesCorrect { get; private set; } = false;

    // Public event fired once, exactly when every target becomes correctly matched
    public event Action OnAllMatchesCorrect;

    void Update()
    {
        // Only keep checking if we haven't already confirmed completion
        if (!AllMatchesCorrect)
        {
            CheckAllMatches();
        }
    }

    void CheckAllMatches()
    {
        if (requiredTargets.Count == 0)
            return;

        foreach (NameMatchTarget target in requiredTargets)
        {
            if (target == null || target.LastMatchWasCorrect != true)
            {
                return;
            }
        }

        AllMatchesCorrect = true;

        // NEW: prints confirmation to the Console
        Debug.Log("All name matches are correct!");
        completionPanel.SetActive(true);

        OnAllMatchesCorrect?.Invoke();
    }
    // Optional public method other scripts can call to manually re-check right now
    public bool CheckNow()
    {
        CheckAllMatches();
        return AllMatchesCorrect;
    }
}