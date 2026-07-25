using UnityEngine;
using System;
using System.Collections.Generic;

// CategoryChecker is the final validation step in the bond-category puzzle.
// Its ONLY responsibility is comparing a dropped compound's BondType
// against the BondType required by the zone it was dropped into.
//
// It does not know anything about dragging, snapping, UI, scoring, or sound.
// It communicates results purely through public events, so any other system
// (UI, audio, game manager, etc.) can react without this script knowing they exist.
public class CategoryChecker : MonoBehaviour
{
    [Header("Zones To Monitor")]
    [Tooltip("Assign every CategoryZone in the scene that this checker should listen to (e.g. Ionic zone, Covalent zone).")]
    public List<CategoryZone> zones = new List<CategoryZone>();

    // Public events - other systems subscribe to these to know the result.
    // CategoryChecker never calls into UI, audio, or game manager scripts directly.
    public event Action<CompoundReference, CategoryZone> OnCorrectPlacement;
    public event Action<CompoundReference, CategoryZone> OnWrongPlacement;

    // Subscribe to every assigned zone's drop event when this component becomes active.
    // Using OnEnable/OnDisable (instead of Start/OnDestroy) ensures subscriptions are
    // correctly re-established if this object is ever disabled and re-enabled.
    void OnEnable()
    {
        foreach (CategoryZone zone in zones)
        {
            if (zone != null)
            {
                zone.OnCompoundDropped += HandleCompoundDropped;
            }
        }
    }

    // Always unsubscribe to prevent memory leaks or duplicate event calls,
    // especially important if zones or checkers are destroyed/recreated at runtime.
    void OnDisable()
    {
        foreach (CategoryZone zone in zones)
        {
            if (zone != null)
            {
                zone.OnCompoundDropped -= HandleCompoundDropped;
            }
        }
    }

    // Called automatically whenever any monitored CategoryZone reports a drop.
    // This is the single place where validation logic happens.
    private void HandleCompoundDropped(CompoundReference compound, CategoryZone zone)
    {
        // Safety check: make sure the dropped object actually has valid compound data.
        if (compound == null || compound.Compound == null)
        {
            Debug.LogWarning("CategoryChecker: Dropped object has no valid CompoundData assigned.");
            return;
        }

        // Safety check: make sure the zone reference itself is valid.
        if (zone == null)
        {
            Debug.LogWarning("CategoryChecker: Received a drop event from a null zone.");
            return;
        }

        // The core comparison: does the compound's bond type match what this zone expects?
        bool isMatch = compound.Compound.bondType == zone.requiredBondType;

        if (isMatch)
        {
            Debug.Log("[CORRECT] " + compound.Compound.compoundName + " (" + compound.Compound.bondType +
                       ") was correctly placed in the " + zone.requiredBondType + " zone.");

            OnCorrectPlacement?.Invoke(compound, zone);
        }
        else
        {
            Debug.Log("[WRONG] " + compound.Compound.compoundName + " (" + compound.Compound.bondType +
                       ") does not belong in the " + zone.requiredBondType + " zone.");

            OnWrongPlacement?.Invoke(compound, zone);
        }
    }
}