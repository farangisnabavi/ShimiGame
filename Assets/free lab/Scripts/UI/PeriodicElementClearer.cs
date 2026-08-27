using UnityEngine;
using PeriodicTableSystem.World;

namespace PeriodicTableSystem.UI
{
    public class PeriodicElementClearer : MonoBehaviour
    {
        [Header("Optional")]
        [SerializeField] private bool clearOnKeyPress = false;
        [SerializeField] private KeyCode clearKey = KeyCode.Delete;

        void Update()
        {
            if (clearOnKeyPress && Input.GetKeyDown(clearKey))
                ClearElements();
        }

        public void ClearElements()
        {
            var allElements = FindObjectsOfType<PeriodicElementInstance>();
            
            int count = 0;
            foreach (var element in allElements)
            {
                if (element.gameObject != null)
                {
                    Destroy(element.gameObject);
                    count++;
                }
            }
            
            Debug.Log($"<color=red>{count} element(s) cleared from scene.</color>");
        }
    }
}