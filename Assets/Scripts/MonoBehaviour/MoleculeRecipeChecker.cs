using UnityEngine;
using System;
using System.Collections.Generic;

// This system stores valid molecule "recipes" (which elements + how many of each
// are needed to form a correct molecule) and compares them against whatever
// elements are currently placed.
//
// It does NOT know about Drag & Drop, Snapping, or Triggers.
// It simply receives a list of ElementData (via a public method call) from
// whichever script gathers that information, and returns whether it matches a recipe.

// A single recipe entry: one element type and how many of it are required.
[Serializable]
public class ElementRequirement
{
    [Tooltip("Which element is required (references the ElementData ScriptableObject).")]
    public ElementData element;

    [Tooltip("How many of this element are required for the recipe.")]
    public int requiredAmount;
}

// A full molecule recipe: a name plus a list of element requirements.
[Serializable]
public class MoleculeRecipe
{
    [Tooltip("Name of the molecule, e.g., 'Water' or 'Carbon Dioxide'.")]
    public string moleculeName;

    [Tooltip("List of elements and quantities required to form this molecule.")]
    public List<ElementRequirement> requirements = new List<ElementRequirement>();
}

public class MoleculeRecipeChecker : MonoBehaviour
{
    [Header("Recipe Setup (Inspector-configurable)")]
    [Tooltip("Define all valid molecule recipes here. Easy to expand for more puzzle levels.")]
    public List<MoleculeRecipe> moleculeRecipes = new List<MoleculeRecipe>();

    // Internal Dictionary built at runtime for fast lookups by molecule name.
    // Keeping the Inspector list AND a Dictionary gives us both editability and speed.
    private Dictionary<string, MoleculeRecipe> recipeLookup = new Dictionary<string, MoleculeRecipe>();

    // Public event other scripts can subscribe to, fired after every check.
    // Passes the molecule name checked and whether it was correct.
    public event Action<string, bool> OnMoleculeChecked;

    void Awake()
    {
        BuildRecipeDictionary();
    }

    // Converts the Inspector-friendly list into a Dictionary for quick access by name.
    void BuildRecipeDictionary()
    {
        recipeLookup.Clear();

        foreach (MoleculeRecipe recipe in moleculeRecipes)
        {
            if (recipe != null && !string.IsNullOrEmpty(recipe.moleculeName))
            {
                recipeLookup[recipe.moleculeName] = recipe;
            }
        }
    }

    // Public method other scripts call to check if a set of placed elements
    // matches a specific molecule recipe by name.
    // "placedElements" is simply a list of ElementData currently placed - 
    // this script doesn't care where that list came from.
    public bool CheckMolecule(string moleculeName, List<ElementData> placedElements)
    {
        bool isCorrect = false;

        // Try to find the recipe by name in our Dictionary
        if (recipeLookup.TryGetValue(moleculeName, out MoleculeRecipe recipe))
        {
            isCorrect = DoesMatchRecipe(recipe, placedElements);
        }
        else
        {
            Debug.LogWarning("No recipe found for molecule name: " + moleculeName);
        }

        // Notify any listening scripts of the result
        OnMoleculeChecked?.Invoke(moleculeName, isCorrect);

        return isCorrect;
    }

    // Compares the placed elements against a single recipe's requirements.
    bool DoesMatchRecipe(MoleculeRecipe recipe, List<ElementData> placedElements)
    {
        // Count how many of each element are currently placed
        Dictionary<ElementData, int> placedCounts = new Dictionary<ElementData, int>();

        foreach (ElementData element in placedElements)
        {
            if (element == null) continue;

            if (placedCounts.ContainsKey(element))
                placedCounts[element]++;
            else
                placedCounts[element] = 1;
        }

        // Check that every requirement is met exactly
        foreach (ElementRequirement requirement in recipe.requirements)
        {
            int placedAmount = placedCounts.ContainsKey(requirement.element) ? placedCounts[requirement.element] : 0;

            if (placedAmount != requirement.requiredAmount)
            {
                return false; // Wrong quantity (too few or too many) - molecule is incorrect
            }
        }

        // Also make sure there are no extra, unrequired elements placed
        int totalRequiredTypes = recipe.requirements.Count;
        int totalPlacedTypes = placedCounts.Count;

        if (totalPlacedTypes != totalRequiredTypes)
        {
            return false; // Extra element types present that aren't part of the recipe
        }

        return true; // All checks passed - molecule matches the recipe
    }

    // Optional public helper for other scripts to fetch a recipe directly, if needed.
    public MoleculeRecipe GetRecipe(string moleculeName)
    {
        recipeLookup.TryGetValue(moleculeName, out MoleculeRecipe recipe);
        return recipe;
    }
}