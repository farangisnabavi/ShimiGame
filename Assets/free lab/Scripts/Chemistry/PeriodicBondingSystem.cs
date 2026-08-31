using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PeriodicTableSystem.World;
using TMPro;

namespace PeriodicTableSystem.Chemistry
{
    public class PeriodicBondingSystem : MonoBehaviour
    {
        [Header("Bonding Settings")]
        [SerializeField] private float bondCheckRadius = 2f;
        [SerializeField] private float bondDistanceThreshold = 1.5f;
        [SerializeField] private LayerMask elementLayer;
        [SerializeField] private TextMeshProUGUI createdWhat;
        
        private List<PeriodicElementInstance> activeElements = new List<PeriodicElementInstance>();
        
        void Update() => CheckPotentialBonds();
        
        void CheckPotentialBonds()
        {
            activeElements = FindObjectsOfType<PeriodicElementInstance>().ToList();
            
            foreach (var element in activeElements)
            {
                if (!element.CanBond()) continue;
                
                Collider[] nearby = Physics.OverlapSphere(element.transform.position, bondCheckRadius, elementLayer);
                
                foreach (var col in nearby)
                {
                    var neighbor = col.GetComponent<PeriodicElementInstance>();
                    if (neighbor == null || neighbor == element) continue;
                    if (!neighbor.CanBond()) continue;
                    
                    float dist = Vector3.Distance(element.transform.position, neighbor.transform.position);
                    if (dist > bondDistanceThreshold) continue;
                    
                    if (ShouldFormBond(element, neighbor))
                    {
                        if (element.TryBondWith(neighbor))
                            CheckMoleculeCompletion(element);
                    }
                }
            }
        }
        
        bool ShouldFormBond(PeriodicElementInstance a, PeriodicElementInstance b)
        {
            float elecDiff = Mathf.Abs(a.ElementData.electronegativity - b.ElementData.electronegativity);
            bool isIonic = (a.ElementData.isMetal != b.ElementData.isMetal) && elecDiff > 1.7f;
            bool isCovalent = elecDiff <= 1.7f;
            return isIonic || isCovalent;
        }
        
        void CheckMoleculeCompletion(PeriodicElementInstance startAtom)
        {
            if (!startAtom.IsStable()) return;
            
            var molecule = new List<PeriodicElementInstance>();
            var visited = new HashSet<PeriodicElementInstance>();
            var queue = new Queue<PeriodicElementInstance>();
            
            queue.Enqueue(startAtom);
            visited.Add(startAtom);
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                molecule.Add(current);
                
                foreach (var neighbor in current.BondedNeighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            
            if (molecule.All(atom => atom.IsStable()) && molecule.Count > 1)
            {
                string formula = GenerateFormula(molecule);
                Debug.Log($"<color=green>Stable molecule: {formula}</color>");
                createdWhat.text = "Created: "+formula;
            }
        }
        
        string GenerateFormula(List<PeriodicElementInstance> molecule)
        {
            var groups = molecule.GroupBy(x => x.ElementData.symbol).OrderBy(g => g.Key);
            string formula = "";
            foreach (var group in groups)
            {
                int count = group.Count();
                formula += group.Key + (count > 1 ? count.ToString() : "");
            }
            return formula;
        }
    }
}
