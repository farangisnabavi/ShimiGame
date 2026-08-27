using UnityEngine;

namespace PeriodicTableSystem.Data
{
    [CreateAssetMenu(fileName = "NewElement", menuName = "PeriodicTableSystem/ElementData")]
    public class PeriodicElementData : ScriptableObject
    {
        [Header("Identity")]
        public string elementName;
        public string symbol;
        public int atomicNumber;
        
        [Header("Visual")]
        public GameObject prefab3D;
        public Color elementColor = Color.white;
        
        [Header("Electron Configuration")]
        [Range(1, 8)]
        public int valenceElectrons = 1;
        
        [Tooltip("Usually 8 - valence for non-metals (except H=1). For metals: valence electrons.")]
        public int maxBonds = 1;
        
        [Header("Bonding Properties")]
        [Range(0.7f, 4.0f)]
        public float electronegativity = 2.1f;
        
        public bool isMetal = false;
        public float atomicRadius = 0.5f;
    }
}