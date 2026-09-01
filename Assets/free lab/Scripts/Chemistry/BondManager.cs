using System.Collections.Generic;
using UnityEngine;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Chemistry
{
    public class BondManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MoleculeBondAnalyzer bondAnalyzer;
        [SerializeField] private BondVisual bondVisualPrefab;

        [Header("Bond Settings")]
        [SerializeField] private float snapDistance = 2.0f;

        private readonly List<Bond> activeBonds = new List<Bond>();

        public IReadOnlyList<Bond> GetActiveBonds()
        {
            return activeBonds;
        }

        // =========================================================
        // PUBLIC API
        // =========================================================

        public void RequestBond(
            PeriodicElementInstance atomA,
            PeriodicElementInstance atomB,
            PeriodicElementInstance draggedAtom)
        {
            // -------------------------
            // 1. Structural validation
            // -------------------------

            if (!ValidateStructural(atomA, atomB))
                return;

            if (draggedAtom == null)
            {
                Debug.LogWarning(
                    "[BondManager] Dragged atom is null.",
                    this
                );

                return;
            }

            // -------------------------
            // 2. Analyzer reference
            // -------------------------

            if (bondAnalyzer == null)
            {
                Debug.LogError(
                    "[BondManager] Bond Analyzer is not assigned.",
                    this
                );

                return;
            }

            // -------------------------
            // 3. Chemical evaluation
            // -------------------------

            BondResultAnalyze result =
                bondAnalyzer.EvaluateBond(
                    atomA.ElementData,
                    atomB.ElementData
                );

            if (result == null)
            {
                Debug.LogWarning(
                    "[BondManager] Analyzer returned null.",
                    this
                );

                return;
            }

            // Analyzer جدید نیست.
            // نتیجه‌ی Analyzer فعلی از نوع BondTypeAnalyze است.

            if (result.BondTypeAnalyze == BondTypeAnalyze.None)
            {
                Debug.Log(
                    $"[BondManager] No valid bond between " +
                    $"{atomA.ElementData.symbol} and " +
                    $"{atomB.ElementData.symbol}."
                );

                return;
            }

            // -------------------------
            // 4. Bond capacity
            // -------------------------

            if (!atomA.CanBond || !atomB.CanBond)
            {
                Debug.Log(
                    "[BondManager] One of the atoms has reached " +
                    "maximum bond capacity."
                );

                return;
            }

            // -------------------------
            // 5. Create
            // -------------------------

            CreateBond(
                atomA,
                atomB,
                result,
                draggedAtom
            );
        }

        public void BreakBond(Bond bond)
        {
            if (bond == null)
                return;

            // Remove state from Atom A
            if (bond.AtomA != null)
            {
                bond.AtomA.UnregisterBond(bond);
            }

            // Remove state from Atom B
            if (bond.AtomB != null)
            {
                bond.AtomB.UnregisterBond(bond);
            }

            // Remove from manager
            activeBonds.Remove(bond);

            // Destroy visual
            if (bond.Visual != null)
            {
                Destroy(bond.Visual.gameObject);
            }

            // Notify other systems
            ChemistryEventBus.BondBroken(bond);

            string atomAName =
                bond.AtomA != null &&
                bond.AtomA.ElementData != null
                    ? bond.AtomA.ElementData.symbol
                    : "Unknown";

            string atomBName =
                bond.AtomB != null &&
                bond.AtomB.ElementData != null
                    ? bond.AtomB.ElementData.symbol
                    : "Unknown";

            Debug.Log(
                $"[BondManager] Bond broken: " +
                $"{atomAName} — {atomBName}"
            );
        }

        public bool HasBondBetween(
            PeriodicElementInstance atomA,
            PeriodicElementInstance atomB)
        {
            if (atomA == null || atomB == null)
                return false;

            foreach (Bond bond in activeBonds)
            {
                if (bond == null)
                    continue;

                bool sameOrder =
                    bond.AtomA == atomA &&
                    bond.AtomB == atomB;

                bool reverseOrder =
                    bond.AtomA == atomB &&
                    bond.AtomB == atomA;

                if (sameOrder || reverseOrder)
                    return true;
            }

            return false;
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        private bool ValidateStructural(
            PeriodicElementInstance atomA,
            PeriodicElementInstance atomB)
        {
            // Null
            if (atomA == null || atomB == null)
            {
                Debug.LogWarning(
                    "[BondManager] One or both atoms are null."
                );

                return false;
            }

            // Same atom
            if (atomA == atomB)
            {
                Debug.LogWarning(
                    "[BondManager] Self-bond is not allowed."
                );

                return false;
            }

            // Element data
            if (atomA.ElementData == null ||
                atomB.ElementData == null)
            {
                Debug.LogWarning(
                    "[BondManager] One or both atoms have no ElementData."
                );

                return false;
            }

            // Duplicate
            if (HasBondBetween(atomA, atomB))
            {
                Debug.Log(
                    "[BondManager] Bond already exists between " +
                    $"{atomA.ElementData.symbol} and " +
                    $"{atomB.ElementData.symbol}."
                );

                return false;
            }

            return true;
        }

        // =========================================================
        // CREATE BOND
        // =========================================================

        private void CreateBond(
            PeriodicElementInstance atomA,
            PeriodicElementInstance atomB,
            BondResultAnalyze result,
            PeriodicElementInstance draggedAtom)
        {
            BondType bondType =
                ConvertBondType(result.BondTypeAnalyze);

            if (bondType == BondType.None)
                return;

            // Create data object
            Bond bond = new Bond(
                atomA,
                atomB,
                bondType,
                1
            );

            // Register on atoms
            atomA.RegisterBond(bond);
            atomB.RegisterBond(bond);

            // Register in manager
            activeBonds.Add(bond);

            // -------------------------
            // Snap
            // -------------------------

            PeriodicElementInstance targetAtom =
                draggedAtom == atomA
                    ? atomB
                    : atomA;

            SnapDraggedAtom(
                draggedAtom,
                targetAtom
            );

            // -------------------------
            // Visual
            // -------------------------

            if (bondVisualPrefab != null)
            {
                BondVisual visual =
                    Instantiate(bondVisualPrefab);

                visual.Initialize(
                    atomA,
                    atomB,
                    bondType
                );

                bond.Visual = visual;
            }
            else
            {
                Debug.LogWarning(
                    "[BondManager] BondVisual Prefab is not assigned.",
                    this
                );
            }

            // -------------------------
            // Event
            // -------------------------

            ChemistryEventBus.BondCreated(bond);

            Debug.Log(
                $"[BondManager] Bond created: " +
                $"{atomA.ElementData.symbol} — " +
                $"{atomB.ElementData.symbol} " +
                $"({bondType})"
            );
        }

        // =========================================================
        // SNAP
        // =========================================================

        private void SnapDraggedAtom(
            PeriodicElementInstance draggedAtom,
            PeriodicElementInstance targetAtom)
        {
            if (draggedAtom == null ||
                targetAtom == null)
            {
                return;
            }

            Vector3 direction =
                draggedAtom.transform.position -
                targetAtom.transform.position;

            // اگر دقیقاً روی هم باشند
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = targetAtom.transform.forward;

                if (direction.sqrMagnitude < 0.0001f)
                {
                    direction = Vector3.right;
                }
            }
            else
            {
                direction.Normalize();
            }

            Vector3 snapPosition =
                targetAtom.transform.position +
                direction * snapDistance;

            draggedAtom.transform.position =
                snapPosition;
        }

        // =========================================================
        // TYPE CONVERSION
        // =========================================================

        private BondType ConvertBondType(
            BondTypeAnalyze analyzeType)
        {
            switch (analyzeType)
            {
                case BondTypeAnalyze.Covalent:
                    return BondType.Covalent;

                case BondTypeAnalyze.Ionic:
                    return BondType.Ionic;

                case BondTypeAnalyze.Metallic:
                    return BondType.Metallic;

                default:
                    return BondType.None;
            }
        }
    }
}