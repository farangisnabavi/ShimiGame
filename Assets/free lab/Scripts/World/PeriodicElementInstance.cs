using System.Collections.Generic;
using UnityEngine;
using PeriodicTableSystem.Data;

namespace PeriodicTableSystem.World
{
    public class PeriodicElementInstance : MonoBehaviour
    {
        public PeriodicElementData ElementData { get; private set; }
        
        [SerializeField] private int currentBonds = 0;
        [SerializeField] private List<PeriodicElementInstance> bondedNeighbors = new List<PeriodicElementInstance>();
        
        public int CurrentBonds => currentBonds;
        public IReadOnlyList<PeriodicElementInstance> BondedNeighbors => bondedNeighbors;

        public void AssignElementData(PeriodicElementData data)
        {
            ElementData = data;
            currentBonds = 0;
            bondedNeighbors.Clear();
            // هیچ کد مربوط به رنگ اینجا نیست!
        }

        public bool CanBond() => ElementData != null && currentBonds < ElementData.maxBonds;
        
        public bool IsStable() => ElementData != null && currentBonds >= ElementData.maxBonds;
        
        public int GetBondDeficit() => ElementData == null ? 0 : ElementData.maxBonds - currentBonds;

        public bool TryBondWith(PeriodicElementInstance other)
        {
            if (other == null || other == this) return false;
            if (!CanBond() || !other.CanBond()) return false;
            if (bondedNeighbors.Contains(other)) return false;
            
            bondedNeighbors.Add(other);
            other.bondedNeighbors.Add(this);
            currentBonds++;
            other.currentBonds++;
            
            return true;
        }

        public void BreakBondWith(PeriodicElementInstance other)
        {
            if (!bondedNeighbors.Contains(other)) return;
            bondedNeighbors.Remove(other);
            other.bondedNeighbors.Remove(this);
            currentBonds--;
            other.currentBonds--;
        }
        
        void OnDrawGizmosSelected()
        {
            if (ElementData == null) return;
            Gizmos.color = Color.white;
            foreach (var neighbor in bondedNeighbors)
                if (neighbor != null) 
                    Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
