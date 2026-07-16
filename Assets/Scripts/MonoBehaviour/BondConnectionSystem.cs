using UnityEngine;
using System;
using System.Collections.Generic;

// This system draws visual "bonds" (lines) between atom objects using LineRenderer.
// It has NO knowledge of chemistry rules, molecule validation, dragging, or snapping.
// It simply draws and updates lines between whatever Transforms it's told to connect.

// Represents a single bond: two atoms connected by one visual line.
[Serializable]
public class BondLink
{
    [Tooltip("First atom in the bond.")]
    public Transform atomA;

    [Tooltip("Second atom in the bond.")]
    public Transform atomB;

    [Tooltip("The LineRenderer used to draw this bond. Assign in Inspector, or leave empty to auto-create one.")]
    public LineRenderer lineRenderer;
}

public class BondConnectionSystem : MonoBehaviour
{
    [Header("Bond Setup (Inspector-configurable)")]
    [Tooltip("List of atom pairs to connect. Add as many as needed for a given molecule.")]
    public List<BondLink> bonds = new List<BondLink>();

    [Header("Line Appearance")]
    [Tooltip("Width of the bond lines.")]
    public float lineWidth = 0.05f;

    [Tooltip("Material used for auto-created LineRenderers (optional).")]
    public Material lineMaterial;

    // Public event fired whenever a new bond is added at runtime
    public event Action<BondLink> OnBondAdded;

    // Public event fired whenever a bond is removed at runtime
    public event Action<BondLink> OnBondRemoved;

    void Start()
    {
        // Make sure every bond defined in the Inspector has a valid LineRenderer
        foreach (BondLink bond in bonds)
        {
            EnsureLineRenderer(bond);
        }
    }

    void Update()
    {
        // Every frame, update each bond's line to match the current atom positions.
        // This keeps bonds visually correct even if atoms move (e.g., while dragging).
        foreach (BondLink bond in bonds)
        {
            UpdateBondPositions(bond);
        }
    }

    // Updates a single bond's LineRenderer to stretch between its two atoms
    void UpdateBondPositions(BondLink bond)
    {
        // Skip if either atom or the line renderer is missing
        if (bond.atomA == null || bond.atomB == null || bond.lineRenderer == null)
            return;

        bond.lineRenderer.positionCount = 2;
        bond.lineRenderer.SetPosition(0, bond.atomA.position);
        bond.lineRenderer.SetPosition(1, bond.atomB.position);
    }

    // Creates a LineRenderer automatically if one wasn't assigned in the Inspector
    void EnsureLineRenderer(BondLink bond)
    {
        if (bond.lineRenderer != null)
            return;

        // Create a new child GameObject to hold the LineRenderer
        GameObject lineObj = new GameObject("Bond_Line");
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;

        if (lineMaterial != null)
        {
            lr.material = lineMaterial;
        }

        bond.lineRenderer = lr;
    }

    // Public method for other scripts to add a new bond at runtime between two atoms.
    // Returns the created BondLink so the caller can keep a reference if needed.
    public BondLink AddBond(Transform atomA, Transform atomB)
    {
        BondLink newBond = new BondLink
        {
            atomA = atomA,
            atomB = atomB
        };

        EnsureLineRenderer(newBond);
        bonds.Add(newBond);

        // Notify any listening scripts that a new bond was created
        OnBondAdded?.Invoke(newBond);

        return newBond;
    }

    // Public method for other scripts to remove a specific bond at runtime.
    public void RemoveBond(BondLink bond)
    {
        if (bond == null || !bonds.Contains(bond))
            return;

        // Destroy the visual line object before removing the bond data
        if (bond.lineRenderer != null)
        {
            Destroy(bond.lineRenderer.gameObject);
        }

        bonds.Remove(bond);

        // Notify any listening scripts that a bond was removed
        OnBondRemoved?.Invoke(bond);
    }

    // Public helper: returns how many bonds are currently active
    public int GetBondCount()
    {
        return bonds.Count;
    }
}