using UnityEngine;

// Defines the type of chemical bond a compound has.
// Used purely as data - no logic here.
public enum BondType
{
    Ionic,
    Covalent
}

// A ScriptableObject representing a single chemical compound.
// Create instances via: right-click in Project window > Create > Chemistry > Compound Data
[CreateAssetMenu(menuName = "Chemistry/Compound Data")]
public class CompoundData : ScriptableObject
{
    [Header("Compound Info")]
    public string compoundName;      // e.g. "Sodium Chloride"
    public string chemicalFormula;   // e.g. "NaCl"

    [Header("Bond Classification")]
    public BondType bondType;        // Ionic or Covalent - this is what the puzzle checks against

    [Header("Optional Visuals")]
    public Sprite icon;              // Optional - not required for logic to work
}