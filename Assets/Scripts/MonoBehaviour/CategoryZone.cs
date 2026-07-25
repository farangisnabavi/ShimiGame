using UnityEngine;
using System;

// Attach this to each drop area (e.g. "IonicZone", "CovalentZone").
// Requires a Collider with "Is Trigger" enabled.
//
// This script only DETECTS which compound is currently overlapping the zone
// and reports it when the mouse is released. It does not validate correctness -
// that responsibility belongs entirely to CategoryChecker.
public class CategoryZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    [Tooltip("The bond type this zone represents (e.g. this zone only accepts Ionic compounds).")]
    public BondType requiredBondType;

    // Tracks whichever compound is currently overlapping this zone's trigger collider.
    private CompoundReference currentCompoundInside;

    // Fired whenever the player releases the mouse while a compound is inside this zone.
    // CategoryChecker subscribes to this to perform the actual validation.
    public event Action<CompoundReference, CategoryZone> OnCompoundDropped;

    void Reset()
    {
        // Helpful reminder in the Editor if the collider isn't set up correctly
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning(gameObject.name + ": Collider should have 'Is Trigger' enabled for CategoryZone to work.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        CompoundReference compound = other.GetComponent<CompoundReference>();
        if (compound != null)
        {
            currentCompoundInside = compound;
        }
    }

    void OnTriggerExit(Collider other)
    {
        CompoundReference compound = other.GetComponent<CompoundReference>();
        if (compound != null && compound == currentCompoundInside)
        {
            currentCompoundInside = null;
        }
    }

    void Update()
    {
        // When the player releases the mouse while a compound is sitting inside this zone,
        // report the drop event. This works alongside any existing drag & drop system
        // without needing a reference to it - it simply reacts to mouse-up + trigger overlap.
        if (Input.GetMouseButtonUp(0) && currentCompoundInside != null)
        {
            OnCompoundDropped?.Invoke(currentCompoundInside, this);
        }
    }
}