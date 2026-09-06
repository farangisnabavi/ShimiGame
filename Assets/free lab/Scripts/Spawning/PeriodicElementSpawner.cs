using UnityEngine;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Spawning
{
    public class PeriodicElementSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float spawnHeightOffset = 0.5f;
        [SerializeField] private GameObject panel;

        public GameObject SpawnElement(PeriodicElementData data, Vector3 position)
        {
            if (data == null || data.prefab3D == null)
            {
                Debug.LogError("Element Data or Prefab is missing!");
                return null;
            }

            Vector3 spawnPos = position + Vector3.up * spawnHeightOffset;

            GameObject instance = Instantiate(
                data.prefab3D,
                spawnPos,
                Quaternion.identity
            );

            instance.name = $"{data.symbol}_{data.atomicNumber}";

            PeriodicElementInstance elementInstance =
                instance.GetComponent<PeriodicElementInstance>();

            if (elementInstance == null)
                elementInstance = instance.AddComponent<PeriodicElementInstance>();

            elementInstance.AssignElementData(data);

            if (instance.GetComponent<PeriodicWorldElementDrag>() == null)
                instance.AddComponent<PeriodicWorldElementDrag>();

            if (panel != null)
                panel.SetActive(false);

            return instance;
        }

        private PeriodicElementData selectedElement;

        public void SetSelectedElement(PeriodicElementData element)
        {
            selectedElement = element;
            Debug.Log($"Selected: {element?.elementName}");
        }

        public PeriodicElementData GetSelectedElement()
        {
            return selectedElement;
        }
    }
}