using UnityEngine;
using UnityEngine.UI;

namespace PeriodicTableSystem.Data
{
    public class PeriodicTableElementItem : MonoBehaviour
    {
        public PeriodicElementData elementData;
        
        [Header("UI References")]
        [SerializeField] private Text symbolText;
        [SerializeField] private Text atomicNumberText;
        [SerializeField] private Image backgroundImage;
        
        private void Start()
        {
            if (elementData != null) UpdateVisuals();
        }
        
        public void AssignData(PeriodicElementData data)
        {
            elementData = data;
            UpdateVisuals();
        }
        
        private void UpdateVisuals()
        {
            if (symbolText != null) symbolText.text = elementData.symbol;
            if (atomicNumberText != null) atomicNumberText.text = elementData.atomicNumber.ToString();
            if (backgroundImage != null) backgroundImage.color = elementData.elementColor;
        }
    }
}