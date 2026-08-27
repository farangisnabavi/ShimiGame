using UnityEngine;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Spawning
{
    public class PeriodicElementSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float spawnHeightOffset = 0.5f;
        
        public GameObject SpawnElement(PeriodicElementData data, Vector3 position)
        {
            if (data?.prefab3D == null) return null;

            Vector3 spawnPos = position + Vector3.up * spawnHeightOffset;
            GameObject instance = Instantiate(data.prefab3D, spawnPos, Quaternion.identity);
            instance.name = $"{data.symbol}_{data.atomicNumber}";
            
            PeriodicElementInstance elementInstance = instance.GetComponent<PeriodicElementInstance>();
            if (elementInstance == null) elementInstance = instance.AddComponent<PeriodicElementInstance>();
            elementInstance.AssignElementData(data);
            
            if (instance.GetComponent<PeriodicWorldElementDrag>() == null)
                instance.AddComponent<PeriodicWorldElementDrag>();
            
            return instance;
        }
    }
}