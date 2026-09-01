using System.Collections.Generic;
using UnityEngine;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.Chemistry;

namespace PeriodicTableSystem.World
{
    public class PeriodicElementInstance : MonoBehaviour
    {
        [SerializeField] private PeriodicElementData elementData;

        private readonly List<Bond> activeBonds = new List<Bond>();

        public PeriodicElementData ElementData => elementData;

        public IReadOnlyList<Bond> ActiveBonds => activeBonds;

        public int CurrentBondCount => activeBonds.Count;

        public bool CanBond =>
            elementData != null &&
            CurrentBondCount < elementData.maxBonds;

        public void AssignElementData(PeriodicElementData data)
        {
            elementData = data;
            activeBonds.Clear();
        }

        public void RegisterBond(Bond bond)
        {
            if (bond == null)
                return;

            if (!activeBonds.Contains(bond))
            {
                activeBonds.Add(bond);
            }
        }

        public void UnregisterBond(Bond bond)
        {
            if (bond == null)
                return;

            activeBonds.Remove(bond);
        }

        public bool HasBondWith(PeriodicElementInstance other)
        {
            if (other == null)
                return false;

            foreach (Bond bond in activeBonds)
            {
                if (bond == null)
                    continue;

                if (bond.AtomA == other || bond.AtomB == other)
                    return true;
            }

            return false;
        }

        public int GetBondDeficit()
        {
            if (elementData == null)
                return 0;

            return Mathf.Max(0, elementData.maxBonds - CurrentBondCount);
        }

        public bool IsStable()
        {
            return elementData != null &&
                   CurrentBondCount >= elementData.maxBonds;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            foreach (Bond bond in activeBonds)
            {
                if (bond == null)
                    continue;

                PeriodicElementInstance other = null;

                if (bond.AtomA == this)
                    other = bond.AtomB;
                else if (bond.AtomB == this)
                    other = bond.AtomA;

                if (other != null)
                {
                    Gizmos.DrawLine(
                        transform.position,
                        other.transform.position
                    );
                }
            }
        }
    }
}