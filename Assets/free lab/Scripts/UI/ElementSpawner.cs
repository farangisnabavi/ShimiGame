using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject elementPrefab;
    public Transform spawnPoint;

    public void SpawnElement()
    {
        if (elementPrefab == null)
        {
            Debug.LogError("Element Prefab is not assigned!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is not assigned!");
            return;
        }

        Instantiate(
            elementPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}