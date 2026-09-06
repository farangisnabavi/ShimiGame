using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Chemistry
{
    public class MoleculeFormulaGenerator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BondManager bondManager;
        [SerializeField] private TMP_Text text;

        private void OnEnable()
        {
            ChemistryEventBus.OnBondCreated += OnBondCreated;
            ChemistryEventBus.OnBondBroken += OnBondBroken;

            Debug.Log("[FormulaGenerator] OnEnable");
        }

        private void OnDisable()
        {
            ChemistryEventBus.OnBondCreated -= OnBondCreated;
            ChemistryEventBus.OnBondBroken -= OnBondBroken;
        }

        private void OnBondCreated(Bond bond)
        {
            if (bond == null)
            {
                Debug.LogWarning("[FormulaGenerator] Received NULL bond.");
                return;
            }

            Debug.Log(
                $"[FormulaGenerator] Bond received: " +
                $"{GetSymbol(bond.AtomA)} - {GetSymbol(bond.AtomB)}"
            );

            if (bondManager == null)
            {
                Debug.LogError(
                    "[FormulaGenerator] BondManager is NULL.",
                    this
                );
                return;
            }

            if (bondManager.GetActiveBonds() == null)
            {
                Debug.LogError(
                    "[FormulaGenerator] Active bond list is NULL.",
                    this
                );
                return;
            }

            GenerateFormula(bond.AtomA);
        }

        private void OnBondBroken(Bond bond)
        {
            if (bond == null)
                return;

            if (bond.AtomA != null)
            {
                GenerateFormula(bond.AtomA);
            }
            else if (bond.AtomB != null)
            {
                GenerateFormula(bond.AtomB);
            }
        }

        private void GenerateFormula(PeriodicElementInstance startAtom)
        {
            if (startAtom == null)
                return;

            List<PeriodicElementInstance> molecule =
                FindConnectedAtoms(startAtom);

            if (molecule.Count == 0)
                return;

            string formula = BuildFormula(molecule);

            Debug.Log(
                $"<color=green>[FormulaGenerator] MOLECULE FORMULA → {formula}</color>",
                this
            );
            text.text =  formula;
        }

        private List<PeriodicElementInstance> FindConnectedAtoms(
            PeriodicElementInstance startAtom)
        {
            List<PeriodicElementInstance> result =
                new List<PeriodicElementInstance>();

            HashSet<PeriodicElementInstance> visited =
                new HashSet<PeriodicElementInstance>();

            Queue<PeriodicElementInstance> queue =
                new Queue<PeriodicElementInstance>();

            queue.Enqueue(startAtom);
            visited.Add(startAtom);

            IReadOnlyList<Bond> bonds =
                bondManager.GetActiveBonds();

            while (queue.Count > 0)
            {
                PeriodicElementInstance current =
                    queue.Dequeue();

                if (current == null)
                    continue;

                result.Add(current);

                foreach (Bond bond in bonds)
                {
                    if (bond == null)
                        continue;

                    PeriodicElementInstance neighbor = null;

                    if (bond.AtomA == current)
                    {
                        neighbor = bond.AtomB;
                    }
                    else if (bond.AtomB == current)
                    {
                        neighbor = bond.AtomA;
                    }

                    if (neighbor == null)
                        continue;

                    if (visited.Contains(neighbor))
                        continue;

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return result;
        }

        private string BuildFormula(
            List<PeriodicElementInstance> molecule)
        {
            Dictionary<string, int> counts =
                new Dictionary<string, int>();

            foreach (PeriodicElementInstance atom in molecule)
            {
                if (atom == null)
                    continue;

                if (atom.ElementData == null)
                    continue;

                string symbol = atom.ElementData.symbol;

                if (string.IsNullOrEmpty(symbol))
                    continue;

                if (!counts.ContainsKey(symbol))
                    counts[symbol] = 0;

                counts[symbol]++;
            }

            if (counts.Count == 0)
                return "";

            /*
             * ترتیب فرمول:
             *
             * C
             * H
             * سایر عناصر
             *
             * مثال:
             * C2H6O
             * H2O
             * CO2
             * NaCl
             */

            List<KeyValuePair<string, int>> sorted =
                counts
                    .OrderBy(x => x.Key == "C" ? 0 :
                                  x.Key == "H" ? 1 : 2)
                    .ThenBy(x => x.Key)
                    .ToList();

            string formula = "";

            foreach (var element in sorted)
            {
                formula += element.Key;

                if (element.Value > 1)
                {
                    formula += ToSubscript(element.Value);
                }
            }

            return formula;
        }

        private string ToSubscript(int number)
        {
            string result = "";

            foreach (char c in number.ToString())
            {
                switch (c)
                {
                    case '0': result += "₀"; break;
                    case '1': result += "₁"; break;
                    case '2': result += "₂"; break;
                    case '3': result += "₃"; break;
                    case '4': result += "₄"; break;
                    case '5': result += "₅"; break;
                    case '6': result += "₆"; break;
                    case '7': result += "₇"; break;
                    case '8': result += "₈"; break;
                    case '9': result += "₉"; break;
                }
            }

            return result;
        }

        private string GetSymbol(
            PeriodicElementInstance atom)
        {
            if (atom == null)
                return "?";

            if (atom.ElementData == null)
                return "?";

            return atom.ElementData.symbol;
        }
    }
}

