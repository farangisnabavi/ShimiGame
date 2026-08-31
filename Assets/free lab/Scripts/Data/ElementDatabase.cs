using System.Collections.Generic;
using UnityEngine;
using PeriodicTableSystem.Data;

namespace PeriodicTableSystem.Database 
{
    [CreateAssetMenu(fileName = "ElementDatabase", menuName = "PeriodicTable/Element Database")]
    public class ElementDatabase : ScriptableObject
    {
        [SerializeField] private List<PeriodicElementData> elements = new List<PeriodicElementData>();
        
        public List<PeriodicElementData> Elements => elements;
        
        public void AddElement(PeriodicElementData element)
        {
            if (!elements.Contains(element))
                elements.Add(element);
        }
    }
}