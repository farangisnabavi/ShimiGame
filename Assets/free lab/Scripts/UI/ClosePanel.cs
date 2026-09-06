using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Open()
    {
        panel.SetActive(false);
    }
}