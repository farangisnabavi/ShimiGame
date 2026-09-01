using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PeriodicTableSystem.World;
using TMPro;

namespace PeriodicTableSystem.Chemistry
{
    public class PeriodicBondingSystem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI createdWhat;

        /// <summary>
        /// این سیستم دیگر Bond ایجاد نمی‌کند.
        /// BondManager مسئول ساخت و شکستن Bond است.
        /// این کلاس فقط به Eventهای Chemistry گوش می‌دهد
        /// و در صورت کامل شدن یک ساختار، آن را بررسی می‌کند.
        /// </summary>

        private void OnEnable()
        {
            ChemistryEventBus.OnBondCreated += HandleBondCreated;
            ChemistryEventBus.OnBondBroken += HandleBondBroken;
        }

        private void OnDisable()
        {
            ChemistryEventBus.OnBondCreated -= HandleBondCreated;
            ChemistryEventBus.OnBondBroken -= HandleBondBroken;
        }

        private void HandleBondCreated(Bond bond)
        {
            if (bond == null)
                return;

            if (bond.AtomA == null || bond.AtomB == null)
                return;

            CheckMoleculeCompletion(bond.AtomA);
        }

        private void HandleBondBroken(Bond bond)
        {
            if (bond == null)
                return;

            // فعلاً فقط پیام قبلی را پاک می‌کنیم.
            // منطق کامل Molecule Detection در Phase بعدی اضافه می‌شود.
            if (createdWhat != null)
                createdWhat.text = "";
        }

        private void CheckMoleculeCompletion(PeriodicElementInstance startAtom)
        {
            if (startAtom == null)
                return;

            if (!startAtom.IsStable())
                return;

            List<PeriodicElementInstance> molecule = new List<PeriodicElementInstance>();
            HashSet<PeriodicElementInstance> visited =
                new HashSet<PeriodicElementInstance>();

            Queue<PeriodicElementInstance> queue =
                new Queue<PeriodicElementInstance>();

            queue.Enqueue(startAtom);
            visited.Add(startAtom);

            while (queue.Count > 0)
            {
                PeriodicElementInstance current = queue.Dequeue();

                if (current == null)
                    continue;

                molecule.Add(current);

                foreach (Bond bond in current.ActiveBonds)
                {
                    if (bond == null)
                        continue;

                    PeriodicElementInstance neighbor = null;

                    if (bond.AtomA == current)
                        neighbor = bond.AtomB;
                    else if (bond.AtomB == current)
                        neighbor = bond.AtomA;

                    if (neighbor == null)
                        continue;

                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            if (molecule.Count <= 1)
                return;

            if (!molecule.All(atom => atom != null && atom.IsStable()))
                return;

            string formula = GenerateFormula(molecule);

            Debug.Log(
                $"<color=green>[PeriodicBondingSystem] Stable molecule: {formula}</color>"
            );

            if (createdWhat != null)
                createdWhat.text = "Created: " + formula;
        }

        private string GenerateFormula(List<PeriodicElementInstance> molecule)
        {
            var groups = molecule
                .Where(atom => atom != null && atom.ElementData != null)
                .GroupBy(atom => atom.ElementData.symbol)
                .OrderBy(group => group.Key);

            string formula = "";

            foreach (var group in groups)
            {
                int count = group.Count();

                formula += group.Key;

                if (count > 1)
                    formula += count.ToString();
            }

            return formula;
        }
    }
}