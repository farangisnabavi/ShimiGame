using UnityEngine;

// This ScriptableObject defines the data for a single chemical element.
// It does NOT contain any gameplay logic - it's purely a data container.
//
// [CreateAssetMenu] lets you right-click in the Project window and create
// new Element assets from this template (Assets > Create > Chemistry > Element Data).
[CreateAssetMenu(fileName = "NewElement", menuName = "Chemistry/Element Data")]
public class ElementData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Full name of the element, e.g., 'Hydrogen'.")]
    public string elementName;

    [Tooltip("Chemical symbol, e.g., 'H', 'O', 'Na'.")]
    public string chemicalSymbol;

    [Header("Atomic Info")]
    [Tooltip("The atomic number of the element, e.g., 1 for Hydrogen.")]
    public int atomicNumber;

    [Header("Visual Representation")]
    [Tooltip("Color used to visually represent this element (e.g., for materials or UI).")]
    public Color elementColor = Color.white;

    [Header("Bonding Info")]
    [Tooltip("Maximum number of bonds/connections this element can form.")]
    public int maxBonds;
}